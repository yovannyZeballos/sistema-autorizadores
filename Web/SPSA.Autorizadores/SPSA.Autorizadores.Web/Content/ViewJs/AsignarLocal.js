var urlLocalesAsignar = baseUrl + 'Autorizadores/AsignarLocal/ListarLocalesAsignar';
var urlAsignar = baseUrl + 'Autorizadores/AsignarLocal/Asignar';


var AsignarLocal = function () {

    var eventos = function () {
        $("#btnAsignar").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro asignar los locales?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        asignarLocal();
                    }
                });
        });


        $('#tableLocalesAsignar tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#cboEmpresa").on("change", function () {
            const valor = $(this).val();
            let regExSearch = '\\b' + valor + '\\b';

            if (valor === "0") {
                dataTableLocalesAsignar.column(1).search('').draw();

            }
            else if (valor === "4") {
                regExSearch = "^[4-8]$";
                dataTableLocalesAsignar.column(1).search(regExSearch, true, false).draw();
            }
            else {
                dataTableLocalesAsignar.column(1).search(regExSearch, true, false).draw();
            }
        });

        $("#cboInd").on("change", function () {
            const valor = $(this).val();
            let regExSearch = '\\b' + valor + '\\b';

            if (valor === "0") {
                dataTableLocalesAsignar.column(5).search('').draw();

            }
            else {
                dataTableLocalesAsignar.column(5).search(regExSearch, true, false).draw();
            }
        });
    }


    var visualizarDataTableLocal = function () {

        $.ajax({
            url: urlLocalesAsignar,
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

                dataTableLocalesAsignar = $('#tableLocalesAsignar').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '300px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    "data": response.Locales,
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

                dataTableLocalesAsignar.buttons().container().prependTo($('#tableLocalesAsignar_filter'));

                $('input[type="search"]').addClass("form-control-sm");
                closeLoading();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });


    };

    const cargarLocales = function () {
        $.ajax({
            url: urlLocalesAsignar,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableLocalesAsignar.clear();
                dataTableLocalesAsignar.rows.add(response.Locales);
                dataTableLocalesAsignar.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const asignarLocal = function () {

        const registrosSeleccionados = dataTableLocalesAsignar.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        showLoading();

        let data = registrosSeleccionados.map(x => {
            return {
                CodCadena: x.Cadena,
                CodLocal: x.Local,
            }
        });


        $.ajax({
            url: urlAsignar,
            type: "post",
            data: {
                request: data
            },
            dataType: "json",
            success: function (response) {
                closeLoading();

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

                cargarLocales();

            },
            error: function (jqXHR, textStatus, errorThrown) {
                closeLoading();
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
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

    return {
        init: function () {
            checkSession(function () {
                eventos();
                visualizarDataTableLocal();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
