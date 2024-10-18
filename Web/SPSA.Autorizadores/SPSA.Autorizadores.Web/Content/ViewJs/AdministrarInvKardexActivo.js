var urlListarInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/ListarInvKardexActivo';

var urlModalCrearInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/CrearFormInvKardexActivo';
var urlModalEditarInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/EditarFormInvKardexActivo';

var urlCrearInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/CrearInvKardexActivo';
var urlActualizarInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/ActualizarInvKardexActivo';
var urlEliminarInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/EliminarInvKardexActivo';
var urlObtenerInvActivo = baseUrl + 'Inventario/InventarioKardexActivo/ObtenerInvKardexActivo';

var dtListaKardex = null;
var AdministrarInvKardexActivo = function () {

    const eventos = function () {

        $("#btnIrKardex").click(function () {
            window.location.href = '/Inventario/InventarioKardex'; // Reemplaza con la ruta a la vista

        });

        $("#btnNuevoInvActivo").click(function () {
            abrirModalNuevoInvActivo();           
        });

        $("#btnEditarInvActivo").click(async function () {

            var filasSeleccionada = document.querySelectorAll("#tableActivos tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }
            const activo_id = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const objActivo = await obtenerActivo(activo_id);

            if (objActivo === undefined) return;

            abrirModalEditarInvActivo(objActivo);
        });

        $("#btnGuardarInvActivo").on("click", async function () {
            var inv_activo = {
                Id: $("#txtId").val(),
                Modelo: $("#txtModelo").val(),
                Descripcion: $("#txtDescripcion").val(),
                Marca: $("#txtMarca").val(),
                Area: $("#cboArea").val(),
                Tipo: $("#cboTipo").val()
            };

            if (validarFormInvActivo(inv_activo))
                await guardarInvActivo(inv_activo, urlCrearInvActivo);
        });

        $("#btnActualizarInvActivo").on("click", async function () {
            var inv_activo = {
                Id: $("#txtId").val(),
                Modelo: $("#txtModelo").val(),
                Descripcion: $("#txtDescripcion").val(),
                Marca: $("#txtMarca").val(),
                Area: $("#cboArea").val(),
                Tipo: $("#cboTipo").val()
            };

            if (validarFormInvActivo(inv_activo))
                await guardarInvActivo(inv_activo, urlActualizarInvActivo);
        });

        $("#btnEliminarInvActivo").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableActivos tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

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
                    await eliminarInvActivo(request);
                }
            });
        });

        $('#tableActivos tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaKardex.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableActivos tbody').on('dblclick', 'tr', function () {
           
            var filasSeleccionada = $(this);

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            console.log("child(1)" + NUM_CAJA);
            console.log("child(2)" + COD_ACTIVO);

            //abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
        });

    }


    const validarFormInvActivo = function (invActivo) {
        let validate = true;

        if (invActivo.Id === '' || invActivo.Modelo === '' || invActivo.Descripcion === '' || invActivo.Marca === '' || invActivo.Area === ''
            || invActivo.Tipo === '') {
            validate = false;
            $("#formInvActivo").addClass("was-validated");
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

    const abrirModalEditarInvActivo = async function (objActivo) {
        $("#titulomodalInvActivo").html("Editar Activo");
        $("#btnActualizarInvActivo").show();
        $("#btnGuardarInvActivo").hide();

        //if (objKardex.Fecha != "" && objKardex.Fecha != null) {
        //    let timestamp = parseInt(objKardex.Fecha.match(/\d+/)[0], 10);
        //    let date = new Date(timestamp);
        //    let formattedDate = date.toISOString().split('T')[0];
        //    objKardex.Fecha = formattedDate;
        //}

        await cargarFormEditarInvActivo(objActivo);
    }

    const abrirModalNuevoInvActivo = async function () {
        $("#tituloModalInvActivo").html("Ingresar Activo");
        $("#btnActualizarInvActivo").hide();
        $("#btnGuardarInvActivo").show();

        const model = {};

        await cargarFormCrearInvActivo(model);
    }

    const cargarFormCrearInvActivo = async function (model) {
        $.ajax({
            url: urlModalCrearInvActivo,
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
                $("#modalInvActivo").find(".modal-body").html(response);
                $("#modalInvActivo").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormEditarInvActivo = async function (model) {
        $.ajax({
            url: urlModalEditarInvActivo,
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
                $("#modalInvActivo").find(".modal-body").html(response);
                $("#modalInvActivo").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarInvActivo = function (invActivo, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: invActivo },
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
                recargarDataTableActivo();
                $("#modalInvActivo").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const obtenerActivo = function (activoId) {
        return new Promise((resolve, reject) => {
            const request = {
                Id: activoId
            };

            $.ajax({
                url: urlObtenerInvActivo,
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

    const eliminarInvActivo = async function (request) {
        $.ajax({
            url: urlEliminarInvActivo,
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
                recargarDataTableActivo(request);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarDataTableActivo = function (request) {
        if ($.fn.DataTable.isDataTable('#tableActivos')) {
            $('#tableActivos').DataTable().clear().draw();
            $('#tableActivos').DataTable().destroy();
        }
        dtListaKardex = $('#tableActivos').DataTable({
            ajax: {
                url: urlListarInvActivo,
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
                        text: 'Error al listar activos: ' + jqXHR,
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
                { data: "Modelo" },
                { data: "Descripcion" },
                { data: "Marca" },
                { data: "Area" },
                { data: "Tipo" }
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
                recargarDataTableActivo();
            });
        }
    }

}(jQuery);
