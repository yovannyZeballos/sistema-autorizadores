const urlListarLocales = '/Local/ListarLocalesAsignados';
const urlListarCajeros = '/CajeroVolante/ListarCajerosVolanteOfiplan';
const urlListarCajerosAsginados = '/CajeroVolante/ListarCajerosVolante'; 
const urlCrearCajeroVolante = '/CajeroVolante/CrearCajerosVolante';
const urlEliminarCajeroVolante = '/CajeroVolante/EliminarCajerosVolante';

var dataTableCajero = null;
var dataTableCajeroVolante = null;
var dataTableLocal = null;

const CajeroVolante = function () {

    var eventos = function () {

        $("#btnAsignar").on('click', function () {
            asignar();
        });

        $("#bntEliminar").on('click', function () {
            eliminar();
        });

        $('#tableCajero tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $('#tableCajeroVolante tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });


        $("#chkActivos,#chkInactivos").on("change", function () {

            if ($("#chkInactivos").prop('checked') && !$("#chkActivos").prop('checked')) {
                dataTableCajeroVolante.column(4).search('N').draw();
            }
            else if ($("#chkActivos").prop('checked') && !$("#chkInactivos").prop('checked')) {
                dataTableCajeroVolante.column(4).search('A').draw();
            }
            else {
                dataTableCajeroVolante.column(4).search('').draw();
            }

        });
    }

    const visualizarDataTableCajero = function () {

        $.ajax({
            url: urlListarCajeros,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                        className: "pointer",
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableCajero != null) {
                    dataTableCajero.clear();
                    dataTableCajero.destroy();
                    dataTableCajero = null;
                }

                var tableId = "#tableCajero";
                $(tableId + " tbody").empty();
                $(tableId + " thead").empty();



                dataTableCajero = $('#tableCajero').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: true,
                    scrollY: '320px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
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

    const visualizarDataTableLocal = function () {

        $.ajax({
            url: urlListarLocales,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableLocal != null) {
                    dataTableLocal.clear();
                    dataTableLocal.destroy();
                    dataTableLocal = null;
                }

                var tableId = "#tableLocal";
                $(tableId + " tbody").empty();
                $(tableId + " thead").empty();



                dataTableLocal = $('#tableLocal').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: false,
                    scrollY: '320px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
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

    const visualizarDataTableCajeroVolante = function () {

        $.ajax({
            url: urlListarCajerosAsginados,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                        className: "pointer",
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableCajeroVolante != null) {
                    dataTableCajeroVolante.clear();
                    dataTableCajeroVolante.destroy();
                    dataTableCajeroVolante = null;
                }

                var tableId = "#tableCajeroVolante";
                $(tableId + " tbody").empty();
                $(tableId + " thead").empty();



                dataTableCajeroVolante = $('#tableCajeroVolante').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: true,
                    scrollY: '660px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
                    bAutoWidth: false,
                    rowCallback: function (row, data, index) {
                        if (data.ACT == "N") {
                            $("td", row).addClass("text-danger");
                        }
                    },
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

    const asignar = function () {
        const registrosSeleccionados = dataTableCajero.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let cajeros = [];

        registrosSeleccionados.map((item) => {
            cajeros.push({
                CodOfisis: item.Codigo,
                NumDocumento: item.NroDocu,
                CodSedeOrigen: item.C_LOCAL
            });
        });

        $.ajax({
            url: urlCrearCajeroVolante,
            type: "post",
            data: { request: cajeros },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                visualizarDataTableCajero();
                visualizarDataTableLocal();
                visualizarDataTableCajeroVolante();


                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }
               
                swal({
                    text: "Registros creados exitosamente.",
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

    const eliminar = function () {
        const registrosSeleccionados = dataTableCajeroVolante.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let cajeros = [];

        registrosSeleccionados.map((item) => {
            cajeros.push({
                CodOfisis: item.CODIGO,
                CodSede: item.DESOFIPLAN
            });
        });

        $.ajax({
            url: urlEliminarCajeroVolante,
            type: "post",
            data: { request: cajeros },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                visualizarDataTableCajero();
                visualizarDataTableLocal();
                visualizarDataTableCajeroVolante();

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                swal({
                    text: "Registros eliminados exitosamente.",
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


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                visualizarDataTableCajero();
                visualizarDataTableLocal();
                visualizarDataTableCajeroVolante();
            });
        }
    }

}(jQuery);