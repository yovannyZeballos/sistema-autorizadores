var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadasPorEmpresa';

var urlListarColaboradoresExt = baseUrl + 'Maestros/MaeColaboradorExt/ListarPaginado';

var urlModalNuevoColabExt = baseUrl + 'Maestros/MaeColaboradorExt/NuevoForm';
var urlModalModificarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/ModificarForm';

var urlObtenerColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Obtener';
var urlCrearColabExt = baseUrl + 'Maestros/MaeColaboradorExt/CrearColaborador';
var urlModificarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/ModificarColaborador';
var urlImportarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Importar';


var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var codLocalAlternoAnterior = "";
var dataTableColaboradoresExt = null;

var AdministrarColaboradorExt = function () {
    const eventos = function () {

        $(document).on('input', '.uppercase', function () {
            $(this).val($(this).val().toUpperCase());
        });

        $("#cboEmpresaBuscar").on("change", async function () {
            await cargarComboLocales('#cboLocalBuscar');
        });

        $("#cboLocalBuscar").on("change", async function () {
            let valueLocalBuscar = $("#cboLocalBuscar").val();
            if (valueLocalBuscar == null || valueLocalBuscar.trim() === "") {
                $('#cboEmpresaBuscar').val('');
                $('#cboLocalBuscar').empty().append('<option label="Todos"></option>');
            } else {
                console.log("El valor es:", valueLocalBuscar);
            }
        });

        $(document).on('change', '#cboEmpresa', async function () {
            await cargarComboLocales('#cboLocal');
        });

        //$(document).on('change', '#cboLocal', async function () {
        //    console.log("Cambió el valor de cboLocal");
        //});

        $("#btnBuscarColabExt").on("click", function (e) {
            var table = $('#tableColaboradoresExt').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $('#tableColaboradoresExt tbody').on('click', 'tr', function () {
            $('#tableColaboradoresExt tbody tr').removeClass('selected');
            $(this).addClass('selected');
        });

        $("#btnModalNuevoColabExt").on("click", function () {
            codLocalAlternoAnterior = "";
            abrirModalNuevoColabExt();
        });

        $("#btnModalModificarColabExt").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableColaboradoresExt tbody tr.selected");

            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }
            var table = $('#tableColaboradoresExt').DataTable();
            var rowData = table.row(filasSeleccionada[0]).data();

            var codLocalAlterno = rowData.CodLocalAlterno;
            var codigoOfisis = rowData.CodigoOfisis;

            codLocalAlternoAnterior = codLocalAlterno;
            abrirModalModificarColabExt(codLocalAlterno, codigoOfisis);
        });

        $("#btnGrabarNuevo").on("click", async function () {
            var model = {
                IndPersonal: $('input[name="IndicadorPersonal"]:checked').val(),
                TipoUsuario: $('input[name="TipoColaborador"]:checked').val(),
                CodEmpresa: $("#cboEmpresa").val(),
                CodLocalAlterno: $("#cboLocal").val(),
                CodigoOfisis: "0",
                TiSitu: $("#cboTiSitu").val(),
                ApelPaterno: $("#txtApPaterno").val(),
                ApelMaterno: $("#txtApMaterno").val(),
                NombreTrabajador: $("#txtNomTrabajador").val(),
                TipoDocIdent: $("#cboTipoDoc").val(),
                NumDocIndent: $("#txtNroDoc").val(),
                PuestoTrabajo: $("#txtPuesto").val(),
                FechaIngresoEmpresa: $("#txtFecIngreso").val(),
                FechaCeseTrabajador: $("#txtFecCese").val(),
                UsuCreacion: $("#txtUsuario").val()
            };

            if (validarFormularioColabExt(model))
                await guardarColabExt(model, urlCrearColabExt);
        });

        $("#btnGrabarModifica").on("click", async function () {
            var model = {
                IndPersonal: $('input[name="IndicadorPersonal"]:checked').val(),
                TipoUsuario: $('input[name="TipoColaborador"]:checked').val(),
                CodEmpresa: $("#cboEmpresa").val(),
                CodLocalAlterno: $("#cboLocal").val(),
                CodigoOfisis: $("#txtCodOfisis").val(),
                TiSitu: $("#cboTiSitu").val(),
                ApelPaterno: $("#txtApPaterno").val(),
                ApelMaterno: $("#txtApMaterno").val(),
                NombreTrabajador: $("#txtNomTrabajador").val(),
                TipoDocIdent: $("#cboTipoDoc").val(),
                NumDocIndent: $("#txtNroDoc").val(),
                PuestoTrabajo: $("#txtPuesto").val(),
                FechaIngresoEmpresa: $("#txtFecIngreso").val(),
                FechaCeseTrabajador: $("#txtFecCese").val(),
                UsuModifica: $("#txtUsuario").val(),
                CodLocalAlternoAnterior: codLocalAlternoAnterior
            };

            if (validarFormularioColabExt(model))
                await guardarColabExt(model, urlModificarColabExt);
        });

        $("#btnImportarColabExt").on("click", function () {
            $("#excelImportar").trigger("click");
        });

        $("#excelImportar").on("change", function () {
            importarExcelMaeColabExt();
        });

        $("#btnDescargarPlantilla").click(function () {
            descargarPlantilla("Plantilla_ColaboradorExt");
        });
    };

    const listarEmpresasAsociadas = function () {
        return new Promise((resolve, reject) => {

            const codUsuario = $("#txtUsuario").val();

            const request = {
                CodUsuario: codUsuario,
                Busqueda: ''
            };

            $.ajax({
                url: urlListarEmpresasAsociadas,
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

    const listarLocalesAsociados = function (selectorEmpresa) {
        return new Promise((resolve, reject) => {
            //const codEmpresa = $("#cboEmpresa").val();
            const codEmpresa = $(selectorEmpresa).val();
            const codUsuario = $(document).find("#txtUsuario").val();

            if (!codEmpresa) return resolve();
            if (!codUsuario) return resolve();

            const query = {
                CodUsuario: codUsuario,
                CodEmpresa: codEmpresa
            };

            $.ajax({
                url: urlListarLocalesAsociados,
                type: "post",
                data: { query },
                success: function (response) {
                    resolve(response);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText);
                }
            });
        });
    };

    const cargarComboEmpresa = async function () {
        try {
            const response = await listarEmpresasAsociadas();

            if (response.Ok) {
                $('#cboEmpresaBuscar').empty().append('<option label="Todos"></option>');
                $('#cboLocalBuscar').empty().append('<option label="Todos"></option>');
                response.Data.map(empresa => {
                    $('#cboEmpresaBuscar').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
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

    const cargarComboLocales = async function (selectorLocal, selectedLocal = null) {
        try {
            let response = null;

            if (selectorLocal === '#cboLocalBuscar') {
                response = await listarLocalesAsociados('#cboEmpresaBuscar');
            } else {
                response = await listarLocalesAsociados('#cboEmpresa');
            }

            if (!response) return;

            if (response.Ok) {

                if (selectorLocal === '#cboLocalBuscar') {
                    $(selectorLocal).empty().append('<option label="Todos"></option>');
                } else {
                    $(selectorLocal).empty().append('<option label="Seleccionar"></option>');
                }

                //$(selectorLocal).empty().append('<option label="Seleccionar"></option>');

                response.Data.map(local => {
                    $(selectorLocal).append($('<option>', { value: local.CodLocalAlterno, text: local.NomLocal }));
                });

                // Si se pasó un valor seleccionado, lo asignamos
                if (selectedLocal) {
                    $(selectorLocal).val(selectedLocal);
                }
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    };

    const visualizarDataTableColaboradores = function () {
        $('#tableColaboradoresExt').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                // Recoger los filtros de la página
                var filtros = {
                    CodLocalAlterno: $("#cboLocalBuscar").val(),
                    CodigoOfisis: $("#txtCodOfisisBuscar").val(),
                    NroDocIdent: $("#txtNroDocBuscar").val()
                };

                // Combinar los parámetros de paginación con los filtros
                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);


                $.ajax({
                    url: urlListarColaboradoresExt,
                    type: "GET",
                    //data: { PageNumber: pageNumber, PageSize: pageSize },
                    data: params,
                    dataType: "json",
                    success: function (response) {
                        if (response.Ok) {
                            var pagedData = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pagedData.TotalRecords,
                                recordsFiltered: pagedData.TotalRecords,
                                data: pagedData.Items
                            });
                        } else {
                            callback({
                                draw: data.draw,
                                recordsTotal: 0,
                                recordsFiltered: 0,
                                data: []
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        swal({
                            text: jqXHR.responseText,
                            icon: "error",
                        });
                        callback({
                            draw: data.draw,
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            data: []
                        });
                    }
                });
            },
            columnDefs: [
                { targets: 0, visible: false }  // Oculta la primera columna "COD. LOCAL"
            ],
            columns: [
                { data: "CodLocalAlterno", title: "Código Local" },
                { data: "NomLocal", title: "Local" },
                { data: "CodigoOfisis", title: "Código" },
                { data: "ApelPaterno", title: "Ape. Paterno" },
                { data: "ApelMaterno", title: "Ape. Materno" },
                { data: "NombreTrabajador", title: "Nombre" },
                { data: "TipoDocIdent", title: "Tipo Doc" },
                { data: "NumDocIndent", title: "Nro Doc" },
                { data: "PuestoTrabajo", title: "Puesto" },
                {
                    data: "FechaIngresoEmpresa",
                    title: "Fec. Ingreso",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                {
                    data: "FechaCeseTrabajador",
                    title: "Fec. Cese",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { data: "TiSitu", title: "Estado" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)"
            },
            scrollY: '450px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });
    };

    const abrirModalNuevoColabExt = async function () {
        $("#tituloModalColabExt").html("Nuevo Colaborador Externo");
        $("#btnGrabarModifica").hide();
        $("#btnGrabarNuevo").show();

        const model = {};
        model.UsuAsociado = $("#txtUsuario").val();
        model.IndPersonal = "S";
        model.CodigoOfisis = "9000000000";

        await cargarFormNuevoColabExt(model);
    }

    const abrirModalModificarColabExt = async function (codLocalAlterno, codigoOfisis) {
        $("#tituloModalColabExt").html("Modificar Colaborador Externo");
        $("#btnGrabarModifica").show();
        $("#btnGrabarNuevo").hide();

        const response = await obtenerColabExt(codLocalAlterno, codigoOfisis);
        const model = response.Data;

        codLocalAlternoAnterior = model.CodLocalAlterno;

        model.UsuAsociado = $("#txtUsuario").val();

        model.FecCreacion = convertToISODate(model.FecCreacion);
        model.FechaIngresoEmpresa = convertToISODate(model.FechaIngresoEmpresa);
        model.FechaCeseTrabajador = convertToISODate(model.FechaCeseTrabajador);

        await cargarFormModificarColabExt(model);
    }

    const cargarFormNuevoColabExt = async function (model) {
        $.ajax({
            url: urlModalNuevoColabExt,
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
                $("#modalColabExt").find(".modal-body").html(response);
                $("#modalColabExt").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormModificarColabExt = async function (model) {
        $.ajax({
            url: urlModalModificarColabExt,
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
                $("#modalColabExt").find(".modal-body").html(response);

                await cargarComboLocales('#cboLocal', model.CodLocalAlterno);

                $("#modalColabExt").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const obtenerColabExt = function (codLocalAlterno, codigoOfisis) {
        return new Promise((resolve, reject) => {
            if (!codLocalAlterno) return resolve();
            if (!codigoOfisis) return resolve();

            const request = {
                CodLocalAlterno: codLocalAlterno,
                CodigoOfisis: codigoOfisis
            };

            $.ajax({
                url: urlObtenerColabExt,
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

    const guardarColabExt = function (model, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: model },
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
                // Actualiza (refresca) el DataTable para mostrar los datos actualizados
                $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);

                swal({ text: response.Mensaje, icon: "success", });

                $("#modalColabExt").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarExcelMaeColabExt = function () {
        var formData = new FormData();
        var uploadFiles = $('#excelImportar').prop('files');
        formData.append("excelImportar", uploadFiles[0]);

        $.ajax({
            url: urlImportarColabExt,
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
                $("#excelImportar").val(null);
            },
            success: function (response) {

                if (!response.Ok) {

                    // Actualiza (refresca) el DataTable para mostrar los datos actualizados
                    $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);

                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErrores').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });

                    return;
                }

                // Actualiza (refresca) el DataTable para mostrar los datos actualizados
                $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);

                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const descargarPlantilla = function (nombreCarpeta) {
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

    const validarFormularioColabExt = function (model) {
        let validate = true;

        if (
            model.CodLocalAlterno === '' ||
            model.CodigoOfisis === '' ||
            model.ApelPaterno === '' ||
            model.ApelMaterno === '' ||
            model.NombreTrabajador === '' ||
            model.TipoDocIdent === '' ||
            model.NumDocIndent === '' ||
            model.TiSitu === '' ||
            model.PuestoTrabajo === '' ||
            model.FechaIngresoEmpresa === ''
        ) {
            validate = false;
            $("#formColabExt").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning" });
        }

        if (model.TipoDocIdent === "DNI") {
            // Para DNI, el número debe tener exactamente 8 dígitos y ser numérico
            if (!/^\d{8}$/.test(model.NumDocIndent)) {
                validate = false;
                $("#txtNroDoc").addClass("error-input");
                swal({ text: 'Para DNI, el número de documento debe tener exactamente 8 dígitos y ser numérico', icon: "warning" });
            }
        } else if (model.TipoDocIdent === "CEX") {
            // Para CEX, el número puede ser alfanumérico pero debe tener al menos 9 caracteres
            if (model.NumDocIndent.length < 9) {
                validate = false;
                $("#txtNroDoc").addClass("error-input");
                swal({ text: 'Para CEX, el número de documento debe tener al menos 9 caracteres.', icon: "warning" });
            }
        }

        return validate;
    }

    const convertToISODate = (dateStr) => {
        if (!dateStr) return "";

        // Si ya está en formato ISO (yyyy-MM-dd), retorna tal cual
        if (/^\d{4}-\d{2}-\d{2}$/.test(dateStr)) {
            return dateStr;
        }

        // Si viene en formato /Date(...)/, extrae el timestamp y lo convierte
        let match = dateStr.match(/\/Date\((-?\d+)\)\//);
        if (match) {
            let timestamp = parseInt(match[1], 10);
            let date = new Date(timestamp);
            return date.toISOString().split('T')[0];
        }

        // Si viene en formato "dd/mm/yyyy HH:mm:ss" (por ejemplo "10/03/2023 00:00:00")
        let datePart = dateStr.split(' ')[0]; // Extrae "10/03/2023"
        let parts = datePart.split('/');
        if (parts.length === 3) {
            let day = parts[0].padStart(2, '0');
            let month = parts[1].padStart(2, '0');
            let year = parts[2];
            return `${year}-${month}-${day}`;
        }

        let date = new Date(dateStr);
        if (!isNaN(date.getTime())) {
            return date.toISOString().split('T')[0];
        }

        return "";
    };

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                visualizarDataTableColaboradores();
            });
        }
    }
}(jQuery);