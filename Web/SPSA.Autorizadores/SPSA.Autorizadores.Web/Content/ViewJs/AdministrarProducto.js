var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProductos = baseUrl + 'Inventario/Productos/ListarPaginado';
var urlListarSeries = baseUrl + 'Inventario/SeriesProducto/ListarPaginado';
var urlListarKardex = baseUrl + 'Inventario/Kardex/ListarPaginadoPorProducto';
var urlListarMarcas = baseUrl + 'Inventario/Marcas/Listar';
var urlListarAreasGestion = baseUrl + 'Inventario/AreasGestion/Listar';

var urlCrearProducto = baseUrl + 'Inventario/Productos/Crear';
var urlCrearSerie = baseUrl + 'Inventario/SeriesProducto/Crear';
var urlEliminarProducto = baseUrl + 'Inventario/Productos/Eliminar';
var urlEditarProducto = baseUrl + 'Inventario/Productos/Editar';

var dataTableProductos = null;

var AdministrarProducto = function () {

    const eventos = function () {

        $("#serieCodEmpresa").on("change", async function () {
            await cargarComboLocales();
        });

        // Cuando marcas un solo checkbox, carga el detalle
        $('#tableProductos tbody').on('change', '.row-checkbox', function () {
            var $checks = $('#tableProductos tbody .row-checkbox:checked');
            if ($checks.length === 1) {
                var data = $('#tableProductos').DataTable().row($checks.closest('tr')).data();
                mostrarDetalleProducto(data);
            } else {
                $('#cardSeries').hide();
            }
        });

        $("#btnBuscar").on("click", function (e) {
            var table = $('#tableProductos').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $('#btnNuevoProducto').on('click', function () {
            $('#formNuevoProducto')[0].reset();

            $('#btnGuardarNuevoModal').show();
            $('#btnGuardarCambiosModal').hide();

            $('#modalInputCodProducto').val('');
            $('#modalInputDesProducto').val('');
            $('#modalInputCboMarca').val('');
            $('#modalInputCboTipProducto').val('');
            $('#modalInputCboAreaGestion').val('');
            $('#modalChkNuevo').prop('checked', true);
            $('#modalInputStkMinimo').val('0');
            $('#modalInputStkMaximo').val('0');
            $('#modalChkActivo').prop('checked', true);
            $('#modalChkSerializable').prop('checked', true);

            $('#modalInputCodProducto').prop('disabled', false);

            var modal = new bootstrap.Modal(document.getElementById('modalNuevoProducto'));
            modal.show();
        });

        $('#btnEditarProducto').on('click', async function () {
            $('#formNuevoProducto')[0].reset();
            document.getElementById('modalTituloProductoLabel').textContent = 'Editar Producto';
            $('#btnGuardarNuevoModal').hide();
            $('#btnGuardarCambiosModal').show();

            var $checks = $('#tableProductos tbody .row-checkbox:checked');
            if ($checks.length !== 1) {
                return swal({
                    text: $checks.length === 0
                        ? "Seleccione un registro para modificar."
                        : "Seleccione sólo un registro.",
                    icon: "warning"
                });
            }

            var $tr = $checks.closest('tr');
            var data = $('#tableProductos').DataTable().row($tr).data();

            $('#modalInputCodProducto').val(data.CodProducto);
            $('#modalInputDesProducto').val(data.DesProducto);
            $('#modalInputCboMarca').val(data.MarcaId).trigger('change');
            $('#modalInputCboTipProducto').val(data.TipProducto).trigger('change');
            $('#modalInputCboAreaGestion').val(data.AreaGestionId).trigger('change');
            $('#modalInputNomModelo').val(data.NomModelo);
            $('#modalInputStkMinimo').val(data.StkMinimo);
            $('#modalInputStkMaximo').val(data.StkMaximo);
            $('#modalChkActivo').prop('checked', data.IndActivo === 'S');
            $('#modalChkSerializable').prop('checked', data.IndSerializable === 'S');

            $('#modalInputCodProducto').prop('disabled', true);

            var modal = new bootstrap.Modal(document.getElementById('modalNuevoProducto'));
            modal.show();
        });

        $('#btnGuardarNuevoModal').on('click', async function () {
            guardarProducto({ modo: 'crear' });
        });

        $('#btnGuardarCambiosModal').on('click', async function () {
            guardarProducto({ modo: 'editar' });
        });

        $('#btnEliminarProducto').on('click', async function () {
            var $checks = $('#tableProductos tbody .row-checkbox:checked');
            if ($checks.length === 0) {
                swal({ text: "Debe seleccionar al menos un registro para eliminar.", icon: "warning" });
                return;
            }

            var arrAEliminar = [];
            $checks.each(function () {
                var $ch = $(this);
                arrAEliminar.push({
                    CodProducto: $ch.data('producto')
                });
            });

            swal({
                text: "¿Está seguro que desea eliminar los registros seleccionados?",
                icon: "warning",
                buttons: ["Cancelar", "Eliminar"],
                dangerMode: true
            }).then(async (confirmar) => {
                if (!confirmar) return;

                var payload = {
                    Productos: arrAEliminar
                };

                try {
                    var response = await $.ajax({
                        url: urlEliminarProducto,
                        type: "POST",
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify(payload),
                        dataType: 'json'
                    });

                    if (response.Ok) {
                        swal({ text: response.Mensaje, icon: "success" });
                    } else {
                        swal({ text: response.Mensaje, icon: "warning" });
                    }

                    $('#tableProductos').DataTable().ajax.reload(null, false);
                    $('#checkAllRows').prop('checked', false);

                } catch (err) {
                    swal({ text: err.responseText || err.statusText, icon: "error" });
                }
            });
        });

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

    async function guardarProducto({ modo }) {
        var codProducto = $('#modalInputCodProducto').val().trim();
        var desProducto = $('#modalInputDesProducto').val().trim();
        
        var marcaId = $('#modalInputCboMarca').val().trim();
        var tipProducto = $('#modalInputCboTipProducto').val();
        var areaGestionId = $('#modalInputCboAreaGestion').val().trim();
        var nomModelo = $('#modalInputNomModelo').val().trim();
        var stkMinimo = $('#modalInputStkMinimo').val().trim();
        var stkMaximo = $('#modalInputStkMaximo').val().trim();
        var activo = $('#modalChkActivo').is(':checked') ? 'S' : 'N';
        var serializable = $('#modalChkSerializable').is(':checked') ? 'S' : 'N';

        if (desProducto === '') {
            swal({ text: "La descripción es obligatoria.", icon: "warning" });
            return;
        }

        if (tipProducto === '') {
            swal({ text: "Tipo es obligatoria.", icon: "warning" });
            return;
        }

        if (marcaId === '') {
            swal({ text: "Marca es obligatoria.", icon: "warning" });
            return;
        }

        if (areaGestionId === '') {
            swal({ text: "Área gestión es obligatoria.", icon: "warning" });
            return;
        }

        const urlEleccion = (modo === 'editar') ? urlEditarProducto
                                                : urlCrearProducto;

        var payload = {
            CodProducto: codProducto,
            DesProducto: desProducto,
            MarcaId: marcaId,
            TipProducto: tipProducto,
            AreaGestionId: areaGestionId,
            NomModelo: nomModelo,
            StkMinimo: stkMinimo,
            StkMaximo: stkMaximo,
            IndActivo: activo,
            IndSerializable: serializable,
        };

        try {
            var response = await $.ajax({
                url: urlEleccion,
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (response.Ok) {
                swal({ text: response.Mensaje, icon: "success" });

                var modalEl = document.getElementById('modalNuevoProducto');
                var modalObj = bootstrap.Modal.getInstance(modalEl);
                modalObj.hide();

                $('#tableProductos').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: response.Mensaje, icon: "warning" });
            }
        } catch (err) {
            swal({ text: err.responseText || err.statusText, icon: "error" });
        }
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

    const visualizarDataTableProductos = function () {

        $('#tableProductos').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    IndActivo: $("#cboIndActivoBuscar").val(),
                    MarcaId: $("#cboCodMarcaBuscar").val(),
                    TipProducto: $("#cboTipProductoBuscar").val(),
                    AreaGestionId: $("#cboCodAreaGestionBuscar").val(),
                    FiltroVarios: $("#txtFiltroVariosBuscar").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarProductos,
                    type: "GET",
                    data: params,
                    dataType: "json",
                    success: function (response) {
                        if (response.Ok) {
                            var pagedData = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pagedData.TotalRecords,
                                recordsFiltered: pagedData.TotalRecords,
                                data: pagedData.Items
                            });
                        } else {
                            callback({
                                draw: data.draw,
                                recordsTotal: 0,
                                recordsFiltered: 0,
                                data: []
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        swal({
                            text: jqXHR.responseText,
                            icon: "error",
                        });
                        callback({
                            draw: data.draw,
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            data: []
                        });
                    }
                });
            },
            columns: [
                {
                    data: null,
                    orderable: false,
                    className: 'text-center',
                    render: function (data, type, row) {
                        return ''
                            + '<input type="checkbox" class="row-checkbox" '
                            + 'data-producto="' + row.CodProducto + '" '
                            + '/>';
                    }
                },
                { title: "Código", data: "CodProducto" },
                { title: "Descripción", data: "DesProducto" },
                { title: "Marca", data: "NomMarca" },
                { title: "Modelo", data: "NomModelo" },
                { title: "Tipo", data: "NomTipProducto" },
                { title: "Área", data: "NomAreaGestion" },
                {
                    title: "¿Activo?",
                    data: "IndActivo",
                    className: 'text-center',
                    render: function (data, type, row) {
                        if (data === 'S') {
                            return '<i class="fe fe-check text-success fs-6"></i>';
                        } else {
                            return '<i class="fe fe-x text-danger fs-6"></i>';
                        }
                    }
                },
                //{ title: "Stock Min.", data: "StkMinimo" },
                //{ title: "Stock Max.", data: "StkMaximo" },
                //{ title: "U. Creacion", data: "UsuCreacion" },
                //{
                //    title: "F. Creacion", data: "FecCreacion",
                //    render: function (data, type, row) {
                //        if (data) {
                //            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                //            var date = new Date(timestamp);
                //            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                //        }
                //        return "";
                //    }
                //},
                //{ title: "U. Modifica", data: "UsuModifica" },
                //{
                //    title: "F. Modifica", data: "FecModifica",
                //    render: function (data, type, row) {
                //        if (data) {
                //            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                //            var date = new Date(timestamp);
                //            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                //        }
                //        return "";
                //    }
                //},
                //{ title: "U. Elimina", data: "UsuElimina" },
                //{
                //    title: "F. Elimina", data: "FecElimina",
                //    render: function (data, type, row) {
                //        if (data) {
                //            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                //            var date = new Date(timestamp);
                //            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                //        }
                //        return "";
                //    }
                //},
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
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
        });

        $('#checkAllRows').on('change', function () {
            var isChecked = $(this).is(':checked');
            $('#tableProductos tbody .row-checkbox').each(function () {
                $(this).prop('checked', isChecked);
            });
        });

        $('#tableProductos tbody').on('change', '.row-checkbox', function () {
            if (!$(this).is(':checked')) {
                $('#checkAllRows').prop('checked', false);
            }
        });

    };

    function mostrarDetalleProducto(prod) {
        $('#lblCodProducto').text(prod.CodProducto);
        $('#lblDesProducto').text(prod.DesProducto + ' ' + prod.NomMarca + ' ' + prod.NomModelo);
        $('#cardSeries').show();

        cargarSeries(prod.CodProducto);
    }

    var dtSeries = null;
    function cargarSeries(codProducto) {
        if (dtSeries) { dtSeries.destroy(); $('#tableSeries tbody').empty(); }
        dtSeries = $('#tableSeries').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback) {
                var pageNumber = (data.start / data.length) + 1;
                var params = { codProducto: codProducto, PageNumber: pageNumber, PageSize: data.length };
                $.getJSON(urlListarSeries, params, function (response) {
                    if (response.Ok) {
                        callback({
                            draw: data.draw,
                            recordsTotal: response.Data.TotalRecords,
                            recordsFiltered: response.Data.TotalRecords,
                            data: response.Data.Items
                        });
                    } else {
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    }
                }).fail(function (jq) {
                    swal({ text: jq.responseText || jq.statusText, icon: "error" });
                    callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                });
            },
            columns: [
                { data: "NumSerie" },
                { data: "StkActual" },
                { data: "LocalActual" },
                {
                    title: "Último Ingreso", data: "FecIngreso",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                {
                    title: "Última Salida", data: "FecSalida",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)"
            },
            scrollY: '500px',
            scrollX: true,
            autoWidth: false,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });
    }


    async function cargarCombo($select, data, { placeholder = 'Seleccionar…', todos = false } = {}) {
        $select.empty();

        // Opción inicial
        if (todos) {
            $select.append(new Option('Todos', ''));
        } else {
            $select.append(new Option('', '')); // vacío para allowClear
        }

        // Poblar opciones
        if (data && data.length) {
            data.forEach(d => {
                $select.append(new Option(d.text, d.value));
            });
        }

        // Inicializar con select2
        if ($.fn.select2) {
            $select.select2({
                width: '100%',
                placeholder: placeholder,
                allowClear: true,
                minimumResultsForSearch: 0,
                dropdownParent: $select.closest('.modal').length ? $select.closest('.modal') : $(document.body)
            });
        }

        $select.val('').trigger('change');
    }

    function initCombosFijos() {
        // Filtro en la pantalla principal
        $('#cboIndActivoBuscar').select2({
            width: '100%',
            placeholder: 'Todos',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        $('#cboTipProductoBuscar').select2({
            width: '100%',
            placeholder: 'Todos',
            allowClear: true,
            minimumResultsForSearch: 0
        });

        // Tipo en el modal de producto
        $('#modalInputCboTipProducto').select2({
            dropdownParent: $('#modalNuevoProducto'),
            width: '100%',
            placeholder: 'Seleccionar',
            allowClear: true,
            minimumResultsForSearch: 0
        });
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                await cargarComboMarcas();
                await cargarComboAreasGestion();
                initCombosFijos();   // <- inicializa combos fijos
                visualizarDataTableProductos();
            });
        }
    }
}(jQuery);