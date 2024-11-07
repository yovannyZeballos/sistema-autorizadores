var urlProcesar = baseUrl + 'Monitor/ControlBCT/Procesar';
var idInterval = 0;
var timeoutInterval = 60000;
var dataTableListado = null;
var empresa = '';
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

        let url = baseUrl;  

        switch (empresa) {
            case 'SPSA':
                url += 'Monitor/ControlBCT/ProcesarSpsa';
                $(".titulo").text('Control BCT - SPSA');
                break;
            case 'TPSA':
                url += 'Monitor/ControlBCT/ProcesarTpsa';
                $(".titulo").text('Control BCT - TPSA');
                break;
            case 'HPSA':
                url += 'Monitor/ControlBCT/ProcesarHpsa';
                $(".titulo").text('Control BCT - HPSA');
                break;
        }

        dataTableListado = $('#tableMonitor').DataTable({
            ajax: {
                url: url,
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

    const getQueryParams = function () {
        var params = {};
        var queryString = window.location.search.substring(1);
        var regex = /([^&=]+)=([^&]*)/g;
        var match;

        while (match = regex.exec(queryString)) {
            params[decodeURIComponent(match[1])] = decodeURIComponent(match[2]);
        }

        empresa = params.emp;
        return params;
    }

    return {
        init: function () {
            checkSession(function () {
                getQueryParams();
                eventos();
                inicializarDatePicker();
                fechaActual();
                cargarDatos();
            });
        }
    }
}(jQuery)
