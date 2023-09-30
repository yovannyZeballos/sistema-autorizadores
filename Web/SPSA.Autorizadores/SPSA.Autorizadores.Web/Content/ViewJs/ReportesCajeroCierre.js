const urlListarEmpresa = '/Empresa/ListarOfiplan';
const urlReporteCierre = '/Locales/ReporteCierre/ReporteCierre';
const urlReporteCierrePivot = '/Locales/ReporteCierre/ReportePivotCierre';

var dataTableReporte = null;
var listadoReporte = [];

const ReportesCajeroCierre = function () {

    var eventos = function () {

        $("#btnConsultar").on('click', function () {
            if (!validar()) return;
            visualizarDataTableReporte();
            listarReporte();
        });

        $('#tableReportes').on('click', 'td', function () {
            var row = dataTableReporte.row(this).data();
            var column = dataTableReporte.column(this).header().innerText;

            let dato = [];
            dato = listadoReporte.filter(item => item.LOC_DESCRIPCION == row.Local && item.CIE_FCONTABLE_FORMAT == column);

            if (dato.length > 0) {

                var div = document.createElement("div");
                div.innerHTML = '<h1 class="text-success"><i class="feather feather-alert-circle"></i></h1>';
                div.innerHTML += `<p>Local: <b>${dato[0].LOC_DESCRIPCION}</b></p>`
                div.innerHTML += `<p>Venta del: <b>${dato[0].CIE_FCONTABLE}</b></p>`
                div.innerHTML += `<p>Cerrado el: <b>${dato[0].CIE_FCIERRE}</b></p>`
                swal({
                    content: div,
                    //text: dato[0].LOC_DESCRIPCION + '\n' + dato[0].CIE_FCONTABLE,
                });
            }

        })
    }

    const validar = function () {
        let mensajes = '';
        let ok = true;
        const codEmpresa = $("#cboEmpresa").val();
        const año = $("#cboAño").val();
        const mes = $("#cboMes").val();

        if (codEmpresa === null || codEmpresa === '') {
            mensajes = mensajes + 'Debe seleccionar la empresa \n';
            ok = false;
        }

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

    const visualizarDataTableReporte = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val()
        };

        $.ajax({
            url: urlReporteCierrePivot,
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
                    if (x != "Local")
                        columnas.push({
                            title: x.replace("_", " "),
                            data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                            className: "estado",
                        });
                    else
                        columnas.push({
                            title: x.replace("_", " "),
                            data: quitarTildes(x).replace(/ /g, "").replace(".", "")
                        });

                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableReporte != null) {
                    dataTableReporte.clear();
                    dataTableReporte.destroy();
                    dataTableReporte = null;
                }

                var tableReportesId = "#tableReportes";
                $(tableReportesId + " tbody").empty();
                $(tableReportesId + " thead").empty();

                

                dataTableReporte = $('#tableReportes').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
                    bAutoWidth: false,
                    rowCallback: function (row, data, i) {
                        console.log(data);
                        var keys = Object.keys(data);
                        console.log(keys);

                        keys.sort().forEach((k, index) => {

                            console.log(data[k]);
                            console.log(index);

                            if (data[k] === "A") {
                                $(`td:eq(${index+1})`, row).addClass("bg-danger");
                            }
                            else if (data[k] === "P") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-warning");
                            }
                            else if (data[k] === "C") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-success");
                            }
                        });
                    },
                    buttons: [
                        {
                            extend: 'excel',
                            text: '<i class="fa fa-cloud-download"></i> Excel',
                            titleAttr: 'Descargar Excel',
                            className: 'btn-sm mb-1 ms-2',
                            exportOptions: {
                                modifier: { page: 'all' }
                            }
                        },
                    ],
                });

                dataTableReporte.buttons().container().prependTo($('#tableReportes_filter'));
                $('input[type="search"]').addClass("form-control-sm");
                $('#tableReportes_filter button').addClass("btn-sm");
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
                listarEmpresas();
              
            });
        }
    }

}(jQuery);