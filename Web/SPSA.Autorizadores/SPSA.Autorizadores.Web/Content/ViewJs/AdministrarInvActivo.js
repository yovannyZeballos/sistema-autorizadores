var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresa';
var urlListarCadenas = baseUrl + 'Maestros/MaeCadena/ListarCadena';
var urlListarRegiones = baseUrl + 'Maestros/MaeRegion/ListarRegion';
var urlListarZonas = baseUrl + 'Maestros/MaeZona/ListarZona';
var urlListarLocales = baseUrl + 'Maestros/MaeLocal/ListarLocal';

var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarActivos = baseUrl + 'Inventario/InventarioActivo/ListarActivos';
var urlListarTiposActivo = baseUrl + 'Inventario/InventarioTipoActivo/ListarTiposActivo';

var urlObtenerInvActivo = baseUrl + 'Inventario/InventarioActivo/ObtenerInvActivo';
var urlCrearInvActivo = baseUrl + 'Inventario/InventarioActivo/CrearInvActivo';
var urlActualizarInvActivo = baseUrl + 'Inventario/InventarioActivo/ActualizarInvActivo';
var urlImportarInvActivo = baseUrl + 'Inventario/InventarioActivo/ImportarExcelInventario';

var urlModalCrearEditarInvActivo = baseUrl + 'Inventario/InventarioActivo/CrearEditarInvActivo';

var urlModalCrearInvActivo = baseUrl + 'Inventario/InventarioActivo/CrearFormInvActivo';
var urlModalEditarInvActivo = baseUrl + 'Inventario/InventarioActivo/EditarFormInvActivo';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';
var urlDescargarInvTiposActivo = baseUrl + 'Inventario/InventarioTipoActivo/DescargarInvTiposActivo';
var urlDescargarInvActivoPorEmpresa = baseUrl + 'Inventario/InventarioActivo/DescargarInvActivo';

var dtListaActivos = null;
var dtListaLocales = null;
var AdministrarInvActivo = function () {

    const eventos = function () {

        $("#cboEmpresa").on("change", async function () {
            await cargarComboCadenas();
        });

        $("#cboCadena").on("change", async function () {
            await cargarComboRegiones();
        });

        $("#cboRegion").on("change", async function () {
            await cargarComboZonas();
        });

        $("#cboZona").on("change", async function () {
            await cargarComboLocales();
        });

        $("#cboLocal").on("change", function () {
            if (validarBuscarInvActivo()) {
                const request = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val(),
                };

                recargarDataTableActivos(request);
            }
        });

        $("#btnBuscarLocalPorEmpresa").on("click", function () {
            if (validarBuscarInvActivoPorEmpresa()) {
                $("#modalLocales").modal('show');
                recargarDataTableLocalesPorEmpresa();
            }
        });

        $("#btnNuevoInvActivo").click(function () {

            if (validarNuevoInvActivo()) {
                const codEmpresa = $("#cboEmpresa").val();
                const codCadena = $("#cboCadena").val();
                const codRegion = $("#cboRegion").val();
                const codZona = $("#cboZona").val();
                const codLocal = $("#cboLocal").val();

                abrirModalNuevoInvActivo(codEmpresa, codCadena, codRegion, codZona, codLocal);
            }
        });

        $("#btnEditarInvActivo").click(function () {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            const codLocal = $("#cboLocal").val();

            var filasSeleccionada = document.querySelectorAll("#tableActivos tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_MODELO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;
            const NOM_MARCA = filasSeleccionada[0].querySelector('td:nth-child(3)').textContent;
            const COD_SERIE = filasSeleccionada[0].querySelector('td:nth-child(4)').textContent;

            abrirModalEditarInvActivo(codEmpresa, codCadena, codRegion, codZona, codLocal, COD_ACTIVO, COD_MODELO, NOM_MARCA, COD_SERIE);
        });

        $("#btnGuardarInvActivo").on("click", async function () {
            var inv_activo = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val(),
                CodActivo: $("#cboCodActivo").val(),
                CodModelo: $("#txtCodModelo").val(),
                CodSerie: $("#txtCodSerie").val(),
                NomMarca: $("#txtNomMarca").val(),
                Ip: $("#txtIp").val(),
                NomArea: $("#txtNomArea").val(),
                NumOc: $("#txtNumOc").val(),
                NumGuia: $("#txtNumGuia").val(),
                FecSalida: $("#txtFecSalida").val(),
                Antiguedad: $("#txtAntiguedad").val(),
                IndOperativo: $("#cboIndOperativo").val(),
                Observacion: $("#txtObservacion").val(),
                Garantia: $("#txtGarantia").val(),
                FecActualiza: $("#txtFecActualiza").val()
            };

            if (validarFormInvActivo(inv_activo))
                await guardarInvActivo(inv_activo, urlCrearInvActivo);
        });

        $("#btnActualizarInvActivo").on("click", async function () {
            var inv_activo = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val(),
                CodActivo: $("#txtCodActivo").val(),
                CodModelo: $("#txtCodModelo").val(),
                CodSerie: $("#txtCodSerie").val(),
                NomMarca: $("#txtNomMarca").val(),
                Ip: $("#txtIp").val(),
                NomArea: $("#txtNomArea").val(),
                NumOc: $("#txtNumOc").val(),
                NumGuia: $("#txtNumGuia").val(),
                FecSalida: $("#txtFecSalida").val(),
                Antiguedad: $("#txtAntiguedad").val(),
                IndOperativo: $("#cboIndOperativo").val(),
                Observacion: $("#txtObservacion").val(),
                Garantia: $("#txtGarantia").val(),
                FecActualiza: $("#txtFecActualiza").val()
            };

            if (validarFormInvActivo(inv_activo))
                await guardarInvActivo(inv_activo, urlActualizarInvActivo);
        });

        $("#btnObtenerInvPorLocal").on("click", async function () {
            var filasSeleccionada = document.querySelectorAll("#tableLocales tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            showLoading();
            const COD_EMPRESA = filasSeleccionada[0].querySelector('td:nth-child(6)').textContent;
            const COD_CADENA = filasSeleccionada[0].querySelector('td:nth-child(7)').textContent;
            const COD_REGION = filasSeleccionada[0].querySelector('td:nth-child(8)').textContent;
            const COD_ZONA = filasSeleccionada[0].querySelector('td:nth-child(9)').textContent;
            const COD_LOCAL = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const request = {
                CodEmpresa: COD_EMPRESA,
                CodCadena: COD_CADENA,
                CodRegion: COD_REGION,
                CodZona: COD_ZONA,
                CodLocal: COD_LOCAL,
            };

            await sleep(500);
            $("#cboCadena").val(COD_CADENA).trigger('change');
            await sleep(500);
            $("#cboRegion").val(COD_REGION).trigger('change');
            await sleep(500);
            $("#cboZona").val(COD_ZONA).trigger('change');
            await sleep(500);
            $("#cboLocal").val(COD_LOCAL).trigger('change');

            recargarDataTableActivos(request);
            closeLoading();
            $("#modalLocales").modal('hide');
        });

        $('#tableActivos tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaActivos.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableActivos tbody').on('dblclick', 'tr', function () {

            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            const codLocal = $("#cboLocal").val();

            var filasSeleccionada = $(this);

            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_MODELO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;
            const NOM_MARCA = filasSeleccionada[0].querySelector('td:nth-child(3)').textContent;
            const COD_SERIE = filasSeleccionada[0].querySelector('td:nth-child(4)').textContent;

            abrirModalEditarInvActivo(codEmpresa, codCadena, codRegion, codZona, codLocal, COD_ACTIVO, COD_MODELO, NOM_MARCA, COD_SERIE);

            $("#txtCodActivo").prop("disabled", true);
            $("#txtCodModelo").prop("disabled", true);
            $("#txtNomMarca").prop("disabled", true);
            $("#txtCodSerie").prop("disabled", true);
        });

        $('#tableLocales tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaLocales.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableLocales tbody').on('dblclick', 'tr', async function () {
            showLoading();
            var filasSeleccionada = $(this);

            const COD_EMPRESA = filasSeleccionada[0].querySelector('td:nth-child(6)').textContent;
            const COD_CADENA = filasSeleccionada[0].querySelector('td:nth-child(7)').textContent;
            const COD_REGION = filasSeleccionada[0].querySelector('td:nth-child(8)').textContent;
            const COD_ZONA = filasSeleccionada[0].querySelector('td:nth-child(9)').textContent;
            const COD_LOCAL = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const request = {
                CodEmpresa: COD_EMPRESA,
                CodCadena: COD_CADENA,
                CodRegion: COD_REGION,
                CodZona: COD_ZONA,
                CodLocal: COD_LOCAL,
            };

            await sleep(500);
            $("#cboCadena").val(COD_CADENA).trigger('change');
            await sleep(500);
            $("#cboRegion").val(COD_REGION).trigger('change');
            await sleep(500);
            $("#cboZona").val(COD_ZONA).trigger('change');
            await sleep(500);
            $("#cboLocal").val(COD_LOCAL).trigger('change');

            recargarDataTableActivos(request);
            closeLoading();
            $("#modalLocales").modal('hide');
        });

        $("#btnImportar").on("click", function () {
            $("#excelInventario").trigger("click");
        });

        $('#excelInventario').change(function (e) {
            //importarExcelInventario();
            importarExcelInvActivo();
        });

        $("#btnCargarArchivo").click(function () {
            $("#modalImportarInvActivo").modal('show');
        });

        $("#btnDescargarPlantillas").click(function () {
            descargarPlantillas("Plantilla_InvActivo");
        });

        $("#btnCargarExcelInvActivo").on("click", function () {
            var inputFile = document.getElementById('archivoExcelInvActivo');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de activo existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelInvActivo();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });

        $("#btnDescargarTiposActivo").on("click", function () {
            descargarTiposActivo();
        });

        $("#btnDescargarInvActivoPorEmpresa").on("click", function () {
            if (validarBuscarInvActivoPorEmpresa()) {
                descargarActivosPorEmpresa();
            }
        });
    }

    const listarEmpresas = function () {
        return new Promise((resolve, reject) => {

            const request = {
            };

            $.ajax({
                url: urlListarEmpresas,
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

    const listarCadenas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa
            };

            $.ajax({
                url: urlListarCadenas,
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

    const listarRegiones = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
            };

            $.ajax({
                url: urlListarRegiones,
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

    const listarZonas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();

            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion
            };

            $.ajax({
                url: urlListarZonas,
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
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();

            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();
            if (!codZona) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion,
                CodZona: codZona
            };

            $.ajax({
                url: urlListarLocales,
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

    const obtenerInvActivo = function (codEmpresa, codCadena, codRegion, codZona, codLocal, codActivo, codModelo, nomMarca, codSerie) {
        return new Promise((resolve, reject) => {
            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();
            if (!codZona) return resolve();
            if (!codLocal) return resolve();
            //if (!codActivo) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion,
                CodZona: codZona,
                CodLocal: codLocal,
                CodActivo: codActivo,
                CodModelo: codModelo,
                NomMarca: nomMarca,
                CodSerie: codSerie
            };

            $.ajax({
                url: urlObtenerInvActivo,
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
            const response = await listarEmpresas();

            if (response.Ok) {
                $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
                $('#cboCadena').empty().append('<option label="Seleccionar"></option>');
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(empresa => {
                    $('#cboEmpresa').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
                });
                //$('#cboEmpresa').val("001");
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

    const cargarComboCadenas = async function () {

        try {
            const response = await listarCadenas();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboCadena').empty().append('<option label="Seleccionar"></option>');
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(cadena => {
                    $('#cboCadena').append($('<option>', { value: cadena.CodCadena, text: cadena.NomCadena }));
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

    const cargarComboRegiones = async function () {

        try {
            const response = await listarRegiones();
            if (response === undefined) return;
            if (response.Ok) {
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
                    $('#cboRegion').append($('<option>', { value: region.CodRegion, text: region.NomRegion }));
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

    const cargarComboZonas = async function () {

        try {
            const response = await listarZonas();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
                    $('#cboZona').append($('<option>', { value: region.CodZona, text: region.NomZona }));
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

    const cargarComboLocales = async function () {

        try {
            const response = await listarLocales();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
                    $('#cboLocal').append($('<option>', { value: region.CodLocal, text: region.NomLocal }));
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

    const validarFormInvActivo = function (invactivo) {
        let validate = true;

        if (invactivo.CodEmpresa === '' || invactivo.CodCadena === '' || invactivo.CodRegion === '' || invactivo.CodZona === '' || invactivo.CodLocal === ''
            || invactivo.CodActivo === '' || invactivo.CodModelo === '' || invactivo.NomMarca === '' || invactivo.CodSerie === '') {
            validate = false;
            $("#formInvActivo").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarNuevoInvActivo = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '' || $("#cboLocal").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, cadena, region, zona y local.', icon: "warning", });
        }

        return validate;
    }

    const validarBuscarInvActivo = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, cadena, region y zona.', icon: "warning", });
        }

        return validate;
    }

    const validarBuscarInvActivoPorEmpresa = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa.', icon: "warning", });
        }

        return validate;
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
                const request = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val(),
                };
                recargarDataTableActivos(request);
                $("#modalInvActivo").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const abrirModalEditarInvActivo = async function (codEmpresa, codCadena, codRegion, codZona, codLocal, codActivo, codModelo, nomMarca, codSerie) {
        $("#tituloModalInvActivo").html("Editar Activo");
        $("#btnActualizarInvActivo").show();
        $("#btnGuardarInvActivo").hide();

        const response = await obtenerInvActivo(codEmpresa, codCadena, codRegion, codZona, codLocal, codActivo, codModelo, nomMarca, codSerie);
        const model = response.Data;

        //if (model.FecActualiza != "") {
        //    let dateString = model.FecActualiza;
        //    // Extraer el timestamp del string
        //    let timestamp = parseInt(dateString.match(/\d+/)[0], 10);

        //    // Crear un objeto Date usando el timestamp
        //    let date = new Date(timestamp);

        //    // Formatear la fecha a dd/mm/yyyy
        //    let day = String(date.getDate()).padStart(2, '0');
        //    let month = String(date.getMonth() + 1).padStart(2, '0'); // Los meses comienzan desde 0
        //    let year = date.getFullYear();

        //    let formattedDate = `${day}/${month}/${year}`;


        //    console.log(formattedDate); // Para verificar la salida en la consola

        //    model.FecActualiza = formattedDate;
        //}




        await cargarFormEditarInvActivo(model);
        //await cargarFormInvActivo(model, true);
    }

    const abrirModalNuevoInvActivo = async function (codEmpresa, codCadena, codRegion, codZona, codLocal) {
        $("#tituloModalInvActivo").html("Nuevo Activo");
        $("#btnActualizarInvActivo").hide();
        $("#btnGuardarInvActivo").show();

        const model = {};
        model.CodEmpresa = codEmpresa;
        model.CodCadena = codCadena;
        model.CodRegion = codRegion;
        model.CodZona = codZona;
        model.CodLocal = codLocal;

        await cargarFormCrearInvActivo(model);
        //await cargarFormInvActivo(model, false);
    }

    const cargarFormInvActivo = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarInvActivo,
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
                $("#txtCodActivo").prop("disabled", deshabilitar);
                $("#txtCodModelo").prop("disabled", deshabilitar);
                $("#txtNomMarca").prop("disabled", deshabilitar);
                $("#txtCodSerie").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
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

    const recargarDataTableActivos = function (request) {

        if ($.fn.DataTable.isDataTable('#tableActivos')) {
            $('#tableActivos').DataTable().destroy();
        }
        dtListaActivos = $('#tableActivos').DataTable({
            ajax: {
                url: urlListarActivos,
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
            columns: [
                { data: "CodActivo" },
                { data: "CodModelo" },
                { data: "NomMarca" },
                { data: "CodSerie" },
                { data: "Ip" },
                { data: "NomArea" },
                { data: "NumOc" },
                { data: "NumGuia" },
                { data: "Antiguedad" },
                { data: "Observacion" },
                { data: "Garantia" },
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

            },
            bAutoWidth: false
        });
    }

    const recargarDataTableLocalesPorEmpresa = function () {
        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            //CodCadena: $("#cboCadena").val(),
            //CodRegion: $("#cboRegion").val(),
            //CodZona: $("#cboZona").val()
        };

        if ($.fn.DataTable.isDataTable('#tableLocales')) {
            $('#tableLocales').DataTable().destroy();
        }
        dtListaLocales = $('#tableLocales').DataTable({
            ajax: {
                url: urlListarLocalesPorEmpresa,
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
                        text: 'Error al listar los locales: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "CodLocal" },
                { data: "NomLocal" },
                { data: "CodLocalPMM" },
                { data: "NomLocalOfiplan" },
                { data: "CodLocalSunat" },
                { data: "CodEmpresa" },
                { data: "CodCadena" },
                { data: "CodRegion" },
                { data: "CodZona" },
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

            },
            bAutoWidth: false
        });
    }

    const importarExcelInvActivo = function () {
        //var archivo = document.getElementById('archivoExcelInvActivo').files[0];
        var formData = new FormData();
        //formData.append('archivoExcel', archivo);
        var uploadFiles = $('#excelInventario').prop('files');
        formData.append("excelInventario", uploadFiles[0]);

        $.ajax({
            url: urlImportarInvActivo,
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
                //$("#archivoExcelInvActivo").val(null);
                //$("#modalImportarInvActivo").modal('hide');
                $("#excelInventario").val(null);
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

    const descargarTiposActivo = function () {

        const request = {
        };

        $.ajax({
            url: urlDescargarInvTiposActivo,
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

    const descargarActivosPorEmpresa = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val()
        };

        $.ajax({
            url: urlDescargarInvActivoPorEmpresa,
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

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
            });
        }
    }

}(jQuery);
