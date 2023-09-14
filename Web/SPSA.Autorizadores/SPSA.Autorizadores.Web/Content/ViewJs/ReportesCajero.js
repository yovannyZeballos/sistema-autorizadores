const urlListarEmpresa = '/Empresa/Listar';
const urlListarLocal = '/Local/Listar';
const urlReporteSobre = '/Cajeros/Reportes/ReporteSobres';
const urlReporteDiferencia = '/Cajeros/Reportes/ReporteDiferencias';

var dataTableReporte = null;

const ReportesCajero = function () {

    var eventos = function () {

        $("#btnReporteSobres").on('click', function () {
            if (!validar()) return;
            recargarDataTableReporte(1);
        });

        $("#btnReporteDiferencia").on('click', function () {
            if (!validar()) return;
            recargarDataTableReporte(2);
        });

        $("#cboEmpresa").on("change", function () {
            debugger;
            listarLocales();
        });
    }

    const validar = function () {
        let mensajes = '';
        let ok = true;
        const codEmpresa = $("#cboEmpresa").val();
        const fechaInicio = $("#txtFechaInicio").val();
        const fechaFin = $("#txtFechaFin").val();

        if (codEmpresa === null || codEmpresa === '') {
            mensajes = mensajes + 'Debe seleccionar la empresa \n';
            ok = false;
        }

        if (fechaInicio === '') {
            mensajes = mensajes + 'Debe ingresar la fecha de inicio \n';
            ok = false;
        }

        if (fechaFin === '') {
            mensajes = mensajes + 'Debe ingresar la fecha de fin \n';
            ok = false;
        }

        if (!ok)
            swal({
                text: mensajes,
                icon: "warning"
            });

        return ok;
    }

    const inicializarDataTableReporte = function (tipoReporte) { // 1= sobres, 2= diferencias
        const url = tipoReporte == 1 ? urlReporteSobre : urlReporteDiferencia;


        const request = {
            CodigoEmpresa: $("#cboEmpresa").val(),
            CodigoLocal: $("#cboLocal").val(),
            FechaInicio: $("#txtFechaInicio").val(),
            FechaFin: $("#txtFechaFin").val()
        };

        $.ajax({
            url: url,
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
                        data: quitarTildes(x).replace(/ /g, "").replace(".", "")
                    });

                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                dataTableReporte = $('#tableReportes').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    bAutoWidth: false,
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
                    ],
                });

                dataTableReporte.buttons().container().prependTo($('#tableReportes_filter'));

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const recargarDataTableReporte = function (tipoReporte) {
        const url = tipoReporte == 1 ? urlReporteSobre : urlReporteDiferencia;

        const request = {
            CodigoEmpresa: $("#cboEmpresa").val(),
            CodigoLocal: $("#cboLocal").val(),
            FechaInicio: $("#txtFechaInicio").val(),
            FechaFin: $("#txtFechaFin").val()
        };

        $.ajax({
            url: url,
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
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });

                    dataTableReporte.clear();
                    dataTableReporte.draw();
                    return;

                }
                dataTableReporte.clear();
                dataTableReporte.rows.add(response.Data);
                dataTableReporte.draw();
                dataTableReporte.columns.adjust().draw();

                setTimeout(() => {
                    dataTableReporte.columns.adjust().draw();
                }, 500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error", });
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

    const listarLocales = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val()
        };

        $.ajax({
            url: urlListarLocal,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                    return;
                }

                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                  $('#cboLocal').append($('<option>', { value: '0', text: 'TODOS' }));
                response.Locales.map(local => {
                    $('#cboLocal').append($('<option>', { value: local.CodLocal, text: local.CodLocal + ' - ' + local.NomLocal }));
                });

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarEmpresas = function (empresas) {
        $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
        empresas.map(empresa => {
            $('#cboEmpresa').append($('<option>', { value: empresa.Codigo, text: empresa.Descripcion }));
        });
    }

    const inicializarDatePicker = function () {
        $('.fc-datepicker').datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            closeText: 'Cerrar',
            prevText: '<Ant',
            nextText: 'Sig>',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'dd/mm/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: '',
            changeMonth: true,
            changeYear: true
        });
    }


    const fechaActual = function () {
        let date = new Date()

        let day = `${(date.getDate())}`.padStart(2, '0');
        let month = `${(date.getMonth() + 1)}`.padStart(2, '0');
        let year = date.getFullYear();

        $("#txtFechaInicio").val(`${day}/${month}/${year}`);
        $("#txtFechaFin").val(`${day}/${month}/${year}`);
    }


    return {
        init: function (tipoReporte) {
            checkSession(async function () {
                eventos();
                inicializarDatePicker();
                fechaActual();
                inicializarDataTableReporte(tipoReporte);
                listarEmpresas();

            });
        }
    }

}(jQuery);