var urlListado = baseUrl + 'Monitor/BCT/Listar';
var urlProcesar = baseUrl + 'Monitor/BCT/Procesar';
var urlParametros = baseUrl + 'Monitor/BCT/Parametros';
var urlParametrosFechaNegocio = baseUrl + 'Monitor/BCT/ParametrosFechaNegocio';
var dataTableListado = null;
var dataTableCantidad = null;
var RegistroTotal = {};
var RegistroPorSegundo = [];
var Parametros = {};
var idIntervalSpsa = 0;
var idIntervalPromart = 0;
var idIntervalPromartEcu = 0;
var idIntervalOechsle = 0;
var timeoutInterval = 5000;
var toleranciaSegundosSpa = 0;
var fechaAlertaSpsa = "";
var fechaAlertaPromart = "";
var fechaAlertaPromartEcu = "";
var fechaAlertaOechsle = "";

var BCT = function () {

    const eventos = function () {

        $("#chkEmpresaSpsa").on("change", function () {
            if (!$("#chkEmpresaSpsa").is(":checked")) {
                clearInterval(idIntervalSpsa);
                $("#lblCantidadSpsa").text("");
                $("#lblToleranciaSpsa").text("");
                $("#btnResultadoCantidadSpsa").removeClass("btn-success");
                $("#btnResultadoCantidadSpsa").removeClass("btn-danger");
                $("#btnResultadoCantidadSpsa").removeClass("btn-warning");
            }
            else {
                cargarParametros("02");
                idIntervalSpsa = setInterval(cargarDatos, timeoutInterval, "02");

            }
        });

        $("#chkEmpresaOechsle").on("change", function () {
            if (!$("#chkEmpresaOechsle").is(":checked")) {
                clearInterval(idIntervalOechsle);
                $("#lblCantidadOechsle").text("");
                $("#lblToleranciaOechsle").text("");
                $("#btnResultadoCantidadOechsle").removeClass("btn-success");
                $("#btnResultadoCantidadOechsle").removeClass("btn-danger");
                $("#btnResultadoCantidadOechsle").removeClass("btn-warning");
            }
            else {
                cargarParametros("09");
                idIntervalOechsle = setInterval(cargarDatos, timeoutInterval, "09");
            }
        });

        $("#chkEmpresaPromart").on("change", function () {
            if (!$("#chkEmpresaPromart").is(":checked")) {
                clearInterval(idIntervalPromart);
                $("#lblCantidadPromart").text("");
                $("#lblToleranciaPromart").text("");
                $("#btnResultadoCantidadPromart").removeClass("btn-success");
                $("#btnResultadoCantidadPromart").removeClass("btn-danger");
                $("#btnResultadoCantidadPromart").removeClass("btn-warning");
            }
            else {
                cargarParametros("10");
                idIntervalPromart = setInterval(cargarDatos, timeoutInterval, "10");
            }
        });

        $("#chkEmpresaPromartEcu").on("change", function () {
            if (!$("#chkEmpresaPromartEcu").is(":checked")) {
                clearInterval(idIntervalPromartEcu);
                $("#lblCantidadPromartEcu").text("");
                $("#lblToleranciaPromartEcu").text("");
                $("#btnResultadoCantidadPromartEcu").removeClass("btn-success");
                $("#btnResultadoCantidadPromartEcu").removeClass("btn-danger");
                $("#btnResultadoCantidadPromartEcu").removeClass("btn-warning");
            }
            else {
                cargarParametros("11");
                idIntervalPromartEcu = setInterval(cargarDatos, timeoutInterval, "11");
            }
        });
    }

    const cargarListadoCantidad = function (registro = {}, codEmpresa) {

        if (registro && registro != null) {
            switch (codEmpresa) {
                case "02":
                    $("#lblCantidadSpsa").text(registro.Cantidad);
                    break;
                case "09":
                    $("#lblCantidadOechsle").text(registro.Cantidad);
                    break;
                case "10":
                    $("#lblCantidadPromart").text(registro.Cantidad);
                    break;
                case "11":
                    $("#lblCantidadPromartEcu").text(registro.Cantidad);
                    break;
                default:
                    break;
            }
        }

    }

    const cargarDatos = function (codEmpresa) {


        const request = {
            CodEmpresa: codEmpresa
        }

        $.ajax({
            url: urlListado,
            type: "post",
            dataType: "json",
            data: { request },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                let cantidadAnterior = obtenerCantidadAnterior(codEmpresa);

                cargarListadoCantidad(response.Data, codEmpresa);
                procesar(codEmpresa, response.Data, cantidadAnterior);
                /////
                cargarParametrosFechaNegocio(codEmpresa)
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    const cargarParametrosFechaNegocio = function (codEmpresa = "") {
        $.ajax({
            url: urlParametrosFechaNegocio,
            type: "post",
            dataType: "json",
            success: function (response) {
        
                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (codEmpresa != "")
                    response.Data = response.Data.filter((item) => item.CodEmpresa == codEmpresa);



                response.Data.forEach((item) => {
                    switch (item.CodEmpresa) {
                        case "02":
                            $("#lblFechaNegocioSpsa").text(item.FechaNegocio);
                            $("#lblHoraNegocioSpsa").text(item.HoraNegocio);

                            ColorEtadoFechaNegocio(item.FechaNegocio, "#btnEstadoFechaNegocioSpsa");
                            ColorEtadoConexion(item.EstadoConexion, "#btnEstadoConexionSpsa");
                            
                            break;
                        case "09":
                            $("#lblFechaNegocioOechsle").text(item.FechaNegocio);
                            $("#lblHoraNegocioOechsle").text(item.HoraNegocio);

                            ColorEtadoFechaNegocio(item.FechaNegocio, "#btnEstadoFechaNegocioOechsle");
                            ColorEtadoConexion(item.EstadoConexion, "#btnEstadoConexionOechsle");

                            break;
                        case "10":
                            $("#lblFechaNegocioPromart").text(item.FechaNegocio);
                            $("#lblHoraNegocioPromart").text(item.HoraNegocio);

                            ColorEtadoFechaNegocio(item.FechaNegocio, "#btnEstadoFechaNegocioPromart");
                            ColorEtadoConexion(item.EstadoConexion, "#btnEstadoConexionPromart");

                            break;
                        case "11":
                            $("#lblFechaNegocioPromartEcu").text(item.FechaNegocio);
                            $("#lblHoraNegocioPromartEcu").text(item.HoraNegocio);

                            ColorEtadoFechaNegocio(item.FechaNegocio, "#btnEstadoFechaNegocioPromartEcu");
                            ColorEtadoConexion(item.EstadoConexion, "#btnEstadoConexionPromartEcu");

                            break;
                        default:
                    }
                });

                //if (codEmpresa != "")
                //    cargarDatos(codEmpresa);
                //else
                //    $("input:checkbox:checked").each(function () {
                //        cargarDatos($(this).val());
                //    });

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    function ColorEtadoFechaNegocio(fechaInput, idBoton) {
        const fechaAhora = new Date();
        const fechaHoy = new Date(fechaAhora.getTime() - (fechaAhora.getTimezoneOffset() * 60000) - (5 * 3600 * 1000));

        if (!fechaInput) {
            $(idBoton).addClass("fa-exclamation-circle text-danger");
            $(idBoton).removeClass("fa-check-circle text-success");
        } else {
            var fechaNegocio = new Date(fechaInput + 'T00:00:00-05:00');
            var formatFechanegocio = fechaNegocio.toISOString().slice(0, 10);
            var formatFechaHoy = fechaHoy.toISOString().slice(0, 10);

            if (formatFechanegocio === formatFechaHoy) {
                $(idBoton).addClass("fa-check-circle text-success");
                $(idBoton).removeClass("fa-exclamation-circle text-danger");
            }
            else {
                $(idBoton).addClass("fa-exclamation-circle text-danger");
                $(idBoton).removeClass("fa-check-circle text-success");
            }
        }
    }

    function ColorEtadoConexion(conexionInput, idBoton) {
        if (conexionInput === "SI") {
            $(idBoton).addClass("fa-check-circle text-success");
            $(idBoton).removeClass("fa-exclamation-circle text-danger");
        }
        else {
            $(idBoton).addClass("fa-exclamation-circle text-danger");
            $(idBoton).removeClass("fa-check-circle text-success");
        }
    }

    const obtenerCantidadAnterior = function (codEmpresa) {

        let cantidad = 0;
        switch (codEmpresa) {
            case "02":
                cantidad = $("#lblCantidadSpsa").text();
                break;
            case "09":
                cantidad = $("#lblCantidadOechsle").text();
                break;
            case "10":
                cantidad = $("#lblCantidadPromart").text();
                break;
            case "11":
                cantidad = $("#lblCantidadPromartEcu").text();
                break;
            default:
                break;
        }
        return cantidad;
    }

    const cargarParametros = function (codEmpresa = "") {


        $.ajax({
            url: urlParametros,
            type: "post",
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (codEmpresa != "")
                    response.Data = response.Data.filter((item) => item.CodEmpresa == codEmpresa);

                response.Data.forEach((item) => {
                    switch (item.CodEmpresa) {
                        case "02":
                            $("#lblToleranciaSpsa").text(item.ToleranciaCantidad);
                            break;
                        case "09":
                            $("#lblToleranciaOechsle").text(item.ToleranciaCantidad);
                            break;
                        case "10":
                            $("#lblToleranciaPromart").text(item.ToleranciaCantidad);
                            break;
                        case "11":
                            $("#lblToleranciaPromartEcu").text(item.ToleranciaCantidad);
                            break;
                        default:
                    }
                });

                if (codEmpresa != "")
                    cargarDatos(codEmpresa);
                else
                    $("input:checkbox:checked").each(function () {
                        cargarDatos($(this).val());
                    });

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    const procesar = function (codEmpresa, registroTotal, cantidadAnterior) {

        let fechaAlerta = "";

        switch (codEmpresa) {
            case "02":
                fechaAlerta = fechaAlertaSpsa;
                break;
            case "09":
                fechaAlerta = fechaAlertaOechsle;
                break;
            case "10":
                fechaAlerta = fechaAlertaPromart;
                break;
            case "11":
                fechaAlerta = fechaAlertaPromartEcu;
                break;
            default:
        }

        let colorSemaforo = obtenerColorSemaforo(codEmpresa);

        const request = {
            RegistroTotal: registroTotal,
            CodEmpresa: codEmpresa,
            FechaAlerta: fechaAlerta,
            CantidadAnterior: cantidadAnterior,
            Color: colorSemaforo
        }

        $.ajax({
            url: urlProcesar,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }


                let envioNotificacion = response.Data.Item4; // 0:NO, 1:SI
                let idBoton = "";

                switch (codEmpresa) {
                    case "02":
                        idBoton = "#btnResultadoCantidadSpsa";
                        break;
                    case "09":
                        idBoton = "#btnResultadoCantidadOechsle";
                        break;
                    case "10":
                        idBoton = "#btnResultadoCantidadPromart";
                        break;
                    case "11":
                        idBoton = "#btnResultadoCantidadPromartEcu";
                        break;
                    default:
                }

                if (response.Data.Item1 === true) { //Verde
                    $(idBoton).addClass("btn-success");
                    $(idBoton).removeClass("btn-danger");
                    $(idBoton).removeClass("btn-warning");

                    switch (codEmpresa) {
                        case "02":
                            fechaAlertaSpsa = "";
                            break;
                        case "09":
                            fechaAlertaOechsle = "";
                            break;
                        case "10":
                            fechaAlertaPromart = "";
                            break;
                        case "10":
                            fechaAlertaPromartEcu = "";
                            break;
                        default:
                    }

                } else if (response.Data.Item2 === true) { //Naranja
                    $(idBoton).addClass("btn-warning");
                    $(idBoton).removeClass("btn-danger");
                    $(idBoton).removeClass("btn-success");

                    switch (codEmpresa) {
                        case "02":
                            if (fechaAlertaSpsa == "" || envioNotificacion == 1)
                                fechaAlertaSpsa = convertirfecha(new Date());
                            break;
                        case "09":
                            if (fechaAlertaOechsle == "" || envioNotificacion == 1)
                                fechaAlertaOechsle = convertirfecha(new Date());
                            break;
                        case "10":
                            if (fechaAlertaPromart == "" || envioNotificacion == 1)
                                fechaAlertaPromart = convertirfecha(new Date());
                            break;
                        case "11":
                            if (fechaAlertaPromartEcu == "" || envioNotificacion == 1)
                                fechaAlertaPromartEcu = convertirfecha(new Date());
                            break;
                        default:
                    }



                } else if (response.Data.Item3 === true) {  //Rojo
                    $(idBoton).addClass("btn-danger");
                    $(idBoton).removeClass("btn-success");
                    $(idBoton).removeClass("btn-warning");
                    switch (codEmpresa) {
                        case "02":
                            if (colorSemaforo == 2 || envioNotificacion == 1)
                                fechaAlertaSpsa = convertirfecha(new Date());
                            break;
                        case "09":
                            if (colorSemaforo == 2 || envioNotificacion == 1)
                                fechaAlertaOechsle = convertirfecha(new Date());
                            break;
                        case "10":
                            if (colorSemaforo == 2 || envioNotificacion == 1)
                                fechaAlertaPromart = convertirfecha(new Date());
                            break;
                        case "11":
                            if (colorSemaforo == 2 || envioNotificacion == 1)
                                fechaAlertaPromartEcu = convertirfecha(new Date());
                            break;
                        default:
                    }
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

    const obtenerColorSemaforo = function (codEmpresa) {
        let idBoton = "";
        let colorSemaforo = 0;

        switch (codEmpresa) {
            case "02":
                idBoton = "#btnResultadoCantidadSpsa";
                break;
            case "09":
                idBoton = "#btnResultadoCantidadOechsle";
                break;
            case "10":
                idBoton = "#btnResultadoCantidadPromart";
                break;
            case "11":
                idBoton = "#btnResultadoCantidadPromartEcu";
                break;
            default:
        }

        if ($(idBoton).hasClass('btn-success')) {
            colorSemaforo = 1
        } else if ($(idBoton).hasClass('btn-warning')) {
            colorSemaforo = 2
        } else if ($(idBoton).hasClass('btn-danger')) {
            colorSemaforo = 3
        }

        return colorSemaforo;
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
                //cargarParametrosFechaNegocio();
                idIntervalSpsa = setInterval(cargarDatos, timeoutInterval, "02");
                idIntervalOechsle = setInterval(cargarDatos, timeoutInterval, "09");
                idIntervalPromart = setInterval(cargarDatos, timeoutInterval, "10");
                idIntervalPromartEcu = setInterval(cargarDatos, timeoutInterval, "11");
            });
        }
    }
}(jQuery)
