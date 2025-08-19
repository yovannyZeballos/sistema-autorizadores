var urlListarRecepciones = baseUrl + 'Inventario/GuiaRecepcion/ListarPaginado';
var urlRegistrarRecepcion = baseUrl + 'Inventario/GuiaRecepcion/Registrar';
var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProveedores = baseUrl + 'Inventario/Proveedores/Listar';
var urlListarProductos = baseUrl + 'Inventario/Productos/Listar';
var urlListarSeriesDisponibles = baseUrl + 'Inventario/SeriesProducto/ListarPorProducto';

var AdministrarGuiaRecepcionProveedor = (function ($) {

    // cache de productos para saber si es serializable
    var _productosCache = []; // {CodProducto, DesProducto, NomMarca, NomModelo, IndSerializable}

    function initSelect2EnModal(target, opts) {
        // Acepta string selector o jQuery object
        var $els = (target && target.jquery) ? target : $(target);
        if (!$els.length) return;

        // Detecta el modal más cercano; fallback a #modalRecepcion o #modalMovimiento
        var $modal = $els.closest('.modal');
        if (!$modal.length) {
            $modal = $('#modalRecepcion');
            if (!$modal.length) $modal = $('#modalMovimiento');
        }

        $els.each(function () {
            var $s = $(this);

            // Asegurar placeholder desde la 1ª opción
            var $opt0 = $s.find('option').first();
            var placeholder = $opt0.attr('label') || $opt0.text() || 'Seleccionar…';
            if ($opt0.length && !$opt0.attr('value')) $opt0.attr('value', '');

            // Si ya está inicializado, destruir antes de reinit
            if ($s.hasClass('select2-hidden-accessible')) {
                $s.select2('destroy');
            }

            // Inicializar Select2 dentro del modal
            $s.select2($.extend(true, {
                dropdownParent: $modal,
                width: '100%',
                placeholder: placeholder,
                allowClear: true,
                minimumResultsForSearch: 0 // <-- fuerza ver el buscador
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

    const eventos = function () {

        $('#btnBuscar').on('click', function (e) {
            e.preventDefault();
            $('#tableRecepciones').DataTable().ajax.reload();
        });

        $('#btnNuevaGuia').on('click', async function () {
            limpiarModal();
            await Promise.all([cargarComboEmpresas(), cargarComboProveedores(), ensureProductos()]);
            addDetalleRow();
            new bootstrap.Modal(document.getElementById('modalRecepcion')).show();
        });

        $('#recEmpDest').on('change', async function () {
            await cargarComboLocales();
        });

        // guardar
        $('#btnGuardarRecepcion').on('click', guardarRecepcion);

        // agregar fila detalle
        $('#btnAddItem').on('click', function (e) {
            e.preventDefault();
            addDetalleRow();
        });

        // cuando se muestre el modal, asegurar select2 en cabecera
        $('#modalRecepcion').on('shown.bs.modal', function () {
            initSelect2EnModal('#recAreaGestion');
            initSelect2EnModal('#recClaseStock');
            initSelect2EnModal('#recProveedor');
            initSelect2EnModal('#recEmpDest');
            initSelect2EnModal('#recLocDest');
            initSelect2EnModal('#recEstadoStock');
            // también los selects de detalle existentes
            $('#tblDetalleRecepcion tbody select.selProducto').each(function () { initSelect2EnModal(this); });
        });
    };

    // ==== AJAX helpers ====
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

    // ==== Cargar combos base ====
    async function cargarComboEmpresas() {
        try {
            const resp = await listarEmpresas();
            $('#recEmpDest').empty().append('<option></option>');
            $('#recLocDest').empty().append('<option></option>');
            if (resp.Ok) {
                resp.Data.forEach(e => {
                    $('#recEmpDest').append(new Option(e.NomEmpresa, e.CodEmpresa));
                });
            } else {
                swal({ text: swalText(resp, 'No fue posible listar empresas'), icon: 'error' });
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' }); }
        initSelect2EnModal('#recEmpDest');
        initSelect2EnModal('#recLocDest');
    }

    async function cargarComboLocales() {
        try {
            const codEmpresa = $('#recEmpDest').val();
            const resp = await listarLocales(codEmpresa);
            $('#recLocDest').empty().append('<option></option>');
            if (resp.Ok) {
                resp.Data.forEach(l => {
                    $('#recLocDest').append(new Option(l.NomLocal, l.CodLocal));
                });
            } else {
                swal({ text: swalText(resp, 'No fue posible listar locales'), icon: 'error' });
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' }); }
        $('#recLocDest').val(null).trigger('change');
    }

    async function cargarFiltroProveedores() {
        try {
            const resp = await listarProveedores();
            const $cbo = $('#cboProveedor');        // <-- FALTABA
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

    async function cargarComboProveedores() {
        try {
            const resp = await listarProveedores();
            $('#recProveedor').empty().append('<option></option>');
            if (resp.Ok) {
                resp.Data.forEach(p => {
                    $('#recProveedor').append(new Option(p.RazonSocial, p.Ruc));
                });
            } else {
                swal({ text: swalText(resp, 'No fue posible listar proveedores'), icon: 'error' });
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar proveedores'), icon: 'error' }); }
        initSelect2EnModal('#recProveedor');
    }

    // cache productos
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
                    IndSerializable: (p.IndSerializable || 'S') // 'S' | 'N'
                }));
            } else {
                swal({ text: swalText(resp, 'No fue posible listar productos'), icon: 'error' });
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar productos'), icon: 'error' }); }
    }

    // ==== Detalle (filas) ====
    function addDetalleRow() {
        const $tb = $('#tblDetalleRecepcion tbody');
        const rowId = 'r' + Date.now() + '_' + Math.floor(Math.random() * 1000);

        const tr = $(
            '<tr data-row="' + rowId + '">' +
            '<td><select class="form-select form-select-sm selProducto select2-show-search" style="width:100%"></select></td>' +
            '<td><input type="text" class="form-control form-control-sm inpSerie text-uppercase" placeholder="N° serie (si aplica)" /></td>' +
            '<td><input type="number" class="form-control form-control-sm inpCantidad" min="1" step="1" value="1" /></td>' +
            '<td><input type="text" class="form-control form-control-sm inpCodActivo text-uppercase" /></td>' +
            '<td class="text-center"><button type="button" class="btn btn-sm btn-link text-danger btnDelRow" title="Quitar"><i class="fe fe-trash-2"></i></button></td>' +
            '</tr>'
        );

        $tb.append(tr);

        // init select2 en la fila
        const $selProd = tr.find('select.selProducto');
        initSelect2EnModal($selProd);

        // cargar productos en combo
        $selProd.append(new Option('', '', false, false)); // placeholder
        _productosCache.forEach(p => {
            var texto = (p.DesProducto || '');
            if (p.NomMarca) texto += ' - ' + p.NomMarca;
            if (p.NomModelo) texto += ' - ' + p.NomModelo;
            texto = texto.trim();
            var opt = new Option(texto, p.CodProducto, false, false);
            $(opt).attr('data-serializable', p.IndSerializable);
            $selProd.append(opt);
        });

        // Activar búsqueda de select2 dentro del modal (sin clases extra ni CSS nuevo)
        //if ($.fn.select2) {
        //    $selProd.select2({
        //        dropdownParent: $('#modalRecepcion'),
        //        width: '100%',
        //        placeholder: 'Seleccionar...',
        //        allowClear: true,
        //        minimumResultsForSearch: 0 // fuerza a mostrar el buscador siempre
        //    });
        //}

        // eventos de fila
        $selProd.on('change', async function () {
            await onProductoChange(tr);
        });

        tr.find('.btnDelRow').on('click', function () {
            tr.remove();
        });
    }

    async function onProductoChange(tr) {
        const $selProd = tr.find('select.selProducto');
        const $serie = tr.find('.inpSerie');
        const $cant = tr.find('.inpCantidad');

        const cod = $selProd.val();
        const ind = ($selProd.find(':selected').data('serializable') || 'S'); // 'S'|'N'

        // reset serie
        $serie.val('');
        if (!cod) {
            $serie.prop('disabled', true).attr('placeholder', 'N° serie (si aplica)');
            $cant.prop('disabled', false).val('1');
            return;
        }

        if (ind === 'S') {
            // Serializable: requerimos N° serie, cantidad fija 1
            $serie.prop('disabled', false).attr('placeholder', 'Ingrese N° de serie');
            $cant.val('1').prop('disabled', true);
        } else {
            // No serializable: sin serie, cantidad libre
            $serie.val('').prop('disabled', true).attr('placeholder', '(no serie)');
            $cant.prop('disabled', false);
        }
    }

    // ==== Guardar ====
    async function guardarRecepcion() {
        // cabecera
        const header = {
            NumGuia:            ($('#recNumGuia').val() || '').trim(),
            OrdenCompra:        ($('#recOrdenCompra').val() || '').trim(),
            Fecha:              $('#recFecha').val(),
            ProveedorRuc:       $('#recProveedor').val() || null,
            //AreaGestion:      ($('#recAreaGestion').val() || '').trim(),
            //ClaseStock:       ($('#recClaseStock').val() || '').trim(),
            //EstadoStock:      ($('#recEstadoStock').val() || '').trim(),
            IndTransferencia:   'N',
            Observaciones:      ($('#recObs').val() || '').trim()
        };

        // validaciones mínimas
        //if (!header.NumGuia || !header.ProveedorRuc || !header.Fecha || !header.AreaGestion || !header.ClaseStock || !header.EstadoStock) {
        //    swal({ text: "Complete los campos obligatorios (*) de la cabecera.", icon: "warning" });
        //    return;
        //}

        if (!header.NumGuia || !header.ProveedorRuc || !header.Fecha) {
            swal({ text: "Complete los campos obligatorios (*) de la cabecera.", icon: "warning" });
            return;
        }

        // detalle
        const items = [];
        const errores = [];
        let fila = 0;

        // para evitar series duplicadas en la misma guía (mismo producto)
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
            //const obs = ($tr.find('.inpObs').val() || '').trim();

            if (!codProd) {
                errores.push(`Fila ${fila}: producto es obligatorio.`);
            } else if (ind === 'S') {
                if (!numSerie) errores.push(`Fila ${fila}: N° serie es obligatorio para producto serializable.`);
                if (cant !== 1) errores.push(`Fila ${fila}: cantidad debe ser 1 para producto serializable.`);
            } else {
                if (!cant || cant <= 0) errores.push(`Fila ${fila}: cantidad debe ser mayor a 0.`);
            }

            items.push({
                CodProducto: codProd,
                // siempre enviaremos SerieProductoId = null al crear; el backend creará la serie si corresponde
                SerieProductoId: null,   // null si nueva o no serializable
                NumSerie: (ind === 'S') ? numSerie : null,
                Cantidad: cant,
                CodActivo: codAct,
                //Observaciones: obs
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

        try {
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
                $('#tableRecepciones').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: swalText(resp, 'No se pudo registrar.'), icon: 'warning' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al registrar'), icon: 'error' });
        }
    }

    // ==== DataTable principal ====
    function visualizarDataTable() {
        $('#tableRecepciones').DataTable({
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
                    ProveedorRuc: $('#cboProveedor').val() || null,
                    IndTransferencia: 'N',
                    FiltroVarios: $('#txtFiltro').val() || ''
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
                {
                    data: null, render: function (r) {
                        return (r.CodEmpresaDestino || '') + '-' + (r.CodLocalDestino || '');
                    }
                },
                { data: 'Items', defaultContent: '' },
                { data: 'IndEstado', defaultContent: 'REGISTRADA' },
                { data: 'UsuCreacion', defaultContent: '' }
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
            lengthMenu: [10, 25, 50, 100]
        });
    }

    function limpiarModal() {
        $('#recNumGuia').val('');
        $('#recOrdenCompra').val('');
        $('#recFecha').val(new Date().toISOString().slice(0, 10));
        $('#recProveedor').val(null).trigger('change');
        $('#recAreaGestion,#recClaseStock,#recEstadoStock,#recObs').val('');
        $('#tblDetalleRecepcion tbody').empty();
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarFiltroProveedores();
                visualizarDataTable();
            });
        }
    };
})(jQuery);
