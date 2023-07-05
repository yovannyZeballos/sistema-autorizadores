const urlInsertarInventario = baseUrl + 'Locales/InventarioServidor/InsertarInventario';
const urlInsertarInventarioVirtual = baseUrl + 'Locales/InventarioServidor/InsertarInventarioVirtual';
const urlListarVirtuales = baseUrl + 'Locales/InventarioServidor/ListarVirtuales';
const urlListarServidores = baseUrl + 'Locales/InventarioServidor/ListarServidores';
const urlObtenerServidor = baseUrl + 'Locales/InventarioServidor/ObtenerServidor';
const urlNuevaVirtual = baseUrl + 'Locales/InventarioServidor/NuevaVirtual';
const urlEliminarVirtual = baseUrl + 'Locales/InventarioServidor/EliminarVirtual';
const urlListarInventario = baseUrl + 'Locales/InventarioCaja/ListarInventario';
const urlObtenerInventario = baseUrl + 'Locales/InventarioCaja/ObtenerInventario';
const urlListarTipo = baseUrl + 'Locales/InventarioServidor/ListarInventarioTipo';
const urlEmpresas = baseUrl + 'Locales/AdministrarLocal/ListarEmpresas';
const urlFormatos = baseUrl + 'Locales/AdministrarLocal/ListarFormatos';
const urlLocales = baseUrl + 'Locales/AdministrarLocal/ListarLocales';
const urlCajas = baseUrl + 'Locales/AdministrarLocal/ListarCajas';
var urlFechaSistema = baseUrl + 'Locales/AdministrarLocal/ObtenerFechaSistema';
var urlDescargarMaestro = baseUrl + 'Locales/InventarioServidor/DescargarMaestro';
var urlImportarInventario = baseUrl + 'Locales/InventarioServidor/ImportarInventario';
var urlDescargarPlantilla = baseUrl + 'Locales/InventarioServidor/DescargarPlantillas';
var dataTableVirtuales = null;
var dataTableServidores = null;

const InventarioServidor = function () {


    const eventos = function () {

        $("#cboEmpresa").on("change", function (event, codFormato, caract1, caract2, caract3) {
            listarFormatos(codFormato);
        });

        $("#cboFormato").on("change", function (event, codFormato, codLocal) {
            listarLocales(codFormato, codLocal);
        });

        $("#btnGuardar").on("click", function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro guardar la información ingresada?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {

                    const servidor = {
                        CodEmpresa: $("#cboEmpresa").val(),
                        CodFormato: $("#cboFormato").val(),
                        CodLocal: $("#cboLocal").val(),
                        NumServer: $("#txtNumero").val(),
                        TipoServer: $("#cboTipo").val(),
                        CodMarca: $("#cboMarca").val(),
                        CodModelo: $("#cboModelo").val(),
                        Hostname: $("#txtHostname").val(),
                        Serie: $("#txtSerie").val(),
                        Ip: $("#txtIp").val(),
                        Ram: $("#txtRam").val(),
                        Hdd: $("#txtHdd").val(),
                        CodSo: $("#cboSo").val(),
                        Replica: $("#cboReplica").val(),
                        IpRemota: $("#txtIpRemota").val(),
                        Antiguedad: $("#txtAntiguedad").val(),
                        Observaciones: $("#txtObservaciones").val(),
                        Antivirus: $("#txtAntivirus").val(),
                    };

                    if (validar(servidor))
                        guardar(servidor);
                }
            });


        });

        $("#btnNuevo").on("click", function () {
            limpiar();
        });

        $("#btnBuscar").on("click", function () {
            $("#modalInventario").modal('show');
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

        $("#btnNuevaVirtual").on("click", function () {
            abrirModalNuevaVirtual();
        });

        $("#btnGuardarVirtual").on("click", function () {

            var virtual = {
                Id: $("#txtId").val(),
                CodEmpresa: $("#cboEmpresa").val(),
                CodFormato: $("#cboFormato").val(),
                CodLocal: $("#cboLocal").val(),
                NumServer: $("#txtNumero").val(),
                Tipo: $("#cboTipoVirtual").val(),
                Ram: $("#txtRamVirtual").val(),
                Cpu: $("#txtCpuVirtual").val(),
                Hdd: $("#txtHddVirtual").val(),
                So: $("#cboSoVirtual").val(),
            };

            if (validarVirtual(virtual))
                guardarVirtual(virtual);

        });

        $("#btnBuscar").on("click", function () {
            if (validarBuscar()) {
                $("#modalServidores").modal('show');
                recargarDataTableServidores();
            }

        });

        $("#btnAceptarServidor").on("click", function () {
            const registrosSeleccionados = dataTableServidores.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }

            obtenerServidor(registrosSeleccionados[0].NumServer);

        });

        $('#tableServidores tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableServidores.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableVirtuales tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');

        });

        $("#btnActualizarVirtual").on("click", function () {

            const registrosSeleccionados = dataTableVirtuales.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length, true)) {
                return;
            }

            abrirModalActualizarVirtual(registrosSeleccionados[0]);
        });

        $("#btnEliminarVirtual").on("click", function () {
            const registrosSeleccionados = dataTableVirtuales.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar las virtuales seleccionadas?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    const virtuales = registrosSeleccionados.map(x => x.ID).join();
                    eliminarVirtuales(virtuales);
                }
            });
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

    const listarTipo = function (control, codigo, valor = null) {

        const request = {
            Codigo: codigo,
        };

        $.ajax({
            url: urlListarTipo,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                control.empty().append('<option label="Seleccionar"></option>');
                response.Tipos.map(tipo => {
                    control.append($('<option>', { value: tipo.Codigo, text: tipo.Nombre }));
                });

                if (valor != null)
                    control.val(valor);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardar = function (servidor) {

        $.ajax({
            url: urlInsertarInventario,
            type: "post",
            data: { request: servidor },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                desabilitarControles(true);
                desabilitarBotonosVirtuales(false)
                $("#btnGuardar").prop("disabled", true);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validar = function (servidor) {
        let validate = true;

        if (servidor.CodEmpresa === '' || servidor.CodFormato === '' || servidor.CodLocal === '' || servidor.NumServer === '') {
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
        $("#txtNumero").val('');
        $("#cboTipo").val('').trigger('change');
        $("#cboMarca").val('').trigger('change');
        $("#cboModelo").val('').trigger('change');
        $("#txtHostname").val('');
        $("#txtSerie").val('');
        $("#txtRam").val('');
        $("#txtHdd").val('');
        $("#cboSo").val('').trigger('change');
        $("#txtAntivirus").val('');
        $("#txtIp").val('');
        $("#txtIpRemota").val('');
        $("#cboReplica").val('').trigger('change');
        $("#txtAntiguedad").val('');
        $("#txtObservaciones").val('');
        $("#txtUsuarioCreacion").val(usuarioLogueado);
        $("#txtFechaCreacion").val('');
        $("#txtUsuarioModificacion").val('');
        $("#txtFechaModificacion").val('');
        obtenerFechaSistema();
        desabilitarBotonosVirtuales(true);
        desabilitarControles(false);
        dataTableVirtuales.clear();
        dataTableVirtuales.draw();
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

    const inicializarDataTableVirtuales = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            CodLocal: $("#cboLocal").val(),
            NumeroServidor: $("#txtNumero").val()
        }

        $.ajax({
            url: urlListarVirtuales,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                var columnas = [];

                response.Columnas.forEach((x) => {
                    if (x === "VIRTUAL_TIPO" || x === "VIRTUAL_SO" || x === "ID")
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

                dataTableVirtuales = $('#tableVirtuales').DataTable({
                    searching: false,
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    data: response.Virtuales,
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

    const recargarDataTableVirtuales = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            CodLocal: $("#cboLocal").val(),
            NumeroServidor: $("#txtNumero").val()
        }


        $.ajax({
            url: urlListarVirtuales,
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
                    swal({ text: response.Mensaje, icon: "warning" });

                    dataTableVirtuales.clear();
                    dataTableVirtuales.draw();
                    return;

                }
                dataTableVirtuales.clear();
                dataTableVirtuales.rows.add(response.Virtuales);
                dataTableVirtuales.draw();
                dataTableVirtuales.columns.adjust().draw();

                setTimeout(() => {
                    dataTableVirtuales.columns.adjust().draw();
                }, 500);
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
            success: function (response) {

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
        $("#txtNumero").attr("disabled", disable);
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
            success: function (response) {

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

    const desabilitarBotonosVirtuales = function (disable) {
        $("#btn-virtuales :input").attr("disabled", disable);
    }

    const abrirModalNuevaVirtual = function () {

        $("#tituloModalVirtual").html("Nueva Virtual")

        const model = {};

        $.ajax({
            url: urlNuevaVirtual,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                $("#modalVirtual").find(".modal-body").html(response);
                $("#modalVirtual").modal('show');

                $('.select2-show-modal').select2({
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $('#modalVirtual .modal-content')
                });

                listarTipo($("#cboSoVirtual"), "VSO");
                listarTipo($("#cboTipoVirtual"), "VST");

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validarVirtual = function (virtual) {
        let validate = true;

        if (virtual.CodEmpresa === '' || virtual.CodLocal === '' || virtual.CodFormato === '' || virtual.NumServer === '' ||
            virtual.Tipo === '' || virtual.Ram === '' || virtual.Cpu === '' || virtual.hdd === '' || virtual.So === '') {
            validate = false;
            $("#formCaja").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const guardarVirtual = function (virtual) {

        $.ajax({
            url: urlInsertarInventarioVirtual,
            type: "post",
            data: { request: virtual },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                recargarDataTableVirtuales();
                $("#modalVirtual").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validarBuscar = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboFormato").val() === '' || $("#cboLocal").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, el formato y el local', icon: "warning", });
        }

        return validate;
    }

    const inicializarDataTableServidores = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            CodLocal: $("#cboLocal").val()
        }

        $.ajax({
            url: urlListarServidores,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

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

                dataTableServidores = $('#tableServidores').DataTable({
                    searching: false,
                    scrollY: '250px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    data: response.Servidores,
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

    const recargarDataTableServidores = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            CodLocal: $("#cboLocal").val()
        }


        $.ajax({
            url: urlListarServidores,
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
                    swal({ text: response.Mensaje, icon: "warning" });

                    dataTableServidores.clear();
                    dataTableServidores.draw();
                    return;

                }
                dataTableServidores.clear();
                dataTableServidores.rows.add(response.Servidores);
                dataTableServidores.draw();
                dataTableServidores.columns.adjust().draw();

                setTimeout(() => {
                    dataTableServidores.columns.adjust().draw();
                }, 500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const obtenerServidor = function (numeroServer) {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val(),
            Codlocal: $("#cboLocal").val(),
            NumServer: numeroServer
        };

        $.ajax({
            url: urlObtenerServidor,
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
                $("#modalServidores").modal('hide');

                setearServidor(response);
                desabilitarBotonosVirtuales(false);
                desabilitarControles(true)
                $("#btnGuardar").prop("disabled", false);
                await recargarDataTableVirtuales();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const setearServidor = function (servidor) {
        $("#txtNumero").val(servidor.NumServer);
        $("#cboTipo").val(servidor.TipoServer).trigger('change');
        $("#cboMarca").val(servidor.CodMarca).trigger('change');
        $("#cboModelo").val(servidor.CodModelo).trigger('change');
        $("#txtHostname").val(servidor.Hostname);
        $("#txtSerie").val(servidor.Serie);
        $("#txtRam").val(servidor.Ram);
        $("#txtHdd").val(servidor.Hdd);
        $("#cboSo").val(servidor.CodSo).trigger('change');
        $("#txtAntivirus").val(servidor.Antivirus);
        $("#txtIp").val(servidor.Ip);
        $("#txtIpRemota").val(servidor.IpRemota);
        $("#cboReplica").val(servidor.Replica).trigger('change');
        $("#txtAntiguedad").val(servidor.Antiguedad);
        $("#txtObservaciones").val(servidor.Observaciones);
        $("#txtUsuarioCreacion").val(servidor.UsuarioCreacion);
        $("#txtFechaCreacion").val(servidor.FechaCreacion);
        $("#txtUsuarioModificacion").val(servidor.UsuarioModificacion);
        $("#txtFechaModificacion").val(servidor.FechaModificacion);
    }

    const abrirModalActualizarVirtual = function (registro) {

        $("#tituloModalVirtual").html("Actualizar virtual")

        const model = {
            Id: registro.ID,
            Tipo: registro.VIRTUAL_TIPO,
            Ram: registro.RAM,
            Cpu: registro.CPU,
            Hdd: registro.HDD,
            So: registro.VIRTUAL_SO
        };

        $.ajax({
            url: urlNuevaVirtual,
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
                $("#modalVirtual").find(".modal-body").html(response);
                $("#modalVirtual").modal('show');

                $('.select2-show-modal').select2({
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $('#modalVirtual .modal-content')
                });

                listarTipo($("#cboSoVirtual"), "VSO", model.So);
                listarTipo($("#cboTipoVirtual"), "VST", model.Tipo);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const eliminarVirtuales = function (virtuales) {

        const request = {
            Ids: virtuales,
        };

        $.ajax({
            url: urlEliminarVirtual,
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
                await recargarDataTableVirtuales();
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
                            $('#tbodyErrores').html(html);
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
            checkSession(function () {
                eventos();
                listarTipo($("#cboTipo"), "FST");
                listarTipo($("#cboMarca"), "FSB");
                listarTipo($("#cboModelo"), "FSM");
                listarTipo($("#cboSo"), "FSO");
                listarEmpresas();
                inicializarDataTableVirtuales();
                inicializarDataTableServidores();
            });
        }
    }
}(jQuery);