var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresa';
var urlListarCadenas = baseUrl + 'Maestros/MaeCadena/ListarCadena';
var urlListarRegiones = baseUrl + 'Maestros/MaeRegion/ListarRegion';
var urlListarZonas = baseUrl + 'Maestros/MaeZona/ListarZona';
var urlListarLocales = baseUrl + 'Maestros/MaeLocal/ListarLocal';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarCajas = baseUrl + 'Maestros/MaeCaja/ListarCaja';

var urlCrearLocal = baseUrl + 'Maestros/MaeLocal/CrearLocal';
var urlActualizarLocal = baseUrl + 'Maestros/MaeLocal/ActualizarLocal';
var urlCrearCaja = baseUrl + 'Maestros/MaeCaja/CrearCaja';
var urlActualizarCaja = baseUrl + 'Maestros/MaeCaja/ActualizarCaja';
var urlEliminarCajas = baseUrl + 'Maestros/MaeCaja/EliminarCajas';
var urlObtenerLocal = baseUrl + 'Maestros/MaeLocal/ObtenerLocal';

var urlModalCrearEditarCaja = baseUrl + 'Maestros/MaeLocal/CrearEditarCaja';

var urlImportarLocales = baseUrl + 'Maestros/MaeLocal/ImportarExcelLocal';
var urlImportarCajas = baseUrl + 'Maestros/MaeCaja/ImportarExcel';

var urlDescargarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/DescargarLocalPorEmpresa';
var urlDescargarCajasPorLocal = baseUrl + 'Maestros/MaeCaja/DescargarCajaPorLocal';
var urlDescargarCajasPorEmpresa = baseUrl + 'Maestros/MaeCaja/DescargarCajaPorEmpresa';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var dataTableLocales = null;
var codLocalCaja = null;

var urlFechaSistema = baseUrl + 'Locales/AdministrarLocal/ObtenerFechaSistema';

var codRegionAnterior = "";
var codZonaAnterior = "";
var permitirCambioZonaRegion = false;


var AdministrarMaestroLocal = function () {
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
            if (!permitirCambioZonaRegion) {
                await cargarComboLocales();
            }
        });

        $("#cboLocal").on("change", function () {

            if (validarLocalChange()) {
                showLoading();
                const request = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val()
                };
                codRegionAnterior = request.CodRegion;
                codZonaAnterior = request.CodZona;
                obtenerLocal(request);
                closeLoading();
            }
        });

        $("#btnNuevoLocal").on("click", function () {
            limpiar();
        });

        $("#btnGuardarLocal").on("click", function () {

            if (validarGuardarLocal()) {
                swal({
                    title: "Confirmar!",
                    text: "¿Está seguro guardar la información ingresada?",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then(async (willDelete) => {
                    if (willDelete) {
                        var local = {
                            CodEmpresa: $("#cboEmpresa").val(),
                            CodCadena: $("#cboCadena").val(),
                            CodRegion: $("#cboRegion").val(),
                            CodZona: $("#cboZona").val(),
                            CodLocal: $("#txtCodLocal").val(),
                            NomLocal: $("#txtNomLocal").val(),
                            TipEstado: $("#cboTipEstado").val(),
                            CodLocalPMM: $("#txtCodLocalPMM").val(),
                            Ip: $("#txtIP").val(),
                            CodLocalOfiplan: $("#txtCodLocalOfiplan").val(),
                            NomLocalOfiplan: $("#txtNomLocalOfiplan").val(),
                            CodLocalSunat: $("#txtCodLocalSunat").val()
                        };
                        if (validarLocal(local))
                            await guardarLocal(local);
                    }
                });
            }
        });

        $("#btnGuardarCambiosLocal").on("click", function () {
            if (validarGuardarLocal()) {
                swal({
                    title: "Confirmar!",
                    text: "¿Está seguro guardar la información modificada?",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then(async (willDelete) => {
                    if (willDelete) {
                        permitirCambioZonaRegion = false;
                        var local = {
                            CodEmpresa: $("#cboEmpresa").val(),
                            CodCadena: $("#cboCadena").val(),
                            CodRegion: $("#cboRegion").val(),
                            CodZona: $("#cboZona").val(),
                            CodLocal: $("#txtCodLocal").val(),
                            NomLocal: $("#txtNomLocal").val(),
                            TipEstado: $("#cboTipEstado").val(),
                            CodLocalPMM: $("#txtCodLocalPMM").val(),
                            Ip: $("#txtIP").val(),
                            CodLocalOfiplan: $("#txtCodLocalOfiplan").val(),
                            NomLocalOfiplan: $("#txtNomLocalOfiplan").val(),
                            CodLocalSunat: $("#txtCodLocalSunat").val(),
                            CodRegionAnterior: codRegionAnterior,
                            CodZonaAnterior: codZonaAnterior,
                        };
                        if (validarLocal(local)) {
                            await actualizarLocal(local);
                        }    
                    }
                });
            }
        });

        $("#btnHabilitarlCambioRegionZona").on("click", function () {
            $("#cboRegion").prop("disabled", false);
            $("#cboZona").prop("disabled", false);
            $("#btnGuardarCambiosLocal").prop("disabled", false);
            permitirCambioZonaRegion = true;
        });

        $("#btnBuscarLocalPorEmpresa").on("click", function () {
            if (validarBuscarLocalPorEmpresa()) {
                permitirCambioZonaRegion = false;

                $("#modalLocales").modal('show');
                recargarDataTableLocalesPorEmpresa();
            }
        });

        $("#btnObtenerLocal").on("click", async function () {
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

            codRegionAnterior = request.CodRegion;
            codZonaAnterior = request.CodZona;

            await sleep(1000);
            $("#cboCadena").val(COD_CADENA).trigger('change');
            await sleep(1000);
            $("#cboRegion").val(COD_REGION).trigger('change');
            await sleep(1200);
            $("#cboZona").val(COD_ZONA).trigger('change');
            await sleep(1500);
            $("#cboLocal").val(COD_LOCAL).trigger('change');

            obtenerLocal(request);

            closeLoading();
        });

        $('#tableLocales tbody').on('click', 'tr', function () {
            $('#tableLocales tbody tr').removeClass('selected');
            $(this).toggleClass('selected');

        });

        $('#tableCajas tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');

        });

        $("#btnNuevaCaja").on("click", function () {
            abrirModalNuevaCaja();
        });

        $("#btnEditarCaja").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length, true)) {
                return;
            }

            var codLocal = codLocalCaja;
            var numCaja = filasSeleccionada[0].querySelector('td:first-child').textContent;
            var ipAddress = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;
            var tipOs = filasSeleccionada[0].querySelector('td:nth-child(3)').textContent;
            var tipEstado = filasSeleccionada[0].querySelector('td:nth-child(4)').textContent;

            const model = {
                CodLocal: codLocal,
                NumCaja: numCaja,
                IpAddress: ipAddress,
                TipOs: tipOs,
                TipEstado: tipEstado
            };

            abrirModalEditarCaja(model);
        });

        $("#btnGuardarCaja").on("click", async function () {

            var caja = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#txtCodLocal").val(),
                NumCaja: $("#txtNumCaja").val(),
                IpAddress: $("#txtIpAddress").val(),
                TipOS: $("#cboTipOs").val(),
                TipEstado: $("#cboTipEstado").val()
            };

            if (validarCaja(caja))
                await guardarCaja(caja, urlCrearCaja);

        });

        $("#btnActualizarCaja").on("click", async function () {

            var caja = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#txtCodLocal").val(),
                NumCaja: $("#txtNumCaja").val(),
                IpAddress: $("#txtIpAddress").val(),
                TipOS: $("#cboTipOs").val(),
                TipEstado: $("#cboTipEstado").val()
            };

            if (validarCaja(caja))
                await guardarCaja(caja, urlActualizarCaja);

        });

        $("#btnEliminarCaja").on("click", function () {
            var filasSeleccionadas = document.querySelectorAll("#tableCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionadas.length)) {
                return;
            }

            var cajasParaEliminar = [];

            filasSeleccionadas.forEach(function (filas) {
                var caja = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val(),
                    NumCaja: filas.querySelector('td:first-child').textContent,
                    ipAddress: filas.querySelector('td:nth-child(2)').textContent,
                    tipOs: filas.querySelector('td:nth-child(3)').textContent,
                    //tipEstado: filas.querySelector('td:nth-child(4)').textContent
                    tipEstado: 'E'
                };
                cajasParaEliminar.push(caja);
            });

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar las cajas seleccionadas?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    eliminarCajas(cajasParaEliminar);
                }
            });
        });

        $("#btnAbrirModalExcelLocal").click(function () {
            $("#modalImportarLocal").modal('show');
        });

        $("#btnCargarExcelLocal").on("click", function () {
            var inputFile = document.getElementById('archivoExcelLocal');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de local existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelLocales();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });

        $("#btnDescargarExcelCaja").on("click", function () {
            var caja = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#txtCodLocal").val()
            };

            if (validarDescargarCajas(caja)) {
                descargarCajas(urlDescargarCajasPorLocal);
            }
        });

        $("#btnDescargarPlantillas").on("click", function () {
            descargarPlantillas("Plantilla_Caja");
        });

        $("#btnDescargarMaestroLocal").on("click", function () {
            if (validarBuscarEmpresa()) {
                descargarLocalesPorEmpresa();
            }
        });

        $("#btnDescargarMaestroCajas").on("click", function () {
            if (validarBuscarEmpresa()) {
                descargarCajas(urlDescargarCajasPorEmpresa);
            }
        });

        $("#btnImportarCajas").on("click", function () {
            $("#excelImportarCajas").trigger("click");
        });

        $('#excelImportarCajas').change(function (e) {
            importarCajasDesdeExcel();
        });
    };


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

    const listarCajas = async function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZOna: $("#cboZona").val(),
            /*CodFormato: $("#cboFormato").val(),*/
            CodLocal: $("#txtCodLocal").val()
        };

        const response = await $.ajax({
            url: urlListarCajas,
            type: "post",
            data: { request }
        });

        return response;
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
                //$('#cboLocal').empty().append('<option label="Seleccionar"></option>');
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
                //$('#cboLocal').empty().append('<option label="Seleccionar"></option>');
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
                response.Data.map(local => {
                    $('#cboLocal').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
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

    const visualizarDataTableLocales = function () {

        $.ajax({
            url: urlListarLocales,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {

                dataTableLocales = $('#tableLocales').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Locales,
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

    const recargarDataTableLocales = function () {
        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val()
        };

        if ($.fn.DataTable.isDataTable('#tableLocales')) {
            $('#tableLocales').DataTable().destroy();
        }
        dtListaLocales = $('#tableLocales').DataTable({
            ajax: {
                url: urlListarLocales,
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
                { data: "CodZona" }
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
            CodEmpresa: $("#cboEmpresa").val()
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

    const limpiar = function () {
        permitirCambioZonaRegion = false;
        $("#cboEmpresa").val('').trigger('change');
        $("#cboCadena").val('').trigger('change');
        $("#cboRegion").val('').trigger('change');
        $("#cboZona").val('').trigger('change');
        $("#cboLocal").val('').trigger('change');
        $("#cboTipEstado").val('A').trigger('change');
        $("#txtCodLocal").val('');
        $("#txtNomLocal").val('');
        $("#txtCodLocalPMM").val('0');
        $("#txtCodLocalOfiplan").val('0');
        $("#txtNomLocalOfiplan").val('');
        $("#txtCodLocalSunat").val('0');
        $("#btnGuardarLocal").prop("disabled", false);
        desabilitarBotonosLocal(false);
        desabilitarBotonosCaja(true);
        desabilitarControles(false);
        obtenerFechaSistema();
        $("#btnGuardarCambiosLocal").prop("disabled", true);
        $("#btnHabilitarlCambioRegionZona").prop("disabled", true);
        //dataTableCajas.clear();
        //dataTableCajas.draw();
    }

    const validarLocal = function (local) {
        let validate = true;
        if (local.CodEmpresa === '' || local.CodCadena === '' || local.CodRegion === '' || local.CodZona === '' || local.CodLocal === '' ||
            local.NomLocal === '' || local.TipEstado === '' || local.CodLocalPMM === '' || local.CodLocalSunat === '') {
            validate = false;
            $("#formLocal").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const guardarLocal = function (local) {
        $.ajax({
            url: urlCrearLocal,
            type: "post",
            data: { command: local },
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
                //desabilitarControles(true);
                desabilitarBotonosCaja(false);
                $("#btnGuardarLocal").prop("disabled", true);
                await recargarDataTableCajas();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const actualizarLocal = function (local) {
        $.ajax({
            url: urlActualizarLocal,
            type: "post",
            data: { command: local },
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
                //desabilitarControles(true);
                desabilitarBotonosCaja(false);
                $("#btnGuardarLocal").prop("disabled", true);
                $("#cboRegion").prop("disabled", true);
                $("#cboZona").prop("disabled", true);
                $("#btnGuardarCambiosLocal").prop("disabled", true);
                //$("#btnHabilitarlCambioRegionZona").prop("disabled", true);
                await recargarDataTableCajas();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validarBuscarEmpresa = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa', icon: "warning", });
        }

        return validate;
    }

    const validarBuscarLocal = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, cadena, region y zona', icon: "warning", });
        }

        return validate;
    }

    const validarGuardarLocal = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '' || $("#txtCodLocal").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, cadena, region, zona y local', icon: "warning", });
        }

        return validate;
    }

    const validarLocalChange = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '' || $("#cboLocal").val() === '') {
            validate = false;
            //swal({ text: 'Debe seleccionar la empresa, cadena, region, zona y local', icon: "warning", });
        }

        return validate;
    }

    const validarBuscarLocalPorEmpresa = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa.', icon: "warning", });
        }

        return validate;
    }


    const validarSelecion = function (count, unSoloRegistro = false) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar como mínimo un registro",
                icon: "warning",
            });
            return false;
        }

        if (unSoloRegistro && count > 1) {
            swal({
                text: "Solo debe seleccionar un registro",
                icon: "warning",
            });
            return false;
        }

        return true;
    }

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    const obtenerLocal = function (request) {

        $.ajax({
            url: urlObtenerLocal,
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

                $("#modalLocales").modal('hide');

                setearLocal(response.Data);
                desabilitarBotonosCaja(false);
                desabilitarControles(true)
                $("#btnGuardarLocal").prop("disabled", true);
                $("#btnGuardarCambiosLocal").prop("disabled", false);
                $("#btnHabilitarlCambioRegionZona").prop("disabled", false);

                await recargarDataTableCajas();
                /*obtenerFechaSistema();*/
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const setearLocal = function (local) {
        $("#cboTipEstado").val(local.TipEstado).trigger('change');
        $("#txtCodLocal").val(local.CodLocal);
        $("#txtNomLocal").val(local.NomLocal);
        $("#txtCodLocalPMM").val(local.CodLocalPMM);
        $("#txtIP").val(local.Ip);
        $("#txtCodLocalOfiplan").val(local.CodLocalOfiplan);
        $("#txtNomLocalOfiplan").val(local.NomLocalOfiplan);
        $("#txtCodLocalSunat").val(local.CodLocalSunat);
    }

    const importarExcelLocales = function () {
        var archivo = document.getElementById('archivoExcelLocal').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarLocales,
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
                $("#archivoExcelLocal").val(null);
                $("#modalImportarLocal").modal('hide');
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

    const visualizarDataTableCajas = async function () {
        $.ajax({
            url: urlListarCajas,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {

                dataTableLocales = $('#tableCajas').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Locales,
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

    const recargarDataTableCajas = async function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val(),
            CodLocal: $("#cboLocal").val()
        };

        if ($.fn.DataTable.isDataTable('#tableCajas')) {
            $('#tableCajas').DataTable().destroy();
        }
        dtListaCajas = $('#tableCajas').DataTable({
            ajax: {
                url: urlListarCajas,
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
                        text: 'Error al listar los cajas: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "NumCaja" },
                { data: "IpAddress" },
                { data: "TipOs" },
                { data: "TipEstado" },
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

    const desabilitarBotonosCaja = function (disable) {
        $("#btn-cajas :input").attr("disabled", disable);
    }

    const desabilitarBotonosLocal = function (disable) {
        $("#btn-local :input").attr("disabled", disable);
    }

    const desabilitarControles = function (disable) {
        $("#cboEmpresa").prop("disabled", disable);
        $("#cboCadena").prop("disabled", disable);
        $("#cboRegion").prop("disabled", disable);
        $("#cboZona").prop("disabled", disable);
        $("#cboLocal").prop("disabled", disable);
        $("#txtCodLocal").prop("disabled", disable);
    }

    const abrirModalNuevaCaja = function () {
        $("#tituloModalCaja").html("Nueva Caja");
        $("#btnActualizarCaja").hide();
        $("#btnGuardarCaja").show();

        const model = {};
        cargarFormCaja(model, false);
    }

    const abrirModalEditarCaja = function (model) {
        $("#tituloModalCaja").html("Actualizar Caja");
        $("#btnActualizarCaja").show();
        $("#btnGuardarCaja").hide();
        cargarFormCaja(model, true);
    }

    const cargarFormCaja = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarCaja,
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
                $("#modalCaja").find(".modal-body").html(response);
                $("#modalCaja").modal('show');

                $('.select2-show-modal').select2({
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $('#modalCaja .modal-content')
                });
                $('#txtIpAddress').mask('099.099.099.099');
                $('#txtNumCaja').mask('ZZZ', {
                    translation: {
                        'Z': {
                            pattern: /[1-9]/, optional: true
                        }
                    }
                });

                $("#txtNumCaja").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validarCaja = function (caja) {
        let validate = true;

        if (caja.CodEmpresa === '' || caja.CodCadena === '' || caja.CodRegion === '' || caja.CodZona === '' ||
            caja.CodLocal === '' || caja.NumCaja === '' || caja.IpAddress === '' || caja.TipOS === '' || caja.TipEstado === '') {
            validate = false;
            $("#formCaja").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarDescargarCajas = function (caja) {
        let validate = true;

        if (caja.CodEmpresa === '' || caja.CodCadena === '' || caja.CodRegion === '' || caja.CodZona === '' || caja.CodLocal === '') {
            validate = false;
            $("#formCaja").addClass("was-validated");
            swal({ text: 'Faltan seleccionar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const guardarCaja = function (caja, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: caja },
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
                await recargarDataTableCajas();
                $("#modalCaja").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const eliminarCajas = function (cajas) {

        const command = {
            Cajas: cajas
        };

        $.ajax({
            url: urlEliminarCajas,
            type: "post",
            data: { command },
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
                await recargarDataTableCajas();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarCajasDesdeExcel = function () {
        var formData = new FormData();
        var uploadFiles = $('#excelImportarCajas').prop('files');
        formData.append("excelImportarCajas", uploadFiles[0]);

        $.ajax({
            url: urlImportarCajas,
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
                $("#excelImportarCajas").val(null);
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
                            $('#modalErroresImportacion').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
                //await recargarDataTableCajas();
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

    const descargarLocalesPorEmpresa = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val()
        };

        $.ajax({
            url: urlDescargarLocalesPorEmpresa,
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

    const descargarCajas = function (url) {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val(),
            CodLocal: $("#cboLocal").val(),
        };

        $.ajax({
            url: url,
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

    const inputMask = function () {
        //$('#txtIpServidor').mask('099.099.099.099');
        $('#txtCodLocal').mask('ZZZZZZZZZZ', {
            translation: {
                'Z': {
                    pattern: /[0-9]/, optional: true
                }
            }

        });
        $('#txtCodLocalSunat').mask('0999');
    }

    ////////////////////7

    const obtenerFechaSistema = function () {

        $.ajax({
            url: urlFechaSistema,
            type: "post",
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                $("#txtEl").val(response.Mensaje);

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
                inputMask();
                showLoading();
                await cargarComboEmpresa();
                //await cargarComboFormato();
                //await visualizarDataTableCajas();
                closeLoading();

                visualizarDataTableLocales();

            });
        }
    }

}(jQuery);
