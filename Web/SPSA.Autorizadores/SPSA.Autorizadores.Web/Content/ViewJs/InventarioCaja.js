const urlInsertarInventario = baseUrl + 'Locales/InventarioCaja/InsertarInventario';
const urlListarInventario = baseUrl + 'Locales/InventarioCaja/ListarInventario';
const urlObtenerInventario = baseUrl + 'Locales/InventarioCaja/ObtenerInventario';
const urlListarCaracteristicas = baseUrl + 'Locales/InventarioCaja/ListarCaracteristicas';
const urlEmpresas = baseUrl + 'Locales/AdministrarLocal/ListarEmpresas';
const urlFormatos = baseUrl + 'Locales/AdministrarLocal/ListarFormatos';
const urlLocales = baseUrl + 'Locales/AdministrarLocal/ListarLocales';
const urlCajas = baseUrl + 'Locales/AdministrarLocal/ListarCajas';
var urlFechaSistema = baseUrl + 'Locales/AdministrarLocal/ObtenerFechaSistema';
var urlDescargarMaestro = baseUrl + 'Locales/InventarioCaja/DescargarMaestro';
var urlImportarInventario = baseUrl + 'Locales/InventarioCaja/ImportarInventario';
var urlDescargarPlantilla = baseUrl + 'Locales/InventarioCaja/DescargarPlantillas';

var dataTableInventario = null;

const InventarioCaja = function () {


    const eventos = function () {

        $("#cboEmpresa").on("change", function (event, codFormato, caract1, caract2, caract3) {
            listarFormatos(codFormato);
            listarCaract(1, caract1);
            listarCaract(2, caract2);
            listarCaract(3, caract3);

        });

        $("#cboFormato").on("change", function (event, codFormato, codLocal) {
            listarLocales(codFormato, codLocal);
        });

        $("#cboLocal").on("change", function (event, codFormato, codLocal, numPos) {
            listarCajas(codFormato, codLocal, numPos);
        });

        $("#btnGuardar").on("click", function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro guardar la información ingresada?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then(async (willDelete) => {
                if (willDelete) {

                    const caja = {
                        CodEmpresa: $("#cboEmpresa").val(),
                        CodFormato: $("#cboFormato").val(),
                        CodLocal: $("#cboLocal").val(),
                        NumPos: $("#cboNumPos").val(),
                        Ranking: $("#txtRanking").val(),
                        Estado: $("#cboEstado").val(),
                        Sede: $("#txtSede").val(),
                        Ubicacion: $("#txtUbicación").val(),
                        Caja: $("#txtCaja").val(),
                        ModeloCpu: $("#txtModeloCpu").val(),
                        Serie: $("#txtSerieCpu").val(),
                        ModeloPrint: $("#txtModeloImpresora").val(),
                        SeriePrint: $("#txtSerieImpresora").val(),
                        ModeloDinakey: $("#txtModeloDynakey").val(),
                        SerieDinakey: $("#txtSerieDynakey").val(),
                        ModeloScanner: $("#txtModeloScanner").val(),
                        SerieScanner: $("#txtSerieScanner").val(),
                        ModeloGaveta: $("#txtModeloGaveta").val(),
                        SerieGaveta: $("#txtSerieGaveta").val(),
                        ModeloMonitor: $("#txtModeloMonitor").val(),
                        SerieMonitor: $("#txtSerieMonitor").val(),
                        FechaApertura: $("#txtFechaApertura").val(),
                        Caract1: $("#cboCaract1").val(),
                        Caract2: $("#cboCaract2").val(),
                        Caract3: $("#cboCaract3").val(),
                        FechaLising: $("#txtFechaLising").val(),
                        So: $("#cboSo").val(),
                        VesionSo: $("#txtVersion").val(),
                        FechaAsignacion: $("#txtFechaAsignacion").val(),
                    };

                    if (validar(caja))
                        await guardar(caja);
                }
            });


        });

        $("#btnNuevo").on("click", function () {
            limpiar();
            desabilitarControles(false);
        });

        $("#btnBuscar").on("click", function () {
            $("#modalInventario").modal('show');
            recargarDataTableInventario();
        });

        $('#tableInventario tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableInventario.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $("#btnAceptar").on("click", function () {
            const registrosSeleccionados = dataTableInventario.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length, true)) {
                return;
            }
            obtenerInventario(registrosSeleccionados[0].COD_EMPRESA, registrosSeleccionados[0].COD_FORMATO, registrosSeleccionados[0].COD_LOCAL, registrosSeleccionados[0].POS);

        });

        $("#btnDescargarMaestro").on("click", function () {
            descargarMaestro();
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

    const listarEmpresas = function () {
        $.ajax({
            url: urlEmpresas,
            type: "post",
            success: function (response) {

                if (response.Ok) {
                    $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
                    response.Empresas.map(empresa => {
                        $('#cboEmpresa').append($('<option>', { value: empresa.Codigo, text: empresa.Descripcion }));
                    });
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });

    }

    const listarFormatos = function (codFormato = null) {
        const codEmpresa = $("#cboEmpresa").val();
        if (!codEmpresa) return;

        const request = {
            CodEmpresa: codEmpresa
        };

        $.ajax({
            url: urlFormatos,
            type: "post",
            data: { request },
            success: function (response) {
                if (response === undefined) return;
                if (response.Ok) {
                    $('#cboFormato').empty().append('<option label="Seleccionar"></option>');
                    response.Formatos.map(formato => {
                        $('#cboFormato').append($('<option>', { value: formato.CodFormato, text: formato.Nombre }));
                    });

                    if (codFormato != null) {
                        $('#cboFormato').val(codFormato);
                    }

                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });

    }

    const listarLocales = function (codFormato = null, codLocal = null) {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: codFormato == null ? $("#cboFormato").val() : codFormato
        };

        $.ajax({
            url: urlLocales,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                    return;
                }

                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Locales.map(local => {
                    $('#cboLocal').append($('<option>', { value: local.CodLocal, text: local.CodLocal + ' - ' + local.NombreLocal }));
                });

                if (codLocal != null)
                    $('#cboLocal').val(codLocal);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const listarCajas = function (codFormato = null, codLocal = null, numPos = null) {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: codFormato == null ? $("#cboFormato").val() : codFormato,
            CodLocal: codLocal == null ? $("#cboLocal").val() : codLocal
        };
        console.log(request);
        $.ajax({
            url: urlCajas,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                    return;
                }
                $('#cboNumPos').empty().append('<option label="Seleccionar"></option>');
                response.Cajas.map(caja => {
                    $('#cboNumPos').append($('<option>', { value: caja.NroCaja, text: caja.NroCaja }));
                });

                if (numPos != null) $('#cboNumPos').val(numPos);

                console.log(numPos);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const listarCaract = function (tipo, valor = null) {

        if (!$("#cboEmpresa").val()) return;

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            Tipo: tipo,
        };



        $.ajax({
            url: urlListarCaracteristicas,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                $('#cboCaract' + tipo).empty().append('<option label="Seleccionar"></option>');
                response.CaracetristicasCaja.map(caract => {
                    $('#cboCaract' + tipo).append($('<option>', { value: caract.Id, text: caract.Descripcion }));
                });

                if (valor != null) $('#cboCaract' + tipo).val(valor);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const inicializarDatePicker = function () {
        $('.fc-datepicker').datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            closeText: 'Cerrar',
            prevText: '<Ant',
            nextText: 'Sig>',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'dd/mm/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: '',
            changeMonth: true,
            changeYear: true
        });
    }

    const guardar = function (caja) {

        $.ajax({
            url: urlInsertarInventario,
            type: "post",
            data: { request: caja },
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
                limpiar();
                desabilitarControles(false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validar = function (caja) {
        let validate = true;

        if (caja.CodEmpresa === '' || caja.CodFormato === '' || caja.CodLocal === '' || caja.NumPos === '') {
            validate = false;
            $("#formInventario").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const limpiar = function () {
        $("#cboEmpresa").val('').trigger('change');
        $("#cboFormato").val('').trigger('change');
        $("#cboLocal").val('').trigger('change');
        $("#cboNumPos").val('').trigger('change');
        $("#cboEstado").val('').trigger('change');
        $("#cboSo").val('').trigger('change');
        $("#cboCaract1").val('').trigger('change');
        $("#cboCaract2").val('').trigger('change');
        $("#cboCaract3").val('').trigger('change');
        $("#txtFechaLising").val('');
        $("#txtVersion").val('');
        $("#txtFechaAsignacion").val('');
        $("#txtRanking").val('');
        $("#txtUbicación").val('');
        $("#txtSede").val('');
        $("#txtCaja").val('');
        $("#txtFechaApertura").val('');
        $("#txtModeloCpu").val('');
        $("#txtSerieCpu").val('');
        $("#txtModeloImpresora").val('');
        $("#txtSerieImpresora").val('');
        $("#txtModeloDynakey").val('');
        $("#txtSerieDynakey").val('');
        $("#txtModeloScanner").val('');
        $("#txtSerieScanner").val('');
        $("#txtModeloGaveta").val('');
        $("#txtSerieGaveta").val('');
        $("#txtModeloMonitor").val('');
        $("#txtSerieMonitor").val('');
        $("#txtUsuarioCreacion").val(usuarioLogueado);
        $("#txtFechaCreacion").val('');
        $("#txtUsuarioModificacion").val('');
        $("#txtFechaModificacion").val('');
        obtenerFechaSistema();
    }

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

                $("#txtFechaCreacion").val(response.Mensaje);

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const inicializarDataTableInventario = function () {

        $.ajax({
            url: urlListarInventario,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    if (x === "COD_EMPRESA" || x === "COD_FORMATO" || x === "COD_LOCAL")
                        columnas.push({
                            visible: false,
                            title: x,
                            data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u")
                        });
                    else
                        columnas.push({
                            title: x,
                            data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u")
                        });
                });

                dataTableInventario = $('#tableInventario').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    data: response.Cajas,
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

    const recargarDataTableInventario = function () {

        $.ajax({
            url: urlListarInventario,
            type: "post",
            data: { request: {} },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });

                    dataTableInventario.clear();
                    dataTableInventario.draw();
                    return;

                }
                dataTableInventario.clear();
                dataTableInventario.rows.add(response.Cajas);
                dataTableInventario.draw();
                dataTableInventario.columns.adjust().draw();

                setTimeout(() => {
                    dataTableInventario.columns.adjust().draw();
                }, 1000);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const obtenerInventario = function (codEmpresa, codFormato, codLocal, numPos) {

        const request = {
            CodEmpresa: codEmpresa,
            CodFormato: codFormato,
            Codlocal: codLocal,
            NumPos: numPos
        };

        $.ajax({
            url: urlObtenerInventario,
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
                $("#modalInventario").modal('hide');

                setearInventario(response);
                desabilitarControles(true);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const desabilitarControles = function (disable) {
        $("#cboEmpresa").attr("disabled", disable);
        $("#cboFormato").attr("disabled", disable);
        $("#cboLocal").attr("disabled", disable);
        $("#cboNumPos").attr("disabled", disable);
    }

    const setearInventario = function (inventario) {
        $("#cboEmpresa").val(inventario.CodEmpresa).trigger('change', [inventario.CodFormato, inventario.Caract1, inventario.Caract2, inventario.Caract3]);
        $("#cboFormato").trigger('change', [inventario.CodFormato, inventario.CodLocal]);
        $("#cboLocal").trigger('change', [inventario.CodFormato, inventario.CodLocal, inventario.NumPos]);
        $("#txtFechaLising").val(inventario.FechaLising);
        $("#cboEstado").val(inventario.Estado).trigger('change');
        $("#cboSo").val(inventario.So).trigger('change');
        $("#txtVersion").val(inventario.VesionSo);
        $("#txtFechaAsignacion").val(inventario.FechaAsignacion);
        $("#txtRanking").val(inventario.Ranking);
        $("#txtUbicación").val(inventario.Ubicacion);
        $("#txtSede").val(inventario.Sede);
        $("#txtCaja").val(inventario.Caja);
        $("#txtFechaApertura").val(inventario.FechaApertura);
        $("#txtModeloCpu").val(inventario.ModeloCpu);
        $("#txtSerieCpu").val(inventario.Serie);
        $("#txtModeloImpresora").val(inventario.ModeloPrint);
        $("#txtSerieImpresora").val(inventario.SeriePrint);
        $("#txtModeloDynakey").val(inventario.ModeloDinakey);
        $("#txtSerieDynakey").val(inventario.SerieDinakey);
        $("#txtModeloScanner").val(inventario.ModeloScanner);
        $("#txtSerieScanner").val(inventario.SerieScanner);
        $("#txtModeloGaveta").val(inventario.ModeloGaveta);
        $("#txtSerieGaveta").val(inventario.SerieGaveta);
        $("#txtModeloMonitor").val(inventario.ModeloMonitor);
        $("#txtSerieMonitor").val(inventario.SerieMonitor);
        //$("#cboCaract1").val(inventario.Caract1).trigger('change');
        //$("#cboCaract2").val(inventario.Caract2).trigger('change');
        //$("#cboCaract3").val(inventario.Caract3).trigger('change');
        $("#txtUsuarioCreacion").val(inventario.UsuarioCreacion);
        $("#txtFechaCreacion").val(inventario.FechaCreacion);
        $("#txtUsuarioModificacion").val(inventario.UsuarioModificacion);
        $("#txtFechaModificacion").val(inventario.FechaModificacion);

    }

    const descargarMaestro = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            CodLocal: $("#cboLocal").val()
        };

        $.ajax({
            url: urlDescargarMaestro,
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

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                inicializarDatePicker();
                listarEmpresas();
                inicializarDataTableInventario();
            });
        }
    }
}(jQuery);