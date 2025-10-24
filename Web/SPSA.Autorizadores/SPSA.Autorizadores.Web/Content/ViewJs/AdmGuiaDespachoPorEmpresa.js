var urlListarDespachos = baseUrl + 'Inventario/GuiaDespacho/ListarPaginado';
var urlRegistrarDespacho = baseUrl + 'Inventario/GuiaDespacho/Registrar';
var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProductos = baseUrl + 'Inventario/Productos/Listar';
var urlListarSeriesDisponibles = baseUrl + 'Inventario/SeriesProducto/ListarPorProductoDisponibles';
var urlObtenerDespacho = baseUrl + 'Inventario/GuiaDespacho/Obtener';
var urlObtenerSerieProducto = baseUrl + 'Inventario/SeriesProducto/Obtener';

var AdmGuiaDespachoPorEmpresa = (function ($) {

    // ======================== Estado ========================
    var _productosCache = [];   // {CodProducto, DesProducto, NomMarca, NomModelo, IndSerializable('S'|'N')}
    var dt = null;              // DataTable cache

    // ================== Eventos ==================
    const eventos = function () {
        // Nueva guía
        $('#btnNuevaGuia').on('click', async function () {
            limpiarModal();
            await Promise.all([cargarComboEmpresasModal(), ensureProductos()]);
            addDetalleRow();
            new bootstrap.Modal(document.getElementById('modalDespacho')).show();
        });

        // Dependientes (empresa → locales)
        $('#desEmpDest').on('change', function () { cargarComboLocalesModal('#desEmpDest', '#desLocDest'); });
        $('#fEmpDest').on('change', function () { cargarComboLocales('#fEmpDest', '#fLocDest'); });

        // Tipo movimiento
        $('#desTipoMov').on('change', onTipoMovimientoChange);

        // Guardar
        $('#btnGuardarDespacho').on('click', guardarDespacho);

        // Agregar fila detalle
        $('#btnAddItem').on('click', function (e) { e.preventDefault(); addDetalleRow(); });

        // Ver detalle de una guía
        $('#tableDespachos').on('click', '.action-ver', function () {
            const id = $(this).data('id');
            abrirModalDetalleDespacho(id);
        });

        $('#fEmpDest, #fLocDest, #fTipoMov, #fDesde, #fHasta')
            .off('change._desp')
            .on('change._desp', function () {
                if ($.fn.DataTable.isDataTable('#tableDespachos')) {
                    $('#tableDespachos').DataTable().ajax.reload();
                }
            });
    };

    // ================== AJAX helpers ==================
    function listarEmpresas() {
        return $.ajax({ url: urlListarEmpresas, type: 'POST', data: { request: { CodUsuario: '', Busqueda: '' } } });
    }
    function listarLocales(codEmpresa) {
        if (!codEmpresa) return Promise.resolve({ Ok: true, Data: [] });
        return $.ajax({ url: urlListarLocalesPorEmpresa, type: 'POST', data: { request: { CodEmpresa: codEmpresa } } });
    }
    function listarProductos() {
        return $.ajax({ url: urlListarProductos, type: 'POST', data: { request: {} } });
    }
    function listarSeriesPorProducto(codProducto) {
        if (!codProducto) return Promise.resolve({ Ok: true, Data: [] });
        // Si tu endpoint soporta filtros de origen, envíalos; si no, los ignorará.
        return $.ajax({
            url: urlListarSeriesDisponibles,
            type: 'POST',
            data: { request: { CodProducto: codProducto } }
        });
    }

    function obtenerSeriePorProducto(codProducto, numSerie) {
        if (!codProducto || !numSerie) return Promise.resolve({ Ok: false, Mensaje: 'Faltan parámetros' });
        return $.getJSON(urlObtenerSerieProducto, { CodProducto: codProducto, NumSerie: numSerie });
    }

    // ======================== Cargas de combos (pantalla) ========================
    const cargarComboEmpresas = async function () {
        try {
            const response = await listarEmpresas();
            if (response.Ok) {
                var empresas = response.Data.map(e => ({ text: e.NomEmpresa, value: e.CodEmpresa }));
                await cargarCombo($('#fEmpDest'), empresas, { placeholder: 'Todos', todos: true });
                await cargarCombo($('#fLocDest'), [], { placeholder: 'Todos', todos: true });
                $('#fLocDest').prop('disabled', true).trigger('change.select2');
            } else {
                swal({ text: String((response && response.Mensaje) || 'Error al listar empresas'), icon: "error" });
            }
        } catch (error) {
            swal({ text: String((error && (error.Mensaje || error.message || error.responseText || error.statusText)) || 'Error al listar empresas'), icon: "error" });
        }
    };

    async function cargarComboLocales(selEmpresa, selLocal) {
        try {
            const $loc = $(selLocal);
            const codEmpresa = $(selEmpresa).val();

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

    // ======================== Cargas de combos (modal) ========================
    const cargarComboEmpresasModal = async function () {
        try {
            const response = await listarEmpresas();
            if (response.Ok) {
                var empresas = response.Data.map(e => ({ text: e.NomEmpresa, value: e.CodEmpresa }));
                await cargarCombo($('#desEmpDest'), empresas, { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
                await cargarCombo($('#desLocDest'), [], { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
                $('#desLocDest').prop('disabled', true).trigger('change.select2');

                // Fijar empresa desde sesión
                var ce = codEmpresaSession();
                if (ce) {
                    const $emp = $('#desEmpDest');
                    // si por cualquier motivo no vino en el listado, la agregamos para evitar inconsistencia
                    if ($emp.find('option[value="' + ce + '"]').length === 0) {
                        const found = (response.Data || []).find(x => (x.CodEmpresa + '').padStart(2, '0') === ce);
                        $emp.append(new Option(found ? found.NomEmpresa : ce, ce, true, true));
                    } else {
                        $emp.val(ce).trigger('change'); // dispara carga de locales
                    }
                    $emp.prop('disabled', true).trigger('change.select2');

                    // Asegura cargar locales de esa empresa
                    await cargarComboLocalesModal('#desEmpDest', '#desLocDest');
                }

            } else {
                swal({ text: String((response && response.Mensaje) || 'Error al listar empresas'), icon: "error" });
            }
        } catch (error) {
            swal({ text: String((error && (error.Mensaje || error.message || error.responseText || error.statusText)) || 'Error al listar empresas'), icon: "error" });
        }
    };

    async function cargarComboLocalesModal(selEmpresa, selLocal) {
        try {
            const $loc = $(selLocal);
            const codEmpresa = $(selEmpresa).val();

            if (!codEmpresa) {
                await cargarCombo($loc, [], { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
                $loc.prop('disabled', true).trigger('change.select2');
                return;
            }

            const resp = await listarLocales(codEmpresa);
            if (resp.Ok) {
                await cargarCombo($loc,
                    resp.Data.map(l => ({ text: l.NomLocal, value: l.CodLocal })),
                    { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' }
                );
                $loc.prop('disabled', false).trigger('change.select2');
            } else {
                swal({ text: swalText(resp, 'No fue posible listar locales'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' });
        }
    }

    // ======================== Cache de productos ========================
    async function ensureProductos() {
        if (_productosCache.length) return;
        try {
            const resp = await listarProductos();
            if (resp.Ok) {
                _productosCache = resp.Data.map(p => ({
                    CodProducto: p.CodProducto,
                    DesProducto: p.DesProducto,
                    NomMarca: p.NomMarca,
                    NomModelo: p.NomModelo,
                    IndSerializable: (p.IndSerializable || 'N') // ← default seguro: no serializable
                }));
            } else {
                swal({ text: swalText(resp, 'No fue posible listar productos'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar productos'), icon: 'error' });
        }
    }

    // ======================== Detalle (filas) ========================
    function addDetalleRow() {
        const $tb = $('#tblDetalleDespacho tbody');
        const rowId = 'r' + Date.now() + '_' + Math.floor(Math.random() * 1000);

        const tr = $(
            '<tr data-row="' + rowId + '">' +
            '<td class="select2-sm"><select class="form-select form-select-sm selProducto select2-show-search" style="width:100%"></select></td>' +
            '<td class="select2-sm"><select class="form-select form-select-sm selSerie select2-show-search" style="width:100%" disabled><option value=""></option></select></td>' +
            '<td><input type="number" class="form-control form-control-sm inpCantidad" min="1" step="1" value="1" /></td>' +
            '<td><input type="text" class="form-control form-control-sm inpCodActivo text-uppercase" /></td>' +
            '<td class="select2-sm"><select class="form-select form-select-sm selEstadoStock select2-show-search" style="width:100%"></select></td>' +
            '<td class="text-center"><button type="button" class="btn btn-sm btn-link text-danger btnDelRow" title="Quitar"><i class="fe fe-trash-2"></i></button></td>' +
            '</tr>'
        );

        $tb.append(tr);

        const $selProd = tr.find('select.selProducto');
        const $selSerie = tr.find('select.selSerie');
        const $selEstado = tr.find('select.selEstadoStock');

        // productos con data-serializable como atributo (no .data() por cache)
        const prodOptions = _productosCache.map(p => {
            var texto = (p.DesProducto || '');
            if (p.NomMarca) texto += ' - ' + p.NomMarca;
            if (p.NomModelo) texto += ' - ' + p.NomModelo;
            return { text: texto.trim(), value: p.CodProducto, attrs: { 'data-serializable': p.IndSerializable } };
        });

        cargarCombo($selProd, prodOptions, { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
        cargarCombo($selSerie, [], { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
        $selSerie.prop('disabled', true);

        const estOptions = [
            { value: 'NUEVO', text: 'NUEVO' },
            { value: 'USADO', text: 'USADO' }
        ];

        cargarCombo($selEstado, estOptions, { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
        // deshabilitado por defecto y sin valor
        $selEstado.prop('disabled', true).val('').trigger('change');

        // eventos
        $selProd.on('change', function () { onProductoChange(tr); });
        tr.find('.btnDelRow').on('click', function () {
            // destruye select2 para evitar fugas de memoria
            tr.find('select').each(function () {
                if ($.fn.select2 && $(this).hasClass('select2-hidden-accessible')) {
                    $(this).select2('destroy');
                }
            });
            tr.remove();
        });
    }

    async function onProductoChange(tr) {
        const $selProd = tr.find('select.selProducto');
        const $selSerie = tr.find('select.selSerie');
        const $cant = tr.find('.inpCantidad');
        const $selEstado = tr.find('select.selEstadoStock');

        const cod = $selProd.val();
        const ind = ($selProd.find(':selected').attr('data-serializable') || 'N');

        // reset dependientes
        await cargarCombo($selSerie, [], { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
        $selSerie.prop('disabled', true);
        $cant.prop('disabled', false).val('1');

        $selEstado.val('').prop('disabled', !cod).trigger('change');

        if (!cod) return;

        $selEstado.prop('disabled', false);

        if (ind === 'S') {
            // serializable → listar series y fijar cantidad=1
            try {
                const resp = await listarSeriesPorProducto(cod);
                if (resp.Ok) {

                    const series = (resp.Data || []).map(s => ({ text: s.NumSerie, value: s.NumSerie }));
                    await cargarCombo($selSerie, series, { placeholder: 'Seleccionar', dropdownParent: '#modalDespacho' });
                    $selSerie.prop('disabled', false);

                } else {
                    swal({ text: swalText(resp, 'No fue posible listar series disponibles.'), icon: 'warning' });
                }
                $cant.val('1').prop('disabled', true);
            } catch (err) {
                swal({ text: swalText(err, 'Error al listar series'), icon: 'error' });
            }
        } else {
            // no serializable
            $selSerie.prop('disabled', true).val(null).trigger('change');
            $cant.prop('disabled', false);
        }

        $selSerie.off('change.estado').on('change.estado', async function () {  // 👈 elimina select2:select
            const numSerie = $(this).val();
            if (!numSerie) { $selEstado.val('').trigger('change'); return; }
            try {
                const r = await obtenerSeriePorProducto(cod, numSerie);
                const est = (r && r.Ok && r.Data) ? (r.Data.StkEstado) : '';
                if (est === 'NUEVO' || est === 'USADO') {
                    $selEstado.val(est).trigger('change');
                } else {
                    $selEstado.val('').trigger('change');
                }
            } catch {
                $selEstado.val('').trigger('change');
            }
        });
      
    }

    // ================== Guardar ==================
    async function guardarDespacho() {

        // -------- Anti-doble clic (flag + deshabilitar botón) --------
        if (guardarDespacho._busy) return;
        guardarDespacho._busy = true;

        const $btn = $('#btnGuardarDespacho');
        const oldHtml = $btn.html();
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Guardando…');

        // Helper para cortar el flujo mostrando alerta y lanzando excepción controlada
        const fail = (text, icon = 'warning') => {
            swal({ text, icon });
            const e = new Error(text);
            e._swalShown = true;
            throw e;
        };
        try {
            // -------- Cabecera --------
            const tipoMov = ($('#desTipoMov').val() || 'TRANSFERENCIA').toUpperCase();
            const isTransf = (tipoMov === 'TRANSFERENCIA');
            const isBaja = (tipoMov === 'BAJA');

            if (!isTransf && !isBaja) fail('Tipo de movimiento inválido. Solo TRANSFERENCIA o BAJA.');


            const header = {
                NumGuia: ($('#desNumGuia').val() || '').trim(),
                Fecha: $('#desFecha').val(),
                CodEmpresaDestino: isTransf ? ($('#desEmpDest').val() || null) : null,
                CodLocalDestino: isTransf ? ($('#desLocDest').val() || null) : null,
                TipoMovimiento: tipoMov,
                UsarTransitoDestino: isTransf,   // true solo en TRANSFERENCIA
                AreaGestion: ($('#desAreaGestion').val() || '').trim(),
                ClaseStock: ($('#desClaseStock').val() || '').trim(),
                Observaciones: ($('#desObs').val() || '').trim()
            };

            if (!header.NumGuia || !header.Fecha || !header.AreaGestion || !header.ClaseStock) {
                swal({ text: "Complete los campos obligatorios (*) de la cabecera.", icon: "warning" });
                return;
            }

            if (isTransf && (!header.CodEmpresaDestino || !header.CodLocalDestino)) {
                swal({ text: "Destino (Empresa y Local) es obligatorio para TRANSFERENCIA.", icon: "warning" });
                return;
            }

            const items = [];
            const errores = [];
            let fila = 0;
            // evitar series duplicadas (mismo producto) en el mismo payload
            const seriesKeys = new Set();
            const estadosValidos = new Set(['NUEVO', 'USADO']);

            $('#tblDetalleDespacho tbody tr').each(function () {
                fila++;
                const $tr = $(this);
                const $optSel = $tr.find('.selProducto option:selected');
                const $optEstStock = $tr.find('.selEstadoStock option:selected');

                const codProd = $optSel.val();
                const estStock = $optEstStock.val();
                const ind = ($optSel.data('serializable') || 'S');
                const numSerie = ($tr.find('.selSerie').val() || '').trim();
                const cant = parseInt($tr.find('.inpCantidad').val() || '0', 10);
                const codAct = ($tr.find('.inpCodActivo').val() || '').trim();

                if (!codProd) {
                    errores.push(`Fila ${fila}: producto es obligatorio.`);
                    return;
                }

                if (ind === 'S') {
                    if (!numSerie) errores.push(`Fila ${fila}: Debe seleccionar una serie disponible para producto.`);
                    if (cant !== 1) errores.push(`Fila ${fila}: Cantidad debe ser 1 para producto con numeros de serie.`);
                    // duplicidad de serie
                    const key = codProd + '|' + numSerie;
                    if (seriesKeys.has(key)) errores.push(`Fila ${fila}: La serie '${numSerie}' del producto ${codProd} está repetida.`);
                    else seriesKeys.add(key);
                } else {
                    if (!cant || cant <= 0) errores.push(`Fila ${fila}: Cantidad debe ser mayor a 0.`);
                }

                if (!estStock) {
                    errores.push(`Fila ${fila}: Seleccione estado de stock.`);
                } else if (!estadosValidos.has(estStock)) {
                    errores.push(`Fila ${fila}: Estado de stock inválido (${estStock}).`);
                }

                items.push({
                    CodProducto: codProd,
                    NumSerie: (ind === 'S') ? numSerie : null,
                    Cantidad: cant,
                    CodActivo: codAct,
                    StkEstado: estStock,
                    Observaciones: null
                });
            });

            if (items.length === 0) {
                swal({ text: "Agregue al menos un ítem en el detalle.", icon: "warning" });
                return;
            }
            if (errores.length) {
                swal({ text: errores.join('\n'), icon: "warning" });
                return;
            }

            const payload = { Cabecera: header, Detalle: items };

            // -------- POST --------
            if (typeof showLoading === 'function') try { showLoading(); } catch { }

            const resp = await $.ajax({
                url: urlRegistrarDespacho,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (resp.Ok) {
                swal({ text: resp.Mensaje || 'Guía de despacho registrada correctamente.', icon: 'success' });
                // cerrar modal
                const modalEl = document.getElementById('modalDespacho');
                if (modalEl) {
                    const inst = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                    inst.hide();
                }
                // recargar DT usando la variable global dt si existe, si no, fallback
                if (typeof dt !== 'undefined' && dt && typeof dt.ajax === 'object' && typeof dt.ajax.reload === 'function') {
                    dt.ajax.reload(null, false);
                } else {
                    const dtLocal = $('#tableDespachos').data('DataTable') || $('#tableDespachos').DataTable();
                    if (dtLocal) dtLocal.ajax.reload(null, false);
                }
            } else {
                swal({ text: swalText(resp, 'No se pudo registrar.'), icon: 'warning' });
            }
        } catch (err) {
            if (!err || err._swalShown !== true) {
                swal({ text: swalText(err, 'Error al registrar'), icon: 'error' });
            }
        } finally {
            if (typeof closeLoading === 'function') try { closeLoading(); } catch { }
            guardarDespacho._busy = false;
            $btn.prop('disabled', false).html(oldHtml);
        }
    }

    // ================== DataTable ==================
    function visualizarDataTable() {
        dt = $('#tableDespachos').DataTable({
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
                    CodEmpresaDestino: $('#fEmpDest').val() || null,
                    CodLocalDestino: $('#fLocDest').val() || null,
                    TipoMovimiento: $('#fTipoMov').val() || null,
                    FiltroVarios: (data.search.value || '').toUpperCase()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarDespachos,
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
                { data: 'IndEstado', defaultContent: 'REGISTRADA' },
                { data: 'UsuCreacion', defaultContent: '' },
                {
                    data: null,
                    className: 'text-center nowrap',
                    orderable: false,
                    render: function (r) {
                        return '<i class="fe fe-eye fs-6 text-primary action-ver" ' +
                            'data-id="' + (r.Id || '') + '" ' +
                            'style="cursor:pointer;" ' +
                            'title="Ver detalle" aria-label="Ver detalle"></i>';
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
                $('#tableDespachos_filter input').addClass('form-control-sm').attr('placeholder', 'Buscar...');
            },
            scrollY: '500px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100]
        });
    }

    // ======================== Auxiliares UI ========================
    function onTipoMovimientoChange() {
        var tipo = ($('#desTipoMov').val() || 'TRANSFERENCIA').toUpperCase();
        var isTransf = (tipo === 'TRANSFERENCIA');

        var ce = codEmpresaSession();

        // Empresa destino: si hay empresa de sesión, siempre fija y bloqueada
        if (ce) {
            $('#desEmpDest').val(ce).prop('disabled', true).trigger('change.select2');
        } else {
            // sin sesión: solo habilita en transferencia
            $('#desEmpDest').prop('disabled', !isTransf);
            if (!isTransf) $('#desEmpDest').val(null).trigger('change');
        }

        // Local destino: activo solo en transferencia
        $('#desLocDest').prop('disabled', !isTransf);
        if (!isTransf) {
            $('#desLocDest').val(null).trigger('change');
        } else if (ce) {
            // si hay empresa sesión y estamos en transferencia, asegúrate de listar locales
            cargarComboLocalesModal('#desEmpDest', '#desLocDest');
        }

        // Solo TRANSFERENCIA usa destino
        //$('#desEmpDest').prop('disabled', !isTransf).val(null).trigger('change');
        //$('#desLocDest').prop('disabled', !isTransf).val(null).trigger('change');
    }

    function limpiarModal() {
        $('#modalDespacho  input').val('');
        $('#desTipoMov, #desAreaGestion, #desClaseStock').val(null).trigger('change');
        $('#desFecha').val(new Date().toISOString().slice(0, 10));
        $('#desTipoMov').val('TRANSFERENCIA').trigger('change');

        //$('#desEmpDest').val(null).trigger('change');

        // Empresa destino: si hay en sesión, no la limpies aquí
        var ce = codEmpresaSession();
        if (ce) {
            $('#desEmpDest').val(ce).prop('disabled', true).trigger('change.select2');
        } else {
            $('#desEmpDest').val(null).prop('disabled', false).trigger('change');
        }


        $('#desLocDest').empty().append('<option></option>').val(null).trigger('change');
        $('#desLocDest').prop('disabled', true).trigger('change.select2');

        $('#tblDetalleDespacho tbody').empty();
    }

    function initCombosFijos() {
        // Selects fijos del modal (no dependientes)
        $('#desTipoMov').select2({
            dropdownParent: $('#modalDespacho'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        $('#desAreaGestion').select2({
            dropdownParent: $('#modalDespacho'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        $('#desClaseStock').select2({
            dropdownParent: $('#modalDespacho'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        // Filtro "Tipo" en pantalla
        $('#fTipoMov').select2({
            width: '100%',
            placeholder: 'Todos',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        // Atajos: Enter para buscar
        $('#txtFiltro, #fDesde, #fHasta').on('keyup', function (e) {
            if (e.key === 'Enter') $('#btnBuscar').click();
        });
    }

    function parseDotNetDate(value) {
        if (!value) return '';
        var m = /\/Date\((\d+)\)\//.exec(value + '');
        if (m) {
            var d = new Date(parseInt(m[1], 10));
            return d.toLocaleDateString('es-PE');
        }
        return (value + '').substring(0, 10);
    }

    async function abrirModalDetalleDespacho(id) {
        // limpia/placeholder
        $('#detalleDespachoBody').html('<div class="text-muted">Cargando detalle…</div>');
        const $modalEl = document.getElementById('modalDetalleDespacho');
        const modal = bootstrap.Modal.getInstance($modalEl) || new bootstrap.Modal($modalEl);
        modal.show();

        try {
            const resp = await $.getJSON(urlObtenerDespacho, { id: id });
            if (!resp || !resp.Ok || !resp.Data) {
                $('#detalleDespachoBody').html('<div class="text-danger">No se pudo cargar el detalle.</div>');
                return;
            }
            const html = detalleDespachoTemplate(resp.Data);
            $('#detalleDespachoBody').html(html);
        } catch (err) {
            $('#detalleDespachoBody').html('<div class="text-danger">' + (swalText(err, 'Error al obtener el detalle.') || 'Error') + '</div>');
        }
    }

    function safe(v) { return (v == null ? '' : (v + '')); }

    function detalleDespachoTemplate(gd) {
        // por si viniera envuelto
        if (gd && gd.Data) gd = gd.Data;

        const fecha = parseDotNetDate(gd.Fecha);
        const detalles = Array.isArray(gd.Detalles) ? gd.Detalles : [];

        const rows = detalles.map(d => {
            return `
      <tr>
        <td>${safe(d.CodProducto)}</td>
        <td>${safe(d.DesProducto)}</td>
        <td>${safe(d.NumSerie)}</td>
        <td class="text-end">${safe(d.Cantidad)}</td>
        <td>${safe(d.CodActivo)}</td>
        <td>${safe(d.StkEstado)}</td>
      </tr>`;
        }).join('');

        return `
<div class="detalle-despacho">
    <div class="mb-3">
        <div class="row g-2">
            <div class="col-md-3"><b>N° Guía:</b> ${safe(gd.NumGuia)}</div>
            <div class="col-md-3"><b>Fecha:</b> ${fecha}</div>
            <div class="col-md-3"><b>Tipo:</b> ${safe(gd.TipoMovimiento)}</div>
            <div class="col-md-3"><b>Estado:</b> ${safe(gd.IndEstado || 'REGISTRADA')}</div>

            <div class="col-md-3"><b>Origen:</b> ${safe(gd.NomLocalOrigen)}</div>
            <div class="col-md-3"><b>Destino:</b> ${safe(gd.NomLocalDestino)}</div>
            <div class="col-md-3"><b>Área Gestión:</b> ${safe(gd.AreaGestion)}</div>
            <div class="col-md-3"><b>Clase Stock:</b> ${safe(gd.ClaseStock)}</div>

            <div class="col-md-3"><b>Recepción Destino:</b> ${safe(gd.IndConfirmacion)}</div>
            <div class="col-md-9"><b>Observaciones:</b> ${safe(gd.Observaciones)}</div>
        </div>
    </div>

    <div class="table-responsive">
        <table class="table table-bordered w-100">
            <thead class="thead-light">
                <tr>
                    <th>Código</th>
                    <th>Producto</th>
                    <th>Serie</th>
                    <th class="text-end">Cantidad</th>
                    <th>Cód. Activo</th>
                    <th>Est. Stock</th>
                </tr>
            </thead>
            <tbody>
                ${rows || '<tr><td colspan="6" class="text-muted">Sin detalle.</td></tr>'}
            </tbody>
        </table>
    </div>
</div>
`;
    }

    function codEmpresaSession() {
        // Asegura string y (opcional) relleno a 2 dígitos
        return (typeof codigoEmpresa !== 'undefined' && codigoEmpresa != null && codigoEmpresa !== '')
            ? (codigoEmpresa + '').padStart(2, '0')
            : null;
    }


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                initCombosFijos();
                await cargarComboEmpresas();
                await cargarComboEmpresasModal();
                visualizarDataTable();
            });
        }
    };
})(jQuery);
