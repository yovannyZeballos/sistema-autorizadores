var urlProcesar = baseUrl + 'Monitor/DiferenciaTransacciones/Procesar';
var urlEmpresas = baseUrl + 'Empresa/ListarEmpresasPorProceso';
var urlLocales = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';
var dataTableMonitor = null;

var DiferenciaTransacciones = function () {

    const eventos = function () {

        $("#btnProcesar").on('click', function () {
            procesar();
        });

        $("#cboEmpresa").on('change', function () {
            listarLocales();
        });

    }

    const visualizarDataTableMonitor = function () {

        var columnas = [
            {
                title: 'NOM EMPRESA',
                data: 'NomEmpresa',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'COD',
                data: 'CodLocal',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'NOM LOCAL',
                data: 'NomLocal',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'IP',
                data: 'IP',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'CANT. TRX LOCAL',
                data: 'CantTransaccionesLocal',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'CANT. TRX BCT',
                data: 'CanTransaccionesBCT',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'MONTO TRX LOCAL',
                data: 'NontoTransaccionesLocal',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'MONTO TRX BCT',
                data: 'MontoTransaccionesBCT',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'DIF. TRX',
                data: 'DiferenciaCantidad',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'DIF. MONTO',
                data: 'DiferenciaMonto',
                className: "text-center",
                defaultContent: ""
            },
            {
                title: 'ESTADO',
                data: 'Estado',
                className: "estado text-center",
                defaultContent: ""
            },
            {
                title: 'COLOR ESTADO',
                data: 'ColorEstado',
                className: "text-center",
                visible: false,
                defaultContent: ""
            },
            {
                title: 'OBS',
                data: 'Observacion',
                className: "text-center",
                defaultContent: ""
            }
        ];


        dataTableMonitor = $('#tableMonitor').DataTable({
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            searching: false,
            scrollY: '300px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            columns: columnas,
            // data: response.Data,
            bAutoWidth: false,
            rowCallback: function (row, data, index) {
                $("td.estado", row).addClass("text-white");
                if (data.ColorEstado == "VERDE") {
                    $("td.estado", row).addClass("bg-ok");
                } else if (data.ColorEstado == "AMARILLO") {
                    $("td.estado", row).addClass("bg-warn");
                } else if (data.ColorEstado == "ROJO") {
                    $("td.estado", row).addClass("bg-error");
                }
            },
            buttons: [
                {
                    extend: 'excel',
                    text: 'Exportar excel',
                    titleAttr: 'Exportar Excel',
                    className: 'btn btn-primary btn-block btn-sm',
                    exportOptions: {
                        modifier: { page: 'all' }
                    },
                    filename: function () {
                        const fecha = $("#txtFecha").val().replace('/', '');
                        return `DIFERENCIA_DE_TRANSACCIONES_MONTOS_${fecha}`;
                    },
                    action: function (e, dt, node, config) {

                        if (!this.data().count()) {
                            swal({
                                text: "No hay información disponible para Exportar.",
                                icon: "warning"
                            });
                            return;
                        }
                        $.fn.dataTable.ext.buttons.excelHtml5.action.call(this, e, dt, node, config);
                    }
                },
            ],
            order: []
        });

        $("#container-btn-exportar").append(dataTableMonitor.buttons().container());
    };

    const procesar = function () {

        const codEmpresa = $("#cboEmpresa").val();
        const fecha = $("#txtFecha").val();

        if (codEmpresa === null || codEmpresa === "" || fecha === "") {
            swal({
                text: "Seleccione la Empresa e ingrese la fecha.",
                icon: "warning"
            });
            return;
        }


        const request = {
            Empresas: [codEmpresa],
            Fecha: fecha,
            CodLocal: $("#cboLocal").val()
        }

        $.ajax({
            url: urlProcesar,
            type: "post",
            data: { command: request },
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
                        icon: "error"
                    });
                    return;
                }

                swal({
                    text: response.Mensaje,
                    icon: "success"
                }).then(function () {
                    dataTableMonitor.clear();
                    dataTableMonitor.rows.add(response.Data);
                    dataTableMonitor.draw();
                });;


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


        const request = {
            CodProceso: 31,
        }

        $.ajax({
            url: urlEmpresas,
            type: "post",
            data: { query: request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (response.Ok === true) {
                    cargarEmpresas(response.Data);
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"

                });
            }
        });
    }

    const cargarEmpresas = function (empresas) {
        $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
        empresas.map(empresa => {
            $('#cboEmpresa').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
        });
        $('#cboEmpresa').val(rucSession);
    }

    const cargarLocales = function (locales) {
        $('#cboLocal').empty().append($('<option>', { value: '0', text: 'TODOS' }));
        locales.map(local => {
            $('#cboLocal').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
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

    const listarLocales = function () {


        const request = {
            CodEmpresa: $('#cboEmpresa').val()
        }

        $.ajax({
            url: urlLocales,
            type: "post",
            data: { request: request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (response.Ok === true) {
                    cargarLocales(response.Data);
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"

                });
            }
        });
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                inicializarDatePicker();
                fechaActual();
                listarEmpresas();
                visualizarDataTableMonitor();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
