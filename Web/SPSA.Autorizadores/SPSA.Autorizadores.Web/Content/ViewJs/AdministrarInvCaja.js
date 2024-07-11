var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresa';
var urlListarCadenas = baseUrl + 'Maestros/MaeCadena/ListarCadena';
var urlListarRegiones = baseUrl + 'Maestros/MaeRegion/ListarRegion';
var urlListarZonas = baseUrl + 'Maestros/MaeZona/ListarZona';
var urlListarLocales = baseUrl + 'Maestros/MaeLocal/ListarLocal';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarCajas = baseUrl + 'Maestros/MaeCaja/ListarCaja';
var urlListarInvCajas = baseUrl + 'Inventario/InventarioCaja/ListarCajas';

var urlObtenerInvCaja = baseUrl + 'Inventario/InventarioCaja/ObtenerInvCaja';
var urlCrearInvCaja = baseUrl + 'Inventario/InventarioCaja/CrearInvCaja';
var urlActualizarInvCaja = baseUrl + 'Inventario/InventarioCaja/ActualizarInvCaja';
var urlCrearFormInvCaja = baseUrl + 'Inventario/InventarioCaja/CrearFormInvCaja';
var urlEditarFormInvCaja = baseUrl + 'Inventario/InventarioCaja/EditarFormInvCaja';
var urlEliminarInvCaja = baseUrl + 'Inventario/InventarioCaja/EliminarInvCaja';
var urlEliminarInvCajaPorLocal = baseUrl + 'Inventario/InventarioCaja/EliminarInvCajaPorLocal';

var urlImportarInventario = baseUrl + 'Inventario/InventarioCaja/Importar';
var urlDescargarPlantilla = baseUrl + 'Inventario/InventarioCaja/DescargarPlantillas';

var dtListaNumCajas = null;
var dtListaCajas = null;
var dtListaLocales = null;

var AdministrarInvCajas = function () {

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
            const request = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val()
            };
            recargarDataTableNumCajas(request);
        });

        $("#btnBuscarLocalPorEmpresa").on("click", function () {
            if (validarBuscarInvCajaPorEmpresa()) {
                $("#modalLocales").modal('show');
                recargarDataTableLocalesPorEmpresa();
            }
        });

        $("#btnEliminarPorLocal").on("click", function () {
            if (validarBuscarInvCaja()) {
                const request = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val()
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
                        await eliminarInvCajaPorLocal(request);
                        swal("Eliminado!", "Los registros han sido eliminados.", "success");
                    }
                });
            }
        });

        $("#btnNuevoInvCaja").click(function () {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            const codLocal = $("#cboLocal").val();

            var filasSeleccionada = document.querySelectorAll("#tableNumCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            console.log(NUM_CAJA);

            abrirModalNuevoInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA);
        });

        $("#btnEditarInvCaja").click(function () {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            const codLocal = $("#cboLocal").val();

            var filasSeleccionada = document.querySelectorAll("#tableCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
        });

        $("#btnEliminarInvCaja").click(async function () {
            var filasSeleccionada = document.querySelectorAll("#tableCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            const request = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val(),
                NumCaja: NUM_CAJA,
                codActivo: COD_ACTIVO
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
                    await eliminarInvCaja(request);
                    swal("Eliminado!", "El registro ha sido eliminado.", "success");
                }
            });
        });

        $("#btnResumenPorNumCaja").click(function () {
            var filasSeleccionada = document.querySelectorAll("#tableNumCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const request = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val(),
                NumCaja: NUM_CAJA
            };
            abrirModalDatosNumCaja(request);
        });

        $("#btnGuardarInvCaja").on("click", async function () {
            var inv_caja = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val(),
                NumCaja: $("#txtNumCaja").val(),
                CodActivo: $("#cboCodActivo").val(),
                CodModelo: $("#txtCodModelo").val(),
                CodSerie: $("#txtCodSerie").val(),
                NumAdenda: $("#txtNumAdenda").val(),
                TipProcesador: $("#txtTipProcesador").val(),
                Memoria: $("#txtMemoria").val(),
                DesSo: $("#txtDesSo").val(),
                VerSo: $("#txtVerSo").val(),
                CapDisco: $("#txtCapDisco").val(),
                DesPuertoBalanza: $("#txtDesPuertoBalanza").val(),
                TipEstado: $("#cboTipEstado").val(),
                FecGarantia: $("#txtFecGarantia").val()
            };

            if (validarFormInvCaja(inv_caja))
                await guardarInvCaja(inv_caja, urlCrearInvCaja);
        });

        $("#btnActualizarInvCaja").on("click", async function () {
            var inv_caja = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#cboLocal").val(),
                NumCaja: $("#txtNumCaja").val(),
                CodActivo: $("#txtCodActivo").val(),
                CodModelo: $("#txtCodModelo").val(),
                CodSerie: $("#txtCodSerie").val(),
                NumAdenda: $("#txtNumAdenda").val(),
                TipProcesador: $("#txtTipProcesador").val(),
                Memoria: $("#txtMemoria").val(),
                DesSo: $("#txtDesSo").val(),
                VerSo: $("#txtVerSo").val(),
                CapDisco: $("#txtCapDisco").val(),
                DesPuertoBalanza: $("#txtDesPuertoBalanza").val(),
                TipEstado: $("#cboTipEstado").val(),
                FecGarantia: $("#txtFecGarantia").val()
            };

            if (validarFormInvCaja(inv_caja))
                await guardarInvCaja(inv_caja, urlActualizarInvCaja);
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
            await sleep(1200);
            $("#cboZona").val(COD_ZONA).trigger('change');
            await sleep(1500);
            $("#cboLocal").val(COD_LOCAL).trigger('change');

            if ($.fn.DataTable.isDataTable('#tableCajas')) {
                $('#tableCajas').DataTable().clear().draw();
                $('#tableCajas').DataTable().destroy();
            }

            recargarDataTableNumCajas(request);

            $("#modalLocales").modal('hide');
            closeLoading();
        });

        $('#tableNumCajas tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
                numeroCaja = 0;
            } else {
                dtListaNumCajas.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
                var filasSeleccionada = $(this);
                const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
                const request = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val(),
                    NumCaja: NUM_CAJA
                };
                recargarDataTableCajas(request);
            }
        });

        $('#tableCajas tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaCajas.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableCajas tbody').on('dblclick', 'tr', function () {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            const codLocal = $("#cboLocal").val();

            var filasSeleccionada = $(this);

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
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
            await sleep(1200);
            $("#cboZona").val(COD_ZONA).trigger('change');
            await sleep(1500);
            $("#cboLocal").val(COD_LOCAL).trigger('change');

            if ($.fn.DataTable.isDataTable('#tableCajas')) {
                $('#tableCajas').DataTable().clear().draw();
                $('#tableCajas').DataTable().destroy();
            }

            recargarDataTableNumCajas(request);
            closeLoading();
            $("#modalLocales").modal('hide');
        });

        $("#btnImportar").on("click", function () {
            $("#excelInventario").trigger("click");
        });

        $('#excelInventario').change(function (e) {
            importarExcelInventario();
        });

        $("#btnDescargarPlantillas").on("click", function () {
            descargarPlantillas();
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

    const listarCajas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            const codLocal = $("#cboLocal").val();
            const numCaja = $("#cboCaja").val();

            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();
            if (!codZona) return resolve();
            if (!codLocal) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion,
                CodZona: codZona,
                CodLocal: codLocal
            };

            $.ajax({
                url: urlListarCajas,
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

    const listarActivosPorCaja = function (request) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: urlListarInvCajas,
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

    const obtenerInvCaja = function (codEmpresa, codCadena, codRegion, codZona, codLocal, numCaja, codActivo) {
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
                NumCaja: numCaja,
                CodActivo: codActivo
            };

            $.ajax({
                url: urlObtenerInvCaja,
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
                response.Data.map(zona => {
                    $('#cboZona').append($('<option>', { value: zona.CodZona, text: zona.NomZona }));
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

    const validarFormInvCaja = function (invcaja) {
        let validate = true;

        if (invcaja.CodEmpresa === '' || invcaja.CodCadena === '' || invcaja.CodRegion === '' || invcaja.CodZona === '' || invcaja.CodLocal === ''
            || invcaja.CodActivo === '' || invcaja.NumCaja === '') {
            validate = false;
            $("#formInvCaja").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarBuscarInvCaja = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '' || $("#cboLocal").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, cadena, region, zona y local', icon: "warning", });
        }

        return validate;
    }

    const validarBuscarInvCajaPorEmpresa = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa.', icon: "warning", });
        }

        return validate;
    }

    const guardarInvCaja = function (invCaja, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: invCaja },
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
                    NumCaja: invCaja.NumCaja,
                };
                recargarDataTableCajas(request);
                $("#modalInvCaja").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const abrirModalEditarInvCaja = async function (codEmpresa, codCadena, codRegion, codZona, codLocal, numCaja, codActivo) {
        $("#tituloModalInvCaja").html("Editar Inventario");
        $("#btnActualizarInvCaja").show();
        $("#btnGuardarInvCaja").hide();

        const response = await obtenerInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, numCaja, codActivo);
        const model = response.Data;

        await cargarFormInvCaja(model, true);
    }

    const abrirModalNuevoInvCaja = async function (codEmpresa, codCadena, codRegion, codZona, codLocal, numCaja) {
        $("#tituloModalInvCaja").html("Nuevo Inventario");
        $("#btnActualizarInvCaja").hide();
        $("#btnGuardarInvCaja").show();

        const model = {};
        model.CodEmpresa = codEmpresa;
        model.CodCadena = codCadena;
        model.CodRegion = codRegion;
        model.CodZona = codZona;
        model.CodLocal = codLocal;
        model.NumCaja = numCaja;
        model.TipEstado = "A";

        await cargarFormCrearInvCaja(model);
    }

    const abrirModalDatosNumCaja = async function (model) {
        var dataActivos = await listarActivosPorCaja(model);

        const codActivo01 = dataActivos.Data.find(item => item.CodActivo === "01");
        if (codActivo01) {
            $('#txtModeloCpu').val(codActivo01.CodModelo || '');
            $('#txtSerieCpu').val(codActivo01.CodSerie || '');
        }

        const codActivo02 = dataActivos.Data.find(item => item.CodActivo === "02");
        if (codActivo02) {
            $('#txtModeloImpresora').val(codActivo02.CodModelo || '');
            $('#txtSerieImpresora').val(codActivo02.CodSerie || '');
        }

        const codActivo03 = dataActivos.Data.find(item => item.CodActivo === "03");
        if (codActivo03) {
            $('#txtModeloDynakey').val(codActivo03.CodModelo || '');
            $('#txtSerieDynakey').val(codActivo03.CodSerie || '');
        }

        const codActivo04 = dataActivos.Data.find(item => item.CodActivo === "04");
        if (codActivo04) {
            $('#txtModeloScanner').val(codActivo04.CodModelo || '');
            $('#txtSerieScanner').val(codActivo04.CodSerie || '');
        }

        const codActivo05 = dataActivos.Data.find(item => item.CodActivo === "05");
        if (codActivo05) {
            $('#txtModeloGaveta').val(codActivo05.CodModelo || '');
            $('#txtSerieGaveta').val(codActivo05.CodSerie || '');
        }

        const codActivo15 = dataActivos.Data.find(item => item.CodActivo === "15");
        if (codActivo15) {
            $('#txtModeloMonitor').val(codActivo15.CodModelo || '');
            $('#txtSerieMonitor').val(codActivo15.CodSerie || '');
        }

        $("#modalDatosNumCaja").modal('show');
    }

    const cargarFormInvCaja = async function (model, deshabilitar) {
        $.ajax({
            url: urlEditarFormInvCaja,
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
                $("#modalInvCaja").find(".modal-body").html(response);
                $("#modalInvCaja").modal('show');
                $("#txtNumCaja").prop("disabled", deshabilitar);
                $("#txtCodActivo").prop("disabled", deshabilitar);
                $("#txtNomActivo").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormCrearInvCaja = async function (model) {
        $.ajax({
            url: urlCrearFormInvCaja,
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
                $("#modalInvCaja").find(".modal-body").html(response);
                $("#modalInvCaja").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const eliminarInvCaja = async function (request) {
        $.ajax({
            url: urlEliminarInvCaja,
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
                //$("#modalInvCaja").find(".modal-body").html(response);
                //$("#modalInvCaja").modal('show');
                console.log(response);
                recargarDataTableCajas(request);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const eliminarInvCajaPorLocal = async function (request) {
        $.ajax({
            url: urlEliminarInvCajaPorLocal,
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
                console.log(response);

                if ($.fn.DataTable.isDataTable('#tableCajas')) {
                    $('#tableCajas').DataTable().clear().draw();
                    $('#tableCajas').DataTable().destroy();
                }

                recargarDataTableNumCajas(request);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarDataTableNumCajas = function (request) {
        if ($.fn.DataTable.isDataTable('#tableNumCajas')) {
            $('#tableNumCajas').DataTable().destroy();
        }
        dtListaNumCajas = $('#tableNumCajas').DataTable({
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
                        text: 'Error al listar num cajas: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "NumCaja" },
                { data: "IpAddress" },
                { data: "TipOs" },
                //{ data: "TipEstado" },
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

    const recargarDataTableCajas = function (request) {
        if ($.fn.DataTable.isDataTable('#tableCajas')) {
            $('#tableCajas').DataTable().destroy();
        }
        dtListaCajas = $('#tableCajas').DataTable({
            ajax: {
                url: urlListarInvCajas,
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
                        text: 'Error al listar cajas: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "NumCaja" },
                { data: "CodActivo" },
                { data: "InvTipoActivo.NomActivo" },
                { data: "CodModelo" },
                { data: "CodSerie" },
                { data: "NumAdenda" },
                { data: "TipEstado" },
                { data: "TipProcesador" },
                { data: "Memoria" },
                { data: "DesSo" },
                { data: "VerSo" },
                { data: "CapDisco" },
                { data: "DesPuertoBalanza" },
                { data: "FecGarantia" }
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

    const importarExcelInventario = function () {

        var formData = new FormData();
        var uploadFiles = $('#excelInventario').prop('files');
        formData.append("excelInventario", uploadFiles[0]);

        $.ajax({
            url: urlImportarInventario,
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
                            $('#modalErroresImportacionCaja').modal("show");
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

    const descargarPlantillas = function () {

        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
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
