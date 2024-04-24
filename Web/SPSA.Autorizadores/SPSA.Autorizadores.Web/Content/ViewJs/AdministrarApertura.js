var urlListarDepartamentos = baseUrl + 'Ubigeos/Ubigeo/ListarDepartamentos';
var urlListarProvincias = baseUrl + 'Ubigeos/Ubigeo/ListarProvincias';
var urlListarDistritos = baseUrl + 'Ubigeos/Ubigeo/ListarDistritos';
var urlObtenerUbigeo = baseUrl + 'Ubigeos/Ubigeo/ObtenerUbigeo';

var urlListarApertura = baseUrl + 'Aperturas/AdministrarApertura/ListarApertura';
var urlModalCrearEditarApertura = baseUrl + 'Aperturas/AdministrarApertura/CrearEditarApertura';
var urlCrearApertura = baseUrl + 'Aperturas/AdministrarApertura/CrearApertura';
var urlActualizarApertura = baseUrl + 'Aperturas/AdministrarApertura/ActualizarApertura';
var urlImportarApertura = baseUrl + 'Aperturas/AdministrarApertura/ImportarExcelApertura';
var urlDescargarAperturas = baseUrl + 'Aperturas/AdministrarApertura/DescargarExcelApertura';
var urlObtenerApertura = baseUrl + 'Aperturas/AdministrarApertura/ObtenerApertura';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

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
                /* Ubigeo: $("#txtUbigeo").val(),*/
                CodComercio: $("#txtCodComercio").val(),
                CodCentroCosto: $("#txtCodCentroCosto").val(),
                FecApertura: $("#txtFecApertura").val(),
                FecCierre: $("#txtFecCierre").val(),
                TipEstado: $("#cboTipEstado").val(),
                /* Nuevo Campos */
                CodEAN: $("#txtCodEAN").val(),
                CodSUNAT: $("#txtCodSUNAT").val(),
                NumGuias: $("#txtNumGuias").val(),
                CentroDistribu: $("#txtCentroDistribu").val(),
                TdaEspejo: $("#txtTdaEspejo").val(),
                Mt2Sala: $("#txtMt2Sala").val(),
                Spaceman: $("#txtSpaceman").val(),
                ZonaPrincing: $("#txtZonaPrincing").val(),
                Geolocalizacion: $("#txtGeolocalizacion").val()
            };

            apertura.Ubigeo = $("#cboDepartamento").val() + $("#cboProvincia").val() + $("#cboDistrito").val();

            if (validarApertura(apertura))
                await guardarApertura(apertura, urlCrearApertura);
        });

        $("#btnActualizarApertura").on("click", async function () {
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
                /* Ubigeo: $("#txtUbigeo").val(),*/
                CodComercio: $("#txtCodComercio").val(),
                CodCentroCosto: $("#txtCodCentroCosto").val(),
                FecApertura: $("#txtFecApertura").val(),
                FecCierre: $("#txtFecCierre").val(),
                TipEstado: $("#cboTipEstado").val(),
                /* Nuevo Campos */
                CodEAN: $("#txtCodEAN").val(),
                CodSUNAT: $("#txtCodSUNAT").val(),
                NumGuias: $("#txtNumGuias").val(),
                CentroDistribu: $("#txtCentroDistribu").val(),
                TdaEspejo: $("#txtTdaEspejo").val(),
                Mt2Sala: $("#txtMt2Sala").val(),
                Spaceman: $("#txtSpaceman").val(),
                ZonaPrincing: $("#txtZonaPrincing").val(),
                Geolocalizacion: $("#txtGeolocalizacion").val()

            };

            apertura.Ubigeo = $("#cboDepartamento").val() + $("#cboProvincia").val() + $("#cboDistrito").val();

            if (validarApertura(apertura))
                await guardarApertura(apertura, urlActualizarApertura);
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

        $("#cboDepartamento").on("change", async function () {
            await cargarComboProvincias();
        });

        $("#cboProvincia").on("change", async function () {
            await cargarComboDistritos();
        });

        $("#btnCargarArchivo").click(function () {
            $("#modalImportarApertura").modal('show');
        });

        $("#btnCargarExcelApertura").on("click", function () {
            var inputFile = document.getElementById('archivoExcelApertura');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de local existe, este no será actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelAperturas();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });

        $("#btnPlantillaArchivo").on("click", function () {
            descargarPlantillas("Plantilla_Aperturas");
        });

    }

    const cargarComboDepartamentos = async function () {

        try {
            const response = await listarDepartamentos();

            if (response.Ok) {
                $('#cboDepartamento').empty().append('<option label="Seleccionar"></option>');
                $('#cboProvincia').empty().append('<option label="Seleccionar"></option>');
                $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(departamento => {
                    $('#cboDepartamento').append($('<option>', { value: departamento.CodDepartamento, text: departamento.NomDepartamento }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const cargarComboProvincias = async function () {

        try {
            const response = await listarProvincias();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboProvincia').empty().append('<option label="Seleccionar"></option>');
                $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(provincia => {
                    $('#cboProvincia').append($('<option>', { value: provincia.CodProvincia, text: provincia.NomProvincia }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const cargarComboDistritos = async function () {

        try {
            const response = await listarDistritos();
            if (response === undefined) return;
            if (response.Ok) {
                $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(distrito => {
                    $('#cboDistrito').append($('<option>', { value: distrito.CodDistrito, text: distrito.NomDistrito }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const listarDepartamentos = function () {
        return new Promise((resolve, reject) => {

            const request = {
            };

            $.ajax({
                url: urlListarDepartamentos,
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

    const listarProvincias = function () {
        return new Promise((resolve, reject) => {
            const codDepartamento = $("#cboDepartamento").val();
            if (!codDepartamento) return resolve();

            const request = {
                CodDepartamento: codDepartamento
            };

            $.ajax({
                url: urlListarProvincias,
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

    const listarDistritos = function () {
        return new Promise((resolve, reject) => {
            const codDepartamento = $("#cboDepartamento").val();
            const codProvincia = $("#cboProvincia").val();
            if (!codDepartamento) return resolve();

            const request = {
                CodDepartamento: codDepartamento,
                CodProvincia: codProvincia,
            };

            $.ajax({
                url: urlListarDistritos,
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

    const abrirModalNuevaApertura = async function () {
        $("#tituloModalApertura").html("Nuevo Local Apertura");
        $("#btnActualizarApertura").hide();
        $("#btnGuardarApertura").show();
        $("#modalAperturas").modal('show');
        $("#txtCodLocalPMM").prop("disabled", false);

        $("#txtCodLocalPMM").val('');
        $("#txtNomLocalPMM").val('');
        $("#txtCodLocalSAP").val('');
        $("#txtNomLocalSAP").val('');
        $("#txtCodLocalSAPNew").val('');
        $("#txtCodLocalOfiplan").val('');
        $("#txtNomLocalOfiplan").val('');
        $("#txtAdministrador").val('');
        $("#txtNumTelefono").val('');
        $("#txtEmail").val('');
        $("#txtDireccion").val('');
        $("#txtUbigeo").val('');
        $("#txtCodComercio").val('');
        $("#txtCodCentroCosto").val('');
        $("#txtFecApertura").val('');
        $("#cboTipEstado").val('');

        $("#txtFecCierre").val('');
        $("#txtCodEAN").val('');
        $("#txtCodSUNAT").val('');
        $("#txtNumGuias").val('');
        $("#txtCentroDistribu").val('');
        $("#txtTdaEspejo").val('');
        $("#txtMt2Sala").val('');
        $("#txtSpaceman").val('');
        $("#txtZonaPrincing").val('');
        $("#txtGeolocalizacion").val('');

        await cargarComboDepartamentos();

        //await cargarFormAperturaNuevo(model, false);
    }

    const abrirModalEditarCadena = async function (codLocalPMM) {
        $("#tituloModalCadena").html("Editar Cadena");
        $("#btnActualizarCadena").show();
        $("#btnGuardarCadena").hide();

        $('#cboDepartamento').empty().append('<option label="Seleccionar"></option>');
        $('#cboProvincia').empty().append('<option label="Seleccionar"></option>');
        $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');

        $("#modalAperturas").modal('show');
        $("#txtCodLocalPMM").prop("disabled", true);

        const response = await obtenerApertura(codLocalPMM);
        const model = response.Data;

        $("#txtCodLocalPMM").val(model.CodLocalPMM);
        $("#txtNomLocalPMM").val(model.NomLocalPMM);
        $("#txtCodLocalSAP").val(model.CodLocalSAP);
        $("#txtNomLocalSAP").val(model.NomLocalSAP);
        $("#txtCodLocalSAPNew").val(model.CodLocalSAPNew);
        $("#txtCodLocalOfiplan").val(model.CodLocalOfiplan);
        $("#txtNomLocalOfiplan").val(model.NomLocalOfiplan);
        $("#txtAdministrador").val(model.Administrador);
        $("#txtNumTelefono").val(model.NumTelefono);
        $("#txtEmail").val(model.Email);
        $("#txtDireccion").val(model.Direccion);
        $("#txtUbigeo").val(model.Ubigeo);
        $("#txtCodComercio").val(model.CodComercio);
        $("#txtCodCentroCosto").val(model.CodCentroCosto);

        if (model.FecApertura == null) {
            $("#txtFecApertura").val(model.FecApertura);
        }
        else {
            var fechaAperturaCorrecta = convertirFecha(model.FecApertura).toISOString().substring(0, 10);
            $("#txtFecApertura").val(fechaAperturaCorrecta);
        }

        $("#cboTipEstado").val(model.TipEstado);

        if (model.FecCierre == null) {
            $("#txtFecCierre").val(model.FecCierre);
        }
        else {
            var fechaCierreCorrecta = convertirFecha(model.FecCierre).toISOString().substring(0, 10);
            $("#txtFecCierre").val(fechaCierreCorrecta);

        }

        $("#txtCodEAN").val(model.CodEAN);
        $("#txtCodSUNAT").val(model.CodSUNAT);
        $("#txtNumGuias").val(model.NumGuias);
        $("#txtCentroDistribu").val(model.CentroDistribu);
        $("#txtTdaEspejo").val(model.TdaEspejo);
        $("#txtMt2Sala").val(model.Mt2Sala);
        $("#txtSpaceman").val(model.Spaceman);
        $("#txtZonaPrincing").val(model.ZonaPrincing);
        $("#txtGeolocalizacion").val(model.Geolocalizacion);

        if (model.Ubigeo == null) {
            await cargarComboDepartamentos();
        }
        else {
            var objUbigeo = await obtenerUbigeo(model.Ubigeo);
            await cargarComboDepartamentos();
            $("#cboDepartamento").val(objUbigeo.Data.CodDepartamento);
            await cargarComboProvincias();
            $("#cboProvincia").val(objUbigeo.Data.CodProvincia);
            await cargarComboDistritos();
            $("#cboDistrito").val(objUbigeo.Data.CodDistrito);
        }

        
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

    function convertirFecha(fechaEnFormatoJSON) {
        // Extraemos el número de milisegundos de la cadena de fecha
        var milisegundos = parseInt(fechaEnFormatoJSON.replace("/Date(", "").replace(")/", ""));

        // CrEANos un objeto de fecha con los milisegundos
        var fecha = new Date(milisegundos);

        return fecha;
    }

    const obtenerUbigeo = function (codUbigeo) {
        return new Promise((resolve, reject) => {
            if (!codUbigeo) return resolve();

            const request = {
                CodUbigeo: codUbigeo
            };

            $.ajax({
                url: urlObtenerUbigeo,
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
                recargarDataTableAperturas();
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
                //{ data: "NomLocalSAP" },
                //{ data: "CodLocalOfiplan" },
                //{ data: "NomLocalOfiplan" },
                { data: "Administrador" },
                { data: "NumTelefono" },
                { data: "CodCentroCosto" },
                { data: "CodComercio" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '600px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            rowCallback: function (row, data, index) {
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

    const importarExcelAperturas = function () {
        var archivo = document.getElementById('archivoExcelApertura').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarApertura,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#archivoExcelApertura").val(null);
                $("#modalImportarApertura").modal('hide');
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErroresCaja').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const descargarPlantillas = function (nombreCarpeta) {
        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
            data: { nombreCarpeta: nombreCarpeta },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                const linkSource = `data:application/zip;base64,` + response.Archivo + '\n';
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
                showLoading();
                recargarDataTableAperturas();
                closeLoading();
            });
        }
    }

}(jQuery);