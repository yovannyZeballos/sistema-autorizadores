var urlColaborador = baseUrl + 'Autorizador/ListarColaborador';
var urlTodosColaborador = baseUrl + 'Autorizador/ListarTodosColaborador';
var urlAutorizador = baseUrl + 'Autorizador/ListarAutorizador';
var urlAsignarAutorizador = baseUrl + 'Autorizador/AsignarAutorizador';
var urlEliminarAutorizador = baseUrl + 'Autorizador/EliminarAutorizador';
var urlActualizarEstadoArchivoAutorizador = baseUrl + 'Autorizador/ActualizarEstadoArchivoAutorizador';


var Autorizador = function () {

    var eventos = function () {
        $("#btnAsignar").on('click', function () {
            asignarAutorizador();
        });

        $("#btnGenerar").on('click', function () {
            generacionArchivo();
        });

        $("#btnEliminar").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar a los autorizadores?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        eliminarAutorizador();
                    }
                });



        });

        $("#btnBuscarColaborador").on('click', function () {
            $('#modalBusqueda').modal('show');
            cargarTodosColaboradores();
        });

        $("#btnAsignarAutorizadorBusqueda").on('click', function () {
            asignarAutorizadorBusqueda();
        });

        $("#chkAnulados,#chkActivos").on("change", function () {
            checkActivoAnulado();
        })

        $("#chkAnuladosTodos,#chkActivosTodos").on("change", function () {
            checkActivoAnuladoTodos();
        })

        $("#chkInactivosAutorizadores,#chkActivosAutorizadores").on("change", function () {
            checkActivoAnuladoAutorizadores();
        })

    }

    $('#tableColaborador tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $('#tableAutorizador tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $('#tableBusquedaColaborador tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    var visualizarDataTableColaborador = function () {
        dataTableColaborador = $('#tableColaborador').DataTable({
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '180px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            "ajax": {
                "url": urlColaborador,
                "type": "POST",
                "dataSrc": "Colaboradores"
            },
            "bAutoWidth": true,
            "columns": [
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.Codigo;
                        else
                            return '<span class="text-danger">' + obj.Codigo + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.ApellidoPaterno;
                        else
                            return '<span class="text-danger">' + obj.ApellidoPaterno + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.ApellidoMaterno;
                        else
                            return '<span class="text-danger">' + obj.ApellidoMaterno + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.Nombres;
                        else
                            return '<span class="text-danger">' + obj.Nombres + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.FechaIngreso;
                        else
                            return '<span class="text-danger">' + obj.FechaIngreso + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return 'ACTIVO';
                        else
                            return '<span class="text-danger">ANULADO</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.NumeroDocumento;
                        else
                            return '<span class="text-danger">' + obj.NumeroDocumento + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.DescPuesto;
                        else
                            return '<span class="text-danger">' + obj.DescPuesto + '</span>';
                    }
                }
            ]
        });
    };

    var visualizarDataTableAutorizador = function () {

        $.ajax({
            url: urlAutorizador,
            type: "post",
            dataType: "json",
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", ""),
                        defaultContent: "",
                    });
                });

                dataTableAutorizador = $('#tableAutorizador').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '180px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    //"ajax": {
                    //    "url": urlAutorizador,
                    //    "type": "POST",
                    //    "dataSrc": "Autorizadores"
                    //},
                    "columns": columnas,
                    "data": response.Autorizadores,
                    "bAutoWidth": false,
                    rowCallback: function (row, data, index) {
                        if (data.Estado == "ELI") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    //"columns": [
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.CodigoAutorizador;
                    //            else
                    //                return '<span class="text-danger">' + obj.CodigoAutorizador + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.Codigo;
                    //            else
                    //                return '<span class="text-danger">' + obj.Codigo + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.ApellidoPaterno;
                    //            else
                    //                return '<span class="text-danger">' + obj.ApellidoPaterno + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.ApellidoMaterno;
                    //            else
                    //                return '<span class="text-danger">' + obj.ApellidoMaterno + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.Nombres;
                    //            else
                    //                return '<span class="text-danger">' + obj.Nombres + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return 'ACTIVO';
                    //            else
                    //                return '<span class="text-danger">INACTIVO</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.NumeroDocumento;
                    //            else
                    //                return '<span class="text-danger">' + obj.NumeroDocumento + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.FechaCreacion;
                    //            else
                    //                return '<span class="text-danger">' + obj.FechaCreacion + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.UsuarioCreacion;
                    //            else
                    //                return '<span class="text-danger">' + obj.UsuarioCreacion + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.NumeroTarjeta;
                    //            else
                    //                return '<span class="text-danger">' + obj.NumeroTarjeta + '</span>';
                    //        }
                    //    },
                    //    {
                    //        "data": function (obj) {
                    //            if (obj.Estado == 'A')
                    //                return obj.Impreso;
                    //            else
                    //                return '<span class="text-danger">' + obj.Impreso + '</span>';
                    //        }
                    //    },
                    //]
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });


    };

    var visualizarDataTableBusquedaColaborador = function () {
        dataTableBusquedaColaborador = $('#tableBusquedaColaborador').DataTable({
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '180px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            "bAutoWidth": false,
            "columns": [
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.Codigo;
                        else
                            return '<span class="text-danger">' + obj.Codigo + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.ApellidoPaterno;
                        else
                            return '<span class="text-danger">' + obj.ApellidoPaterno + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.ApellidoMaterno;
                        else
                            return '<span class="text-danger">' + obj.ApellidoMaterno + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.Nombres;
                        else
                            return '<span class="text-danger">' + obj.Nombres + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.FechaIngreso;
                        else
                            return '<span class="text-danger">' + obj.FechaIngreso + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return 'ACTIVO';
                        else
                            return '<span class="text-danger">ANULADO</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.NumeroDocumento;
                        else
                            return '<span class="text-danger">' + obj.NumeroDocumento + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.CodigoLocal;
                        else
                            return '<span class="text-danger">' + obj.CodigoLocal + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.DescLocal;
                        else
                            return '<span class="text-danger">' + obj.DescLocal + '</span>';
                    }
                },
                {
                    "data": function (obj) {
                        if (obj.Estado == 'ACT')
                            return obj.DescPuesto;
                        else
                            return '<span class="text-danger">' + obj.DescPuesto + '</span>';
                    }
                }
            ]
        });
    };

    const asignarAutorizador = function () {

        const registrosSeleccionados = dataTableColaborador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        btnLoading($("#btnAsignar"), true);

        $.ajax({
            url: urlAsignarAutorizador,
            type: "post",
            data: { autorizadores: registrosSeleccionados },
            dataType: "json",
            success: function (response) {
                dataTableColaborador.ajax.reload();
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });
                btnLoading($("#btnAsignar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnAsignar"), false);
            }
        });
    }

    const asignarAutorizadorBusqueda = function () {

        const registrosSeleccionados = dataTableBusquedaColaborador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        btnLoading($("#btnAsignarAutorizadorBusqueda"), true);

        $.ajax({
            url: urlAsignarAutorizador,
            type: "post",
            data: { autorizadores: registrosSeleccionados },
            dataType: "json",
            success: function (response) {
                dataTableColaborador.ajax.reload();
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });

                btnLoading($("#btnAsignarAutorizadorBusqueda"), false);

                $('#modalBusqueda').modal('hide');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnAsignarAutorizadorBusqueda"), false);
            }
        });
    }

    const eliminarAutorizador = function () {

        const registrosSeleccionados = dataTableAutorizador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }


        let autorizadores = registrosSeleccionados.map(x => {
            return {
                Codigo: x.Codigo,
                CodigoAutorizador: x.Autorizador,
                UsuarioCreacion: x.UCreación
            }
        });



        btnLoading($("#btnEliminar"), true);

        $.ajax({
            url: urlEliminarAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            success: function (response) {
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });
                btnLoading($("#btnEliminar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
                btnLoading($("#btnEliminar"), false);
            }
        });
    }

    const generacionArchivo = function () {

        const registrosSeleccionados = dataTableAutorizador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        btnLoading($("#btnGenerar"), true);

        let autorizadores = registrosSeleccionados.map(x => {
            return {
                Codigo: x.Codigo,
                CodigoAutorizador: x.Autorizador,
                CodLocal: x.LOCAL,
                NumeroTarjeta: x.Tarjeta
            }
        });

        $.ajax({
            url: urlActualizarEstadoArchivoAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            success: function (response) {
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });
                btnLoading($("#btnGenerar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnGenerar"), false);
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

    const cargarTodosColaboradores = function () {
        $.ajax({
            url: urlTodosColaborador,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableBusquedaColaborador.clear();
                dataTableBusquedaColaborador.rows.add(response.Colaboradores);
                dataTableBusquedaColaborador.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const cargarAutorizadores = function () {
        $.ajax({
            url: urlAutorizador,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableAutorizador.clear();
                dataTableAutorizador.rows.add(response.Autorizadores);
                dataTableAutorizador.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const checkActivoAnulado = function () {

        if ($("#chkAnulados").prop('checked') && !$("#chkActivos").prop('checked')) {
            dataTableColaborador.column(5).search('ANU').draw();
        }
        else if ($("#chkActivos").prop('checked') && !$("#chkAnulados").prop('checked')) {
            dataTableColaborador.column(5).search('ACT').draw();
        }
        else {
            dataTableColaborador.column(5).search('').draw();
        }
    }

    const checkActivoAnuladoAutorizadores = function () {

        if ($("#chkInactivosAutorizadores").prop('checked') && !$("#chkActivosAutorizadores").prop('checked')) {
            dataTableAutorizador.column(8).search('ELI').draw();
        }
        else if ($("#chkActivosAutorizadores").prop('checked') && !$("#chkInactivosAutorizadores").prop('checked')) {
            var regExSearch = '\\b' + 'ACT' + '\\b';
            dataTableAutorizador.column(8).search(regExSearch, true, false).draw();
        }
        else {
            dataTableAutorizador.column(8).search('').draw();
        }
    }

    const checkActivoAnuladoTodos = function () {

        if ($("#chkAnuladosTodos").prop('checked') && !$("#chkActivosTodos").prop('checked')) {
            dataTableBusquedaColaborador.column(5).search('ANU').draw();
        }
        else if ($("#chkActivosTodos").prop('checked') && !$("#chkAnuladosTodos").prop('checked')) {
            dataTableBusquedaColaborador.column(5).search('ACT').draw();
        }
        else {
            dataTableBusquedaColaborador.column(5).search('').draw();
        }
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                visualizarDataTableColaborador();
                visualizarDataTableAutorizador();
                visualizarDataTableBusquedaColaborador();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
