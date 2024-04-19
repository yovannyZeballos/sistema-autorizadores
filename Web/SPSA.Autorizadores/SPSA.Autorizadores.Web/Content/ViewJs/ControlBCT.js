var urlProcesar = baseUrl + 'Monitor/ControlBCT/Procesar';
var idInterval = 0;
var timeoutInterval = 60000;
var dataTableListado = null;
var ControlBCT = function () {

    const eventos = function () {

        $("#chkAutomatico").on("change", function () {
            if ($("#chkAutomatico").is(":checked")) {
                idInterval = setInterval(recargarDatos, timeoutInterval);
            }
            else {
                clearInterval(idInterval);
            }
        });

        $("#btnConsultar").on("click", function () {
            recargarDatos();
        });
    }

    const recargarDatos = function () {
        dataTableListado.ajax.reload();
    }

    const cargarDatos = function () {

        dataTableListado = $('#tableMonitor').DataTable({
            ajax: {
                url: urlProcesar,
                type: "post",
                dataType: "json",
                "data": function (d) {
                    d.Fecha = $('#txtFecha').val();
                },
                dataSrc: function (response) {
                    if (!response.Ok) {
                        swal({
                            text: response.Mensaje,
                            icon: "error"
                        });
                    }
                    return response.Data;
                },
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal({
                        text: 'Error al listar ' + jqXHR.responseText,
                        icon: "error"
                    });
                }
            },
            
            columns: [
                { data: "DesSucursal" },
                { data: "Fecha", className: "text-center" },
                { data: "Horario", className: "text-center" },
                { data: "TiempoLim", className: "text-center" },
                { data: "UltimaTransf", className: "text-center" },
                {
                    data: "Diferencia",
                    className: "text-center",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        if (data === 10000)
                            return '0';
                        else
                            return data;
                    },
                },
                {
                    data: "Semaforo",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        if (data === "SI")
                            return '<i class="fa fa-circle text-success fa-3x"></i>';
                        else
                            return '<i class="fa fa-circle text-danger fa-3x"></i>';
                    },
                    className: "text-center"
                }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            rowCallback: function (row, data, index) {
                if (data.Semaforo == "NO") {
                    $("td", row).addClass("bg-warning");
                }
            },
            bAutoWidth: false,
            order: [],
            searching: false
        });

    }
    const fechaActual = function () {
        let date = new Date()

        let day = `${(date.getDate())}`.padStart(2, '0');
        let month = `${(date.getMonth() + 1)}`.padStart(2, '0');
        let year = date.getFullYear();

        $("#txtFecha").val(`${day}/${month}/${year}`);
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

    return {
        init: function () {
            checkSession(function () {
                eventos();
                inicializarDatePicker();
                fechaActual();
                cargarDatos();
            });
        }
    }
}(jQuery)
