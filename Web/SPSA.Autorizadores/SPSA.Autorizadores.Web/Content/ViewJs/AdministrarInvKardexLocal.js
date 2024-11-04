var urlListarInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/ListarInvKardexLocal';

var urlModalCrearInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/CrearFormInvKardexLocal';
var urlModalEditarInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/EditarFormInvKardexLocal';

var urlCrearInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/CrearInvKardexLocal';
var urlActualizarInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/ActualizarInvKardexLocal';
var urlEliminarInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/EliminarInvKardexLocal';
var urlObtenerInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/ObtenerInvKardexLocal';
var urlDescargarInvLocal = baseUrl + 'Inventario/InventarioKardexLocal/DescargarInvKardexLocal';

var dtListaKardex = null;
var AdministrarInvKardexLocal = function () {

    const eventos = function () {

        $("#btnIrKardex").click(function () {
            window.location.href = '/Inventario/InventarioKardex'; // Reemplaza con la ruta a la vista
        });

        $("#btnNuevoInvLocal").click(function () {
            abrirModalNuevoInvLocal();           
        });

        $("#btnEditarInvLocal").click(async function () {
            var filasSeleccionada = document.querySelectorAll("#tableLocales tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }
            const local_id = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const objLocal = await obtenerLocal(local_id);

            if (objLocal === undefined) return;

            abrirModalEditarInvLocal(objLocal);
        });

        $("#btnGuardarInvLocal").on("click", async function () {
            var inv_local = {
                Id: $("#txtId").val(),
                Sociedad: $("#txtSociedad").val(),
                NomLocal: $("#txtNomLocal").val()
            };

            if (validarFormInvLocal(inv_local))
                await guardarInvLocal(inv_local, urlCrearInvLocal);
        });

        $("#btnActualizarInvLocal").on("click", async function () {
            var inv_local = {
                Id: $("#txtId").val(),
                Sociedad: $("#txtSociedad").val(),
                NomLocal: $("#txtNomLocal").val()
            };

            if (validarFormInvLocal(inv_local))
                await guardarInvLocal(inv_local, urlActualizarInvLocal);
        });

        $("#btnEliminarInvLocal").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableLocales tbody tr.selected");
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
                    await eliminarInvLocal(request);
                }
            });
        });

        $("#btnDescargarInvLocal").on("click", function () {
            descargarInvLocales();
        });

        $('#tableLocales tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaKardex.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableLocales tbody').on('dblclick', 'tr', function () {
           
            var filasSeleccionada = $(this);

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            //abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
        });

    }


    const validarFormInvLocal = function (invLocal) {
        let validate = true;

        if (invLocal.Id === '' || invLocal.Sociedad === '' || invLocal.NomLocal === '') {
            validate = false;
            $("#formInvLocal").addClass("was-validated");
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

    const abrirModalEditarInvLocal = async function (objActivo) {
        $("#titulomodalInvLocal").html("Editar Activo");
        $("#btnActualizarInvLocal").show();
        $("#btnGuardarInvLocal").hide();

        await cargarFormEditarInvLocal(objActivo);
    }

    const abrirModalNuevoInvLocal = async function () {
        $("#tituloModalInvLocal").html("Ingresar Local");
        $("#btnActualizarInvLocal").hide();
        $("#btnGuardarInvLocal").show();

        const model = {};

        await cargarFormCrearInvLocal(model);
    }

    const cargarFormCrearInvLocal = async function (model) {
        $.ajax({
            url: urlModalCrearInvLocal,
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
                $("#modalInvLocal").find(".modal-body").html(response);
                $("#modalInvLocal").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormEditarInvLocal = async function (model) {
        $.ajax({
            url: urlModalEditarInvLocal,
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
                $("#modalInvLocal").find(".modal-body").html(response);
                $("#modalInvLocal").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarInvLocal = function (invLocal, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: invLocal },
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
                recargarDataTableLocales();
                $("#modalInvLocal").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const obtenerLocal = function (localId) {
        return new Promise((resolve, reject) => {
            const request = {
                Id: localId
            };

            $.ajax({
                url: urlObtenerInvLocal,
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

    const eliminarInvLocal = async function (request) {
        $.ajax({
            url: urlEliminarInvLocal,
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
                recargarDataTableLocales(request);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const descargarInvLocales = function () {

        const request = {
        };

        $.ajax({
            url: urlDescargarInvLocal,
            type: "post",
            data: { request },
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

                const linkSource = `data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                const fileName = response.NombreArchivo;
                downloadLink.href = linkSource;
                downloadLink.download = fileName;
                downloadLink.click();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarDataTableLocales = function (request) {
        if ($.fn.DataTable.isDataTable('#tableLocales')) {
            $('#tableLocales').DataTable().clear().draw();
            $('#tableLocales').DataTable().destroy();
        }
        dtListaKardex = $('#tableLocales').DataTable({
            ajax: {
                url: urlListarInvLocal,
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
                        text: 'Error al listar locales: ' + jqXHR,
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
                { data: "Sociedad" },
                { data: "NomLocal" },
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
                recargarDataTableLocales();
            });
        }
    }

}(jQuery);
