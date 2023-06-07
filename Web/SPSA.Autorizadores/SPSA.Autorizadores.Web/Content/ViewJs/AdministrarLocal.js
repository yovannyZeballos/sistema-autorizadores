var urlCrearLocal = baseUrl + 'Locales/AdministrarLocal/CrearLocal';
var urlEmpresas = baseUrl + 'Locales/AdministrarLocal/ListarEmpresas';
var urlFormatos = baseUrl + 'Locales/AdministrarLocal/ListarFormatos';
var urlCajas = baseUrl + 'Locales/AdministrarLocal/ListarCajas';
var urlLocales = baseUrl + 'Locales/AdministrarLocal/ListarLocales';
var urlNuevaCaja = baseUrl + 'Locales/AdministrarLocal/NuevaCaja';
var urlCrearCaja = baseUrl + 'Locales/AdministrarLocal/CrearCaja';
var urlObtenerLocal = baseUrl + 'Locales/AdministrarLocal/ObtenerLocal';
var urlEliminarCaja = baseUrl + 'Locales/AdministrarLocal/EliminarCajas';
var urlImportarCajas = baseUrl + 'Locales/AdministrarLocal/ImportarCajas';
var urlFechaSistema = baseUrl + 'Locales/AdministrarLocal/ObtenerFechaSistema';
var urlDescargarMaestro = baseUrl + 'Locales/AdministrarLocal/DescargarMaestro';
var urlImportarInventario = baseUrl + 'Locales/AdministrarLocal/ImportarInventario';
var urlDescargarPlantilla = baseUrl + 'Locales/AdministrarLocal/DescargarPlantillas';
var dataTableCajas = null;
var dataTableLocales = null;

var AdministrarLocal = function () {

    const eventos = function () {

        $("#btnGuardarLocal").on("click", function () {

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
                        CodLocal: $("#txtCodigo").val(),
                        CodFormato: $("#cboFormato").val(),
                        NomLocal: $("#txtNombreLocal").val(),
                        Ip: $("#txtIpServidor").val(),
                        SO: $("#cboSO").val(),
                        Estado: $("#cboEstado").val(),
                        TipoLocal: $("#cboTipo").val(),
                        IndFactura: $("#cboFactura").val(),
                        CodigoSunat: $("#txtCodigoSunat").val(),
                        Usuario: $("#txtUsuarioActualiza").val(),
                        Fecha: $("#txtEl").val()
                    };

                    if (validarLocal(local))
                        await guardarLocal(local);
                }
            });


        });

        $("#btnNuevoLocal").on("click", function () {
            limpiar();
        });

        $("#cboEmpresa").on("change", async function () {
            await cargarComboFormato();
        });

        $("#btnNuevaCaja").on("click", function () {
            abrirModalNuevaCaja();
        });

        $("#btnActualizarCaja").on("click", function () {

            const registrosSeleccionados = dataTableCajas.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length, true)) {
                return;
            }

            abrirModalActualizarCaja(registrosSeleccionados[0]);
        });

        $("#btnGuardarCaja").on("click", async function () {

            var caja = {
                CodEmpresa: $("#cboEmpresa").val(),
                CodLocal: $("#txtCodigo").val(),
                CodFormato: $("#cboFormato").val(),
                NumeroCaja: $("#txtNroCaja").val(),
                Ip: $("#txtIpCaja").val(),
                So: $("#cboSOCaja").val(),
                Estado: $("#cboEstadoCaja").val()
            };

            if (validarCaja(caja))
                await guardarCaja(caja);

        });

        $('#tableLocales tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableLocales.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableCajas tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');

        });

        $("#btnBuscarLocal").on("click", function () {
            if (validarBuscarLocal()) {
                $("#txtEmpresaLocal").val($("#cboEmpresa option:selected").text());
                $("#txtFormatoLocal").val($("#cboFormato option:selected").text());
                $("#modalLocales").modal('show');
                recargarDataTableLocales();
            }

        });

        $("#btnAceptarLocal").on("click", function () {
            const registrosSeleccionados = dataTableLocales.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }

            obtenerLocal(registrosSeleccionados[0].CodLocal);

        });

        $("#btnEliminarCaja").on("click", function () {
            const registrosSeleccionados = dataTableCajas.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar las cajas seleccionadas?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    const cajas = registrosSeleccionados.map(x => x.NroCaja).join();
                    eliminarCajas(cajas);
                }
            });
        });

        $("#btnImportarExcel").on("click", function () {
            $("#excelCajas").trigger("click");
        });

        $('#excelCajas').change(function (e) {
            importarExcelCajas();
        });

        $("#btnDescargarMaestro").on("click", function () {
            if (validarBuscarLocal()) {
                descargarMaestro();
            }

        });

        $("#btnInventarioCaja").on("click", function () {
            $("#excelInventario").trigger("click");
        });

        $('#excelInventario').change(function (e) {
            importarExcelInventario();
        });

        $("#btnDescargarPlantillas").on("click", function () {
            descargarPlantillas();
        });

    };

    const limpiar = function () {
        $("#cboEmpresa").val('').trigger('change');
        $("#cboFormato").val('').trigger('change');
        $("#cboEstado").val('A').trigger('change');
        $("#cboTipo").val('T').trigger('change');
        $("#cboSO").val('').trigger('change');
        $("#cboFactura").val('S').trigger('change');
        $("#txtCodigo").val('');
        $("#txtNombreLocal").val('');
        $("#txtIpServidor").val('');
        $("#txtCodigoSunat").val('0000');
        desabilitarBotonosLocal(false);
        desabilitarBotonosCaja(true);
        desabilitarControles(false);
        obtenerFechaSistema();
        dataTableCajas.clear();
        dataTableCajas.draw();
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
        return new Promise((resolve, reject) => {
            $.ajax({
                url: urlEmpresas,
                type: "post",
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });

    }

    const listarFormatos = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();

            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa
            };

            $.ajax({
                url: urlFormatos,
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
                response.Empresas.map(empresa => {
                    $('#cboEmpresa').append($('<option>', { value: empresa.Codigo, text: empresa.Descripcion }));
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

    const cargarComboFormato = async function () {

        try {
            const response = await listarFormatos();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboFormato').empty().append('<option label="Seleccionar"></option>');
                response.Formatos.map(formato => {
                    $('#cboFormato').append($('<option>', { value: formato.CodFormato, text: formato.Nombre }));
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

    const guardarLocal = function (local) {

        $.ajax({
            url: urlCrearLocal,
            type: "post",
            data: { request: local },
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
                desabilitarControles(true);
                desabilitarBotonosCaja(false);
                $("#btnGuardarLocal").prop("disabled", true);
                await recargarDataTableCajas();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const desabilitarControles = function (disable) {
        $("#cboEmpresa").prop("disabled", disable);
        $("#txtCodigo").prop("disabled", disable);
        $("#cboFormato").prop("disabled", disable);
        //$("#txtNombreLocal").prop("disabled", disable);
        //$("#txtIpServidor").prop("disabled", disable);
        //$("#cboSO").prop("disabled", disable);
        //$("#cboEstado").prop("disabled", disable);
        //$("#cboTipo").prop("disabled", disable);
        //$("#cboFactura").prop("disabled", disable);
        //$("#txtCodigoSunat").prop("disabled", disable);
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

                $("#txtEl").val(response.Mensaje);

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const desabilitarBotonosCaja = function (disable) {
        $("#btn-cajas :input").attr("disabled", disable);
    }

    const desabilitarBotonosLocal = function (disable) {
        $("#btn-local :input").attr("disabled", disable);

    }

    const validarLocal = function (local) {
        let validate = true;

        if (local.CodEmpresa === '' || local.CodLocal === '' || local.CodFormato === '' || local.NomLocal === '' ||
            local.Ip === '' || local.SO === '' || local.Estado === '' || local.TipoLocal === '' || local.IndFactura === '' || local.CodigoSunat === '') {
            validate = false;
            $("#formLocal").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarCaja = function (caja) {
        let validate = true;

        if (caja.CodEmpresa === '' || caja.CodLocal === '' || caja.CodFormato === '' || caja.NumeroCaja === '' ||
            caja.Ip === '' || caja.So === '') {
            validate = false;
            $("#formCaja").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const listarCajas = async function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            CodLocal: $("#txtCodigo").val()
        };

        const response = await $.ajax({
            url: urlCajas,
            type: "post",
            data: { request }
        });

        return response;
    }

    const visualizarDataTableCajas = async function () {

        try {
            const response = await listarCajas();
            var columnas = [];

            response.Columnas.forEach((x) => {

                if (x === "COD_EMPRESA" || x === "COD_FORMATO" || x === "COD_LOCAL" || x === 'TIP_ESTADO' ) {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
                        visible: false,
                    });
                } else {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
      
                    });
                }
            });

            dataTableCajas = $('#tableCajas').DataTable({
                language: {
                    searchPlaceholder: 'Buscar...',
                    sSearch: '',
                },
                autoWidth: false,
                scrollY: '180px',
                scrollX: true,
                scrollCollapse: true,
                paging: false,
                columns: columnas,
                data: response.Cajas,
                buttons: [
                    {
                        extend: 'excel',
                        text: 'Excel <i class="fa fa-cloud-download"></i>',
                        titleAttr: 'Descargar Excel',
                        className: 'btn-sm mb-1 ms-2',
                        exportOptions: {
                            modifier: { page: 'all' }
                        }
                    },
                ],
                columnDefs: [
                    { "width": "60px", "targets": [3,4,5] }
                ]
            });

            dataTableCajas.buttons().container().prependTo($('#tableCajas_filter'));

        } catch (e) {
            console.error(e);
            swal({ text: e.toString(), icon: "error" });
        }
    }

    const recargarDataTableCajas = async function () {

        showLoading();
        try {
            const response = await listarCajas();
            if (!response.Ok) {
                swal({
                    text: response.Mensaje,
                    icon: "warning"
                });

                dataTableCajas.clear();
                dataTableCajas.draw();
                return;

            }
            dataTableCajas.clear();
            dataTableCajas.rows.add(response.Cajas);
            dataTableCajas.draw();
        } catch (e) {
            console.error(e);
            swal({ text: e.toString(), icon: "error" });
        }
        closeLoading();
    }

    const abrirModalNuevaCaja = function () {

        $("#tituloModalCaja").html("Nueva Caja")

        const model = {};

        $.ajax({
            url: urlNuevaCaja,
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
                $('#txtIpCaja').mask('099.099.099.099');
                $('#txtNroCaja').mask('ZZZ', {
                    translation: {
                        'Z': {
                            pattern: /[1-9]/, optional: true
                        }
                    }

                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarCaja = function (caja) {

        $.ajax({
            url: urlCrearCaja,
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
                await recargarDataTableCajas();
                $("#modalCaja").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const abrirModalActualizarCaja = function (registro) {

        $("#tituloModalCaja").html("Actualizar Caja")

        const model = {
            NumeroCaja: registro.NroCaja,
            Ip: registro.IP,
            So: registro.SO,
            Estado: registro.TIP_ESTADO
        };

        $.ajax({
            url: urlNuevaCaja,
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
                $('#txtIpCaja').mask('099.099.099.099');
                $('#txtNroCaja').mask('ZZZ', {
                    translation: {
                        'Z': {
                            pattern: /[1-9]/, optional: true
                        }
                    }

                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const visualizarDataTableLocales = function () {

        $.ajax({
            url: urlLocales,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u")
                    });
                });

                dataTableLocales = $('#tableLocales').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
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
            CodFormato: $("#cboFormato").val()
        };

        $.ajax({
            url: urlLocales,
            type: "post",
            data: { request },
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

                    dataTableLocales.clear();
                    dataTableLocales.draw();
                    return;

                }

                dataTableLocales.clear();
                dataTableLocales.rows.add(response.Locales);
                dataTableLocales.draw();
                dataTableLocales.columns.adjust().draw();

                setTimeout(() => {
                    dataTableLocales.columns.adjust().draw();
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

    const validarBuscarLocal = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboFormato").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa y el formato', icon: "warning", });
        }

        return validate;
    }

    const obtenerLocal = function (codLocal) {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            Codlocal: codLocal
        };

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

                setearLocal(response);
                desabilitarBotonosCaja(false);
                desabilitarControles(true)
                $("#btnGuardarLocal").prop("disabled", false);
                await recargarDataTableCajas();
                obtenerFechaSistema();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const setearLocal = function (local) {
        $("#txtCodigo").val(local.CodLocal);
        $("#txtNombreLocal").val(local.NomLocal);
        $("#txtIpServidor").val(local.Ip);
        $("#cboSO").val(local.SO).trigger('change');
        $("#cboEstado").val(local.Estado).trigger('change');
        $("#cboTipo").val(local.TipoLocal).trigger('change');
        $("#cboFactura").val(local.IndFactura).trigger('change');
        $("#txtCodigoSunat").val(local.CodigoSunat);
    }

    const eliminarCajas = function (cajas) {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            Codlocal: $("#txtCodigo").val(),
            Cajas: cajas
        };

        $.ajax({
            url: urlEliminarCaja,
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

                swal({ text: response.Mensaje, icon: "success", });
                await recargarDataTableCajas();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarExcelCajas = function () {

        var formData = new FormData();
        var uploadFiles = $('#excelCajas').prop('files');
        formData.append("excelCajas", uploadFiles[0]);
        formData.append("codEmpresa", $('#cboEmpresa').val());
        formData.append("codFormato", $('#cboFormato').val());
        formData.append("codLocal", $('#txtCodigo').val());


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
                $("#excelCajas").val(null);
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

                 recargarDataTableCajas();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });

        //$.ajax({
        //    type: "POST",
        //    url: 'Controller/Upload',
        //    data: formData,
        //    dataType: 'json',
        //    contentType: false,
        //    processData: false,
        //    complete: this.onComplete.bind(this)
        //});
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

    const inputMask = function () {
        $('#txtIpServidor').mask('099.099.099.099');
       
        $('#txtCodigo').mask('ZZZZZZZZZZ', {
            translation: {
                'Z': {
                    pattern: /[0-9]/, optional: true
                }
            }

        });
        $('#txtCodigoSunat').mask('0999');

    }

    const descargarMaestro = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val()
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


    const descargarPlantillas = function () {

        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
            dataType: "json",
            success:  function (response) {

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
                inputMask();
                showLoading();
                await cargarComboEmpresa();
                await cargarComboFormato();
                await visualizarDataTableCajas();
                closeLoading();

                visualizarDataTableLocales();

            });
        }
    }

}(jQuery);
