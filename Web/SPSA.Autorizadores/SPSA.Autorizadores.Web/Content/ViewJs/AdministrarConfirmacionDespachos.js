var urlListarDespachosPendientes = baseUrl + 'Inventario/GuiaDespacho/ListarPaginadoConfirmacion';
var urlObtenerDespacho = baseUrl + 'Inventario/GuiaDespacho/Obtener';           // { id }
var urlConfirmarDespacho = baseUrl + 'Inventario/GuiaDespacho/ConfirmarEnDestino';// POST

var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';

var AdministrarConfirmacionDespachos = (function ($) {

    function initSelect2EnModal(target, opts) {
        var $els = (target && target.jquery) ? target : $(target);
        if (!$els.length) return;
        var $modal = $els.closest('.modal');
        if (!$modal.length) $modal = $('#modalConfirmar');
        $els.each(function () {
            var $s = $(this);
            if ($s.hasClass('select2-hidden-accessible')) $s.select2('destroy');
            var $opt0 = $s.find('option').first();
            var placeholder = ($opt0.attr('label') || $opt0.text() || 'Seleccionar…');
            $s.select2($.extend(true, {
                dropdownParent: $modal,
                width: '100%',
                placeholder: placeholder,
                allowClear: true,
                minimumResultsForSearch: 0
            }, opts || {}));
        });
    }

    function swalText(err, fallback) {
        if (!err) return fallback || '';
        if (typeof err === 'string') return err;
        if (err.responseText) return err.responseText;
        if (err.statusText) return err.statusText;
        if (err.Mensaje) return err.Mensaje;
        try { return JSON.stringify(err); } catch { return fallback || ''; }
    }

    // ========= Combos de filtros =========
    function listarEmpresas() {
        return $.ajax({ url: urlListarEmpresas, type: 'POST', data: { request: { CodUsuario: '', Busqueda: '' } } });
    }
    function listarLocales(codEmpresa) {
        if (!codEmpresa) return Promise.resolve({ Ok: true, Data: [] });
        return $.ajax({ url: urlListarLocalesPorEmpresa, type: 'POST', data: { request: { CodEmpresa: codEmpresa } } });
    }
    async function cargarComboEmpresas(selector) {
        try {
            const resp = await listarEmpresas();
            const $sel = $(selector);
            $sel.empty().append('<option></option>');
            if (resp.Ok) resp.Data.forEach(e => $sel.append(new Option(e.NomEmpresa, e.CodEmpresa)));
            else swal({ text: swalText(resp, 'No fue posible listar empresas'), icon: 'error' });
        } catch (err) { swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' }); }
    }
    async function cargarComboLocales(selEmpresa, selLocal) {
        try {
            const codEmpresa = $(selEmpresa).val();
            const resp = await listarLocales(codEmpresa);
            const $loc = $(selLocal);
            $loc.empty().append('<option></option>');
            if (resp.Ok) resp.Data.forEach(l => $loc.append(new Option(l.NomLocal, l.CodLocal)));
            else swal({ text: swalText(resp, 'No fue posible listar locales'), icon: 'error' });
            $loc.val(null).trigger('change');
        } catch (err) { swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' }); }
    }

    // ========= DataTable =========
    var dt, seleccionados = new Set();

    function detalleTemplate(row) {
        // Render detalle (se asume que backend devuelve campos: Detalles: [{CodProducto, NumSerie, Cantidad, CodActivo}])
        if (!row || !row.Detalles || !row.Detalles.length) return '<div class="p-2 text-muted">Sin detalle.</div>';
        var html = ['<div class="p-2">'];
        html.push('<table class="table table-sm table-bordered mb-0 w-100"><thead><tr>',
            '<th>Producto</th><th>Serie</th><th class="text-end">Cant.</th><th>Cód. Activo</th>',
            '</tr></thead><tbody>');
        row.Detalles.forEach(d => {
            html.push('<tr>',
                '<td>', (d.CodProducto || ''), '</td>',
                '<td>', (d.NumSerie || ''), '</td>',
                '<td class="text-end">', (d.Cantidad || 0), '</td>',
                '<td>', (d.CodActivo || ''), '</td>',
                '</tr>');
        });
        html.push('</tbody></table></div>');
        return html.join('');
    }

    function visualizarDataTable() {
        dt = $('#tablePendientes').DataTable({
            serverSide: true,
            processing: true,
            searching: false,
            ordering: false,
            ajax: function (data, callback) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    FechaDesde: $('#fDesde').val() || null,
                    FechaHasta: $('#fHasta').val() || null,
                    CodEmpresaDestino: $('#fEmpDest').val() || null,
                    CodLocalDestino: $('#fLocDest').val() || null,
                    TipoMovimiento: $('#fTipoMov').val() || null,
                    IndEstado: 'PENDIENTE_CONFIRMACION' // o EN_TRANSITO / PENDIENTE_CONFIRMACION según tu backend
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
                            // Esperamos Items con: Id, Fecha, NumGuia, CodEmpresaOrigen, CodLocalOrigen,
                            // CodEmpresaDestino, CodLocalDestino, TipoMovimiento, Items, IndEstado, Detalles[]
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
                    data: 'Id', className: 'dt-center', render: function (id, t, r) {
                        var checked = seleccionados.has(id) ? 'checked' : '';
                        return '<input type="checkbox" class="chkRow" data-id="' + id + '" ' + checked + ' />';
                    }, orderable: false
                },
                {
                    data: null, className: 'details-control', defaultContent: ''
                },
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
                {
                    data: null, render: function (r) {
                        return (r.CodEmpresaOrigen || '') + '-' + (r.CodLocalOrigen || '');
                    }
                },
                {
                    data: null, render: function (r) {
                        return (r.CodEmpresaDestino || '') + '-' + (r.CodLocalDestino || '');
                    }
                },
                { data: 'TipoMovimiento', defaultContent: '' },
                { data: 'Items', defaultContent: '' },
                { data: 'IndEstado', defaultContent: '' },
                {
                    data: null, className: 'dt-center nowrap', render: function (r) {
                        return '' +
                            '<button type="button" class="btn btn-sm btn-success btnConfirmarUno" data-id="' + r.Id + '">' +
                            '<i class="fe fe-check me-1"></i>Confirmar</button>';
                    }
                }
            ],
            language: {
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)"
            },
            scrollY: '500px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
            drawCallback: function () {
                actualizarSeleccionContador();
            }
        });

        // Toggle detalle
        $('#tablePendientes tbody').on('click', 'td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = dt.row(tr);

            if (row.child.isShown()) {
                row.child.hide();
                tr.removeClass('shown');
            } else {
                var data = row.data();
                row.child(detalleTemplate(data)).show();
                tr.addClass('shown');
            }
        });

        // Selección por fila
        $('#tablePendientes tbody').on('change', '.chkRow', function () {
            var id = parseInt($(this).data('id'), 10);
            if ($(this).is(':checked')) seleccionados.add(id);
            else seleccionados.delete(id);
            actualizarSeleccionContador();
        });

        // Seleccionar todo
        $('#chkAll').on('change', function () {
            var check = $(this).is(':checked');
            $('#tablePendientes tbody .chkRow').each(function () {
                var id = parseInt($(this).data('id'), 10);
                $(this).prop('checked', check);
                if (check) seleccionados.add(id); else seleccionados.delete(id);
            });
            actualizarSeleccionContador();
        });

        // Confirmar una guía (botón por fila)
        $('#tablePendientes tbody').on('click', '.btnConfirmarUno', function () {
            var id = $(this).data('id');
            abrirModalConfirmacion(id);
        });
    }

    function actualizarSeleccionContador() {
        $('#lblSel').text(seleccionados.size + ' seleccionados');
    }

    // ========= Modal Confirmación =========
    function abrirModalConfirmacion(id) {
        $('#hidGuiaId').val(id);
        $('#mFecha').val(new Date().toISOString().slice(0, 10));
        $('#mNumGR').val('');
        $('#mObs').val('');
        $('#mGenerarGR').prop('checked', true);
        new bootstrap.Modal(document.getElementById('modalConfirmar')).show();
    }

    async function confirmarUno(id, fecha, numGR, obs, generarGR) {
        // si numGR vacío, backend generará "REC-{NumGuia}"
        try {
            const payload = {
                GuiaDespachoId: id,
                NumGuiaRecepcion: (numGR || null),
                Fecha: fecha,
                Observaciones: (obs || null),
                GenerarGuiaRecepcion: !!generarGR
            };
            const resp = await $.ajax({
                url: urlConfirmarDespacho,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });
            return resp;
        } catch (err) {
            return { Ok: false, Mensaje: swalText(err, 'Error al confirmar.') };
        }
    }

    // ========= Eventos =========
    function eventos() {
        $('#btnBuscar').on('click', function (e) {
            e.preventDefault();
            dt.ajax.reload();
        });

        $('#fEmpDest').on('change', function () { cargarComboLocales('#fEmpDest', '#fLocDest'); });

        // Confirmación desde modal (1 guía)
        $('#btnConfirmarModal').on('click', async function () {
            const id = parseInt($('#hidGuiaId').val() || '0', 10);
            const fecha = $('#mFecha').val();
            if (!id || !fecha) {
                swal({ text: "Complete la fecha de recepción.", icon: "warning" }); return;
            }
            const numGR = ($('#mNumGR').val() || '').trim();
            const obs = ($('#mObs').val() || '').trim();
            const genGR = $('#mGenerarGR').is(':checked');

            const r = await confirmarUno(id, fecha, numGR, obs, genGR);
            if (r && r.Ok) {
                swal({ text: r.Mensaje || 'Confirmado correctamente.', icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalConfirmar')).hide();
                dt.ajax.reload(null, false);
            } else {
                swal({ text: swalText(r, 'No se pudo confirmar.'), icon: 'warning' });
            }
        });

        // Confirmación MASIVA (guías seleccionadas)
        $('#btnConfirmarSeleccion').on('click', async function () {
            if (seleccionados.size === 0) {
                swal({ text: "Seleccione al menos una guía para confirmar.", icon: "warning" });
                return;
            }

            // Parámetros comunes (puedes abrir un modal masivo si prefieres)
            const fecha = new Date().toISOString().slice(0, 10);
            const obs = '';
            const genGR = true;

            const ids = Array.from(seleccionados);
            let okCount = 0, failCount = 0, msgs = [];

            for (let i = 0; i < ids.length; i++) {
                // numGR vacío => backend genera REC-{NumGuia}
                const r = await confirmarUno(ids[i], fecha, '', obs, genGR);
                if (r && r.Ok) okCount++; else { failCount++; msgs.push((r && r.Mensaje) || ('Id ' + ids[i] + ' falló')); }
            }

            seleccionados.clear();
            $('#chkAll').prop('checked', false);
            dt.ajax.reload(null, false);

            const msg = `Confirmaciones OK: ${okCount}. Errores: ${failCount}.` + (msgs.length ? '\n' + msgs.join('\n') : '');
            swal({ text: msg, icon: (failCount ? 'warning' : 'success') });
        });
    }

    return {
        init: function () {
            checkSession(async function () {
                // combos
                await cargarComboEmpresas('#fEmpDest');
                await cargarComboLocales('#fEmpDest', '#fLocDest');
                // init select2 en filtros
                $('#fEmpDest,#fLocDest,#fTipoMov').each(function () {
                    $(this).select2({ width: '100%', placeholder: 'Todos' });
                });
                visualizarDataTable();
                eventos();
            });
        }
    };
})(jQuery);
