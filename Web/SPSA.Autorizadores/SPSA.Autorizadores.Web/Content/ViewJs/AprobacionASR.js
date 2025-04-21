var urlListarLocalesAsociados = baseUrl + 'SolicitudASR/Aprobacion/ListarLocalesAsociadasPorEmpresa';
const urlListarUsuarios = baseUrl + 'DataTables/Listas/ListarUsuario';
const urlListarSolicitudes = baseUrl + 'SolicitudASR/Aprobacion/ListarSolicitudes';
var urlListarUsuariosASR = baseUrl + 'SolicitudASR/Aprobacion/ListarUsuarios';
var urlRechazar = baseUrl + 'SolicitudASR/Aprobacion/Rechazar';
var urlAprobar = baseUrl + 'SolicitudASR/Aprobacion/Aprobar';
var dataTableUsuario = null;
var dataTableSolicitud = null;

var AprobacionASR = function () {
    const eventos = function () {
        $(".filtro").on('change', function () {
            visualizarDataTableUsuario();
        });

        $("#btnModalSolicitud").on('click', function () {
            $('#solicitudesModal').modal('show');
            visualizarDataTableSolicitud();
        });

        $("#btnRechazarModal").on('click', function () {
            const registrosSeleccionados = dataTableSolicitud.rows('.selected').data().toArray();
            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }
            $('#motivoRechazoModal').modal('show');
        });

        $("#btnRechazar").on('click', function () {
            $('#motivoRechazoModal').modal('hide');
            rechazar();
        });

        $("#btnAprobar").on('click', function () {
            aprobar();
        });

        $('input[name="tipoUsuarioInt"]').on('change', function (e) {
            visualizarDataTableSolicitud();
        });

        $('#txtFiltroUsuario').on('change', function (e) {
            visualizarDataTableUsuario();
        });

        $('#txtFiltroSolicitud').on('change', function (e) {
            visualizarDataTableSolicitud();
        });

        // evento para seleccionar filas en múltiples tablas
        ['#tableSolicitudes tbody'].forEach(selector => {
            $(selector).on('click', 'tr', function () {
                $(this).toggleClass('selected');
            });
        });
    };

    const listarLocales = function () {

        $.ajax({
            url: urlListarLocalesAsociados,
            type: "post",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (response.Ok === true) {
                    cargarLocales(response.Data);
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
            }
        });
    }

    const listarUsuariosAprobador = function () {
        $.ajax({
            url: urlListarUsuarios,
            type: "post",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (response.Ok === true) {
                    cargarUsuarios(response.Usuarios)
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
            }
        });
    }

    const visualizarDataTableUsuario = function () {

        if (dataTableUsuario != null) {
            dataTableUsuario.destroy();
        }

        dataTableUsuario = $('#tableUsuario').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                // Recoger los filtros de la página
                var request = {
                    CodLocal: $("#cboLocalBuscar").val(),
                    UsuAprobacion: $("#cboUsuarioBuscar").val(),
                    TipAccion: $("#cboTipoSolicitudBuscar").val(),
                    IndAprobado: $("#cboEstadoBuscar").val(),
                    NumeroPagina: data.start / data.length + 1,
                    TamañoPagina: data.length,
                    Busqueda: $("#txtFiltroUsuario").val(),
                };


                $.ajax({
                    url: urlListarUsuariosASR,
                    type: "POST",
                    data: request,
                    dataType: "json",
                    beforeSend: function () {
                        showLoading();
                    },
                    complete: function () {
                        closeLoading();
                    },
                    success: function (response) {
                        if (response.Ok) {
                            var pagedData = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pagedData.TotalRecords,
                                recordsFiltered: pagedData.TotalRecords,
                                data: pagedData.Items
                            });
                            $('#txtFiltroUsuario').focus();
                        } else {
                            swal({ text: response.Mensaje, icon: "error" });
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
            columns: [
                { title: "Nro. Solicitud", data: "NumSolicitud" },
                { title: "Local", data: "NomLocal" },
                { title: "Codigo", data: "CodColaborador" }, 
                {
                    title: "Nombre y apellido",
                    data: null,
                    render: function (data, type, row) {
                        return `${row.NoTrab} ${row.NoApelPate} ${row.NoApelMate}`;
                    }
                },
                { title: "Accion", data: "TipAccion" },
                { title: "Tipo Usuario", data: "TipUsuario" },
                { title: "Tipo colaborador", data: "TipColaborador" },
                { title: "Puesto", data: "DePuesTrab" },
                { title: "Estado", data: "IndAprobado" },
                {
                    title: "Solicitante",
                    data: null,
                    render: function (data, type, row) {
                        return `${row.UsuSolicita} - ${row.UsuSolNoTrab} ${row.UsuSolNoApelPate} ${row.UsuSolNoApelMate}`;
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

                        //if (data) {
                        //    return formatearFecha(data);
                        //}
                        //return "";
                    }
                },
                {
                    title: "Aprobador",
                    data: null,
                    render: function (data, type, row) {
                        return `${row.UsuAprobacion} - ${row.UsuAprNoTrab} ${row.UsuAprNoApelPate} ${row.UsuAprNoApelMate}`;
                    }
                },
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

                        //if (data) {
                        //    return formatearFecha(data);
                        //}
                        //return "";
                    }
                },
                { title: "Motivo", data: "Motivo" },
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

    const cargarLocales = function (locales) {
        $('#cboLocalBuscar').append($('<option>', { value: '0', text: 'TODOS' }));
        locales.map(local => {
            $('#cboLocalBuscar').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
        });
        $('#cboLocalBuscar').val('0');


    }

    const cargarUsuarios = function (usuarios) {
        $('#cboUsuarioBuscar').append($('<option>', { value: '0', text: 'TODOS' }));
        usuarios.map(usuario => {
            $('#cboUsuarioBuscar').append($('<option>', { value: usuario.out_codigo, text: `${usuario.out_codigo} - ${usuario.out_nombre}` }));
        });
        $('#cboUsuarioBuscar').val('0');


    }

    const visualizarDataTableSolicitud = function () {

        if (dataTableSolicitud != null) {
            dataTableSolicitud.destroy();
        }

        dataTableSolicitud = $('#tableSolicitudes').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                // Recoger los filtros de la página
                const request = {
                    TipColaborador: $('input[name="tipoUsuarioInt"]:checked').val(),
                    NumeroPagina: data.start / data.length + 1,
                    TamañoPagina: data.length,
                    Busqueda: $("#txtFiltroSolicitud").val(),
                }

                $.ajax({
                    url: urlListarSolicitudes,
                    type: "POST",
                    data: request,
                    dataType: "json",
                    beforeSend: function () {
                        showLoading();
                    },
                    complete: function () {
                        closeLoading();
                    },
                    success: function (response) {
                        if (response.Ok) {
                            var pagedData = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pagedData.TotalRecords,
                                recordsFiltered: pagedData.TotalRecords,
                                data: pagedData.Items
                            });
                            $('#txtFiltroSolicitud').focus();
                        } else {
                            swal({ text: response.Mensaje, icon: "error" });
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
            columns: [
                { title: "Nro. Solicitud", data: "NumSolicitud" },
                { title: "Codigo", data: "CodColaborador" },
                {
                    title: "Nombre y apellido",
                    data: null,
                    render: function (data, type, row) {
                        return `${row.NoTrab} ${row.NoApelPate} ${row.NoApelMate}`;
                    }
                },
                { title: "Puesto", data: "DePuesTrab" },
                { title: "Estado", data: "IndAprobado" },
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
                        //if (data) {
                        //    return formatearFecha(data);
                        //}
                        //return "";
                    }
                },
                { title: "Documento", data: "NumDocumentoIdentidad" },
                { title: "Local", data: "NomLocal" }
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

    const rechazar = function () {
        const registrosSeleccionados = dataTableSolicitud.rows('.selected').data().toArray();
        let numSolicitudes = [];
        registrosSeleccionados.map((item) => {
            numSolicitudes.push(item.NumSolicitud);
        });

        const request = {
            NumSolicitudes: numSolicitudes,
            Motivo: $('#txtMotivo').val()
        }

        $.ajax({
            url: urlRechazar,
            type: "post",
            dataType: "json",
            data: { request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                let icon = response.Ok ? "success" : "error";
                swal({ text: response.Mensaje, icon: icon });
                visualizarDataTableSolicitud();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    const aprobar = function () {
        const registrosSeleccionados = dataTableSolicitud.rows('.selected').data().toArray();
        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let numSolicitudes = [];
        registrosSeleccionados.map((item) => {
            numSolicitudes.push(item.NumSolicitud);
        });

        const request = {
            NumSolicitudes: numSolicitudes
        }

        $.ajax({
            url: urlAprobar,
            type: "post",
            dataType: "json",
            data: { request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                let icon = response.Ok ? "success" : "error";
                swal({ text: response.Mensaje, icon: icon });
                $('#solicitudesModal').modal('hide');
                visualizarDataTableUsuario();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

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

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                listarLocales();
                listarUsuariosAprobador();
                visualizarDataTableUsuario();
            });
        }
    }
}(jQuery);