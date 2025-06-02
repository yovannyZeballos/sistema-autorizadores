var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesAsociados = baseUrl + 'SolicitudASR/Aprobacion/ListarLocalesAsociadasPorEmpresa';
var urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadasPorEmpresa';
const urlListarUsuarios = baseUrl + 'DataTables/Listas/ListarUsuario';
const urlListarSolicitudes = baseUrl + 'SolicitudASR/Aprobacion/ListarSolicitudes';
var urlListarUsuariosASR = baseUrl + 'SolicitudASR/Aprobacion/ListarUsuarios';
var urlRechazar = baseUrl + 'SolicitudASR/Aprobacion/Rechazar';
var urlAprobarCrear = baseUrl + 'SolicitudASR/Aprobacion/AprobarSolicitudCrear';
var urlAprobarEliminar = baseUrl + 'SolicitudASR/Aprobacion/AprobarSolicitudEliminar';
var dataTableUsuario = null;
var dataTableSolicitud = null;

var AprobacionASR = function () {
    const eventos = function () {

        $("#cboEmpresaBuscar").on("change", async function () {
            await cargarComboLocales();
        });


        $(".filtro").on('change', function () {
            visualizarDataTableUsuario();
        });

        $("#btnModalSolicitud").on('click', function () {
            if (dataTableSolicitud != null) {
                dataTableSolicitud.columns.adjust().draw();
            }
            
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

        $('input[name="tipoUsuario"]').on('change', function (e) {
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

    const listarEmpresasAsociadas = function () {
        return new Promise((resolve, reject) => {

            const request = {
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

    const listarLocalesAsociados = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $('#cboEmpresaBuscar').val();
            if (!codEmpresa) return resolve();

            const query = {
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
                //$('#cboEmpresaBuscar').empty().append('<option label="Todos"></option>');
                $('#cboEmpresaBuscar').empty().append($('<option>', { value: '0', text: 'Todos' }));
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

    const cargarComboLocales = async function (selectedLocal = null) {
        try {
            const response = await listarLocalesAsociados();

            if (!response) return;

            if (response.Ok) {

                $('#cboLocalBuscar').empty().append($('<option>', { value: '0', text: 'Todos' }));

                response.Data.map(local => {
                    $('#cboLocalBuscar').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
                });

                // Si se pasó un valor seleccionado, lo asignamos
                if (selectedLocal) {
                    $('#cboEmpresaBuscar').val(selectedLocal);
                }
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "warning"
                });
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
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
                { title: "Nro", data: "NumSolicitud" },
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
        $('#cboLocalBuscar').append($('<option>', { value: '0', text: 'Todos' }));
        locales.map(local => {
            $('#cboLocalBuscar').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
        });
        $('#cboLocalBuscar').val('0');


    }

    const cargarUsuarios = function (usuarios) {
        $('#cboUsuarioBuscar').append($('<option>', { value: '0', text: 'Todos' }));
        usuarios.map(usuario => {
            $('#cboUsuarioBuscar').append($('<option>', { value: usuario.out_codigo, text: `${usuario.out_codigo} - ${usuario.out_nombre}` }));
        });
        $('#cboUsuarioBuscar').val('0');


    }

    const visualizarDataTableSolicitud = function () {

        //if (dataTableSolicitud != null) {
        //    dataTableSolicitud.destroy();
        //}

        dataTableSolicitud = $('#tableSolicitudes').DataTable({
            destroy: true,
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                // Recoger los filtros de la página
                const request = {
                    TipColaborador: $('input[name="tipoUsuario"]:checked').val(),
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
                { title: "Nro.", data: "NumSolicitud" },
                { title: "Codigo", data: "CodColaborador" },
                {
                    title: "Nombre y apellido",
                    data: null,
                    render: function (data, type, row) {
                        return `${row.NoTrab} ${row.NoApelPate} ${row.NoApelMate}`;
                    }
                },
                { title: "Puesto", data: "DePuesTrab" },
                /*{ title: "Estado", data: "IndAprobado" },*/
                { title: "Accion", data: "TipAccion" },
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
                { title: "Tipo Doc.", data: "TipDocumentoIdentidad" },
                { title: "Num. Doc.", data: "NumDocumentoIdentidad" },
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

        //dataTableSolicitud.ajax.reload(null, false);
        /*dataTableSolicitud.columns.adjust().draw();*/
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
        // 1. Obtenemos todos los registros seleccionados
        const filas = dataTableSolicitud.rows('.selected').data().toArray();
        if (!validarSelecion(filas.length)) return;

        // 2. Mapeamos cada fila a nuestro modelo, incluyendo TipAccion
        const modelos = filas.map(item => ({
            NumSolicitud:           item.NumSolicitud,
            CodLocal:               item.CodLocal,
            CodLocalAlterno:        item.CodLocalAlterno,
            CodPais:                item.CodPais,
            CodComercio:            item.CodComercio,
            DePuesTrab:             item.DePuesTrab,
            IndAprobado:            item.IndAprobado,
            NoApelMate:             item.NoApelMate,
            NoApelPate:             item.NoApelPate,
            NoTrab:                 item.NoTrab,
            NomLocal:               item.NomLocal,
            TipDocumentoIdentidad:  item.TipDocumentoIdentidad,
            NumDocumentoIdentidad:  item.NumDocumentoIdentidad,
            CodColaborador:         item.CodColaborador,
            TipUsuario:             item.TipUsuario,
            TipAccion:              item.TipAccion
        }));

        // 3. Separamos las solicitudes por acción
        const solicitudesCrear = modelos.filter(m => m.TipAccion === 'CREAR');
        const solicitudesEliminar = modelos.filter(m => m.TipAccion === 'ELIMINAR');

        // 4. Generamos las llamadas AJAX
        const llamadas = [];

        if (solicitudesCrear.length) {
            llamadas.push(
                $.ajax({
                    url: urlAprobarCrear,      // tu controller de “crear”
                    type: 'POST',
                    dataType: 'json',
                    data: { request: { Solicitudes: solicitudesCrear } }
                })
            );
        }

        if (solicitudesEliminar.length) {
            llamadas.push(
                $.ajax({
                    url: urlAprobarEliminar,   // tu controller de “eliminar”
                    type: 'POST',
                    dataType: 'json',
                    data: { request: { Solicitudes: solicitudesEliminar } }
                })
            );
        }

        // 5. Ejecutamos todas las llamadas
        showLoading();
        Promise.all(llamadas)
            .then(responses => {
                // responses es un array de objetos JSON { Ok, Mensaje, ... }
                const mensajes = responses.map(r => r.Mensaje);
                const todasOk = responses.every(r => r.Ok === true);

                // 6. Un solo swal con todos los mensajes concatenados
                swal({
                    text: mensajes.join('\n'),
                    icon: todasOk ? 'success' : 'warning'
                });

                // 7. Cerrar modal sólo si todasOk
                if (todasOk) {
                    $('#solicitudesModal').modal('hide');
                }

                // 8. Recargar la tabla siempre
                visualizarDataTableUsuario();
            })
            .catch(err => {
                console.error('Error inesperado:', err);
                swal({
                    text: 'Ocurrió un error inesperado al aprobar las solicitudes.',
                    icon: 'error'
                });
            })
            .finally(() => {
                closeLoading();
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
                await cargarComboEmpresa();
                await cargarComboLocales();
                //listarLocales();
                listarUsuariosAprobador();
                visualizarDataTableUsuario();
            });
        }
    }
}(jQuery);