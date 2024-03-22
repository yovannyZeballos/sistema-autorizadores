var urlListarApertura = baseUrl + 'Aperturas/AdministrarApertura/ListarApertura';
var urlModalCrearEditarApertura = baseUrl + 'Aperturas/AdministrarApertura/CrearEditarApertura';
var urlCrearApertura = baseUrl + 'Aperturas/AdministrarApertura/CrearApertura';
var urlActualizarApertura = baseUrl + 'Aperturas/AdministrarApertura/ActualizarApertura';
var urlImportarApertura = baseUrl + 'Aperturas/AdministrarApertura/ImportarExcelApertura';
var urlDescargarAperturas = baseUrl + 'Aperturas/AdministrarApertura/DescargarExcelApertura';
var urlObtenerApertura = baseUrl + 'Aperturas/AdministrarApertura/ObtenerApertura';

var dataTableAperturas = null;

const AdministrarLocalAperturas = function () {

    var inicalizarTablaAperturas = true;

    var eventos = function () {

        $("#btnNuevaApertura").on('click', function () {
            abrirModalNuevaApertura();
        });

        $("#btnEditarApertura").on("click", async function () {

            $("#tituloModalApertura").html("Editar Local Apertura");
            $("#btnActualizarApertura").show();
            $("#btnGuardarApertura").hide();

            var filasSeleccionada = document.querySelectorAll("#tableAperturas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            var codLocalPMM = filasSeleccionada[0].querySelector('td:first-child').textContent;
            abrirModalEditarCadena(codLocalPMM);
            $("#txtCodLocalPMM").prop("disabled", true);
        });

        $("#btnGuardarApertura").on("click", async function () {
            var apertura = {
                CodLocalPMM: $("#txtCodLocalPMM").val(),
                NomLocalPMM: $("#txtNomLocalPMM").val(),
                CodLocalSAP: $("#txtCodLocalSAP").val(),
                NomLocalSAP: $("#txtNomLocalSAP").val(),
                CodLocalSAPNew: $("#txtCodLocalSAPNew").val(),
                CodLocalOfiplan: $("#txtCodLocalOfiplan").val(),
                NomLocalOfiplan: $("#txtNomLocalOfiplan").val(),
                Administrador: $("#txtAdministrador").val(),
                NumTelefono: $("#txtNumTelefono").val(),
                Email: $("#txtEmail").val(),
                Direccion: $("#txtDireccion").val(),
                Ubigeo: $("#txtUbigeo").val(),
                CodComercio: $("#txtCodComercio").val(),
                CodCentroCosto: $("#txtCodCentroCosto").val(),
                FecApertura: $("#txtFecApertura").val(),
                TipEstado: $("#txtTipEstado").val()
            };

            if (validarApertura(apertura))
                await guardarApertura(apertura, urlCrearApertura);
        });

        $("#btnDescargarArchivo").on('click', function () {
            descargarAperturas();
        });

        $("#btnEliminarApertura").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar a local?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        eliminarApertura();
                    }
                });


        });


        $("#tableAperturas tbody").on('click', 'tr', function () {
            $('#tableAperturas tbody tr').removeClass('selected');
            $(this).toggleClass('selected');
        });

        $("#tableAperturas tbody").on('click', 'tr', function () {
            $(this).addClass('selected');
        });
      
    }

    const abrirModalNuevaApertura = async function () {
        $("#tituloModalApertura").html("Nuevo Local Apertura");
        $("#btnActualizarApertura").hide();
        $("#btnGuardarApertura").show();
        const model = {};

        await cargarFormApertura(model, false);
    }

    const abrirModalEditarCadena = async function (codLocalPMM) {
        $("#tituloModalCadena").html("Editar Cadena");
        $("#btnActualizarCadena").show();
        $("#btnGuardarCadena").hide();

        const response = await obtenerApertura(codLocalPMM);
        const model = response.Data;

        await cargarFormApertura(model, true);
    }

    const obtenerApertura = function (codLocalPMM) {
        return new Promise((resolve, reject) => {
            if (!codLocalPMM) return resolve();

            const request = {
                CodLocalPMM: codLocalPMM
            };

            $.ajax({
                url: urlObtenerApertura,
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


    const cargarFormApertura = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarApertura,
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
                $("#modalAperturas").find(".modal-body").html(response);
                $("#modalAperturas").modal('show');

                $("#txtCodLocalPMM").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validarApertura = function (apertura) {
        let validate = true;

        if (apertura.CodLocalPMM === '' || apertura.NomLocalPMM === '' || apertura.CodLocalSAP === '' || apertura.NomLocalSAP === '' || apertura.CodLocalSAPNew === ''
            || apertura.CodLocalOfiplan === '' || apertura.NomLocalOfiplan === '' || apertura.Administrador === '' || apertura.NumTelefono === '' || apertura.Email === ''
            || apertura.Direccion === '' || apertura.Ubigeo === '' || apertura.CodComercio === '' || apertura.CodCentroCosto === '' || apertura.FecApertura === '' || apertura.TipEstado === '') {
            validate = false;
            $("#formApertura").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const guardarApertura = function (apertura, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: apertura },
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
                /*await cargarArbolEmpresas();*/
                $("#modalAperturas").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const visualizarDataTableAperturas = function () {

        $.ajax({
            url: urlListarApertura,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {

                dataTableAperturas = $('#tableAperturas').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    bAutoWidth: false,
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const recargarDataTableAperturas = function () {
        const request = {};

        if ($.fn.DataTable.isDataTable('#tableAperturas')) {
            $('#tableAperturas').DataTable().destroy();
        }

        dtListaAperturas = $('#tableAperturas').DataTable({
            ajax: {
                url: urlListarApertura,
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
                        text: 'Error al listar los aperturas: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "CodLocalPMM" },
                { data: "NomLocalPMM" },
                { data: "CodLocalSAP" },
                { data: "CodLocalSAPNew" },
                { data: "NomLocalSAP" },
                { data: "CodLocalOfiplan" },
                { data: "NomLocalOfiplan" },
                { data: "Administrador" },
                { data: "CodCentroCosto" },
                { data: "CodComercio" },
                { data: "FecApertura" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            rowCallback: function (row, data, index) {
                //$(row).on('click', function () {
                //    if ($(this).hasClass('selected')) {
                //        $(this).removeClass('selected');
                //    } else {
                //        dtListaAperturas.$('tr.selected').removeClass('selected');
                //        $(this).addClass('selected');
                //    }
                //});
            },
            bAutoWidth: false
        });
    }

    const validarSelecion = function (count) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar como minimo un registro",
                icon: "warning",
            });
            return false;
        }

        return true;
    }


    const eliminarApertura = function () {

        const registrosSeleccionados = dataTableAperturas.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        //let cajeros = [];

        //registrosSeleccionados.map((item) => {
        //    cajeros.push(item.DocIdentidad);
        //});

        //$.ajax({
        //    url: urlEliminarCajero,
        //    type: "post",
        //    data: { nroDocumentos: cajeros },
        //    dataType: "json",
        //    beforeSend: function () {
        //        showLoading();
        //    },
        //    complete: function () {
        //        closeLoading();
        //    },
        //    success: function (response) {

        //        swal({
        //            text: response.Mensaje,
        //            icon: "success"
        //        });

        //        recargardataTableAperturas();

        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        swal({
        //            text: jqXHR.responseText,
        //            icon: "error",
        //        });
        //    }
        //});
    }

    const descargarAperturas = function () {

        const request = {};

        $.ajax({
            url: urlDescargarAperturas,
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


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                /*visualizarDataTableAperturas();*/
                recargarDataTableAperturas();
            });
        }
    }

}(jQuery);