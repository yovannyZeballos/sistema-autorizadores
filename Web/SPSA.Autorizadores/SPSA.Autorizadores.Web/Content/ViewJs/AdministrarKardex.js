var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarProductos = baseUrl + 'Inventario/Productos/Listar';
var urlListarSeries = baseUrl + 'Inventario/SeriesProducto/ListarPorProducto';
var urlListarKardex = baseUrl + 'Inventario/Kardex/ListarPaginado';
var urlRegistrar = baseUrl + 'Inventario/Kardex/Registrar';

//var dataTableProductos = null;

var AdministrarKardex = function () {

    const eventos = function () {

        $("#movEmpDest").on("change", async function () {
            await cargarComboLocales();
        });

        $("#movCodProducto").on("change", async function () {
            await cargarComboSeries();
        });

        $("#btnBuscar").on("click", function (e) {
            var table = $('#tableKardex').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $('#btnNuevoMov').on('click', function () {
            limpiarModal();
            var modal = new bootstrap.Modal(document.getElementById('modalMovimiento'));
            modal.show();
        });

        $('#btnGuardarMov').on('click', guardarMovimiento);
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
            const codEmpresa = $("#movEmpDest").val();

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

    const listarProductos = function () {
        return new Promise((resolve, reject) => {

            const request = {
            };

            $.ajax({
                url: urlListarProductos,
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

    const listarSeriesPorProducto = function () {
        return new Promise((resolve, reject) => {

            const codProducto = $("#movCodProducto").val();

            if (!codProducto) return resolve();

            const request = {
                CodProducto: codProducto
            };

            $.ajax({
                url: urlListarSeries,
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
                $('#movEmpDest').empty().append('<option value="">Seleccionar...</option>');
                $('#movLocDest').empty().append('<option value="">Seleccionar...</option>');
                response.Data.map(empresa => {
                    $('#movEmpDest').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
                });
            } else {
                swal({ text: String(response && response.Mensaje || 'Error al listar empresas'), icon: "error" });
                return;
            }
        } catch (error) {
            swal({ text: String(error && (error.Mensaje || error.message || error.responseText || error.statusText) || 'Error al listar empresas'), icon: "error" });
        }
    }

    const cargarComboLocales = async function () {

        try {
            $('#movLocDest').empty().append('<option value=""></option>').val('').trigger('change');

            const response = await listarLocales();

            if (response === undefined) return;

            if (response.Ok) {

                /*$('#movLocDest').empty().append('<option value="">Seleccionar</option>');*/
                response.Data.map(local => {
                    $('#movLocDest').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
                });
            } else {
                swal({ text: String(response && response.Mensaje || 'Error al listar locales'), icon: "error" });
                return;
            }
        } catch (error) {
            swal({ text: String(error && (error.Mensaje || error.message || error.responseText || error.statusText) || 'Error al listar locales'), icon: "error" });
        }
    }

    const cargarComboProductos = async function () {

        try {
            const response = await listarProductos();

            if (response.Ok) {
                $('#movCodProducto').empty().append('<option value="">Seleccionar...</option>');
                $('#movNumSerie').empty().append('<option value="">Seleccionar...</option>');
                response.Data.map(producto => {
                    const texto = `${producto.DesProducto} - ${producto.NomMarca} - ${producto.NomModelo ?? ''}`.trim();

                    $('#movCodProducto').append($('<option>', { value: producto.CodProducto, text: texto }));
                });
            } else {
                swal({ text: String(response && response.Mensaje || 'Error al listar productos'), icon: "error" });
                return;
            }
        } catch (error) {
            swal({ text: String(error && (error.Mensaje || error.message || error.responseText || error.statusText) || 'Error al listar productos'), icon: "error" });
        }
    }

    const cargarComboSeries = async function () {

        try {
            $('#movNumSerie').empty().append('<option value=""></option>').val('').trigger('change');

            const response = await listarSeriesPorProducto();

            if (response === undefined) return;

            if (response.Ok) {

                /*$('#movNumSerie').empty().append('<option value="">Seleccionar</option>');*/
                response.Data.map(serie => {
                    $('#movNumSerie').append($('<option>', { value: serie.Id, text: serie.NumSerie }));
                });
            } else {
                swal({ text: String(response && response.Mensaje || 'Error al listar series'), icon: "error" });
                return;
            }
        } catch (error) {
            swal({ text: String(error && (error.Mensaje || error.message || error.responseText || error.statusText) || 'Error al listar series'), icon: "error" });
        }
    }

    function limpiarModal() {
        //$('#movTipo').val('INGRESO');
        $('#movFecha').val(new Date().toISOString().slice(0, 10));
        $('#movCantidad').val('1');

        // limpia selects
        $('#movTipo').val('').trigger('change');
        $('#movAreaGestion').val('').trigger('change');
        $('#movClaseStock').val('').trigger('change');
        $('#movEstadoStock').val('').trigger('change');

        $('#movEmpDest').val('').trigger('change');
        $('#movLocDest').val('').trigger('change');
        $('#movCodProducto').val('').trigger('change');
        $('#movNumSerie').val('').trigger('change');

        // limpia textos
        $('#movGuia,#movOrdenCompra,#movTicket,#movObs,#movCodActivoo').val('');
    }

    async function guardarMovimiento() {

        var payload = {
            CodProducto: $('#movCodProducto').val(),
            SerieProductoId: $('#movNumSerie').val(),
            TipoMovimiento: $('#movTipo').val(),
            Fecha: $('#movFecha').val(),
            DesAreaGestion: $('#movAreaGestion').val(),
            DesClaseStock: $('#movClaseStock').val(),
            DesEstadoStock: $('#movEstadoStock').val(),
            CodEmpresaDestino: $('#movEmpDest').val().trim(),
            CodLocalDestino: $('#movLocDest').val().trim(),
            Cantidad: $('#movCantidad').val(),
            NumGuia: $('#movGuia').val().trim(),
            OrdenCompra: $('#movOrdenCompra').val().trim(),
            NumTicket: $('#movTicket').val().trim(),
            Observaciones: $('#movObs').val().trim()
        };

        if (!payload.TipoMovimiento || !payload.CodProducto || !payload.SerieProductoId || !payload.Fecha
            || !payload.CodEmpresaDestino || !payload.CodLocalDestino || !payload.Cantidad || !payload.DesAreaGestion
            || !payload.DesClaseStock || !payload.DesEstadoStock) {
            swal({ text: "Complete los campos obligatorios (*)", icon: "warning" });
            return;
        }

        if (payload.Cantidad === '0') {
            swal({ text: "Cantidad debe ser mayor a cero", icon: "warning" });
            return;
        }

        try {
            var resp = await $.ajax({
                url: urlRegistrar,
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (resp.Ok) {
                swal({ text: resp.Mensaje, icon: "success" });
                bootstrap.Modal.getInstance(document.getElementById('modalMovimiento')).hide();

                $('#tableKardex').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: resp.Mensaje, icon: "warning" });
            }
        } catch (e) {
            swal({ text: e.responseText || e.statusText, icon: "error" });
        }
    }

    function visualizarDataTableKardex() {

        $('#tableKardex').DataTable({
            serverSide: true,
            processing: true,
            searching: false,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    TipoMovimiento: $("#cboTipo").val(),
                    FechaDesde: $("#fDesde").val(),
                    FechaHasta: $("#fHasta").val(),
                    FiltroVarios: $("#txtGuiaOc").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarKardex,
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
                /*{ data: "Fecha" },*/
                {
                    title: "Fecha", data: "Fecha",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { data: "TipoMovimiento" },
                { data: "DesAreaGestion" },
                { data: "DesProducto" },
                { data: "NumSerie" },
                { data: "NumGuia" },
                { data: "LocalOrigen" },
                { data: "LocalDestino" },
                { data: "CodActivo" },
                { data: "Observaciones" }
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
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                await cargarComboProductos();
                visualizarDataTableKardex();
            });
        }
    }
}(jQuery);