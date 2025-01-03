const urlListarEmpresa = '/Empresa/ListarOfiplan';
const urlReporteCierre = '/Monitor/ReporteCierre/ReporteCierreResumen';

var dataTableReporte_1 = null;
var dataTableReporte_2 = null;
var dataTableReporte_3 = null;

const ReportesCajeroCierreResumen = function () {

    var eventos = function () {

        $("#btnConsultar").on('click', function () {
            if (!validar()) return;
            visualizarDataTableReporte_1();
            visualizarDataTableReporte_2("");
            visualizarDataTableReporte_3("");
        });

        $('#tableReportes_1').on('click', 'td', function () {
            var row = dataTableReporte_1.row(this).data();
            visualizarDataTableReporte_2(row.SOC);
            visualizarDataTableReporte_3(row.SOC);
        });

        $('#tableReportes_1 tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableReporte_1.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $("#downloadExcel").on('click', function () {
            const table = document.getElementById('tableReportes_3');

            const rows = Array.from(table.rows); // Obtener filas de la tabla

            // Crear un workbook y una hoja manualmente
            const workbook = XLSX.utils.book_new();
            const sheetData = rows.map(row =>
                Array.from(row.cells).map(cell => {
                    const value = cell.innerText.trim(); // Obtener el texto de la celda

                    // Verificar si es una fecha en formato dd/mm/yyyy
                    if (value.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
                        return value; // Mantener como texto
                    }

                    // Convertir otros valores según sea necesario
                    return value;
                })
            );

            // Crear la hoja de cálculo a partir de los datos procesados
            const worksheet = XLSX.utils.aoa_to_sheet(sheetData);
            XLSX.utils.book_append_sheet(workbook, worksheet, "Hoja1");

            XLSX.writeFile(workbook, "RpteCierreResumen.xlsx");
        });
    }

    const validar = function () {
        let mensajes = '';
        let ok = true;
        const año = $("#cboAño").val();
        const mes = $("#cboMes").val();

        if (año === '' || año === null) {
            mensajes = mensajes + 'Debe ingresar el Año \n';
            ok = false;
        }

        if (mes === '' || mes === null) {
            mensajes = mensajes + 'Debe ingresar el Mes \n';
            ok = false;
        }

        if (!ok)
            swal({
                text: mensajes,
                icon: "warning"
            });

        return ok;
    }

    const visualizarDataTableReporte_1 = function () {

        const request = {
            CodSociedad: "",
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val(),
            Opcion: "1"
        };

        $.ajax({
            url: urlReporteCierre,
            type: "post",
            data: { request },
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

                if (dataTableReporte_1 != null) {
                    dataTableReporte_1.clear();
                    dataTableReporte_1.destroy();
                    dataTableReporte_1 = null;
                }

                var tableReportesId = "#tableReportes_1";
                $(tableReportesId + " tbody").empty();
                $(tableReportesId + " thead").empty();



                dataTableReporte_1 = $('#tableReportes_1').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: false,
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
                    bAutoWidth: false,
                    rowCallback: function (row, data, i) {
                        var keys = Object.keys(data);
                        keys.sort().forEach((k, index) => {

                            if (data[k] === "A") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-danger");
                            }
                            else if (data[k] === "P") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-warning");
                            }
                            else if (data[k] === "C") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-success");
                            }
                        });
                    },
                    //buttons: [
                    //    {
                    //        extend: 'excel',
                    //        text: '<i class="fa fa-cloud-download"></i> Excel',
                    //        titleAttr: 'Descargar Excel',
                    //        className: 'btn-sm mb-1 ms-2',
                    //        exportOptions: {
                    //            modifier: { page: 'all' }
                    //        }
                    //    },
                    //],
                });

                //dataTableReporte_1.buttons().container().prependTo($('#tableReportes_1_filter'));
                //$('input[type="search"]').addClass("form-control-sm");
                //$('#tableReportes_1_filter button').addClass("btn-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const visualizarDataTableReporte_2 = function (codSociedad) {

        const request = {
            CodSociedad: codSociedad,
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val(),
            Opcion: "2"
        };

        $.ajax({
            url: urlReporteCierre,
            type: "post",
            data: { request },
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
                        //className: "pointer",
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableReporte_2 != null) {
                    dataTableReporte_2.clear();
                    dataTableReporte_2.destroy();
                    dataTableReporte_2 = null;
                }

                var tableReportesId = "#tableReportes_2";
                $(tableReportesId + " tbody").empty();
                $(tableReportesId + " thead").empty();



                dataTableReporte_2 = $('#tableReportes_2').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: false,
                    scrollY: '400px',
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

    const visualizarDataTableReporte_3 = function (codSociedad) {

        const request = {
            CodSociedad: codSociedad,
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val(),
            Opcion: "3"
        };

        $.ajax({
            url: urlReporteCierre,
            type: "post",
            data: { request },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                // Mostrar el botón de descarga cuando se carguen los datos
                document.getElementById('downloadExcel').classList.remove('d-none');

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                        //className: "pointer",
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableReporte_3 != null) {
                    dataTableReporte_3.clear();
                    dataTableReporte_3.destroy();
                    dataTableReporte_3 = null;
                }

                var tableReportesId = "#tableReportes_3";
                $(tableReportesId + " tbody").empty();
                $(tableReportesId + " thead").empty();



                dataTableReporte_3 = $('#tableReportes_3').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: false,
                    scrollY: '400px',
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

    const listarEmpresas = function () {
        $.ajax({
            url: urlListarEmpresa,
            type: "post",
            success: function (response) {

                if (response.Ok === true) {
                    cargarEmpresas(response.Empresas);
                } else {
                    notif({
                        type: "error",
                        msg: response.Mensaje,
                        height: 100,
                        position: "right"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notif({
                    type: "error",
                    msg: jqXHR.responseText,
                    height: 100,
                    position: "right"
                });
            }
        });
    }

    const cargarEmpresas = function (empresas) {
        $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
        empresas.map(empresa => {
            $('#cboEmpresa').append($('<option>', { value: empresa.Sociedad, text: empresa.Descripcion }));
        });
    }

    const cargarAños = function () {

        const date = new Date()
        const añoActual = date.getFullYear();
        const añoInical = 2018;
        const añoFinal = añoActual + 5;

        $('#cboAño').empty().append('<option label="Seleccionar"></option>');

        for (var i = añoInical; i <= añoFinal; i++) {
            $('#cboAño').append($('<option>', { value: i, text: i }));
        }

    }

    const cargarMeses = function () {

        $('#cboMes').empty().append('<option label="Seleccionar"></option>');
        $('#cboMes').append($('<option>', { value: 1, text: 'Enero' }));
        $('#cboMes').append($('<option>', { value: 2, text: 'Febrero' }));
        $('#cboMes').append($('<option>', { value: 3, text: 'Marzo' }));
        $('#cboMes').append($('<option>', { value: 4, text: 'Abril' }));
        $('#cboMes').append($('<option>', { value: 5, text: 'Mayo' }));
        $('#cboMes').append($('<option>', { value: 6, text: 'Junio' }));
        $('#cboMes').append($('<option>', { value: 7, text: 'Julio' }));
        $('#cboMes').append($('<option>', { value: 8, text: 'Agosto' }));
        $('#cboMes').append($('<option>', { value: 9, text: 'Setiembre' }));
        $('#cboMes').append($('<option>', { value: 10, text: 'Octubre' }));
        $('#cboMes').append($('<option>', { value: 11, text: 'Noviembre' }));
        $('#cboMes').append($('<option>', { value: 12, text: 'Diciembre' }));

    }


    const fechaActual = function () {
        let date = new Date()

        let month = `${(date.getMonth() + 1)}`;
        let year = date.getFullYear();

        $("#cboAño").val(year);
        $("#cboMes").val(month);
    }

    const listarReporte = function () {
        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val()
        };

        $.ajax({
            url: urlReporteCierre,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {
                listadoReporte = response.Data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error", });
            }
        });
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                cargarAños();
                cargarMeses();
                fechaActual();
            });
        }
    }

}(jQuery);