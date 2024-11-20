const urlListarEmpresasAsociadas = baseUrl + 'Monitor/MonitorArchivos/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = baseUrl + 'Monitor/MonitorArchivos/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = baseUrl + 'Monitor/MonitorArchivos/ListarRegionesAsociadas';
const urlListarZonasAsociadas = baseUrl + 'Monitor/MonitorArchivos/ListarZonasAsociadas';
const urlListarLocalesAsociadas = baseUrl + 'Monitor/MonitorArchivos/ListarLocalesAsociadas';
const urlObtenerParametros = baseUrl + 'Monitor/MonitorArchivos/ObtenerParametros';
const urlProcesar = baseUrl + 'Monitor/MonitorArchivos/Procesar';

var dataTableMonitor = null;

const MonitorArchivos = function () {

    var eventos = function () {
        $("#cboEmpresa").on("change", function () {
            listarCadenasAsociadas();
        });

        $("#cboCadena").on("change", function () {
            listarRegionesAsociadas();
        });

        $("#cboRegion").on("change", function () {
            listarZonasAsociadas();
        });

        $("#cboZona").on("change", function () {
            listarLocalesAsociadas();
        });

        $("#btnProcesar").on('click', function () {
            procesar()
        });

    }

    const listarEmpresasAsociadas = function () {

        $.ajax({
            url: urlListarEmpresasAsociadas,
            type: 'POST',
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las empresas: ' + data.Mensaje);
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodEmpresa,
                        text: item.NomEmpresa
                    }
                });

                $("#cboEmpresa").empty();

                $("#cboEmpresa").select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $("#cboEmpresa").trigger('change');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar las empresas: ' + jqXHR);
            }
        });
    }

    const listarCadenasAsociadas = function () {

        let data = { CodEmpresa: $("#cboEmpresa").val() };

        $.ajax({
            url: urlListarCadenasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las cadenas asociadas: ' + data.Mensaje);
                    return;
                }

                let results = [];
                if (data.Data.length > 0) {
                    results.push({ id: "0", text: "TODOS" })
                }

                data.Data.forEach(item => {
                    results.push({
                        id: item.CodCadena,
                        text: item.NomCadena
                    });
                });

                $("#cboCadena").empty();

                $("#cboCadena").select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $("#cboCadena").trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const listarRegionesAsociadas = function () {

        let data = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val()
        };

        $.ajax({
            url: urlListarRegionesAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las regiones asociadas: ' + data.Mensaje);
                    return;
                }

                let results = [];
                if (data.Data.length > 0 || $("#cboCadena").val() == 0) {
                    results.push({ id: "0", text: "TODOS" })
                }

                data.Data.forEach(item => {
                    results.push({
                        id: item.CodRegion,
                        text: item.NomRegion
                    });
                });

                $("#cboRegion").empty();

                $("#cboRegion").select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $("#cboRegion").trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const listarZonasAsociadas = function () {

        let data = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val()
        };

        $.ajax({
            url: urlListarZonasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las zonas asociadas: ' + data.Mensaje);
                    return;
                }

                let results = [];
                if (data.Data.length > 0 || $("#cboRegion").val() == 0) {
                    results.push({ id: "0", text: "TODOS" })
                }

                data.Data.forEach(item => {
                    results.push({
                        id: item.CodZona,
                        text: item.NomZona
                    });
                });

                $("#cboZona").empty();

                $("#cboZona").select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $("#cboZona").trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar las zonas: ' + jqXHR);
            }
        });
    }

    const listarLocalesAsociadas = function () {

        let data = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val()
        };

        $.ajax({
            url: urlListarLocalesAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las locales asociadas: ' + data.Mensaje);
                    return;
                }

                let results = [];
                if (data.Data.length > 0 || $("#cboZona").val() == 0) {
                    results.push({ id: "0", text: "TODOS" })
                }

                data.Data.forEach(item => {
                    results.push({
                        id: item.CodLocal,
                        text: item.NomLocal
                    });
                });

                $("#cboLocal").empty();

                $("#cboLocal").select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $("#cboLocal").trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar locales: ' + jqXHR);
            }
        });
    }

    const listarParametros = function () {

        $.ajax({
            url: urlObtenerParametros,
            type: 'POST',
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al obtener los parametros: ' + data.Mensaje);
                    return;
                }

                $("#txtRuta").val(data.Data.Ruta);
                $("#txtClave").val(data.Data.Clave);
                $("#txtUsuario").val(data.Data.Usuario);


            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar locales: ' + jqXHR);
            }
        });
    }

    const procesar = function () {

        if (!validar()) {
            return;
        }

        let data = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val(),
            CodLocal: $("#cboLocal").val(),
            Ruta: $("#txtRuta").val(),
            Clave: $("#txtClave").val(),
            Usuario: $("#txtUsuario").val()
        };

        $.ajax({
            url: urlProcesar,
            data: JSON.stringify(data),
            type: 'POST',
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al procesar: ' + data.Mensaje);
                    return;
                }


                swal({
                    text: "Proceso exitoso",
                    icon: "success"
                }).then(function () {
                    dataTableMonitor.clear();
                    dataTableMonitor.rows.add(data.Data);
                    dataTableMonitor.draw();
                });


            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al procesar: ' + jqXHR);
            }
        });
    }

    const inicializarTable = function () {

        var columnas = [
            {
                title: "Cod Local",
                data: "CodLocal"
            },
            {
                title: "Des. Local",
                data: "DesLocal"
            },
            {
                title: "Caja",
                data: "NumCaja"
            },
            {
                title: "IP",
                data: "IpCaja"
            },
            {
                title: "Cant. Archivos",
                data: "CantidadArchivos"
            },
            {
                title: "Observacion",
                data: "Observacion"
            }
        ];


        dataTableMonitor = $('#tableMonitor').DataTable({
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            searching: false,
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            columns: columnas,
            bAutoWidth: false,
            buttons: [
                {
                    extend: 'excel',
                    text: 'Exportar excel',
                    titleAttr: 'Exportar Excel',
                    className: 'btn btn-primary btn-block',
                    exportOptions: {
                        modifier: { page: 'all' }
                    },
                    action: function (e, dt, node, config) {

                        if (!this.data().count()) {
                            swal({
                                text: "No hay información disponible para Exportar.",
                                icon: "warning"
                            });
                            return;
                        }
                        $.fn.dataTable.ext.buttons.excelHtml5.action.call(this, e, dt, node, config);
                    }
                },
            ],
            order: []
        });
        $("#container-btn-exportar").append(dataTableMonitor.buttons().container());
    };

    const validar = function () {

        console.log($("#cboRegion").val());

        if ($("#cboEmpresa").val() == "") {
            mensajeAdvertencia('Debe seleccionar una empresa');
            return false;
        }

        if ($("#cboCadena").val() == null) {
            mensajeAdvertencia('Debe seleccionar una cadena');
            return false;
        }


        if ($("#cboRegion").val() == null) {
            mensajeAdvertencia('Debe seleccionar una región');
            return false;
        }

        if ($("#cboZona").val() == null) {
            mensajeAdvertencia('Debe seleccionar una zona');
            return false;
        }

        if ($("#cboLocal").val() == null) {
            mensajeAdvertencia('Debe seleccionar un local');
            return false;
        }

        if ($("#txtRuta").val() == "") {
            mensajeAdvertencia('Debe ingresar una ruta');
            return false;
        }

        if ($("#txtClave").val() == "") {
            mensajeAdvertencia('Debe ingresar una clave');
            return false;
        }

        if ($("#txtUsuario").val() == "") {
            mensajeAdvertencia('Debe ingresar un usuario');
            return false;
        }

        return true;


    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                listarEmpresasAsociadas();
                listarParametros();
                inicializarTable();
            });
        }
    }

}(jQuery);