var urlProcesar = baseUrl + 'Monitor/Proceso/EjecutarProceso';
var urlListarProcesos = baseUrl + 'Monitor/Proceso/ListarProcesos';

var Proceso = function () {

    const eventos = function () {

        $("#btnProcesar").on('click', function () {
            procesar();
        });
    }


    const procesar = function () {

        const codProceso = $("#cboProceso").val();

        if (codProceso === null || codProceso === "") {
            swal({
                text: "Seleccione el proceso.",
                icon: "warning"
            });
            return;
        }


        $.ajax({
            url: urlProcesar,
            type: "post",
            data: { codProceso },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                let icon = response.Ok ? "success" : "error";

                swal({
                    text: response.Mensaje,
                    icon: icon
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

    const listarProcesos = function () {


        $.ajax({
            url: urlListarProcesos,
            type: "post",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (response.Ok === true) {
                    cargarProcesos(response.Data);
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

    const cargarProcesos = function (empresas) {
        $('#cboProceso').empty().append('<option label="Seleccionar"></option>');
        empresas.map(proceso => {
            $('#cboProceso').append($('<option>', { value: proceso.CodProceso, text: proceso.DesProceso }));
        });
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                listarProcesos();
            });
        }
    }
}(jQuery)
