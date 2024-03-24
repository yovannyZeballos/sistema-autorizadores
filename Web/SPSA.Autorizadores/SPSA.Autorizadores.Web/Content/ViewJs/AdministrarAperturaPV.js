var urlListarDepartamentos = baseUrl + 'Ubigeos/Ubigeo/ListarDepartamentos';
var urlListarProvincias = baseUrl + 'Ubigeos/Ubigeo/ListarProvincias';
var urlListarDistritos = baseUrl + 'Ubigeos/Ubigeo/ListarDistritos';
var urlObtenerUbigeo = baseUrl + 'Ubigeos/Ubigeo/ObtenerUbigeo';

const AdministrarLocalAperturasPV = function () {

    var eventos = async function () {


        $("#cboDepartamento").on("change", async function () {
            await cargarComboProvincias();
            var t_ubigeo = $('#txtUbigeo').val();
            
            if (t_ubigeo === '') {

            } else {
                var distrito = await obtenerUbigeo();
                $("#cboDepartamento").val(distrito.Data.CodDepartamento);
            }
        });

        $("#cboProvincia").on("change", async function () {
            await cargarComboDistritos();
            var t_ubigeo = $('#txtUbigeo').val();

            if (t_ubigeo === '') {

            } else {
                var distrito = await obtenerUbigeo();
                $("#cboProvincia").val(distrito.Data.CodProvincia);
            }
            //if (t_ubigeo === '') {

            //} else {
            //    $("#cboProvincia").val(distrito.Data.CodProvincia);
            //}
        });

        $("#cboDistrito").on("change", async function () {
            var t_ubigeo = $('#txtUbigeo').val();

            if (t_ubigeo === '') {

            } else {
                var distrito = await obtenerUbigeo();
                $("#cboProvincia").val(distrito.Data.CodProvincia);
            }
        });
    }

    const cargarComboDepartamentos = async function () {

        try {
            const response = await listarDepartamentos();

            if (response.Ok) {
                $('#cboDepartamento').empty().append('<option label="Seleccionar"></option>');
                $('#cboProvincia').empty().append('<option label="Seleccionar"></option>');
                $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(departamento => {
                    $('#cboDepartamento').append($('<option>', { value: departamento.CodDepartamento, text: departamento.NomDepartamento }));
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

    const cargarComboProvincias = async function () {

        try {
            const response = await listarProvincias();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboProvincia').empty().append('<option label="Seleccionar"></option>');
                $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(provincia => {
                    $('#cboProvincia').append($('<option>', { value: provincia.CodProvincia, text: provincia.NomProvincia }));
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

    const cargarComboDistritos = async function () {

        console.log('*')

        try {
            const response = await listarDistritos();
            if (response === undefined) return;
            if (response.Ok) {
                $('#cboDistrito').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(distrito => {
                    $('#cboDistrito').append($('<option>', { value: distrito.CodDistrito, text: distrito.NomDistrito }));
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

    const listarDepartamentos = function () {
        return new Promise((resolve, reject) => {

            const request = {
            };

            $.ajax({
                url: urlListarDepartamentos,
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

    const listarProvincias = function () {
        return new Promise((resolve, reject) => {
            const codDepartamento = $("#cboDepartamento").val();
            if (!codDepartamento) return resolve();

            const request = {
                CodDepartamento: codDepartamento
            };

            $.ajax({
                url: urlListarProvincias,
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

    const listarDistritos = function () {
        return new Promise((resolve, reject) => {
            const codDepartamento = $("#cboDepartamento").val();
            const codProvincia = $("#cboProvincia").val();
            if (!codDepartamento) return resolve();

            const request = {
                CodDepartamento: codDepartamento,
                CodProvincia: codProvincia,
            };

            $.ajax({
                url: urlListarDistritos,
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

    const obtenerUbigeo = function (codUbigeo) {
        return new Promise((resolve, reject) => {
            if (!codUbigeo) return resolve();

            const request = {
                CodUbigeo: codUbigeo
            };

            $.ajax({
                url: urlObtenerUbigeo,
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

    return {
        init: function () {
            checkSession(async function () {
                await cargarComboDepartamentos();
                await eventos();
            });
        }
    }

}(jQuery);