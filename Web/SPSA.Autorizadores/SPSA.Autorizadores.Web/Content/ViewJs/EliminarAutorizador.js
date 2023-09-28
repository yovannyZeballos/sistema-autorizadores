var urlColaboradorCesados = baseUrl + 'Autorizadores/EliminarAutorizador/ListarColaboradoresCesados';
var urlEliminar = baseUrl + 'Autorizadores/EliminarAutorizador/EliminarAutorizador';
var urlListarListBox = baseUrl + 'Autorizadores/EliminarAutorizador/ListarListBox';
var urlListarGrilla = baseUrl + 'Autorizadores/EliminarAutorizador/ListarGrilla';
const urlListarEmpresa = '/Empresa/Listar';
const urlListarLocal = '/Local/Listar';

var EliminarAutorizador = function () {

    var eventos = function () {
        $("#btnEliminar").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar a los autorizadores?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        eliminarAutorizador();
                    }
                });
        });

        $("#btnConsultar").on('click', function () {
            visualizarDataTable();
        });


        $('#tableColaboradorCesado tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });


        $("#cboEmpresa").on("change", function () {
            listarLocales();
        });
    }


    var visualizarDataTable = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodLocal: $("#cboLocal").val(),
            FechaInicio: $("#txtFechaInicio").val(),
            FechaFin: $("#txtFechaFin").val(),
            Opcion: $("#cboOpcion").val()
        };

        $.ajax({
            url: urlListarGrilla,
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
                        title: x,
                        data: x.replace(" ", "").replace(".", ""),
                        defaultContent: "",
                    });
                });

                if (dataTableColaboradorCesados != null) {
                    dataTableColaboradorCesados.clear();
                    dataTableColaboradorCesados.destroy();
                    dataTableColaboradorCesados = null;
                }

                var tableColaboradorCesadoId = "#tableColaboradorCesado";
                $(tableColaboradorCesadoId + " tbody").empty();
                $(tableColaboradorCesadoId + " thead").empty();

                if (columnas.length == 0) return;

                dataTableColaboradorCesados = $(tableColaboradorCesadoId).DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    "data": response.Data,
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

                dataTableColaboradorCesados.buttons().container().prependTo($('#tableColaboradorCesado_filter'));
                $('input[type="search"]').addClass("form-control-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });


    };

    const eliminarAutorizador = function () {

        const registrosSeleccionados = dataTableColaboradorCesados.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        showLoading();

        let autorizadores = registrosSeleccionados.map(x => {
            return {
                Codigo: x.Codigo,
                CodigoAutorizador: x.Autorizador
            }
        });


        $.ajax({
            url: urlEliminar,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            success: function (response) {

                cargarAutorizadores();

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

                btnLoading($("#btnEliminar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                btnLoading($("#btnEliminar"), false);

                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
                closeLoading();
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

    const cargarAutorizadores = function () {
        $.ajax({
            url: urlColaboradorCesados,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableColaboradorCesados.clear();
                dataTableColaboradorCesados.rows.add(response.Colaboradores);
                dataTableColaboradorCesados.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
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

    const listarEmpresas = function () {
        $.ajax({
            url: urlListarEmpresa,
            type: "post",
            success: function (response) {
                debugger;
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

    const listarListBox = function () {
        $.ajax({
            url: urlListarListBox,
            type: "post",
            success: function (response) {
                debugger;
                if (!response.Ok)
                    swal({ text: response.Mensaje, icon: "error" });

                $('#cboOpcion').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(data => {
                    $('#cboOpcion').append($('<option>', { value: data.Opcion, text: data.Nombre }));
                });
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
                //visualizarDataTableColaboradorCesado();
                $('input[type="search"]').addClass("form-control-sm");
                inicializarDatePicker();
                fechaActual();
                listarEmpresas();
                listarListBox();
            });
        }
    }
}(jQuery)
