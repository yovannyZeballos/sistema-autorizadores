var urlListarDespachos = baseUrl + 'Inventario/GuiaDespacho/ListarPaginado';
var urlRegistrarDespacho = baseUrl + 'Inventario/GuiaDespacho/Registrar';

var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProductos = baseUrl + 'Inventario/Productos/Listar';
var urlListarSeriesDisponibles = baseUrl + 'Inventario/SeriesProducto/ListarPorProductoDisponibles';

var AdministrarGuiaDespacho = (function ($) {

    // cache de productos para conocer IndSerializable
    var _productosCache = []; // {CodProducto, DesProducto, NomMarca, NomModelo, IndSerializable}

    function initSelect2EnModal(target, opts) {
        var $els = (target && target.jquery) ? target : $(target);
        if (!$els.length) return;

        var $modal = $els.closest('.modal');
        if (!$modal.length) $modal = $('#modalDespacho');

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

    // ================== Eventos ==================
    const eventos = function () {

        $('#btnBuscar').on('click', function (e) {
            e.preventDefault();
            $('#tableDespachos').DataTable().ajax.reload();
        });

        $('#btnNuevaGuia').on('click', async function () {
            limpiarModal();
            await Promise.all([cargarComboEmpresas('#desEmpOrig'), cargarComboEmpresas('#desEmpDest'), ensureProductos()]);
            new bootstrap.Modal(document.getElementById('modalDespacho')).show();
        });

        // Dependientes: Origen y Destino
        //$('#desEmpOrig').on('change', function () { cargarComboLocales('#desEmpOrig', '#desLocOrig'); });
        $('#desEmpDest').on('change', function () { cargarComboLocales('#desEmpDest', '#desLocDest'); });

        // Tipo movimiento
        $('#desTipoMov').on('change', onTipoMovimientoChange);

        // Guardar
        $('#btnGuardarDespacho').on('click', guardarDespacho);

        // Agregar fila detalle
        $('#btnAddItem').on('click', function (e) { e.preventDefault(); addDetalleRow(); });

        // Cuando se muestre el modal, asegurar select2
        $('#modalDespacho').on('shown.bs.modal', function () {
            initSelect2EnModal('#desTipoMov');
            //initSelect2EnModal('#desEmpOrig'); initSelect2EnModal('#desLocOrig');
            initSelect2EnModal('#desEmpDest'); initSelect2EnModal('#desLocDest');
            // Para series y productos ya agregados
            $('#tblDetalleDespacho tbody select.selProducto').each(function () { initSelect2EnModal(this); });
            $('#tblDetalleDespacho tbody select.selSerie').each(function () { initSelect2EnModal(this); });
        });

        // Filtros combos en index
        //initSelect2EnModal('#fEmpOrig'); initSelect2EnModal('#fLocOrig');
        initSelect2EnModal('#fEmpDest'); initSelect2EnModal('#fLocDest');
        initSelect2EnModal('#fTipoMov');

        // Cargar combos de filtros base
        //cargarComboEmpresas('#fEmpOrig').then(() => cargarComboLocales('#fEmpOrig', '#fLocOrig'));
        cargarComboEmpresas('#fEmpDest').then(() => cargarComboLocales('#fEmpDest', '#fLocDest'));
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

    // ================== Combos base ==================
    async function cargarComboEmpresas(selector) {
        try {
            const resp = await listarEmpresas();
            const $sel = $(selector);
            $sel.empty().append('<option></option>');
            if (resp.Ok) resp.Data.forEach(e => $sel.append(new Option(e.NomEmpresa, e.CodEmpresa)));
            else swal({ text: swalText(resp, 'No fue posible listar empresas'), icon: 'error' });
        } catch (err) { swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' }); }
        initSelect2EnModal(selector);
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
        initSelect2EnModal(selLocal);
    }

    // Cache de productos
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
            } else swal({ text: swalText(resp, 'No fue posible listar productos'), icon: 'error' });
        } catch (err) { swal({ text: swalText(err, 'Error al listar productos'), icon: 'error' }); }
    }

    // ================== Detalle ==================
    function addDetalleRow() {
        const $tb = $('#tblDetalleDespacho tbody');
        const rowId = 'r' + Date.now() + '_' + Math.floor(Math.random() * 1000);

        const tr = $(
            '<tr data-row="' + rowId + '">' +
            '<td><select class="form-select form-select-sm selProducto select2-show-search" style="width:100%"></select></td>' +
            '<td><select class="form-select form-select-sm selSerie select2-show-search" style="width:100%" disabled><option value=""></option></select></td>' +
            '<td><input type="number" class="form-control form-control-sm inpCantidad" min="1" step="1" value="1" /></td>' +
            '<td><input type="text" class="form-control form-control-sm inpCodActivo text-uppercase" /></td>' +
            '<td class="text-center"><button type="button" class="btn btn-sm btn-link text-danger btnDelRow" title="Quitar"><i class="fe fe-trash-2"></i></button></td>' +
            '</tr>'
        );

        $tb.append(tr);

        const $selProd = tr.find('select.selProducto');
        const $selSerie = tr.find('select.selSerie');

        // init select2
        initSelect2EnModal($selProd);
        initSelect2EnModal($selSerie);

        // cargar productos
        $selProd.append(new Option('', '', false, false));
        _productosCache.forEach(p => {
            var texto = (p.DesProducto || '');
            if (p.NomMarca) texto += ' - ' + p.NomMarca;
            if (p.NomModelo) texto += ' - ' + p.NomModelo;
            texto = texto.trim();
            var opt = new Option(texto, p.CodProducto, false, false);
            $(opt).attr('data-serializable', p.IndSerializable);
            $selProd.append(opt);
        });

        // eventos
        $selProd.on('change', function () { onProductoChange(tr); });
        $selSerie.on('change', function () { /* opcional: validar selección */ });
        tr.find('.btnDelRow').on('click', function () { tr.remove(); });
    }

    async function onProductoChange(tr) {
        const $selProd = tr.find('select.selProducto');
        const $selSerie = tr.find('select.selSerie');
        const $cant = tr.find('.inpCantidad');

        const cod = $selProd.val();
        const ind = ($selProd.find(':selected').data('serializable') || 'S'); // 'S'|'N'

        // reset
        $selSerie.empty().append('<option value=""></option>').prop('disabled', true).trigger('change');
        $cant.prop('disabled', false).val('1');

        if (!cod) return;

        if (ind === 'S') {

            // cargar series disponibles en origen
            try {
                const resp = await listarSeriesPorProducto(cod);
                $selSerie.empty().append('<option value=""></option>');
                if (resp.Ok) {
                    (resp.Data || []).forEach(s => {
                        // Asumimos que el servicio devuelve s.NumSerie y (opcional) s.Id
                        var opt = new Option(s.NumSerie, s.NumSerie, false, false);
                        $selSerie.append(opt);
                    });
                } else {
                    swal({ text: swalText(resp, 'No fue posible listar series disponibles.'), icon: 'warning' });
                }
                initSelect2EnModal($selSerie);
                $selSerie.prop('disabled', false);
                // Cantidad fija 1 en serializables
                $cant.val('1').prop('disabled', true);
            } catch (err) {
                swal({ text: swalText(err, 'Error al listar series'), icon: 'error' });
            }
        } else {
            // no serializable
            $selSerie.prop('disabled', true).val(null).trigger('change');
            $cant.prop('disabled', false);
        }
    }

    // ================== Guardar ==================
    async function guardarDespacho() {
        const tipoMov = ($('#desTipoMov').val() || 'TRANSFERENCIA').toUpperCase();

        const requiereDestino = (tipoMov === 'TRANSFERENCIA' || tipoMov === 'ASIGNACION_ACTIVO');


        const header = {
            NumGuia: ($('#desNumGuia').val() || '').trim(),
            Fecha: $('#desFecha').val(),
            CodEmpresaDestino: $('#desEmpDest').val() || null,
            CodLocalDestino: $('#desLocDest').val() || null,
            CodEmpresaDestino: requiereDestino ? ($('#desEmpDest').val() || null) : null,
            CodLocalDestino: requiereDestino ? ($('#desLocDest').val() || null) : null,
            TipoMovimiento: tipoMov,
            // Política: verificación en destino ⇒ usar tránsito en TRANSFERENCIA y ASIGNACION
            UsarTransitoDestino: $('#desUsarTransito').is(':checked'),
            AreaGestion: ($('#desAreaGestion').val() || '').trim(),
            ClaseStock: ($('#desClaseStock').val() || '').trim(),
            EstadoStock: ($('#desEstadoStock').val() || '').trim(),
            Observaciones: ($('#desObs').val() || '').trim()
        };

        // Validaciones cabecera mínimas
        if (!header.NumGuia || !header.Fecha) {
            swal({ text: "Complete los campos obligatorios de la cabecera (Fecha, N° Guía).", icon: "warning" });
            return;
        }

        if (!header.AreaGestion || !header.ClaseStock || !header.EstadoStock) {
            swal({ text: "Complete Área Gestión, Clase Stock y Estado Stock.", icon: "warning" });
            return;
        }

        if (requiereDestino && (!header.CodEmpresaDestino || !header.CodLocalDestino)) {
            swal({ text: "Destino (Empresa y Local) es obligatorio para TRANSFERENCIA y ASIGNACION_ACTIVO.", icon: "warning" });
            return;
        }

        const items = [];
        const errores = [];
        let fila = 0;

        // evitar series duplicadas (mismo producto) en el mismo payload
        const seriesKeys = new Set();

        $('#tblDetalleDespacho tbody tr').each(function () {
            fila++;
            const $tr = $(this);
            const $optSel = $tr.find('.selProducto option:selected');

            const codProd = $optSel.val();
            const ind = ($optSel.data('serializable') || 'S');
            const numSerie = ($tr.find('.selSerie').val() || '').trim();
            const cant = parseInt($tr.find('.inpCantidad').val() || '0', 10);
            const codAct = ($tr.find('.inpCodActivo').val() || '').trim();

            if (!codProd) {
                errores.push(`Fila ${fila}: producto es obligatorio.`);
            } else if (ind === 'S') {
                if (!numSerie) errores.push(`Fila ${fila}: debe seleccionar una serie disponible para producto serializable.`);
                if (cant !== 1) errores.push(`Fila ${fila}: cantidad debe ser 1 para producto serializable.`);
                // duplicidad de serie
                var key = codProd + '|' + numSerie;
                if (seriesKeys.has(key)) errores.push(`Fila ${fila}: la serie '${numSerie}' del producto ${codProd} está repetida.`);
                else seriesKeys.add(key);
            } else {
                if (!cant || cant <= 0) errores.push(`Fila ${fila}: cantidad debe ser mayor a 0.`);
            }

            if (!codProd) return;

            items.push({
                CodProducto: codProd,
                NumSerie: (ind === 'S') ? numSerie : null,
                Cantidad: cant,
                CodActivo: codAct,
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

        try {
            const resp = await $.ajax({
                url: urlRegistrarDespacho,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json',
                beforeSend: showLoading,
                complete: closeLoading
            });

            if (resp.Ok) {
                swal({ text: resp.Mensaje || 'Guía de despacho registrada correctamente.', icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalDespacho')).hide();
                $('#tableDespachos').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: swalText(resp, 'No se pudo registrar.'), icon: 'warning' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al registrar'), icon: 'error' });
        }
    }

    // ================== DataTable ==================
    function visualizarDataTable() {
        $('#tableDespachos').DataTable({
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
                    //CodEmpresaOrigen: $('#fEmpOrig').val() || null,
                    //CodLocalOrigen: $('#fLocOrig').val() || null,
                    CodEmpresaDestino: $('#fEmpDest').val() || null,
                    CodLocalDestino: $('#fLocDest').val() || null,
                    TipoMovimiento: $('#fTipoMov').val() || null,
                    FiltroVarios: $('#txtFiltro').val() || ''
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
                {
                    data: null, render: function (r) {
                        return (r.CodEmpresaOrigen || '') + '-' + (r.CodLocalOrigen || '');
                    }
                },
                {
                    data: null, render: function (r) {
                        var d = '';
                        if (r.CodEmpresaDestino) d = r.CodEmpresaDestino;
                        if (r.CodLocalDestino) d += (d ? '-' : '') + r.CodLocalDestino;
                        return d;
                    }
                },
                { data: 'TipoMovimiento', defaultContent: '' },
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

    // ================== Auxiliares ==================
    function onTipoMovimientoChange() {
        var tipo = ($('#desTipoMov').val() || 'TRANSFERENCIA').toUpperCase();
        var isTransf = (tipo === 'TRANSFERENCIA');
        var isAsign = (tipo === 'ASIGNACION_ACTIVO');
        var enableDestino = (isTransf || isAsign);

        // Destino requerido para TRANSFERENCIA y ASIGNACION_ACTIVO
        $('#desEmpDest').prop('disabled', !enableDestino).val(null).trigger('change');
        $('#desLocDest').prop('disabled', !enableDestino).val(null).trigger('change');

        //$('#desEmpDest').prop('disabled', !isTransf).val(null).trigger('change');
        //$('#desLocDest').prop('disabled', !isTransf).val(null).trigger('change');

        $('#desUsarTransito').prop('disabled', !isTransf).prop('checked', isTransf);
        initSelect2EnModal('#desEmpDest');
        initSelect2EnModal('#desLocDest');
    }

    function limpiarModal() {
        $('#desFecha').val(new Date().toISOString().slice(0, 10));
        $('#desNumGuia').val('');
        $('#desTipoMov').val('TRANSFERENCIA').trigger('change');
        $('#desUsarTransito').prop('checked', true).prop('disabled', false);

        // Origen/Destino
        //$('#desEmpOrig').val(null).trigger('change');
        //$('#desLocOrig').empty().append('<option></option>').val(null).trigger('change');

        $('#desEmpDest').val(null).trigger('change');
        $('#desLocDest').empty().append('<option></option>').val(null).trigger('change');

        $('#desObs').val('');
        $('#tblDetalleDespacho tbody').empty();

        // preparar Select2 cabecera
        initSelect2EnModal('#desTipoMov');
        //initSelect2EnModal('#desEmpOrig'); initSelect2EnModal('#desLocOrig');
        initSelect2EnModal('#desEmpDest'); initSelect2EnModal('#desLocDest');
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                visualizarDataTable();
            });
        }
    };
})(jQuery);
