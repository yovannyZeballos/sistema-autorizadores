var urlListarInvKardex = baseUrl + 'Inventario/InventarioKardex/ListarInvKardex';

var urlModalCrearInvKardex = baseUrl + 'Inventario/InventarioKardex/CrearFormInvKardex';
var urlModalEditarInvKardex = baseUrl + 'Inventario/InventarioKardex/EditarFormInvKardex';

var urlCrearInvKardex = baseUrl + 'Inventario/InventarioKardex/CrearInvKardex';
var urlActualizarInvKardex = baseUrl + 'Inventario/InventarioKardex/ActualizarInvKardex';
var urlEliminarInvKardex = baseUrl + 'Inventario/InventarioKardex/EliminarInvKardex';
var urlObtenerInvKardex = baseUrl + 'Inventario/InventarioKardex/ObtenerInvKardex';

var dtListaKardex = null;
var AdministrarInvKardex = function () {

    const eventos = function () {

        $("#btnIrActivos").click(function () {
            window.location.href = '/Inventario/InventarioKardexActivo'; // Reemplaza con la ruta a la vista

        });

        $("#btnNuevoInvKardex").click(function () {

            //if (validarNuevoInvActivo()) {
            abrirModalNuevoInvKardex();           
            //}
        });

        $("#btnEditarInvKardex").click(async function () {

            var filasSeleccionada = document.querySelectorAll("#tableKardex tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }
            const kardex_id = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const objKardex = await obtenerKardex(kardex_id);

            if (objKardex === undefined) return;

            abrirModalEditarInvKardex(objKardex);
        });

        $("#btnGuardarInvKardex").on("click", async function () {
            var inv_kardex = {
                ActivoId: $("#cboActivoId").val(),
                Kardex: $("#cboIndKardex").val(),
                Fecha: $("#txtFecha").val(),
                Guia: $("#txtGuia").val(),
                Serie: $("#txtSerie").val(),
                OrigenId: $("#cboOrigen").val(),
                DestinoId: $("#cboDestino").val(),
                Tk: $("#txtTk").val(),
                Cantidad: $("#txtCantidad").val(),
                TipoStock: $("#txtTipoStock").val(),
                Oc: $("#txtOc").val(),
                Sociedad: $("#txtSociedad").val()
            };

            if (validarFormInvKardex(inv_kardex))
                await guardarInvKardex(inv_kardex, urlCrearInvKardex);
        });

        $("#btnActualizarInvKardex").on("click", async function () {
            var inv_kardex = {
                Id: $("#txtId").val(),
                ActivoId: $("#cboActivoId").val(),
                Kardex: $("#cboIndKardex").val(),
                Fecha: $("#txtFecha").val(),
                Guia: $("#txtGuia").val(),
                Serie: $("#txtSerie").val(),
                OrigenId: $("#cboOrigen").val(),
                DestinoId: $("#cboDestino").val(),
                Tk: $("#txtTk").val(),
                Cantidad: $("#txtCantidad").val(),
                TipoStock: $("#txtTipoStock").val(),
                Oc: $("#txtOc").val(),
                Sociedad: $("#txtSociedad").val()
            };

            if (validarFormInvKardex(inv_kardex))
                await guardarInvKardex(inv_kardex, urlActualizarInvKardex);
        });

        $("#btnEliminarInvKardex").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableKardex tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            //const kardex_id = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const request = {
                Id: filasSeleccionada[0].querySelector('td:nth-child(1)').textContent
            };

            swal({
                title: "¿Estás seguro?",
                text: "No podrás revertir esto",
                icon: "warning",
                buttons: {
                    cancel: {
                        text: "Cancelar",
                        value: null,
                        visible: true,
                        className: "",
                        closeModal: true,
                    },
                    confirm: {
                        text: "Sí, eliminarlo",
                        value: true,
                        visible: true,
                        className: "",
                        closeModal: true
                    }
                }
            }).then(async (isConfirm) => {
                if (isConfirm) {
                    await eliminarInvKardex(request);
                }
            });
        });

        $('#tableKardex tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaKardex.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableKardex tbody').on('dblclick', 'tr', function () {
           
            var filasSeleccionada = $(this);

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            //console.log("child(1)" + NUM_CAJA);
            //console.log("child(2)" + COD_ACTIVO);

            //abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
        });

    }


    const validarFormInvKardex = function (invKardex) {
        let validate = true;

        if (invKardex.ActivoId === '' || invKardex.Kardex === '' || invKardex.Fecha === '' || invKardex.Guia === '' || invKardex.Serie === ''
            || invKardex.Origen === '' || invKardex.Destino === '') {
            validate = false;
            $("#formInvKardex").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarSelecion = function (count) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar un registro",
                icon: "warning",
            });
            return false;
        }
        return true;
    }

    const abrirModalEditarInvKardex = async function (objKardex) {
        $("#tituloModalInvKardex").html("Editar Activo");
        $("#btnActualizarInvKardex").show();
        $("#btnGuardarInvKardex").hide();

        if (objKardex.Fecha != "" && objKardex.Fecha != null) {
            let timestamp = parseInt(objKardex.Fecha.match(/\d+/)[0], 10);
            let date = new Date(timestamp);
            let formattedDate = date.toISOString().split('T')[0];
            objKardex.Fecha = formattedDate;
        }

        await cargarFormEditarInvKardex(objKardex);
    }

    const abrirModalNuevoInvKardex = async function () {
        $("#tituloModalInvKardex").html("Ingresar Kardex");
        $("#btnActualizarInvKardex").hide();
        $("#btnGuardarInvKardex").show();

        const model = {};

        await cargarFormCrearInvKardex(model);
    }

    const cargarFormCrearInvKardex = async function (model) {
        $.ajax({
            url: urlModalCrearInvKardex,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalInvKardex").find(".modal-body").html(response);
                $("#modalInvKardex").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormEditarInvKardex = async function (model) {
        $.ajax({
            url: urlModalEditarInvKardex,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalInvKardex").find(".modal-body").html(response);
                $("#modalInvKardex").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarInvKardex = function (invKardex, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: invKardex },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                recargarDataTableKardex();
                $("#modalInvKardex").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const obtenerKardex = function (kardexId) {
        return new Promise((resolve, reject) => {
            const request = {
                Id: kardexId
            };

            $.ajax({
                url: urlObtenerInvKardex,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response.Data)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const eliminarInvKardex = async function (request) {
        $.ajax({
            url: urlEliminarInvKardex,
            type: "post",
            data: { request },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                swal("Eliminado!", "Registro eliminado exitosamente.", "success");
                recargarDataTableKardex(request);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarDataTableKardex = function (request) {
        if ($.fn.DataTable.isDataTable('#tableKardex')) {
            $('#tableKardex').DataTable().clear().draw();
            $('#tableKardex').DataTable().destroy();
        }
        dtListaKardex = $('#tableKardex').DataTable({
            ajax: {
                url: urlListarInvKardex,
                type: "post",
                dataType: "JSON",
                dataSrc: "Data",
                data: { request },
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal({
                        text: 'Error al listar kardex: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            //responsive: true, // Habilitar el modo responsivo
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            columns: [
                { data: "Id" },
                { data: "Kardex" },
                {
                    data: "Fecha",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.match(/\d+/)[0]);
                            var fecha = new Date(timestamp);
                            return fecha.toLocaleDateString();
                        }
                        return '';
                    }
                },
                { data: "ActivoArea" },
                { data: "ActivoTipo" },
                { data: "Guia" },
                { data: "ActivoDescripcion" },
                { data: "ActivoModelo" },
                { data: "ActivoMarca" },
                { data: "Serie" },
                { data: "OrigenLocal" },
                { data: "DestinoLocal" },
                { data: "Tk" },
                { data: "Cantidad" },
                { data: "TipoStock" },
                { data: "Oc" },
                { data: "Sociedad" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            rowCallback: function (row, data, index) {
            },
            bAutoWidth: false
        });
    }


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                //await cargarComboEmpresa();
                recargarDataTableKardex();
            });
        }
    }

}(jQuery);
