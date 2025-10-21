var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProductos = baseUrl + 'Inventario/Productos/ListarPaginado';
var urlListarSeries = baseUrl + 'Inventario/SeriesProducto/ListarPaginado';
var urlListarKardex = baseUrl + 'Inventario/Kardex/ListarPaginadoPorProducto';
var urlListarMarcas = baseUrl + 'Inventario/Marcas/Listar';
var urlListarAreasGestion = baseUrl + 'Inventario/AreasGestion/Listar';

var urlCrearProducto = baseUrl + 'Inventario/Productos/Crear';
var urlCrearSerie = baseUrl + 'Inventario/SeriesProducto/Crear';
var urlDarBajaSerie = baseUrl + 'Inventario/SeriesProducto/DarBaja';
var urlEliminarProducto = baseUrl + 'Inventario/Productos/Eliminar';
var urlEditarProducto = baseUrl + 'Inventario/Productos/Editar';

var dtProductos = null;
var dtSeries = null;

var AdministrarProducto = function ($) {

    const eventos = function () {

        $("#btnBuscar").on("click", function (e) {
            e.preventDefault();
            if (dtProductos) dtProductos.ajax.reload();
        });

        $("#serieCodEmpresa").on("change", async function () {
            await cargarComboLocales();
        });

        // Al seleccionar fila, mostrar detalle de series
        $('#tableProductos tbody').on('change', '.row-checkbox', function () {
            var $checks = $('#tableProductos tbody .row-checkbox:checked');
            if ($checks.length === 1) {
                var data = $('#tableProductos').DataTable().row($checks.closest('tr')).data();
                mostrarDetalleProducto(data);
            } else {
                $('#cardSeries').hide();
            }
        });

        // Nuevo producto
        $('#btnNuevoProducto').on('click', function () {
            $('#formNuevoProducto')[0].reset();
            $('#btnGuardarNuevoModal').show();
            $('#btnGuardarCambiosModal').hide();
            $('#modalInputCodProducto').prop('disabled', false);

            $('#modalInputCboMarca').val('').trigger('change');
            $('#modalInputCboTipProducto').val('').trigger('change');
            $('#modalInputCboAreaGestion').val('').trigger('change');


            new bootstrap.Modal(document.getElementById('modalNuevoProducto')).show();
        });

        // Editar producto
        $('#btnEditarProducto').on('click', async function () {
            var $checks = $('#tableProductos tbody .row-checkbox:checked');
            if ($checks.length !== 1) {
                return swal({
                    text: $checks.length === 0 ? "Seleccione un registro para modificar." : "Seleccione sólo un registro.",
                    icon: "warning"
                });
            }

            var data = $('#tableProductos').DataTable().row($checks.closest('tr')).data();

            $('#formNuevoProducto')[0].reset();
            $('#btnGuardarNuevoModal').hide();
            $('#btnGuardarCambiosModal').show();

            $('#modalInputCodProducto').val(data.CodProducto).prop('disabled', true);
            $('#modalInputDesProducto').val(data.DesProducto);
            $('#modalInputCboMarca').val(data.MarcaId).trigger('change');
            $('#modalInputCboTipProducto').val(data.TipProducto).trigger('change');
            $('#modalInputCboAreaGestion').val(data.AreaGestionId).trigger('change');
            $('#modalInputNomModelo').val(data.NomModelo);
            $('#modalInputStkMinimo').val(data.StkMinimo);
            $('#modalInputStkMaximo').val(data.StkMaximo);
            $('#modalChkActivo').prop('checked', data.IndActivo === 'S');
            $('#modalChkSerializable').prop('checked', data.IndSerializable === 'S');

            new bootstrap.Modal(document.getElementById('modalNuevoProducto')).show();
        });

        // Guardar producto
        $('#btnGuardarNuevoModal').on('click', function () { guardarProducto({ modo: 'crear' }); });
        $('#btnGuardarCambiosModal').on('click', function () { guardarProducto({ modo: 'editar' }); });

        // Eliminar producto
        $('#btnEliminarProducto').on('click', eliminarProducto);

        $('#btnNuevaSerie').on('click', function () {
            $('#serieNumSerie').val('');
            $('#serieCodEmpresa').val('').trigger('change');
            $('#serieCodLocal').val('').trigger('change');


            var modal = new bootstrap.Modal(document.getElementById('modalNuevaSerie'));
            modal.show();
        });

        $('#btnGuardarSerie').on('click', async function () {
            var codProducto = $('#lblCodProducto').text();
            var payload = {
                CodProducto: codProducto,
                NumSerie: $('#serieNumSerie').val().trim(),
                CodEmpresa: $('#serieCodEmpresa').val(),
                CodLocal: $('#serieCodLocal').val()
            };

            if (!payload.NumSerie || !payload.CodEmpresa || !payload.CodLocal) {
                swal({ text: "Complete los campos obligatorios (*)", icon: "warning" });
                return;
            }

            try {
                var resp = await $.ajax({
                    url: urlCrearSerie, type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(payload), dataType: 'json'
                });
                if (resp.Ok) {
                    swal({ text: resp.Mensaje, icon: "success" });
                    bootstrap.Modal.getInstance(document.getElementById('modalNuevaSerie')).hide();
                    if (dtSeries) dtSeries.ajax.reload(null, false);
                } else {
                    swal({ text: resp.Mensaje, icon: "warning" });
                }
            } catch (err) {
                swal({ text: err.responseText || err.statusText, icon: "error" });
            }
        });

        $('#tableSeries tbody').on('click', '.action-baja', function () {
            const data = dtSeries.row($(this).closest('tr')).data();
            if (!data) return;

            const codProd = $('#lblCodProducto').text();
            $('#bajaLabelProducto').text(codProd);
            $('#bajaLabelSerie').text(data.NumSerie || '');

            $('#bajaSerieId').val(data.Id || '');
            $('#bajaCodProducto').val(codProd || '');
            $('#bajaNumSerie').val(data.NumSerie || '');

            $('#bajaFecha').val(new Date().toISOString().slice(0, 10));
            $('#bajaObs').val('');

            new bootstrap.Modal(document.getElementById('modalBajaSerie')).show();
        });

        // Confirmar baja
        $('#btnConfirmarBajaSerie').off('click').on('click', async function () {
            var fecha = $('#bajaFecha').val();
            if (!fecha) { swal({ text: "Debe ingresar la fecha de baja.", icon: "warning" }); return; }

            var payload = {
                SerieProductoId: $('#bajaSerieId').val() || null,
                CodProducto: $('#bajaCodProducto').val() || null,
                NumSerie: $('#bajaNumSerie').val() || null,
                Fecha: fecha,
                Observaciones: ($('#bajaObs').val() || '').trim()
            };

            // anti doble clic
            var $btn = $(this);
            var oldHtml = $btn.html();
            $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Procesando…');

            try {
                var resp = await $.ajax({
                    url: urlDarBajaSerie,
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(payload),
                    dataType: 'json'
                });

                if (resp && resp.Ok) {
                    swal({ text: resp.Mensaje || 'Serie dada de baja correctamente.', icon: 'success' });
                    bootstrap.Modal.getInstance(document.getElementById('modalBajaSerie')).hide();
                    if (dtSeries) dtSeries.ajax.reload(null, false);
                } else {
                    swal({ text: (resp && (resp.Mensaje || resp.message)) || 'No se pudo dar de baja.', icon: 'warning' });
                }
            } catch (err) {
                swal({ text: (err && (err.responseText || err.statusText)) || 'Error al dar de baja.', icon: 'error' });
            } finally {
                $btn.prop('disabled', false).html(oldHtml);
            }
        });

        $('#cboCodMarcaBuscar, #cboTipProductoBuscar, #cboCodAreaGestionBuscar, #cboIndActivoBuscar')
            .off('change._prod')
            .on('change._prod', function () {
                if ($.fn.DataTable.isDataTable('#tableProductos')) {
                    $('#tableProductos').DataTable().ajax.reload();
                }
            });


    };

    const listarEmpresasAsociadas = function () {
        return new Promise((resolve, reject) => {

            const request = {
                CodUsuario: '',
                Busqueda: ''
            };

            $.ajax({
                url: urlListarEmpresasAsociadas,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const listarLocales = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#serieCodEmpresa").val();

            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa
            };

            $.ajax({
                url: urlListarLocalesPorEmpresa,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const cargarComboEmpresa = async function () {

        try {
            const response = await listarEmpresasAsociadas();

            if (response.Ok) {
                $('#serieCodEmpresa').empty().append('<option value="">Seleccionar...</option>');
                $('#serieCodLocal').empty().append('<option value="">Seleccionar...</option>');
                response.Data.map(empresa => {
                    $('#serieCodEmpresa').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
                });
            } else {
                swal({ text: String(response && response.Mensaje || 'Error al listar empresas'), icon: "error" });
                return;
            }
        } catch (error) {
            swal({ text: String(error && (error.Mensaje || error.message || error.responseText || error.statusText) || 'Error al listar empresas'), icon: "error" });
        }
    }

    async function cargarComboLocales(selEmpresa, selLocal) {
        try {
            const codEmpresa = $(selEmpresa).val();
            const resp = await listarLocales(codEmpresa);
            if (resp.Ok) {
                await cargarCombo($(selLocal), resp.Data.map(l => ({
                    text: l.NomLocal,
                    value: l.CodLocal
                })), { placeholder: 'Todos', todos: true });
            } else {
                swal({ text: swalText(resp, 'No fue posible listar locales'), icon: 'error' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' });
        }
    }

    // ======================== CRUD ========================
    async function guardarProducto({ modo }) {
        var payload = {
            CodProducto: $('#modalInputCodProducto').val().trim(),
            DesProducto: $('#modalInputDesProducto').val().trim(),
            MarcaId: $('#modalInputCboMarca').val(),
            TipProducto: $('#modalInputCboTipProducto').val(),
            AreaGestionId: $('#modalInputCboAreaGestion').val(),
            NomModelo: $('#modalInputNomModelo').val().trim(),
            StkMinimo: $('#modalInputStkMinimo').val().trim(),
            StkMaximo: $('#modalInputStkMaximo').val().trim(),
            IndActivo: $('#modalChkActivo').is(':checked') ? 'S' : 'N',
            IndSerializable: $('#modalChkSerializable').is(':checked') ? 'S' : 'N',
        };

        if (!payload.DesProducto || !payload.TipProducto || !payload.MarcaId || !payload.AreaGestionId) {
            swal({ text: "Complete los campos obligatorios (*)", icon: "warning" });
            return;
        }

        var url = (modo === 'editar') ? urlEditarProducto : urlCrearProducto;

        try {
            var resp = await $.ajax({
                url, type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload), dataType: 'json'
            });

            if (resp.Ok) {
                swal({ text: resp.Mensaje, icon: "success" });
                bootstrap.Modal.getInstance(document.getElementById('modalNuevoProducto')).hide();
                dtProductos.ajax.reload(null, false);
            } else {
                swal({ text: swalText(resp, 'No se pudo guardar.'), icon: "warning" });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al guardar'), icon: "error" });
        }
    }

    async function eliminarProducto() {
        var $checks = $('#tableProductos tbody .row-checkbox:checked');
        if ($checks.length === 0) {
            swal({ text: "Debe seleccionar al menos un registro para eliminar.", icon: "warning" });
            return;
        }

        var arrAEliminar = $checks.map(function () {
            return { CodProducto: $(this).data('producto') };
        }).get();

        swal({
            text: "¿Está seguro que desea eliminar los registros seleccionados?",
            icon: "warning", buttons: ["Cancelar", "Eliminar"], dangerMode: true
        }).then(async (confirmar) => {
            if (!confirmar) return;
            try {
                var resp = await $.ajax({
                    url: urlEliminarProducto, type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ Productos: arrAEliminar }), dataType: 'json'
                });
                if (resp.Ok) swal({ text: resp.Mensaje, icon: "success" });
                else swal({ text: swalText(resp, 'No se pudo eliminar.'), icon: "warning" });
                dtProductos.ajax.reload(null, false);
            } catch (err) {
                swal({ text: swalText(err, 'Error al eliminar'), icon: "error" });
            }
        });
    }

    const listarMarcas = function () {
        return new Promise((resolve, reject) => {

            const request = {
                Busqueda: ''
            };

            $.ajax({
                url: urlListarMarcas,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const listarAreasGestion = function () {
        return new Promise((resolve, reject) => {

            const request = {
                Busqueda: ''
            };

            $.ajax({
                url: urlListarAreasGestion,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    async function cargarComboMarcas() {
        try {
            const resp = await listarMarcas();

            if (resp.Ok) {
                await cargarCombo($('#cboCodMarcaBuscar'), resp.Data.map(m => ({ text: m.NomMarca, value: m.Id })), { placeholder: 'Todos', todos: true });
                await cargarCombo($('#modalInputCboMarca'), resp.Data.map(m => ({ text: m.NomMarca, value: m.Id })), { placeholder: 'Seleccionar' });

            } else {
                swal({ text: resp.Mensaje, icon: 'error' });
            }
        } catch (err) {
            swal({ text: err, icon: 'error' });
        }
    }

    async function cargarComboAreasGestion() {
        try {
            const resp = await listarAreasGestion();
            if (resp.Ok) {
                await cargarCombo($('#cboCodAreaGestionBuscar'), resp.Data.map(a => ({ text: a.NomAreaGestion, value: a.Id })), { placeholder: 'Todos', todos: true });
                await cargarCombo($('#modalInputCboAreaGestion'), resp.Data.map(a => ({ text: a.NomAreaGestion, value: a.Id })), { placeholder: 'Seleccionar' });
            } else {
                swal({ text: resp.Mensaje, icon: 'error' });
            }
        } catch (err) {
            swal({ text: err, icon: 'error' });
        }
    }

    function initCombosFijos() {

        $('#cboIndActivoBuscar, #cboTipProductoBuscar').select2({
            width: '100%', allowClear: true, minimumResultsForSearch: 0, placeholder: 'Todos'
        });


        $('#modalInputCboTipProducto').select2({
            dropdownParent: $('#modalNuevoProducto'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });
    }

    // ======================== DataTables ========================
    function visualizarDataTableProductos() {
        dtProductos = buildDataTable($('#tableProductos'), {
            url: urlListarProductos,
            filtrosFn: (data) => ({
                IndActivo: $("#cboIndActivoBuscar").val(),
                MarcaId: $("#cboCodMarcaBuscar").val(),
                TipProducto: $("#cboTipProductoBuscar").val(),
                AreaGestionId: $("#cboCodAreaGestionBuscar").val(),
                FiltroVarios: data.search?.value?.trim() || ''
            }),
            columns: [
                {
                    data: null,
                    className: 'text-center',
                    render: function (r) {
                        return '<input type="checkbox" class="row-checkbox" data-producto="' + r.CodProducto + '"/>';
                    }
                },
                { data: "CodProducto", title: "Código" },
                { data: "DesProducto", title: "Descripción" },
                { data: "NomMarca", title: "Marca" },
                { data: "NomModelo", title: "Modelo" },
                { data: "NomTipProducto", title: "Tipo" },
                { data: "NomAreaGestion", title: "Área" },
                {
                    data: "IndActivo", title: "¿Publicado?", className: "text-center",
                    render: d => d === 'S'
                        ? '<i class="fe fe-check text-success fs-6"></i>'
                        : '<i class="fe fe-x text-danger fs-6"></i>'
                }
            ]
        });
    }

    function mostrarDetalleProducto(prod) {
        $('#lblCodProducto').text(prod.CodProducto);
        $('#lblDesProducto').text(prod.DesProducto + ' ' + prod.NomMarca + ' ' + prod.NomModelo);
        $('#cardSeries').show();
        cargarSeries(prod.CodProducto);
    }

    function cargarSeries(codProducto) {
        if (dtSeries) { dtSeries.destroy(); $('#tableSeries tbody').empty(); }
        dtSeries = buildDataTable($('#tableSeries'), {
            url: urlListarSeries,
            filtrosFn: (data) => ({
                codProducto,
                PageNumber: (data.start / data.length) + 1,
                PageSize: data.length,
                FiltroVarios: data.search?.value?.trim() || ''
            }),
            columns: [
                {
                    data: null, className: 'text-center',
                    render: r => {
                        const st = (r.IndEstado || '').toUpperCase();
                        const disabled = (st === 'DE_BAJA') ? 'opacity:.35;pointer-events:none;' : '';
                        return '<i class="fe fe-arrow-down-circle fs-6 text-danger action-baja" ' +
                            'data-id="' + (r.Id || '') + '" data-serie="' + (r.NumSerie || '') + '" ' +
                            'style="cursor:pointer;' + disabled + '" title="Dar de baja"></i>';
                    }
                },
                { data: "NumSerie", title: "N° Serie" },
                {
                    data: "IndEstado", title: "Estado",
                    render: d => {
                        var st = (d || '').toUpperCase();
                        var badge = st === 'DISPONIBLE' ? 'success' :
                            st === 'EN_TRANSITO' ? 'warning' :
                                st === 'EN_USO' ? 'info' :
                                    st === 'DE_BAJA' ? 'danger' : 'secondary';
                        return '<span class="badge bg-' + badge + '">' + (st || '—') + '</span>';
                    }
                },
                { data: "StkActual", title: "Stock" },
                { data: "LocalActual", title: "Local" },
                { data: "FecIngreso", title: "Último Ingreso", render: parseDotNetDate },
                { data: "FecSalida", title: "Última Salida", render: parseDotNetDate }
            ]
        });
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboMarcas();
                await cargarComboAreasGestion();
                initCombosFijos();   // <- inicializa combos fijos
                visualizarDataTableProductos();
            });
        }
    }
}(jQuery);