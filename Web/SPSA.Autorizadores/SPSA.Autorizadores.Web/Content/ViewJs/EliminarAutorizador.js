var urlColaboradorCesados = baseUrl + 'EliminarAutorizador/ListarColaboradoresCesados';
var urlEliminar = baseUrl + 'EliminarAutorizador/EliminarAutorizador';


var EliminarAutorizador = function () {

    var eventos = function () {
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


        $('#tableColaboradorCesado tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });
    }


    var visualizarDataTableColaboradorCesado = function () {

        $.ajax({
            url: urlColaboradorCesados,
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

                dataTableColaboradorCesados = $('#tableColaboradorCesado').DataTable({
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
                    ]
                });

                dataTableColaboradorCesados.buttons().container().prependTo($('#tableColaboradorCesado_filter'));
                $('input[type="search"]').addClass("form-control-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });


    };

    const eliminarAutorizador = function () {

        const registrosSeleccionados = dataTableColaboradorCesados.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        showLoading();

        let autorizadores = registrosSeleccionados.map(x => {
            return {
                Codigo: x.Codigo,
                CodigoAutorizador: x.Autorizador
            }
        });


        $.ajax({
            url: urlEliminar,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            success: function (response) {

                cargarAutorizadores();

                if (response.Ok) {
                    swal({
                        text: "Proceso exitoso",
                        icon: "success"
                    });
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                }

                btnLoading($("#btnEliminar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                btnLoading($("#btnEliminar"), false);

                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
                closeLoading();
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
            url: urlColaboradorCesados,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableColaboradorCesados.clear();
                dataTableColaboradorCesados.rows.add(response.Colaboradores);
                dataTableColaboradorCesados.draw();
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
                visualizarDataTableColaboradorCesado();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
