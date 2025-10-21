var urlListarDespachosPendientes = baseUrl + 'Inventario/GuiaDespacho/ListarPaginadoConfirmacion';
var urlObtenerDespacho = baseUrl + 'Inventario/GuiaDespacho/Obtener'; 
var urlConfirmarDespacho = baseUrl + 'Inventario/GuiaDespacho/ConfirmarEnDestino';
var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';

var AdministrarConfirmacionDespachos = (function ($) {

    let dt = null; // DataTable reutilizable

    // ========= Eventos =========
    function eventos() {
        // Empresa Origen → carga locales y habilita el combo de local
        $('#fEmpOrigen').on('change', function () {
            cargarComboLocales('#fEmpOrigen', '#fLocOrigen'); // habilita al finalizar
        });

        // Guardar confirmación (anti doble clic robusto)
        $('#btnConfirmarModal').off('click').on('click', onConfirmarModalClick);

        $('#fEmpOrigen, #fLocOrigen, #fTipoMov, #fDesde, #fHasta')
            .off('change._pend')
            .on('change._pend', function () {
                if ($.fn.DataTable.isDataTable('#tablePendientes')) {
                    $('#tablePendientes').DataTable().ajax.reload();
                }
            });

    }

    // ================== Handlers ==================
    async function onConfirmarModalClick() {
        const id = parseInt($('#hidGuiaId').val() || '0', 10);
        const fecha = $('#mFecha').val();
        if (!id || !fecha) { swal({ text: "Complete la fecha de recepción.", icon: "warning" }); return; }

        const numGR = ($('#mNumGR').val() || '').trim();
        const obs = ($('#mObs').val() || '').trim();
        const genGR = $('#mGenerarGR').is(':checked');

        const lines = [];
        const errores = [];

        $('#mDetalle tbody tr').each(function () {
            const $tr = $(this);
            const $chk = $tr.find('.chkConf');
            if (!$chk.is(':checked')) return;

            const detId = parseInt($chk.data('id'), 10);
            const codProd = ($tr.data('producto') || '').toString();
            const esSer = ('' + $tr.data('serializable')).toLowerCase() === 'true';
            const pend = parseInt($tr.data('pend') || '0', 10);

            let cant = 1;
            let numSerie = null;

            if (esSer) {
                numSerie = ($tr.find('.cel-serie').text() || '').trim() || null;
                if (pend <= 0) errores.push(`Línea ${detId}: no hay pendiente por confirmar.`);
            } else {
                cant = parseInt($tr.find('.inpCant').val() || '0', 10);
                if (!Number.isInteger(cant) || cant <= 0) {
                    errores.push(`Línea ${detId}: cantidad inválida.`);
                } else if (cant > pend) {
                    errores.push(`Línea ${detId}: la cantidad (${cant}) no puede exceder el pendiente (${pend}).`);
                }
            }

            if (!codProd) errores.push(`Línea ${detId}: producto vacío.`);

            const estStock = ($tr.find('.selEstado').val() || '').toUpperCase();
            if (!estStock) {
                errores.push(`Línea ${detId}: seleccione estado de stock.`);
            } else if (estStock !== 'NUEVO' && estStock !== 'USADO') {
                errores.push(`Línea ${detId}: estado inválido (${estStock}).`);
            }


            lines.push({ DespachoDetalleId: detId, CodProducto: codProd, NumSerie: numSerie, Cantidad: cant, StkEstado: estStock });
        });

        if (!lines.length) { swal({ text: "Selecciona al menos una línea a confirmar.", icon: "warning" }); return; }
        if (errores.length) { swal({ text: errores.join('\n'), icon: "warning" }); return; }

        const payload = {
            GuiaDespachoId: id,
            NumGuiaRecepcion: numGR || null,
            Fecha: fecha,
            Observaciones: obs || null,
            GenerarGuiaRecepcion: !!genGR,
            Lineas: lines
        };

        // Anti doble clic
        const $btn = $('#btnConfirmarModal');
        const oldHtml = $btn.html();
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Confirmando…');

        try {
            const resp = await $.ajax({
                url: urlConfirmarDespacho,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (resp && resp.Ok) {
                swal({ text: resp.Mensaje || 'Confirmado correctamente.', icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalConfirmar')).hide();
                if (dt) dt.ajax.reload(null, false);
            } else {
                swal({ text: (resp && (resp.Mensaje || resp.message)) || 'No se pudo confirmar.', icon: 'warning' });
            }
        } catch (err) {
            swal({ text: (err && (err.responseText || err.statusText)) || 'Error al confirmar.', icon: 'error' });
        } finally {
            $btn.prop('disabled', false).html(oldHtml);
        }
    }

    // ========= Combos =========
    function listarEmpresas() {
        return $.ajax({ url: urlListarEmpresas, type: 'POST', data: { request: { CodUsuario: '', Busqueda: '' } } });
    }
    function listarLocales(codEmpresa) {
        if (!codEmpresa) return Promise.resolve({ Ok: true, Data: [] });
        return $.ajax({ url: urlListarLocalesPorEmpresa, type: 'POST', data: { request: { CodEmpresa: codEmpresa } } });
    }

    const cargarComboEmpresas = async function () {
        try {
            const response = await listarEmpresas();
            if (response.Ok) {
                await cargarCombo($('#fEmpOrigen'),
                    response.Data.map(e => ({ text: e.NomEmpresa, value: e.CodEmpresa })),
                    { placeholder: 'Todos' }
                );
                // Local deshabilitado hasta elegir empresa
                await cargarCombo($('#fLocOrigen'), [], { placeholder: 'Todos', disabled: true });
            } else {
                swal({ text: String((response && response.Mensaje) || 'Error al listar empresas'), icon: "error" });
            }
        } catch (error) {
            swal({ text: String((error && (error.Mensaje || error.message || error.responseText || error.statusText)) || 'Error al listar empresas'), icon: "error" });
        }
    };

    async function cargarComboLocales(selEmpresa, selLocal) {
        const $emp = $(selEmpresa), $loc = $(selLocal);
        if (!$emp.length || !$loc.length) return;

        try {
            const codEmpresa = $emp.val();
            if (!codEmpresa) {
                await cargarCombo($loc, [], { placeholder: 'Todos', todos: true });
                $loc.prop('disabled', true).trigger('change.select2');
                return;
            }
            const resp = await listarLocales(codEmpresa);
            if (resp.Ok) {
                await cargarCombo($loc,
                    resp.Data.map(l => ({ text: l.NomLocal, value: l.CodLocal })),
                    { placeholder: 'Todos', todos: true }
                );
                $loc.prop('disabled', false).trigger('change.select2');
            } else {
                swal({ text: swalText(resp, 'No fue posible listar locales'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' });
        }
    }

    function visualizarDataTable() {
        dt = $('#tablePendientes').DataTable({
            serverSide: true,
            processing: true,
            searching: true,
            ordering: false,
            ajax: function (data, callback) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    FechaDesde: $('#fDesde').val() || null,
                    FechaHasta: $('#fHasta').val() || null,
                    CodEmpresaOrigen: $('#fEmpOrigen').val() || null,
                    CodLocalOrigen: $('#fLocOrigen').val() || null,
                    TipoMovimiento: $('#fTipoMov').val() || null,
                    IndEstado: 'PENDIENTE_CONFIRMACION', // o EN_TRANSITO / PENDIENTE_CONFIRMACION según tu backend
                    FiltroVarios: (data.search.value || '').toUpperCase()
                };

                var params = $.extend({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarDespachosPendientes,
                    type: 'GET',
                    data: params,
                    dataType: 'json',
                    success: function (resp) {
                        if (resp.Ok) {
                            var p = resp.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: p.TotalRecords,
                                recordsFiltered: p.TotalRecords,
                                data: p.Items
                            });
                        } else {
                            callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                        }
                    },
                    error: function (jqXHR) {
                        swal({ text: swalText(jqXHR, 'Error al listar'), icon: 'error' });
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    }
                });
            },
            columns: [
                {
                    data: 'Fecha', render: function (data) {
                        if (!data) return '';
                        var m = /\/Date\((\d+)\)\//.exec(data + '');
                        if (m) {
                            var d = new Date(parseInt(m[1], 10));
                            return d.toLocaleDateString('es-PE');
                        }
                        return (data + '').substring(0, 10);
                    }
                },
                { data: 'NumGuia' },
                { data: 'NomLocalOrigen' },
                { data: 'NomLocalDestino' },
                { data: 'TipoMovimiento', defaultContent: '' },
                { data: 'Items', defaultContent: '' },
                { data: 'IndEstado', defaultContent: '' },
                {
                    data: null, className: 'dt-center nowrap', render: function (r) {
                        return '' +
                            '<button type="button" class="btn btn-sm btn-success btnConfirmarUno" ' +
                            'data-id="' + r.Id + '" data-tipo="' + ((r.TipoMovimiento || '').toUpperCase()) + '">' +
                            '<i class="fe fe-check me-1"></i>Confirmar</button>';
                    }
                }
            ],
            language: {
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                search: "N° Guía:"
            },
            initComplete: function () {
                $('#tablePendientes_filter input').addClass('form-control-sm').attr('placeholder', 'Buscar...');
            },
            scrollY: '500px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
            drawCallback: function () {
            }
        });

        // Confirmar una guía (botón por fila)
        $('#tablePendientes tbody').on('click', '.btnConfirmarUno', function () {
            var id = $(this).data('id');
            var tipo = (($(this).data('tipo') || '') + '').toUpperCase();
            abrirModalConfirmacion(id, tipo);
        });
    }

    // ========= Modal Confirmación =========
    async function abrirModalConfirmacion(id, tipoMov) {
        $('#hidGuiaId').val(id);
        $('#hidTipoMov').val(tipoMov || '');
        $('#mFecha').val(new Date().toISOString().slice(0, 10));
        $('#mNumGR').val('');
        $('#mObs').val('');

        // Si es BAJA, no corresponde GR: deshabilitar checkbox e input de número GR
        if ((tipoMov || '').toUpperCase() === 'BAJA') {
            $('#mGenerarGR').prop('checked', false).prop('disabled', true);
            $('#mNumGR').val('').prop('disabled', true)
                .attr('placeholder', 'No aplica para BAJA');
        } else {
            $('#mGenerarGR').prop('disabled', false).prop('checked', true);
            $('#mNumGR').prop('disabled', false)
                .attr('placeholder', 'Si se deja vacio: REC-{Num guia despacho}');
        }

        // Cargar detalle
        try {
            const resp = await $.getJSON(urlObtenerDespacho, { id: id });
            if (!resp || !resp.Ok || !resp.Data) {
                swal({ text: (resp && resp.Mensaje) || 'No se pudo cargar la guía.', icon: 'warning' });
                return;
            }

            // Renderiza tabla dentro del modal (agrega un contenedor en tu modal: <div id="mDetalle"></div>)
            renderDetalleConfirmacion(resp.Data);
            new bootstrap.Modal(document.getElementById('modalConfirmar')).show();
        } catch (err) {
            swal({ text: swalText(err, 'Error al obtener la guía.'), icon: 'error' });
        }
    }

    function renderDetalleConfirmacion(gd) {
        // Por si por error nos pasan el envoltorio {Ok, Data}
        if (gd && gd.Data) gd = gd.Data;

        const detalles = (gd && gd.Detalles) || [];
        var html = [];

        html.push(
            '<div class="table-responsive"><table class="table table-sm table-bordered text-nowrap border-bottom w-100">',
            '<thead class="thead-light"><tr>',
            '<th class="text-center">Seleccionar</th>',
            '<th>Cód. producto</th>',
            '<th>Descripción</th>',
            '<th>N° serie</th>',
            '<th>Est. Stock</th>', 
            '<th class="text-end">Enviado</th>',
            '<th class="text-end">Recibido</th>',
            '<th class="text-end">Pendiente</th>',
            '<th class="text-end">A confirmar</th>',
            '</tr></thead><tbody>'
        );

        detalles.forEach(d => {
            var conf = d.CantidadConfirmada || 0;
            var cant = d.Cantidad || 0;
            var pend = Math.max(0, cant - conf);
            var esSer = (typeof d.EsSerializable === 'boolean') ? d.EsSerializable : !!(d.NumSerie && (d.NumSerie + '').trim());

            var sel, input;
            if (esSer) {
                sel = '<input type="checkbox" class="chkConf" data-id="' + d.Id + '" ' + (pend > 0 ? '' : 'disabled') + ' />';
                input = '<span class="text-muted">1</span>';
            } else {
                sel = '<input type="checkbox" class="chkConf chk-ns" data-id="' + d.Id + '" ' + (pend > 0 ? '' : 'disabled') + ' />';
                input = '<input type="number" min="1" max="' + pend + '" value="' + Math.min(1, pend) + '" class="form-control form-control-sm inpCant" data-id="' + d.Id + '" ' + (pend > 0 ? '' : 'disabled') + ' />';
            }

            var stkEstado = (d.StkEstado || '').toString().toUpperCase();

            const selEstadoHtml = `
                <select class="form-select form-select-sm selEstado select2-show-search" style="width:100%">
                  <option value="">Seleccionar</option>
                  <option value="NUEVO" ${stkEstado === 'NUEVO' ? 'selected' : ''}>NUEVO</option>
                  <option value="USADO" ${stkEstado === 'USADO' ? 'selected' : ''}>USADO</option>
                </select>`;

            html.push(
                '<tr data-producto="', (d.CodProducto || ''),
                '" data-serializable="', (esSer ? 'true' : 'false'),
                '" data-pend="', pend, '">',
                '<td class="text-center">', sel, '</td>',
                '<td>', (d.CodProducto || ''), '</td>',
                '<td>', (d.DesProducto || ''), '</td>',
                '<td class="cel-serie">', (esSer ? (d.NumSerie || '') : ''), '</td>',
                `<td class="select2-sm">${selEstadoHtml}</td>`,
                '<td class="text-end">', cant, '</td>',
                '<td class="text-end">', conf, '</td>',
                '<td class="text-end">', pend, '</td>',
                '<td class="text-end">', input, '</td>',
                '</tr>'
            );

            
        });

        html.push('</tbody></table></div>');

        $('#mDetalle').remove(); // limpia anterior
        $('#modalConfirmar .modal-body').append('<div id="mDetalle" class="mt-3"></div>');
        $('#mDetalle').html(html.join(''));

        $('#mDetalle .selEstado').select2({
            dropdownParent: $('#modalConfirmar'),
            width: '100%',
            minimumResultsForSearch: 0,
            placeholder: 'Seleccionar'
        });

    }
    function initCombosFijos() {

        $('#fTipoMov').select2({
            width: '100%',
            placeholder: 'Todos',
            allowClear: true,
            minimumResultsForSearch: 0
        });
    }
    
    return {
        init: function () {
            checkSession(async function () {
                eventos();
                initCombosFijos();
                await cargarComboEmpresas();
                visualizarDataTable();             
            });
        }
    };
})(jQuery);
