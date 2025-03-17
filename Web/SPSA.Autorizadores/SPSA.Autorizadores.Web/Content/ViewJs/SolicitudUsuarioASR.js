var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadasPorEmpresa';

var urlListarSolicitudesASR = baseUrl + 'SolicitudASR/SolicitudUsuario/ListarPaginado';
var urlListarColaboradoresExt = baseUrl + 'Maestros/MaeColaboradorExt/ListarPaginado';
var urlListarColaboradoresInt = baseUrl + 'Maestros/MaeColaboradorInt/ListarPaginado';


var urlObtenerLocal = baseUrl + 'Maestros/MaeLocal/ObtenerLocal';

var urlModalNuevoColabExt = baseUrl + 'Maestros/MaeColaboradorExt/NuevoForm';
var urlModalModificarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/ModificarForm';

var urlObtenerColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Obtener';
var urlCrearSolicitud = baseUrl + 'SolicitudASR/SolicitudUsuario/CrearSolicitud';
var urlEliminarSolicitud = baseUrl + 'SolicitudASR/SolicitudUsuario/EliminarSolicitud';
var urlDescargarSolicitudes = baseUrl + 'SolicitudASR/SolicitudUsuario/DescargarSolicitudes';


var urlModificarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/ModificarColaborador';
var urlImportarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Importar';


var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var codLocalAlternoAnterior = "";
var dataTableColaboradoresExt = null;

var SolicitudUsuarioASR = function () {
    const eventos = function () {

        $(document).on('input', '#txtFiltroVariosExt', function (e) {
            var valor = $(this).val().toUpperCase();
            $(this).val(valor);

            var table = $('#tableColaboradoresExt').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $(document).on('input', '#txtFiltroVariosInt', function (e) {
            var valor = $(this).val().toUpperCase();
            $(this).val(valor);

            var table = $('#tableColaboradoresInt').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $('#tableSolicitudes tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $('#tableColaboradoresInt tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $('#tableColaboradoresExt tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#btnModalColabInt").on("click", async function () {
            visualizarDataTableColaboradoresInt();
            $("#modalColabInt").modal('show');
        });

        $("#btnModalColabExt").on("click", async function () {
            const responseObtenerLocal = await obtenerLocal();
            const objLocal = responseObtenerLocal.Data;
            visualizarDataTableColaboradoresExt(objLocal.CodLocalAlterno);
            $("#modalColabExt").modal('show');
        });

        $("#btnDescargarSolicitudes").on("click", async function () {
            const responseObtenerLocal = await obtenerLocal();
            const objLocal = responseObtenerLocal.Data;
            descargarSolicitudesPorLocal(objLocal.CodLocalAlterno);
        });

        $("#modalColabExt").on("shown.bs.modal", function () {
            if ($.fn.DataTable.isDataTable('#tableColaboradoresExt')) {
                $('#tableColaboradoresExt').DataTable().columns.adjust();
            }
        });

        $("#modalColabInt").on("shown.bs.modal", function () {
            if ($.fn.DataTable.isDataTable('#tableColaboradoresInt')) {
                $('#tableColaboradoresInt').DataTable().columns.adjust();
            }
        });

        $('input[name="tipoUsuarioExt"]').on('change', function (e) {
            var table = $('#tableColaboradoresExt').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $('input[name="tipoUsuarioInt"]').on('change', function (e) {
            //var table = $('#tableColaboradoresInt').DataTable();
            //e.preventDefault();
            //table.ajax.reload();
        });

        $("#btnSolicitarColabInt").on("click", function () {
            var filasSeleccionadas = document.querySelectorAll("#tableColaboradoresInt tbody tr.selected");

            if (!validarSelecion(filasSeleccionadas.length)) {
                closeLoading();
                return;
            }

            var table = $('#tableColaboradoresInt').DataTable();
            var solicitudesCrear = [];

            filasSeleccionadas.forEach(function (fila) {
                var rowData = table.row(fila).data();
                var model = {
                    CodLocalAlterno: rowData.CodLocalAlterno,
                    CodColaborador: rowData.CodigoOfisis,
                    //TipUsuario: rowData.TipoUsuario,
                    TipUsuario: 'A',
                    TipColaborador: 'I',
                    UsuSolicita: $("#txtUsuario").val(),
                    Motivo: ''
                };

                solicitudesCrear.push(crearSolicitud(model));
            });

            Promise.all(solicitudesCrear)
                .then(function () {
                    $("#modalColabInt").modal('hide');
                    $('#tableSolicitudes').DataTable().ajax.reload(null, false);
                })
                .catch(function (error) {
                    console.error("Error al crear las solicitudes:", error);
                });
        });

        $("#btnSolicitarColabExt").on("click", function () {
            var filasSeleccionadas = document.querySelectorAll("#tableColaboradoresExt tbody tr.selected");

            if (!validarSelecion(filasSeleccionadas.length)) {
                closeLoading();
                return;
            }

            var table = $('#tableColaboradoresExt').DataTable();
            var solicitudesCrear = [];

            filasSeleccionadas.forEach(function (fila) {
                var rowData = table.row(fila).data();
                var model = {
                    CodLocalAlterno: rowData.CodLocalAlterno,
                    CodColaborador: rowData.CodigoOfisis,
                    TipUsuario: rowData.TipoUsuario,
                    TipColaborador: 'E',
                    UsuSolicita: $("#txtUsuario").val(),
                    Motivo: ''
                };

                solicitudesCrear.push(crearSolicitud(model));
            });

            Promise.all(solicitudesCrear)
                .then(function () {
                    $("#modalColabExt").modal('hide');
                    $('#tableSolicitudes').DataTable().ajax.reload(null, false);
                })
                .catch(function (error) {
                    console.error("Error al crear las solicitudes:", error);
                });
        });

        $("#btnEliminarSolicitud").on("click", function () {
            var filasSeleccionadas = document.querySelectorAll("#tableSolicitudes tbody tr.selected");

            if (!validarSelecion(filasSeleccionadas.length)) {
                return;
            }

            var table = $('#tableSolicitudes').DataTable();
            var solicitudesEliminar = [];

            filasSeleccionadas.forEach(function (fila) {
                var rowData = table.row(fila).data();
                var model = {
                    NumSolicitud: rowData.NumSolicitud,
                    UsuElimina: $("#txtUsuario").val()
                };

                solicitudesEliminar.push(eliminarSolicitud(model));
            });

            Promise.all(solicitudesEliminar)
                .then(function () {
                    table.ajax.reload(null, false);
                })
                .catch(function (error) {
                    console.error("Error al eliminar alguna solicitud:", error);
                });
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

    const visualizarDataTableSolicitudes = function () {
        $('#tableSolicitudes').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                // Recoger los filtros de la página
                var filtros = {
                    CodEmpresa: $("#txtCodEmpresaxx").val(),
                    CodLocal: $("#txtCodLocalxx").val(),
                    //CodLocalAlterno: "3"
                };

                // Combinar los parámetros de paginación con los filtros
                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);


                $.ajax({
                    url: urlListarSolicitudesASR,
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
                { targets: 1, visible: false }  // Oculta la primera columna "COD. LOCAL"
            ],
            columns: [
                { data: "NumSolicitud", title: "Nro. Solicitud" },
                { data: "CodLocalAlterno", title: "CodLocalAlterno" },
                { data: "CodColaborador", title: "Código" },
                //{ data: "TipUsuario", title: "TipUsuario" },
                {
                    data: "TipUsuario",
                    title: "Tipo Usuario",
                    render: function (data, type, row) {
                        if (data === 'A') {
                            return 'AUTORIZADOR';
                        } else if (data === 'C') {
                            return 'CAJERO';
                        }
                        return data;
                    }
                },
                {
                    data: "TipColaborador",
                    title: "Tipo Colaborador",
                    render: function (data, type, row) {
                        if (data === 'E') {
                            return 'EXTERNO';
                        } else if (data === 'I') {
                            return 'INTERNO';
                        }
                        return data;
                    }
                },
                {
                    data: "IndAprobado",
                    title: "Estado",
                    render: function (data, type, row) {
                        switch (data) {
                            case 'S': return 'SOLICITADO';
                            case 'A': return 'APROBADO';
                            case 'R': return 'RECHAZADO';
                            case 'N': return 'ANULADO';
                            default: return data;
                        }
                    }
                },
                { data: "UsuSolicita", title: "Solicitante" },
                {
                    data: "TipAccion",
                    title: "Acción",
                    render: function (data, type, row) {
                        if (data === 'C') {
                            return 'Crear';
                        } else if (data === 'E') {
                            return 'Eliminar';
                        }
                        return data;
                    }
                },
                {
                    data: "FecSolicita",
                    title: "Fec. Solicitud",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                
                { data: "UsuAprobacion", title: "Aprobador" },
                {
                    data: "FecAprobacion",
                    title: "Fec. Aprobación",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                
                { data: "Motivo", title: "Motivo" },
                { data: "UsuElimina", title: "Eliminador" },
                {
                    data: "FecElimina",
                    title: "Fec. Eliminación",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                }
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

    const visualizarDataTableColaboradoresExt = function (codLocalAlterno) {

        if ($.fn.DataTable.isDataTable('#tableColaboradoresExt')) {
            $('#tableColaboradoresExt').DataTable().clear().destroy();
        }

        $('#tableColaboradoresExt').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    CodLocalAlterno: codLocalAlterno,
                    TipoUsuario: $('input[name="tipoUsuarioExt"]:checked').val(),
                    FiltroVarios: $("#txtFiltroVariosExt").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarColaboradoresExt,
                    type: "GET",
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
                { data: "CodigoOfisis", title: "Código" },
                {
                    data: null,
                    title: "Nombre Completo",
                    render: function (data, type, row) {
                        // Concatena ApelPaterno, ApelMaterno y NombreTrabajador
                        return row.ApelPaterno + " " + row.ApelMaterno + " " + row.NombreTrabajador;
                    }
                },
                { data: "PuestoTrabajo", title: "Puesto" },
                { data: "TiSitu", title: "Estado" },
                {
                    data: "FechaIngresoEmpresa",
                    title: "Fecha Ingreso",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { data: "NumDocIndent", title: "Documento" },
                { data: "NomLocal", title: "Local" }
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

    const visualizarDataTableColaboradoresInt = function () {

        if ($.fn.DataTable.isDataTable('#tableColaboradoresInt')) {
            $('#tableColaboradoresInt').DataTable().clear().destroy();
        }

        $('#tableColaboradoresInt').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    CodEmpresa: $("#txtCodEmpresaxx").val(),
                    CodLocal: $("#txtCodLocalxx").val(),
                    //TipoUsuario: $('input[name="tipoUsuarioInt"]:checked').val(),
                    FiltroVarios: $("#txtFiltroVariosInt").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarColaboradoresInt,
                    type: "GET",
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
                { data: "CodigoOfisis", title: "Código" },
                {
                    data: null,
                    title: "Nombre Completo",
                    render: function (data, type, row) {
                        return row.ApePaterno + " " + row.ApeMaterno + " " + row.NomTrabajador;
                    }
                },
                { data: "NomPuesto", title: "Puesto" },
                { data: "TiSitu", title: "Estado" },
                {
                    data: "FecIngrEmp",
                    title: "Fecha Ingreso",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { data: "NumDocIdent", title: "Documento" },
                { data: "NomLocal", title: "Local" }
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

    const obtenerLocal = function () {
        return new Promise((resolve, reject) => {
            const request = {
                CodEmpresa: $("#txtCodEmpresaxx").val(),
                CodLocal: $("#txtCodLocalxx").val()
            };

            if (!request.CodEmpresa) return resolve();
            if (!request.CodLocal) return resolve();

            $.ajax({
                url: urlObtenerLocal,
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

    const crearSolicitud = function (model) {
        return $.ajax({
            url: urlCrearSolicitud,
            type: "post",
            data: { command: model },
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
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const eliminarSolicitud = function (model) {
        return $.ajax({
            url: urlEliminarSolicitud,
            type: "post",
            data: { command: model },
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
                }
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

    const descargarSolicitudesPorLocal = function (codLocalAlterno) {

        const command = {
            CodLocalAlterno: codLocalAlterno
        };

        $.ajax({
            url: urlDescargarSolicitudes,
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
                //await cargarComboEmpresa();
                visualizarDataTableSolicitudes();
                //visualizarDataTableColaboradoresExt();
            });
        }
    }
}(jQuery);