var urlListado = baseUrl + 'Monitor/BCT/Listar';
var urlProcesar = baseUrl + 'Monitor/BCT/Procesar';
var urlParametros = baseUrl + 'Monitor/BCT/Parametros';
var dataTableListado = null;
var dataTableCantidad = null;
var RegistroTotal = {} ;
var RegistroPorSegundo = [];
var Parametros = {};
var idInterval = 0;

var BCT = function () {

    const eventos = function () {

        $("#btnRefrescar").on("click", function () {
            clearInterval(idInterval);
            cargarParametros();
            cargarDatos();
            idInterval = setInterval(cargarDatos, 5000);
        });
    }

    const cargarListadoSegundos = function (listado = []) {
        if (listado.length > 0) {
            let fechaInicalStr = listado[0].Fecha;
            let fechaFinalStr = listado[listado.length - 1].Fecha;

            let fechaInical = new Date(+(fechaInicalStr.replace(/\D/g, '')));
            let fechaFinal = new Date(+(fechaFinalStr.replace(/\D/g, '')));

            let dif = fechaFinal.getTime() - fechaInical.getTime()
            let diferenciaSegundos = Math.abs(dif / 1000);

            $("#txtDiferenciaSegundos").val(diferenciaSegundos);
        }


        let columnas = [
            {
                title: 'Fecha',
                data: 'FechaFormato'
            },
            {
                title: 'Registros',
                data: 'Cantidad'
            }
        ];

        if (dataTableListado != null) {
            dataTableListado.clear();
            dataTableListado.destroy();
            dataTableListado = null;
        }

        var tableId = "#tableListado";
        $(tableId + " tbody").empty();
        $(tableId + " thead").empty();

        dataTableListado = $(tableId).DataTable({
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            searching: false,
            scrollY: '480px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            columns: columnas,
            data: listado,
            bAutoWidth: false
        });
    }

    const cargarListadoCantidad = function (registro = {}) {

        $("#txtFechaRegistro").val(registro.FechaFormato);
        $("#txtCantidad").val(registro.Cantidad);
        //let columnas = [
        //    {
        //        title: 'Fecha',
        //        data: 'FechaFormato'
        //    },
        //    {
        //        title: 'Registros',
        //        data: 'Cantidad'
        //    }
        //];

        //if (dataTableCantidad != null) {
        //    dataTableCantidad.clear();
        //    dataTableCantidad.destroy();
        //    dataTableCantidad = null;
        //}

        //var tableId = "#tableCantidad";
        //$(tableId + " tbody").empty();
        //$(tableId + " thead").empty();

        //dataTableCantidad = $(tableId).DataTable({
        //    language: {
        //        searchPlaceholder: 'Buscar...',
        //        sSearch: '',
        //    },
        //    searching: false,
        //    scrollY: '480px',
        //    scrollX: true,
        //    scrollCollapse: true,
        //    paging: false,
        //    columns: columnas,
        //    data: listado,
        //    bAutoWidth: false
        //});
    }

    const cargarDatos = function () {

        $.ajax({
            url: urlListado,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                RegistroTotal = response.Data.Item1;
                RegistroPorSegundo = response.Data.Item2.map(item => {
                    return {
                        FechaStr: convertirfecha(new Date(+(item.Fecha.replace(/\D/g, '')))),
                        Cantidad: item.Cantidad
                    };
                });

                cargarListadoSegundos(response.Data.Item2);
                cargarListadoCantidad(response.Data.Item1);

                procesar();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    const cargarParametros = function () {

        $.ajax({
            url: urlParametros,
            type: "post",
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                Parametros = response;

                $("#txtToleranciaSegundos").val(response.ToleranciaSegundos);
                $("#txtTolerancia").val(response.ToleranciaCantidad);

            },
            error: function (jqXHR, textStatus, errorThrown) {
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
            RegistroTotal: RegistroTotal,
            RegistroPorSegundo: RegistroPorSegundo,
            Parametros: Parametros,
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

                if (response.Data.Item1 === true) {
                    $("#btnResultadoCantidad").addClass("btn-success");
                    $("#btnResultadoCantidad").removeClass("btn-danger");
                } else {
                    $("#btnResultadoCantidad").removeClass("btn-success");
                    $("#btnResultadoCantidad").addClass("btn-danger");
                }


                if (response.Data.Item2 === true) {
                    $("#btnResultadoSegundos").addClass("btn-success");
                    $("#btnResultadoSegundos").removeClass("btn-danger");
                } else {
                    $("#btnResultadoSegundos").removeClass("btn-success");
                    $("#btnResultadoSegundos").addClass("btn-danger");
                }
                


            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const convertirfecha = function (fecha) {
        var day = fecha.getDate();       // yields date
        var month = fecha.getMonth() + 1;    // yields month (add one as '.getMonth()' is zero indexed)
        var year = fecha.getFullYear();  // yields year
        var hour = fecha.getHours();     // yields hours 
        var minute = fecha.getMinutes(); // yields minutes
        var second = fecha.getSeconds(); // yields seconds

        // After this construct a string with the above results as below
        return day + "/" + month + "/" + year + " " + hour + ':' + minute + ':' + second; 
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                cargarParametros();
                cargarDatos();
                idInterval = setInterval(cargarDatos, 5000);
            });
        }
    }
}(jQuery)
