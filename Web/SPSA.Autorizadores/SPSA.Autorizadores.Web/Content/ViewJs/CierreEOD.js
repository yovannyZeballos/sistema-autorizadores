var urlListado = baseUrl + 'Monitor/CierreEOD/ListarMonitor';
var urlProcesar = baseUrl + 'Monitor/CierreEOD/Procesar';
var urlEmpresas = baseUrl + 'Monitor/CierreEOD/ListarEmpresas';
var dataTableMonitor = null;

var CierreEOD = function () {

    const eventos = function () {
        $("#btnConsultar").on('click', function () {
            cargarMonitor();
        });

        $("#btnProcesar").on('click', function () {
            procesar();
        });
    }

    const visualizarDataTableMonitor = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            Fecha: $("#txtFecha").val(),
            Estado: $("#cboEstado").val()
        }

        $.ajax({
            url: urlListado,
            type: "post",
            dataType: "json",
            data: { request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                var columnas = [];
                response.Columnas.forEach((x) => {
                    if (x === "ESTADO") {
                        columnas.push({
                            title: x,
                            data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
                            className: "estado",
                            defaultContent: ""
                        });

                    } else if (x === "TIP_ESTADO") {
                        columnas.push({
                            title: x,
                            data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
                            defaultContent: "",
                            "visible": false
                        });
                    } else {
                        columnas.push({
                            title: x,
                            data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
                            defaultContent: "",
                        });
                    }

                });

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
                    data: response.Locales,
                    bAutoWidth: false,
                    rowCallback: function (row, data, index) {
                        $("td.estado", row).addClass("text-white");
                        if (data.TIP_ESTADO == "1") {
                            $("td.estado", row).addClass("bg-primary");
                        } else if (data.TIP_ESTADO == "2") {
                            $("td.estado", row).addClass("bg-warning");
                        } else if (data.TIP_ESTADO == "3") {
                            $("td.estado", row).addClass("bg-danger");
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
                                const fecha = $("#txtFecha").val().replace('/','');
                                return `Cierre_EOD_${fecha}`;
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
                //dataTableMonitor.buttons().container().prependTo($('#tableMonitor_filter'));
               
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //closeLoading();
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    const procesar = function () {

        const codEmpresa = $("#cboEmpresa").val();
        const fecha = $("#txtFecha").val();

        if (codEmpresa === null || fecha === "") {
            swal({
                text: "Seleccione la Empresa e ingrese la fecha.",
                icon: "warning"
            });
            return;
        }


        const request = {
            CodEmpresa: codEmpresa,
            Fecha: fecha
        }

        $.ajax({
            url: urlProcesar,
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
                        icon: "error"
                    });
                    return;
                }

                swal({
                    text: response.Mensaje,
                    icon: "success"
                }).then(function () {
                    cargarMonitor();
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

    const cargarMonitor = function () {

        const codEmpresa = $("#cboEmpresa").val();
        const fecha = $("#txtFecha").val();

        if (codEmpresa === null || fecha === "") {
            swal({
                text: "Seleccione la Empresa e ingrese la fecha.",
                icon: "warning"
            });
            return;
        }

        const request = {
            CodEmpresa: codEmpresa,
            Fecha: fecha,
            Estado: $("#cboEstado").val()
        }

        $.ajax({
            url: urlListado,
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

                    dataTableMonitor.clear();
                    dataTableMonitor.draw();
                    return;

                }

                dataTableMonitor.clear();
                dataTableMonitor.rows.add(response.Locales);
                dataTableMonitor.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //closeLoading();
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const listarEmpresas = function () {
        $.ajax({
            url: urlEmpresas,
            type: "get",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (response.Ok === true) {
                    cargarEmpresas(response.Empresas);
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
            $('#cboEmpresa').append($('<option>', { value: empresa.Codigo, text: empresa.Descripcion }));
        });
        $('#cboEmpresa').val(rucSession);


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
                listarEmpresas();
                visualizarDataTableMonitor();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
