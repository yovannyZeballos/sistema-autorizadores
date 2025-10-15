var urlListarRecepciones = baseUrl + 'Inventario/GuiaRecepcion/ListarPaginado';
var urlRegistrarRecepcion = baseUrl + 'Inventario/GuiaRecepcion/Registrar';
var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProveedores = baseUrl + 'Inventario/Proveedores/Listar';
var urlListarProductos = baseUrl + 'Inventario/Productos/Listar';
var urlListarSeriesDisponibles = baseUrl + 'Inventario/SeriesProducto/ListarPorProducto';
var urlObtenerGuiaRecepcion = baseUrl + 'Inventario/GuiaRecepcion/Obtener';

var AdministrarGuiaRecepcion = (function ($) {

    // ======================== Estado ========================
    var _productosCache = [];   // {CodProducto, DesProducto, NomMarca, NomModelo, IndSerializable('S'|'N')}
    var dt = null;              // DataTable cache


    // Select2: render y búsqueda para Productos (Des — Marca — Modelo)
    function s2ProductoOptions() {
        function tpl(item) {
            if (!item.id) return item.text;
            const $el = $(item.element);
            const des = $el.data('des') || '';
            const marca = $el.data('marca') || '';
            const modelo = $el.data('modelo') || '';
            const linea = [des, marca, modelo].filter(Boolean).join(' — ');
            return `<div><div>${linea}</div>${(marca || modelo) ? `<small class="text-muted">${[marca, modelo].filter(Boolean).join(' · ')}</small>` : ''}</div>`;
        }
        function matcher(params, data) {
            if ($.trim(params.term) === '') return data;
            if (typeof data.text !== 'string') return null;
            const term = params.term.toLowerCase();
            const $el = $(data.element);
            const hay = s => (s || '').toString().toLowerCase().includes(term);
            return (hay(data.text) || hay($el.data('des')) || hay($el.data('marca')) || hay($el.data('modelo'))) ? data : null;
        }
        return {
            escapeMarkup: m => m,
            templateResult: tpl,
            templateSelection: tpl,
            matcher,
            minimumInputLength: 1
        };
    }

    const eventos = function () {

        // Buscar
        $('#btnBuscar').on('click', function (e) {
            e.preventDefault();
            if (dt) dt.ajax.reload();
        });

        // Nueva guía
        $('#btnNuevaGuia').on('click', async function () {
            limpiarModal();
            await Promise.all([cargarComboEmpresas(), cargarComboProveedores(), ensureProductos()]);
            addDetalleRow();
            new bootstrap.Modal(document.getElementById('modalRecepcion')).show();
        });

        // Empresa -> Local (deshabilitado hasta elegir empresa)
        $('#fEmpOri').on('change', function () {
            cargarComboLocales('#fEmpOri', '#fLocOri');
        });

        $('#recEmpDest').on('change', function () {
            cargarComboLocales('#recEmpDest', '#recLocDest');
        });

        // Guardar (con anti doble clic)
        $('#btnGuardarRecepcion').on('click', guardarRecepcion);

        // Agregar fila detalle
        $('#btnAddItem').on('click', function (e) { e.preventDefault(); addDetalleRow(); });

        // Al abrir modal, inicializar selects fijos del modal (si existen)
        $('#modalRecepcion').on('shown.bs.modal', function () {
            const fixed = ['#recAreaGestion', '#recClaseStock', '#recEstadoStock', '#recProveedor', '#recEmpDest', '#recLocDest'];
            fixed.forEach(sel => {
                if ($(sel).length && $.fn.select2 && !$(sel).hasClass('select2-hidden-accessible')) {
                    $(sel).select2({
                        width: '100%',
                        placeholder: $(sel).find('option').first().text() || 'Seleccionar…',
                        allowClear: true,
                        minimumResultsForSearch: 0,
                        dropdownParent: $('#modalRecepcion')
                    });
                }
            });
        });

        // Ver detalle de una guía
        $('#tableRecepciones').on('click', '.action-ver', function () {
            const id = $(this).data('id');
            abrirModalDetalleRecepcion(id);
        });

        $('#fEmpOri, #fLocOri, #fProveedor, #fDesde, #fHasta')
            .off('change._rec')
            .on('change._rec', function () {
                if (dt) {
                    dt.ajax.reload();
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
    function listarProveedores() {
        return $.ajax({ url: urlListarProveedores, type: 'POST', data: { request: { Busqueda: '' } } });
    }
    function listarProductos() {
        return $.ajax({ url: urlListarProductos, type: 'POST', data: { request: {} } });
    }
    function listarSeriesPorProducto(codProducto) {
        if (!codProducto) return Promise.resolve({ Ok: true, Data: [] });
        return $.ajax({ url: urlListarSeriesDisponibles, type: 'POST', data: { request: { CodProducto: codProducto } } });
    }

    // ======================== Cargas de combos (pantalla) ========================
    async function cargarComboEmpresas() {
        const $emp = $('#fEmpOri');
        const $loc = $('#fLocOri');
        if (!$emp.length || !$loc.length) return; // por si no existen en esta vista

        try {
            const resp = await listarEmpresas();
            if (resp.Ok) {
                const empresas = resp.Data.map(e => ({ text: e.NomEmpresa, value: e.CodEmpresa }));
                await cargarCombo($emp, empresas, { placeholder: 'Todos', todos: true });
                await cargarCombo($loc, [], { placeholder: 'Todos', todos: true });
                $loc.prop('disabled', true).trigger('change.select2');
            } else {
                swal({ text: swalText(resp, 'No fue posible listar empresas'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' });
        }
    }

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

    async function cargarComboEmpresasModal() {
        const $emp = $('#fEmpOri');
        const $loc = $('#fLocOri');
        if (!$emp.length || !$loc.length) return; // por si no existen en esta vista

        try {
            const resp = await listarEmpresas();
            if (resp.Ok) {
                const empresas = resp.Data.map(e => ({ text: e.NomEmpresa, value: e.CodEmpresa }));
                await cargarCombo($emp, empresas, { placeholder: 'Todos', todos: false, dropdownParent: '#modalRecepcion' });
                await cargarCombo($loc, [], { placeholder: 'Todos', todos: false, dropdownParent: '#modalRecepcion' });
                $loc.prop('disabled', true).trigger('change.select2');
            } else {
                swal({ text: swalText(resp, 'No fue posible listar empresas'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' });
        }
    }

    async function cargarComboLocalesModal(selEmpresa, selLocal) {
        const $emp = $(selEmpresa), $loc = $(selLocal);
        if (!$emp.length || !$loc.length) return;

        try {
            const codEmpresa = $emp.val();
            if (!codEmpresa) {
                await cargarCombo($loc, [], { placeholder: 'Seleccionar…', todos: false, dropdownParent: '#modalRecepcion' });
                $loc.prop('disabled', true).trigger('change.select2');
                return;
            }
            const resp = await listarLocales(codEmpresa);
            if (resp.Ok) {
                await cargarCombo($loc,
                    resp.Data.map(l => ({ text: l.NomLocal, value: l.CodLocal })),
                    { placeholder: 'Seleccionar…', todos: false, dropdownParent: '#modalRecepcion' }
                );
                $loc.prop('disabled', false).trigger('change.select2');
            } else {
                swal({ text: swalText(resp, 'No fue posible listar locales'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' });
        }
    }

    async function cargarComboProveedores() {
        const $prove = $('#recProveedor');
        if (!$prove.length) return;

        try {
            const resp = await listarProveedores();
            if (resp.Ok) {
                await cargarCombo($prove,
                    resp.Data.map(p => ({
                        text: p.RazonSocial,
                        value: p.Ruc,
                        attrs: { 'data-ruc': p.Ruc, 'data-raz': p.RazonSocial }
                    })),
                    { placeholder: 'Seleccionar…', todos: false, dropdownParent: '#modalRecepcion' }
                );
            } else {
                swal({ text: swalText(resp, 'No fue posible listar proveedores'), icon: 'error' });
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar proveedores'), icon: 'error' }); }
    }

    async function cargarFiltroProveedores() {
        try {
            const resp = await listarProveedores();
            const $cbo = $('#fProveedor');        // <-- FALTABA
            $cbo.empty().append('<option value="">Todos</option>');

            if (resp.Ok) {
                resp.Data.forEach(p => {
                    $cbo.append(new Option(p.RazonSocial, p.Ruc));
                });
            } else {
                swal({ text: swalText(resp, 'No fue posible listar proveedores'), icon: 'error' });
            }

            if ($.fn.select2) {
                $cbo.select2({
                    width: '100%',
                    placeholder: 'Todos',
                    allowClear: true,
                    minimumResultsForSearch: 0
                });
            }

            // recargar tabla al cambiar filtro
            $cbo.off('change._rec').on('change._rec', function () {
                if ($.fn.DataTable.isDataTable('#tableRecepciones')) {
                    $('#tableRecepciones').DataTable().ajax.reload();
                }
            });

        } catch (err) {
            swal({ text: swalText(err, 'Error al listar proveedores'), icon: 'error' });
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


    // ================== Detalle (filas) ==================
    function addDetalleRow() {
        const $tb = $('#tblDetalleRecepcion tbody');
        const rowId = 'r' + Date.now() + '_' + Math.floor(Math.random() * 1000);

        const tr = $(
            '<tr data-row="' + rowId + '">' +
            '  <td class="select2-sm"><select class="form-select form-select-sm selProducto select2-show-search" style="width:100%"></select></td>' +
            '  <td><input type="text" class="form-control form-control-sm inpSerie text-uppercase" placeholder="N° serie (si aplica)" /></td>' +
            '  <td><input type="number" class="form-control form-control-sm inpCantidad" min="1" step="1" value="1" /></td>' +
            '  <td><input type="text" class="form-control form-control-sm inpCodActivo text-uppercase" /></td>' +
            '  <td class="text-center"><button type="button" class="btn btn-sm btn-link text-danger btnDelRow" title="Quitar"><i class="fe fe-trash-2"></i></button></td>' +
            '</tr>'
        );
        $tb.append(tr);

        const $selProd = tr.find('select.selProducto');

        // Opciones de producto con data-* para mejor búsqueda
        const prodOptions = _productosCache.map(p => ({
            value: p.CodProducto,
            text: [p.DesProducto, p.NomMarca, p.NomModelo].filter(Boolean).join(' — '), // fallback
            attrs: {
                'data-des': (p.DesProducto || ''),
                'data-marca': (p.NomMarca || ''),
                'data-modelo': (p.NomModelo || ''),
                'data-serializable': (p.IndSerializable || 'S')
            }
        }));

        cargarCombo($selProd, prodOptions, {
            placeholder: 'Buscar producto…',
            dropdownParent: '#modalRecepcion',
            select2Options: s2ProductoOptions()
        });

        // eventos de fila
        $selProd.on('change', function () { onProductoChange(tr); });
        tr.find('.btnDelRow').on('click', function () { tr.remove(); });
    }

    async function onProductoChange(tr) {
        const $selProd = tr.find('select.selProducto');
        const $serie = tr.find('.inpSerie');
        const $cant = tr.find('.inpCantidad');

        const cod = $selProd.val();
        const ind = ($selProd.find(':selected').data('serializable') || 'S'); // 'S'|'N'

        $serie.val('');
        if (!cod) {
            $serie.prop('disabled', true).attr('placeholder', 'N° serie (si aplica)');
            $cant.prop('disabled', false).val('1');
            return;
        }

        if (ind === 'S') {
            $serie.prop('disabled', false).attr('placeholder', 'Ingrese N° de serie');
            $cant.val('1').prop('disabled', true);
        } else {
            $serie.val('').prop('disabled', true).attr('placeholder', '(no serie)');
            $cant.prop('disabled', false);
        }
    }


    // ================== Guardar (anti doble clic) ==================
    async function guardarRecepcion() {
        const $btn = $('#btnGuardarRecepcion');
        const oldHtml = $btn.html();
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Guardando…');

        try {
            // cabecera
            const header = {
                NumGuia: ($('#recNumGuia').val() || '').trim(),
                OrdenCompra: ($('#recOrdenCompra').val() || '').trim(),
                Fecha: $('#recFecha').val(),
                ProveedorRuc: $('#recProveedor').val() || null,
                AreaGestion: ($('#recAreaGestion').val() || '').trim(),
                ClaseStock: ($('#recClaseStock').val() || '').trim(),
                EstadoStock: ($('#recEstadoStock').val() || '').trim(),
                IndTransferencia: 'N',
                Observaciones: ($('#recObs').val() || '').trim()
            };

            if (!header.NumGuia || !header.ProveedorRuc || !header.Fecha || !header.AreaGestion || !header.ClaseStock || !header.EstadoStock) {
                swal({ text: "Complete los campos obligatorios (*) de la cabecera.", icon: "warning" });
                return;
            }

            // detalle
            const items = [];
            const errores = [];
            let fila = 0;
            const seriesKeys = new Set();

            $('#tblDetalleRecepcion tbody tr').each(function () {
                fila++;
                const $tr = $(this);
                const $optSel = $tr.find('.selProducto option:selected');

                const codProd = $optSel.val();
                const ind = ($optSel.data('serializable') || 'S');
                const numSerie = ($tr.find('.inpSerie').val() || '').trim();
                const cant = parseInt($tr.find('.inpCantidad').val() || '0', 10);
                const codAct = ($tr.find('.inpCodActivo').val() || '').trim();

                if (!codProd) {
                    errores.push(`Fila ${fila}: producto es obligatorio.`);
                    return;
                }

                if (ind === 'S') {
                    if (!numSerie) errores.push(`Fila ${fila}: N° serie es obligatorio para producto serializable.`);
                    if (cant !== 1) errores.push(`Fila ${fila}: cantidad debe ser 1 para producto serializable.`);
                    const key = codProd + '|' + numSerie;
                    if (seriesKeys.has(key)) errores.push(`Fila ${fila}: la serie '${numSerie}' del producto ${codProd} está repetida.`);
                    else seriesKeys.add(key);
                } else {
                    if (!cant || cant <= 0) errores.push(`Fila ${fila}: cantidad debe ser mayor a 0.`);
                }

                items.push({
                    CodProducto: codProd,
                    SerieProductoId: null,
                    NumSerie: (ind === 'S') ? numSerie : null,
                    Cantidad: cant,
                    CodActivo: codAct
                });
            });

            if (items.length === 0) { swal({ text: "Agregue al menos un ítem en el detalle.", icon: "warning" }); return; }
            if (errores.length) { swal({ text: errores.join('\n'), icon: "warning" }); return; }

            const payload = { Cabecera: header, Detalle: items };

            const resp = await $.ajax({
                url: urlRegistrarRecepcion,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (resp.Ok) {
                swal({ text: resp.Mensaje || 'Guía registrada correctamente.', icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalRecepcion')).hide();
                if (dt) dt.ajax.reload(null, false);
            } else {
                swal({ text: swalText(resp, 'No se pudo registrar.'), icon: 'warning' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al registrar'), icon: 'error' });
        } finally {
            $btn.prop('disabled', false).html(oldHtml);
        }
    }

    // ================== DataTable ==================
    function visualizarDataTable() {
        dt = $('#tableRecepciones').DataTable({
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
                    ProveedorRuc: $('#fProveedor').val() || null,
                    CodEmpresaOrigen: $('#fEmpOri').val() || null,
                    CodLocalOrigen: $('#fLocOri').val() || null,
                    FiltroVarios: (data.search.value || '').toUpperCase()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarRecepciones,
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
                { data: 'Proveedor', defaultContent: '' },
                { data: 'NomLocalOrigen', defaultContent: '' },
                { data: 'NomLocalDestino', defaultContent: '' },
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
                $('#tableRecepciones_filter input').addClass('form-control-sm').attr('placeholder', 'Buscar...');
            },
            scrollY: '500px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100]
        });
    }

    // ================== Auxiliares ==================
    function limpiarModal() {
        $('#recNumGuia').val('');
        $('#recOrdenCompra').val('');
        $('#recFecha').val(new Date().toISOString().slice(0, 10));
        $('#recProveedor').val(null).trigger('change');
        $('#recAreaGestion,#recClaseStock,#recEstadoStock,#recObs').val('');
        $('#tblDetalleRecepcion tbody').empty();

        // Si existen empresa/local en el modal, resetear y deshabilitar local
        //if ($('#recEmpDest').length) $('#recEmpDest').val(null).trigger('change');
        //if ($('#recLocDest').length) {
        //    $('#recLocDest').empty().append(new Option('', '')).val(null).trigger('change');
        //    $('#recLocDest').prop('disabled', true);
        //}
    }

    function initCombosFijos() {
        // Selects fijos del modal (no dependientes)
        $('#recAreaGestion').select2({
            dropdownParent: $('#modalRecepcion'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        $('#recClaseStock').select2({
            dropdownParent: $('#modalRecepcion'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        $('#recEstadoStock').select2({
            dropdownParent: $('#modalRecepcion'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        $('#desEstadoStock').select2({
            dropdownParent: $('#modalRecepcion'),
            width: '100%',
            placeholder: 'Seleccionar',
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

    async function abrirModalDetalleRecepcion(id) {
        // limpia/placeholder
        $('#detalleRecepcionBody').html('<div class="text-muted">Cargando detalle…</div>');
        const $modalEl = document.getElementById('modalDetalleRecepcion');
        const modal = bootstrap.Modal.getInstance($modalEl) || new bootstrap.Modal($modalEl);
        modal.show();

        try {
            const resp = await $.getJSON(urlObtenerGuiaRecepcion, { id: id });
            if (!resp || !resp.Ok || !resp.Data) {
                $('#detalleRecepcionBody').html('<div class="text-danger">No se pudo cargar el detalle.</div>');
                return;
            }
            const html = detalleDespachoTemplate(resp.Data);
            $('#detalleRecepcionBody').html(html);
        } catch (err) {
            $('#detalleRecepcionBody').html('<div class="text-danger">' + (swalText(err, 'Error al obtener el detalle.') || 'Error') + '</div>');
        }
    }

    function safe(v) { return (v == null ? '' : (v + '')); }

    function detalleDespachoTemplate(gr) {
        // por si viniera envuelto
        if (gr && gr.Data) gr = gr.Data;

        const fecha = parseDotNetDate(gr.Fecha);
        const detalles = Array.isArray(gr.Detalles) ? gr.Detalles : [];

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
<div class="detalle-recepcion">
    <div class="mb-3">
        <div class="row g-2">
            <div class="col-md-3"><b>N° Guía:</b> ${safe(gr.NumGuia)}</div>
            <div class="col-md-3"><b>Fecha:</b> ${fecha}</div>
            <div class="col-md-3"><b>Transferencia:</b> ${safe(gr.IndTransferencia)}</div>
            <div class="col-md-3"><b>Estado:</b> ${safe(gr.IndEstado || 'REGISTRADA')}</div>

            <div class="col-md-3"><b>Origen:</b> ${safe(gr.NomLocalOrigen)}</div>
            <div class="col-md-3"><b>Destino:</b> ${safe(gr.NomLocalDestino)}</div>
            <div class="col-md-3"><b>Área Gestión:</b> ${safe(gr.AreaGestion)}</div>
            <div class="col-md-3"><b>Clase Stock:</b> ${safe(gr.ClaseStock)}</div>

            <div class="col-md-12"><b>Observaciones:</b> ${safe(gr.Observaciones)}</div>
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


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                initCombosFijos();
                await cargarComboEmpresas();
                await cargarFiltroProveedores();
                visualizarDataTable();
            });
        }
    };
})(jQuery);
