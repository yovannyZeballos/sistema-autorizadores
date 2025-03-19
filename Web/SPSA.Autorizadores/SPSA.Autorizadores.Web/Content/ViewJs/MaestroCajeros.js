var urlListarCajeros = baseUrl + 'Reportes/MaestroCajeros/Descargar';
var urlListarCajerosPaginado = baseUrl + 'Reportes/MaestroCajeros/ListarPaginado';

var dataTableRegistros = null;

var MaestroCajeros = function () {

    var eventos = function () {

        $("#btnConsultar").on('click', function () {
            visualizarDataTable();
        });

        $("#btnDescargar").on("click", function () {
            descargarExcel();
        });

        $('#tableReportes tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableRegistros.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    };

    var visualizarDataTable = function () {
        const request = {
        };

        var dataTableRegistrosId = "#tableReportes";

        if ($.fn.DataTable.isDataTable(dataTableRegistrosId)) {
            $(dataTableRegistrosId).DataTable().clear().destroy();
            $(dataTableRegistrosId + " tbody").empty();
            $(dataTableRegistrosId + " thead").empty();
        }

        showLoading();

        $.ajax({
            url: urlListarCajerosPaginado,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                draw: 1,
                startRow: 1,
                pageSize: 50
            }),
            success: function (response) {
                closeLoading();

                if (!response.Ok) {
                    swal({
                        text: "Ocurrió un error: " + response.Mensaje,
                        icon: "error",
                    });
                    return;
                }

                const columnas = [];
                const totalColumnas = response.Columnas.length;
                if (response.Columnas && response.Columnas.length > 0) {
                    response.Columnas.forEach((col, index) => {
                        columnas.push({
                            title: col,
                            data: col.replace(" ", "").replace(".", ""),
                            defaultContent: "",
                            orderable: false,
                            visible: index < totalColumnas - 2
                        });
                    });
                }

                dataTableRegistros = $(dataTableRegistrosId).DataTable({
                    pageLength: 50,
                    searching: false,
                    data: response.Data,
                    columns: columnas,
                    serverSide: true,
                    processing: true,
                    paging: true,
                    scrollY: "400px",
                    scrollX: true,
                    ajax: {
                        url: urlListarCajerosPaginado,
                        type: "POST",
                        contentType: "application/json",
                        data: function (d) {
                            return JSON.stringify({
                                Draw: d.draw,
                                StartRow: d.start + 1,
                                PageSize: d.length,
                            });
                        },
                        dataSrc: function (json) {
                            if (!json || !json.Columnas || !json.Data) {
                                swal({
                                    text: "La respuesta del servidor no contiene las propiedades esperadas.",
                                    icon: "error",
                                });
                                return [];
                            }
                            return json.Data;
                        }
                    },
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                        lengthMenu: "Mostrar _MENU_ registros por página",
                        zeroRecords: "No se encontraron resultados",
                        info: "Mostrando página _PAGE_ de _PAGES_",
                        infoEmpty: "No hay registros disponibles",
                        infoFiltered: "(filtrado de _MAX_ registros totales)"
                    },
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                closeLoading();
                swal({
                    text: "Error de conexión: " + textStatus,
                    icon: "error",
                });
            }
        });
    };

    function descargarExcel() {

        showLoading();

        fetch(urlListarCajeros, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "X-CSRF-TOKEN": $('input[name="__RequestVerificationToken"]').val() || ""
            },

            body: JSON.stringify({
                Parametro1: "Valor1"
            })
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(err => {
                        throw new Error(err.mensaje || "Error desconocido en la descarga");
                    });
                }
                return response.blob(); // Convertir la respuesta a Blob
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url;
                a.download = "RpteMaestroCajeros.xlsx";
                document.body.appendChild(a);
                a.click();
                a.remove();

                window.URL.revokeObjectURL(url);
            })
            .catch(error => {
                swal({
                    text: "Ocurrió un error: " + error.message,
                    icon: "error",
                });
            })
            .finally(() => {
                closeLoading();
            });
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                visualizarDataTable();
            });
        }
    }

}(jQuery);
