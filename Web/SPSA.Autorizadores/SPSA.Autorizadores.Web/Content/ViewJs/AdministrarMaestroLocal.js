var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresa';
var urlListarCadenas = baseUrl + 'Maestros/MaeCadena/ListarCadena';
var urlListarRegiones = baseUrl + 'Maestros/MaeRegion/ListarRegion';
var urlListarZonas = baseUrl + 'Maestros/MaeZona/ListarZona';
var urlListarLocales = baseUrl + 'Maestros/MaeLocal/ListarLocal';
var urlListarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var urlListarCajas = baseUrl + 'Maestros/MaeCaja/ListarCaja';
var urlListarHorarios = baseUrl + 'Maestros/MaeHorario/Listar';

var urlCrearLocal = baseUrl + 'Maestros/MaeLocal/CrearLocal';
var urlCrearCaja = baseUrl + 'Maestros/MaeCaja/CrearCaja';
var urlCrearHorario = baseUrl + 'Maestros/MaeHorario/Crear';
var urlActualizarLocal = baseUrl + 'Maestros/MaeLocal/ActualizarLocal';
var urlActualizarCaja = baseUrl + 'Maestros/MaeCaja/ActualizarCaja';
var urlActualizarHorario = baseUrl + 'Maestros/MaeHorario/Actualizar';
var urlObtenerLocal = baseUrl + 'Maestros/MaeLocal/ObtenerLocal';
var urlEliminarCajas = baseUrl + 'Maestros/MaeCaja/EliminarCajas';
var urlEliminarHorario = baseUrl + 'Maestros/MaeHorario/Eliminar';

var urlModalCrearEditarCaja = baseUrl + 'Maestros/MaeCaja/CrearEditarCaja';
var urlFormCrearHorario = baseUrl + 'Maestros/MaeHorario/FormularioCrear';
var urlFormEditarHorario = baseUrl + 'Maestros/MaeHorario/FormularioEditar';

var urlImportarLocales = baseUrl + 'Maestros/MaeLocal/ImportarExcel';
var urlImportarCajas = baseUrl + 'Maestros/MaeCaja/ImportarExcel';

var urlDescargarLocalesPorEmpresa = baseUrl + 'Maestros/MaeLocal/DescargarLocalPorEmpresa';
var urlDescargarCajasPorLocal = baseUrl + 'Maestros/MaeCaja/DescargarCajaPorLocal';
var urlDescargarCajasPorEmpresa = baseUrl + 'Maestros/MaeCaja/DescargarCajaPorEmpresa';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var urlFechaSistema = baseUrl + 'Locales/AdministrarLocal/ObtenerFechaSistema';

var codRegionAnterior = "";
var codZonaAnterior = "";
var permitirCambioZonaRegion = false;

// DataTables handlers
var dataTableLocales = null;   // #tableLocales
var dtListaCajas = null;       // #tableCajas
var dtListaHorarios = null;    // #tableHorarios

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
            if (!validarFormulario(["#cboEmpresa", "#cboCadena", "#cboRegion", "#cboZona", "#cboLocal"], "")) return;

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
        });

        $("#btnPlantillas").on("click", function () {
            $("#modalPlantillas").modal('show');
        });

        $("#btnNuevoLocal").on("click", function () {
            limpiar();
        });

        // Si existe el botón Limpiar, vincular
        if ($("#btnLimpiarTodo").length) {
            $("#btnLimpiarTodo").on("click", function () {
                limpiar();
            });
        }

        $("#btnGuardarLocal, #btnGuardarCambiosLocal").on("click", async function () {
            if (!validarFormulario(["#cboEmpresa", "#cboCadena", "#cboRegion", "#cboZona", "#txtCodLocal"], "Debe seleccionar la empresa, cadena, región, zona y local")) return;

            const isGuardarCambios = $(this).attr("id") === "btnGuardarCambiosLocal";
            const titulo = isGuardarCambios ? "¿Está seguro guardar la información modificada?" : "¿Está seguro guardar la información ingresada?";

            const willSave = await swal({
                title: "Confirmar!",
                text: titulo,
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            });

            if (willSave) {
                const local = {
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
                    DirLocal: $("#txtDirLocal").val(),
                    Ubigeo: $("#txtUbigeo").val(),
                    FecCierre: $("#txtFecCierre").val(),
                    FecEntrega: $("#txtFecEntrega").val(),
                    ...(isGuardarCambios && {
                        CodRegionAnterior: codRegionAnterior,
                        CodZonaAnterior: codZonaAnterior,
                    }),
                };

                if (validarLocal(local)) {
                    if (isGuardarCambios) {
                        permitirCambioZonaRegion = false;
                        actualizarLocal(local);
                    } else {
                        guardarLocal(local);
                    }
                }
            }
        });

        $("#btnHabilitarlCambioRegionZona").on("click", function () {
            $("#cboRegion").prop("disabled", false);
            $("#cboZona").prop("disabled", false);
            $("#btnGuardarCambiosLocal").prop("disabled", false);
            permitirCambioZonaRegion = true;
        });

        $("#btnBuscarLocalPorEmpresa").on("click", function () {
            if (!validarFormulario(["#cboEmpresa"], "Debe seleccionar una empresa")) return;

            permitirCambioZonaRegion = false;
            $("#modalLocales").modal('show');
            recargarDataTableLocalesPorEmpresa();
        });

        $("#btnObtenerLocal").on("click", async function () {
            var filasSeleccionada = document.querySelectorAll("#tableLocales tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length, true)) return;

            showLoading();
            const fila = filasSeleccionada[0];
            const request = {
                CodEmpresa: fila.querySelector('td:nth-child(6)').textContent.trim(),
                CodCadena: fila.querySelector('td:nth-child(7)').textContent.trim(),
                CodRegion: fila.querySelector('td:nth-child(8)').textContent.trim(),
                CodZona: fila.querySelector('td:nth-child(9)').textContent.trim(),
                CodLocal: fila.querySelector('td:nth-child(1)').textContent.trim(),
            };

            codRegionAnterior = request.CodRegion;
            codZonaAnterior = request.CodZona;

            await asignarValorSelectorConDelay("#cboCadena", request.CodCadena, 1000);
            await asignarValorSelectorConDelay("#cboRegion", request.CodRegion, 1000);
            await asignarValorSelectorConDelay("#cboZona", request.CodZona, 1200);
            await asignarValorSelectorConDelay("#cboLocal", request.CodLocal, 1500);

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

        $('#tableHorarios tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#btnNuevaCaja").on("click", function () {
            abrirModalCaja(false);
        });

        $("#btnNuevoHorario").on("click", function () {
            abrirModalHorario(false);
        });

        $("#btnEditarCaja").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length, true)) return;

            const fila = filasSeleccionada[0];
            const model = {
                CodLocal: $("#txtCodLocal").val(), // evita usar variable global obsoleta
                NumCaja: fila.querySelector('td:first-child').textContent.trim(),
                IpAddress: fila.querySelector('td:nth-child(2)').textContent.trim(),
                TipOs: fila.querySelector('td:nth-child(3)').textContent.trim(),
                TipCaja: fila.querySelector('td:nth-child(4)').textContent.trim(),
                TipUbicacion: fila.querySelector('td:nth-child(5)').textContent.trim(),
                TipEstado: fila.querySelector('td:nth-child(6)').textContent.trim(),
            };

            abrirModalCaja(true, model);
        });

        $("#btnEditarHorario").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableHorarios tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length, true)) return;

            const fila = filasSeleccionada[0];
            const model = {
                NumDia: fila.querySelector('td:first-child').textContent,
                CodDia: fila.querySelector('td:nth-child(2)').textContent.trim(),
                HorOpen: fila.querySelector('td:nth-child(3)').textContent,
                HorClose: fila.querySelector('td:nth-child(4)').textContent.trim(),
                MinLmt: fila.querySelector('td:nth-child(5)').textContent.trim(),
            };

            abrirModalHorario(true, model);
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
                TipEstado: $("#cboTipEstadoCaja").val(),
                TipCaja: $("#cboTipCaja").val(),
                TipUbicacion: $("#cboTipUbicacion").val(),
            };

            if (validarCaja(caja))
                await guardarCaja(caja, urlCrearCaja);
        });

        $("#btnGuardarHorario").on("click", async function () {
            var horario = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#txtCodLocal").val(),
                NumDia: $("#cboNumDia").val(),
                CodDia: $("#cboNumDia option:selected").text().substring(0, 2),
                HorOpen: $("#txtHorOpen").val(),
                HorClose: $("#txtHorClose").val(),
                MinLmt: $("#txtMinLmt").val()
            };

            if (validarHorario(horario))
                await guardarHorario(horario, urlCrearHorario);
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
                TipEstado: $("#cboTipEstadoCaja").val(),
                TipCaja: $("#cboTipCaja").val(),
                TipUbicacion: $("#cboTipUbicacion").val(),
            };

            if (validarCaja(caja))
                await guardarCaja(caja, urlActualizarCaja);

        });

        $("#btnActualizarHorario").on("click", async function () {
            var horario = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodCadena: $("#cboCadena").val(),
                CodRegion: $("#cboRegion").val(),
                CodZona: $("#cboZona").val(),
                CodLocal: $("#txtCodLocal").val(),
                NumDia: $("#cboNumDia").val(),
                CodDia: $("#cboNumDia option:selected").text().substring(0, 2),
                HorOpen: $("#txtHorOpen").val(),
                HorClose: $("#txtHorClose").val(),
                MinLmt: $("#txtMinLmt").val()
            };

            if (validarHorario(horario))
                await guardarHorario(horario, urlActualizarHorario);

        });

        $("#btnEliminarCaja").on("click", function () {
            var filasSeleccionadas = document.querySelectorAll("#tableCajas tbody tr.selected");
            if (!validarSelecion(filasSeleccionadas.length)) return;

            var cajasParaEliminar = [];

            filasSeleccionadas.forEach(function (filas) {
                var caja = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val(),
                    NumCaja: filas.querySelector('td:first-child').textContent,
                    IpAddress: filas.querySelector('td:nth-child(2)').textContent.trim(),
                    TipOs: filas.querySelector('td:nth-child(3)').textContent,
                    TipCaja: filas.querySelector('td:nth-child(4)').textContent.trim(),
                    TipUbicacion: filas.querySelector('td:nth-child(5)').textContent.trim(),
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

        $("#btnEliminarHorario").on("click", function () {
            var filasSeleccionadas = document.querySelectorAll("#tableHorarios tbody tr.selected");
            if (!validarSelecion(filasSeleccionadas.length)) return;

            var horariosParaEliminar = [];

            filasSeleccionadas.forEach(function (filas) {
                var horario = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    CodCadena: $("#cboCadena").val(),
                    CodRegion: $("#cboRegion").val(),
                    CodZona: $("#cboZona").val(),
                    CodLocal: $("#cboLocal").val(),
                    NumDia: filas.querySelector('td:first-child').textContent,
                    CodDia: filas.querySelector('td:nth-child(2)').textContent.trim(),
                    HorOpen: filas.querySelector('td:nth-child(3)').textContent,
                    HorClose: filas.querySelector('td:nth-child(4)').textContent.trim(),
                    MinLmt: filas.querySelector('td:nth-child(5)').textContent.trim(),
                };
                horariosParaEliminar.push(horario);
            });

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar los horarios seleccionados?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    eliminarHorarios(horariosParaEliminar);
                }
            });
        });

        $("#btnPlantillaLocal").on("click", function () {
            descargarPlantillas("Plantilla_Local");
        });

        $("#btnPlantillaCaja").on("click", function () {
            descargarPlantillas("Plantilla_Caja");
        });

        $("#btnPlantillaHorario").on("click", function () {
            descargarPlantillas("Plantilla_Horario");
        });

        $("#btnDescargarLocalesPorEmpresa").on("click", function () {
            if (!validarFormulario(["#cboEmpresa"], "Debe seleccionar la empresa")) return;
            descargarLocalesPorEmpresa();
        });

        $("#btnDescargarCajasPorEmpresa").on("click", function () {
            if (!validarFormulario(["#cboEmpresa"], "Debe seleccionar la empresa")) return;
            descargarCajas(urlDescargarCajasPorEmpresa);
        });

        $("#btnDescargarCajasPorLocal").on("click", function () {
            if (!validarFormulario(["#cboEmpresa", "#cboCadena", "#cboRegion", "#cboZona", "#txtCodLocal"], "Debe seleccionar la empresa, cadena, región, zona y local")) return;
            descargarCajas(urlDescargarCajasPorLocal);
        });

        $("#btnImportarCajas").on("click", function () {
            $("#excelImportarCajas").trigger("click");
        });

        $('#excelImportarCajas').change(function () {
            importarCajasDesdeExcel();
        });

        $("#btnImportarLocales").on("click", function () {
            $("#excelImportarLocales").trigger("click");
        });

        $('#excelImportarLocales').change(function () {
            importarLocalesDesdeExcel();
        });
    };

    // --------- Listados para combos ---------

    const listarEmpresas = function () {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: urlListarEmpresas,
                type: "post",
                data: { request: {} },
                success: function (response) { resolve(response); },
                error: function (jqXHR) { reject(jqXHR.responseText); }
            });
        });
    };

    const listarCadenas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            if (!codEmpresa) return resolve();
            $.ajax({
                url: urlListarCadenas,
                type: "post",
                data: { request: { CodEmpresa: codEmpresa } },
                success: function (response) { resolve(response); },
                error: function (jqXHR) { reject(jqXHR.responseText); }
            });
        });
    };

    const listarRegiones = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            if (!codEmpresa) return resolve();
            $.ajax({
                url: urlListarRegiones,
                type: "post",
                data: { request: { CodEmpresa: codEmpresa, CodCadena: codCadena } },
                success: function (response) { resolve(response); },
                error: function (jqXHR) { reject(jqXHR.responseText); }
            });
        });
    };

    const listarZonas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            if (!codEmpresa || !codCadena || !codRegion) return resolve();
            $.ajax({
                url: urlListarZonas,
                type: "post",
                data: { request: { CodEmpresa: codEmpresa, CodCadena: codCadena, CodRegion: codRegion } },
                success: function (response) { resolve(response); },
                error: function (jqXHR) { reject(jqXHR.responseText); }
            });
        });
    };

    const listarLocales = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();
            if (!codEmpresa || !codCadena || !codRegion || !codZona) return resolve();
            $.ajax({
                url: urlListarLocales,
                type: "post",
                data: { request: { CodEmpresa: codEmpresa, CodCadena: codCadena, CodRegion: codRegion, CodZona: codZona } },
                success: function (response) { resolve(response); },
                error: function (jqXHR) { reject(jqXHR.responseText); }
            });
        });
    };

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
            } else {
                swal({ text: response.Mensaje, icon: "error" });
            }
        } catch (error) {
            swal({ text: error, icon: "error" });
        }
    };

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
                swal({ text: response.Mensaje, icon: "error" });
            }
        } catch (error) {
            swal({ text: error, icon: "error" });
        }
    };

    const cargarComboRegiones = async function () {
        try {
            const response = await listarRegiones();
            if (response === undefined) return;
            if (response.Ok) {
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
                    $('#cboRegion').append($('<option>', { value: region.CodRegion, text: region.NomRegion }));
                });
            } else {
                swal({ text: response.Mensaje, icon: "error" });
            }
        } catch (error) {
            swal({ text: error, icon: "error" });
        }
    };

    const cargarComboZonas = async function () {
        try {
            const response = await listarZonas();
            if (response === undefined) return;
            if (response.Ok) {
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
                    $('#cboZona').append($('<option>', { value: region.CodZona, text: region.NomZona }));
                });
            } else {
                swal({ text: response.Mensaje, icon: "error" });
            }
        } catch (error) {
            swal({ text: error, icon: "error" });
        }
    };

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
                swal({ text: response.Mensaje, icon: "error" });
            }
        } catch (error) {
            swal({ text: error, icon: "error" });
        }
    };

    // --------- DataTables (Locales/Cajas/Horarios) ---------

    const visualizarDataTableLocales = function () {
        $.ajax({
            url: urlListarLocales,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {
                if ($.fn.DataTable.isDataTable('#tableLocales')) {
                    $('#tableLocales').DataTable().destroy();
                }
                dataTableLocales = $('#tableLocales').DataTable({
                    language: { searchPlaceholder: 'Buscar...', sSearch: '' },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Locales,
                    bAutoWidth: false
                });
            },
            error: function (jqXHR) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    };

    const recargarDataTableLocalesPorEmpresa = function () {
        const request = { CodEmpresa: $("#cboEmpresa").val() };

        if ($.fn.DataTable.isDataTable('#tableLocales')) {
            $('#tableLocales').DataTable().destroy();
        }

        dataTableLocales = $('#tableLocales').DataTable({
            ajax: {
                url: urlListarLocalesPorEmpresa,
                type: "post",
                dataType: "JSON",
                dataSrc: "Data",
                data: { request },
                beforeSend: function () { showLoading(); },
                complete: function () { closeLoading(); },
                error: function (jqXHR) {
                    swal({ text: 'Error al listar los locales: ' + jqXHR, icon: "error" });
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
            language: { searchPlaceholder: 'Buscar...', sSearch: '' },
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            bAutoWidth: false
        });
    };

    const limpiar = function () {
        permitirCambioZonaRegion = false;
        codRegionAnterior = "";
        codZonaAnterior = "";

        // Combos
        $("#cboEmpresa").val('').trigger('change');
        $("#cboCadena").val('').trigger('change');
        $("#cboRegion").val('').trigger('change');
        $("#cboZona").val('').trigger('change');
        $("#cboLocal").val('').trigger('change');
        $("#cboTipEstado").val('A').trigger('change');

        // Textos / números
        $("#txtCodLocal").val('');
        $("#txtNomLocal").val('');
        $("#txtCodLocalPMM").val('0');
        $("#txtIP").val('0.0.0.0');
        $("#txtCodLocalOfiplan").val('0');
        $("#txtNomLocalOfiplan").val('');
        $("#txtCodLocalSunat").val('0');
        $("#txtDirLocal").val('0');
        $("#txtUbigeo").val('000000');
        $("#txtFecApertura").val('');
        $("#txtFecCierre").val('');
        $("#txtFecEntrega").val('');

        // Quitar selecciones visuales en tablas
        $('#tableLocales tbody tr, #tableCajas tbody tr, #tableHorarios tbody tr').removeClass('selected');

        // Limpiar DataTable Locales
        if ($.fn.DataTable.isDataTable('#tableLocales')) {
            $('#tableLocales').DataTable().clear().draw();
            $('#tableLocales').DataTable().destroy();
            dataTableLocales = null;
        }
        $('#tableLocales tbody').empty();

        // Limpiar DataTable Cajas
        if ($.fn.DataTable.isDataTable('#tableCajas')) {
            $('#tableCajas').DataTable().clear().draw();
            $('#tableCajas').DataTable().destroy();
            dtListaCajas = null;
        }
        $('#tableCajas tbody').empty();

        // Limpiar DataTable Horarios
        if ($.fn.DataTable.isDataTable('#tableHorarios')) {
            $('#tableHorarios').DataTable().clear().draw();
            $('#tableHorarios').DataTable().destroy();
            dtListaHorarios = null;
        }
        $('#tableHorarios tbody').empty();

        // Botones / estados
        $("#btnGuardarLocal").prop("disabled", false);
        $("#btnGuardarCambiosLocal").prop("disabled", true);
        $("#btnHabilitarlCambioRegionZona").prop("disabled", true);

        deshabilitarGrupoBotones("#btn-cajas", true);
        deshabilitarGrupoBotones("#btn-horarios", true);
        desabilitarControles(false);

        // Quitar validaciones previas
        $("#formLocal").removeClass("was-validated");

        // Volver a dejar la página como al inicio
        obtenerFechaSistema();
        visualizarDataTableLocales();
    };

    // --------- Guardar/Actualizar/Eliminar ---------

    const validarLocal = function (local) {
        let validate = true;

        if (local.CodEmpresa === '' || local.CodCadena === '' || local.CodRegion === '' || local.CodZona === '' || local.CodLocal === '' ||
            local.NomLocal === '' || local.TipEstado === '' || local.CodLocalPMM === '' || local.Ip === '' || local.CodLocalSunat === '') {
            validate = false;
            $("#formLocal").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning" });
        }

        return validate;
    };

    const guardarLocal = function (local) {
        $.ajax({
            url: urlCrearLocal,
            type: "post",
            data: { command: local },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                swal({ text: response.Mensaje, icon: "success" });
                deshabilitarGrupoBotones("#btn-cajas", true);
                deshabilitarGrupoBotones("#btn-horarios", true);
                $("#btnGuardarLocal").prop("disabled", true);
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const actualizarLocal = function (local) {
        $.ajax({
            url: urlActualizarLocal,
            type: "post",
            data: { command: local },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                swal({ text: response.Mensaje, icon: "success" });
                deshabilitarGrupoBotones("#btn-cajas", false);
                deshabilitarGrupoBotones("#btn-horarios", false);
                $("#btnGuardarLocal").prop("disabled", true);
                $("#cboRegion").prop("disabled", true);
                $("#cboZona").prop("disabled", true);
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const validarFormulario = function (campos, mensaje) {
        for (const campo of campos) {
            if ($(campo).val() === '') {
                if (mensaje) swal({ text: mensaje, icon: "warning" });
                return false;
            }
        }
        return true;
    };

    const validarSelecion = function (count, unSoloRegistro = false) {
        if (count === 0) {
            swal({ text: "Debe seleccionar como mínimo un registro", icon: "warning" });
            return false;
        }
        if (unSoloRegistro && count > 1) {
            swal({ text: "Solo debe seleccionar un registro", icon: "warning" });
            return false;
        }
        return true;
    };

    const obtenerLocal = function (request) {
        $.ajax({
            url: urlObtenerLocal,
            type: "post",
            data: { request },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }

                $("#modalLocales").modal('hide');

                setearLocal(response.Data);
                deshabilitarGrupoBotones("#btn-cajas", false);
                deshabilitarGrupoBotones("#btn-horarios", false);
                desabilitarControles(true);
                $("#btnGuardarLocal").prop("disabled", true);
                $("#btnGuardarCambiosLocal").prop("disabled", false);
                $("#btnHabilitarlCambioRegionZona").prop("disabled", false);

                await recargarDataTableCajas();
                await recargarDataTableHorarios();
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const setearLocal = function (local) {
        if (local.FecApertura) {
            let timestamp = parseInt(local.FecApertura.match(/\d+/)[0], 10);
            local.FecApertura = new Date(timestamp).toISOString().split('T')[0];
        }
        if (local.FecCierre) {
            let timestamp = parseInt(local.FecCierre.match(/\d+/)[0], 10);
            local.FecCierre = new Date(timestamp).toISOString().split('T')[0];
        }
        if (local.FecEntrega) {
            let timestamp = parseInt(local.FecEntrega.match(/\d+/)[0], 10);
            local.FecEntrega = new Date(timestamp).toISOString().split('T')[0];
        }

        $("#cboTipEstado").val(local.TipEstado).trigger('change');
        $("#txtCodLocal").val(local.CodLocal);
        $("#txtNomLocal").val(local.NomLocal);
        $("#txtCodLocalPMM").val(local.CodLocalPMM);
        $("#txtIP").val(local.Ip);
        $("#txtCodLocalOfiplan").val(local.CodLocalOfiplan);
        $("#txtNomLocalOfiplan").val(local.NomLocalOfiplan);
        $("#txtCodLocalSunat").val(local.CodLocalSunat);
        $("#txtDirLocal").val(local.DirLocal);
        $("#txtUbigeo").val(local.Ubigeo);
        $("#txtFecApertura").val(local.FecApertura);
        $("#txtFecCierre").val(local.FecCierre);
        $("#txtFecEntrega").val(local.FecEntrega);
    };

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
                beforeSend: function () { showLoading(); },
                complete: function () { closeLoading(); },
                error: function (jqXHR) {
                    swal({ text: 'Error al listar cajas: ' + jqXHR, icon: "error" });
                }
            },
            columns: [
                { data: "NumCaja" },
                { data: "IpAddress" },
                { data: "TipOs" },
                { data: "TipCaja" },
                { data: "TipUbicacion" },
                { data: "TipEstado" }
            ],
            language: { searchPlaceholder: 'Buscar...', sSearch: '' },
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            bAutoWidth: false
        });
    };

    const recargarDataTableHorarios = async function () {
        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val(),
            CodLocal: $("#cboLocal").val()
        };

        if ($.fn.DataTable.isDataTable('#tableHorarios')) {
            $('#tableHorarios').DataTable().destroy();
        }

        dtListaHorarios = $('#tableHorarios').DataTable({
            ajax: {
                url: urlListarHorarios,
                type: "post",
                dataType: "JSON",
                dataSrc: "Data",
                data: { request },
                beforeSend: function () { showLoading(); },
                complete: function () { closeLoading(); },
                error: function (jqXHR) {
                    swal({ text: 'Error al listar horarios: ' + jqXHR, icon: "error" });
                }
            },
            columns: [
                { data: "NumDia" },
                { data: "CodDia" },
                { data: "HorOpen" },
                { data: "HorClose" },
                { data: "MinLmt" }
            ],
            language: { searchPlaceholder: 'Buscar...', sSearch: '' },
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            searching: false,
            bAutoWidth: false
        });
    };

    const deshabilitarGrupoBotones = function (selector, disable) {
        $(`${selector} :input`).attr("disabled", disable);
    };

    const desabilitarControles = function (disable) {
        $("#cboEmpresa").prop("disabled", disable);
        $("#cboCadena").prop("disabled", disable);
        $("#cboRegion").prop("disabled", disable);
        $("#cboZona").prop("disabled", disable);
        $("#cboLocal").prop("disabled", disable);
        $("#txtCodLocal").prop("disabled", disable);
        $("#txtNomLocal").prop("disabled", disable);
    };

    const abrirModalCaja = function (esEdicion, model = {}) {
        $("#tituloModalCaja").html(esEdicion ? "Actualizar Caja" : "Nueva Caja");
        $("#btnActualizarCaja").toggle(esEdicion);
        $("#btnGuardarCaja").toggle(!esEdicion);
        cargarFormCaja(model, esEdicion);
    };

    const abrirModalHorario = function (esEdicion, model = {}) {
        $("#tituloModalHorario").html(esEdicion ? "Actualizar Horario" : "Nueva Horario");
        $("#btnActualizarHorario").toggle(esEdicion);
        $("#btnGuardarHorario").toggle(!esEdicion);
        cargarFormHorario(model, esEdicion);
    };

    const cargarFormCaja = async function (model, deshabilitar) {
        try {
            showLoading();
            const response = await $.ajax({
                url: urlModalCrearEditarCaja,
                type: "post",
                data: { model },
                dataType: "html"
            });

            $("#modalCaja").find(".modal-body").html(response);
            $("#modalCaja").modal('show');

            configurarPlugins(deshabilitar);
        } catch (error) {
            swal({ text: error.responseText, icon: "error" });
        } finally {
            closeLoading();
        }
    };

    const cargarFormHorario = async function (model, esEdicion) {
        try {
            showLoading();
            const response = await $.ajax({
                url: esEdicion ? urlFormEditarHorario : urlFormCrearHorario,
                type: "post",
                data: { model },
                dataType: "html"
            });

            $("#modalHorario").find(".modal-body").html(response);
            $("#modalHorario").modal('show');

            $('.select2-show-modal').select2({
                minimumResultsForSearch: '',
                placeholder: "Seleccionar",
                width: '100%',
                dropdownParent: $('#modalHorario .modal-content')
            });

            $('#txtHorOpen, #txtHorClose').mask('00:00', {
                translation: { '0': { pattern: /[0-9]/, optional: false } },
            });

            $('#txtMinLmt').mask('ZZZ', { translation: { 'Z': { pattern: /[0-9]/, optional: true } } });

            $("#cboNumDia").prop("disabled", esEdicion);

        } catch (error) {
            swal({ text: error.responseText, icon: "error" });
        } finally {
            closeLoading();
        }
    };

    const configurarPlugins = function (deshabilitar) {
        $('.select2-show-modal').select2({
            minimumResultsForSearch: '',
            placeholder: "Seleccionar",
            width: '100%',
            dropdownParent: $('#modalCaja .modal-content')
        });

        $('#txtIpAddress').mask('099.099.099.099');

        // Permite ceros (evita que 30 se convierta en 3)
        $('#txtNumCaja').mask('999', {
            translation: { '9': { pattern: /[0-9]/, optional: true } }
        });

        $("#txtNumCaja").prop("disabled", deshabilitar);
    };

    const validarCaja = function (caja) {
        let validate = true;
        if (caja.CodEmpresa === '' || caja.CodCadena === '' || caja.CodRegion === '' || caja.CodZona === '' || caja.CodLocal === '' || caja.NumCaja === '' ||
            caja.IpAddress === '' || caja.TipOS === '' || caja.TipEstado === '' || caja.TipCaja === '' || caja.TipUbicacion === '') {
            validate = false;
            $("#formCaja").addClass("was-validated");
        }
        return validate;
    };

    const validarHorario = function (horario) {
        let validate = true;

        if (horario.CodEmpresa === '' || horario.CodCadena === '' || horario.CodRegion === '' || horario.CodZona === '' || horario.CodLocal === '' || horario.NumDia === '' ||
            horario.CodDia === '' || horario.HorOpen === '' || horario.HorClose === '' || horario.MinLmt === '') {
            validate = false;
            $("#formHorario").addClass("was-validated");
        }
        return validate;
    };

    const guardarCaja = function (caja, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: caja },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                swal({ text: response.Mensaje, icon: "success" });
                await recargarDataTableCajas();
                $("#modalCaja").modal('hide');
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const guardarHorario = function (horario, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: horario },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                swal({ text: response.Mensaje, icon: "success" });
                await recargarDataTableHorarios();
                $("#modalHorario").modal('hide');
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const eliminarCajas = function (cajas) {
        $.ajax({
            url: urlEliminarCajas,
            type: "post",
            data: { command: { Cajas: cajas } },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                swal({ text: response.Mensaje, icon: "success" });
                await recargarDataTableCajas();
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const eliminarHorarios = function (horarios) {
        $.ajax({
            url: urlEliminarHorario,
            type: "post",
            data: { command: { Horarios: horarios } },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                swal({ text: response.Mensaje, icon: "success" });
                await recargarDataTableHorarios();
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    // --------- Importaciones / Descargas ---------

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
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); $("#excelImportarCajas").val(null); },
            success: function (response) {
                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" }).then(() => {
                        if (response.Errores && response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => { html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`; });
                            $('#tbodyErrores').html(html);
                            $('#modalErrores').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success" });
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const importarLocalesDesdeExcel = function () {
        var formData = new FormData();
        var uploadFiles = $('#excelImportarLocales').prop('files');
        formData.append("excelImportarLocales", uploadFiles[0]);

        $.ajax({
            url: urlImportarLocales,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); $("#excelImportarLocales").val(null); },
            success: function (response) {
                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" }).then(() => {
                        if (response.Errores && response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => { html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`; });
                            $('#tbodyErrores').html(html);
                            $('#modalErrores').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success" });
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const descargarPlantillas = function (nombreCarpeta) {
        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
            data: { nombreCarpeta: nombreCarpeta },
            dataType: "json",
            success: function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                const linkSource = `data:application/zip;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                downloadLink.href = linkSource;
                downloadLink.download = response.NombreArchivo;
                downloadLink.click();
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

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
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                const linkSource = `data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                downloadLink.href = linkSource;
                downloadLink.download = response.NombreArchivo;
                downloadLink.click();
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    const descargarCajas = function (url) {
        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val(),
            CodLocal: $("#cboLocal").val()
        };

        $.ajax({
            url: url,
            type: "post",
            data: { request },
            dataType: "json",
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: async function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                const linkSource = `data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                downloadLink.href = linkSource;
                downloadLink.download = response.NombreArchivo;
                downloadLink.click();
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    // --------- Utilitarios ---------

    const inputMask = function () {
        $('#txtCodLocal').mask('ZZZZZZZZZZ', { translation: { 'Z': { pattern: /[0-9]/, optional: true } } });
        $('#txtCodLocalSunat').mask('0999'); // mantiene ceros a la izquierda
    };

    async function asignarValorSelectorConDelay(selector, value, delay) {
        $(selector).val(value).trigger('change');
        await new Promise(resolve => setTimeout(resolve, delay));
    }

    const obtenerFechaSistema = function () {
        $.ajax({
            url: urlFechaSistema,
            type: "post",
            dataType: "json",
            success: function (response) {
                if (!response.Ok) { swal({ text: response.Mensaje, icon: "warning" }); return; }
                $("#txtEl").val(response.Mensaje);
            },
            error: function (jqXHR) { swal({ text: jqXHR.responseText, icon: "error" }); }
        });
    };

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                inputMask();
                await cargarComboEmpresa();
                visualizarDataTableLocales();
            });
        }
    };

}(jQuery);
