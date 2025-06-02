var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';


var AprobacionFactorMdr = function () {
    const eventos = function () {

        
    };

    const listarEmpresasAsociadas = function () {
        return new Promise((resolve, reject) => {

            const request = {
                Busqueda: ''
            };

            $.ajax({
                url: urlListarEmpresasAsociadas,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }


    const cargarComboEmpresa = async function () {
        try {
            const response = await listarEmpresasAsociadas();

            if (response.Ok) {
                //$('#cboEmpresaBuscar').empty().append('<option label="Todos"></option>');
                $('#cboEmpresaBuscar').empty().append($('<option>', { value: '0', text: 'Todos' }));
                $('#cboLocalBuscar').empty().append('<option label="Todos"></option>');
                response.Data.map(empresa => {
                    $('#cboEmpresaBuscar').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
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

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
            });
        }
    }
}(jQuery);