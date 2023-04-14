var urlColaboradorMass = baseUrl + 'Autorizadores/AutorizadoresMass/ListarColaboradores';
var urlAutorizador = baseUrl + 'Autorizadores/Autorizador/ListarAutorizador';
var urlAsignarAutorizador = baseUrl + 'Autorizadores/AutorizadoresMass/AsignarAutorizador';


var AutorizadorMass = function () {

    var eventos = function () {
        $("#btnAsignar").on('click', function () {
            asignarAutorizador();
        });

        $("#btnAsignarAutorizadorBusqueda").on('click', function () {
            asignarAutorizadorBusqueda();
        });

        $("#chkTodos").on("change", function () {
            $('#tableColaboradorMass tbody tr').toggleClass('selected');
        });
    }

    $('#tableColaboradorMass tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $('#tableAutorizadorMass tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    var visualizarDataTableColaboradorMass = function () {

        $.ajax({
            url: urlColaboradorMass,
            type: "post",
            dataType: "json",
            success: function (response) {

                var columnas = [];

                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", ""),
                        defaultContent: "",
                    });
                });

                dataTableColaboradorMass = $('#tableColaboradorMass').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '300px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    "data": response.Colaboradores,
                    "bAutoWidth": false,
                    rowCallback: function (row, data, index) {
                        if (data.Estado == "ELI") {
                            $("td", row).addClass("text-danger");
                        }
                    }
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

                dataTableAutorizadorMass = $('#tableAutorizadorMass').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '300px',
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

    const asignarAutorizador = function () {

        const registrosSeleccionados = dataTableColaboradorMass.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        showLoading();

        let autorizadores = [];

        registrosSeleccionados.map((item) => {
            autorizadores.push({
                Codigo: item.CODIGO,
                Nombres: item.NOMBRE,
                ApellidoPaterno: item.PATERNO,
                ApellidoMaterno: item.MATERNO,
                NumeroDocumento: item.DOCUMENTO,
                CodLocal: item.LOC
            });
        });

        $.ajax({
            url: urlAsignarAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            success: function (response) {
                cargarColaboradoresMass();
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
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

    const cargarAutorizadores = function () {
        $.ajax({
            url: urlAutorizador,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableAutorizadorMass.clear();
                dataTableAutorizadorMass.rows.add(response.Autorizadores);
                dataTableAutorizadorMass.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const cargarColaboradoresMass = function () {
        $.ajax({
            url: urlColaboradorMass,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableColaboradorMass.clear();
                dataTableColaboradorMass.rows.add(response.Colaboradores);
                dataTableColaboradorMass.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                visualizarDataTableColaboradorMass();
                visualizarDataTableAutorizador();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
