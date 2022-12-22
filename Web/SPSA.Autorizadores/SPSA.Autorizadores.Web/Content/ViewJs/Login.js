var urlLogin = baseUrl + 'Login/Index';
var urlEmpresas = baseUrl + 'Login/ListarEmpresas';
var urlLocal = baseUrl + 'Login/ListarLocales';
var urlGuardarLocalSession = baseUrl + 'Login/GuardarLocalSession';
var estaAutenticado = false;

var Login = function () {

    var eventos = function () {

        $("#btnAutenticarse").on("click", function () {

            if (validarLogin() === false)
                return;

            btnLoading($("#btnAutenticarse"), true);

            if (!estaAutenticado) {
                login();
            } else {
                guardarLocalSession();
            }


        });


        $("#btnCancelar").on("click", function () {

            $("#btnAutenticarse").text("Autenticarse");
            $("#cboEmpresa").prop("disabled", true);
            $("#cboLocal").prop("disabled", true);
            $("#txtUsuario").prop("disabled", false);
            $("#txtClave").prop("disabled", false);
            estaAutenticado = false;
        });


        $("#cboEmpresa").on("change", function () {
            listarLocales($(this).val());
        });

        $("#txtUsuario").on("keyup", function () {
            validarControl($(this));
        });

        $("#txtClave").on("keyup", function () {
            validarControl($(this));
        });

        $('#txtUsuario').keypress(function (e) {
            if (e.which == 13) {
                if (validarLogin() === false)
                    return;

                if (!estaAutenticado) {
                    btnLoading($("#btnAutenticarse"), true);
                    login();
                } else {
                    guardarLocalSession();
                }
            }
        });

        $('#txtClave').keypress(function (e) {
            if (e.which == 13) {
                if (validarLogin() === false)
                    return;

                if (!estaAutenticado) {
                    btnLoading($("#btnAutenticarse"), true);
                    login();
                } else {
                    guardarLocalSession();
                }
            }
        });

        $('#cboLocal').keypress(function (e) {
            if (e.which == 13) {
                if (validarLogin() === false)
                    return;

                if (!estaAutenticado) {
                    btnLoading($("#btnAutenticarse"), true);
                    login();
                } else {
                    guardarLocalSession();
                }
            }
        });
    };

    const login = function () {

        const usuario = $("#txtUsuario").val();
        const clave = $("#txtClave").val();
        const data = {
            Usuario: usuario,
            Password: clave
        };

        $.ajax({
            url: urlLogin,
            type: "post",
            data: data,
            success: function (response) {
                if (response.Ok === true) {
                    estaAutenticado = true;
                    $("#btnAutenticarse").text("Ingresar");
                    //$("#cboEmpresa").prop("disabled", false);
                    //$("#cboLocal").prop("disabled", false);
                    $("#txtUsuario").prop("disabled", true);
                    $("#txtClave").prop("disabled", true);
                    listarEmpresas();
                } else {
                    notif({
                        type: "info",
                        msg: response.Mensaje,
                        height: 100,
                        position: "right"
                    });
                }

                btnLoading($("#btnAutenticarse"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notif({
                    type: "error",
                    msg: jqXHR.responseText,
                    height: 100,
                    position: "right"
                });

                btnLoading($("#btnAutenticarse"), false);
            }
        });
    }



    const listarEmpresas = function () {
        $.ajax({
            url: urlEmpresas,
            type: "get",
            success: function (response) {

                if (response.Ok === true) {
                    cargarEmpresas(response.Empresas);
                    
                } else {
                    notif({
                        type: "error",
                        msg: response.Mensaje,
                        height: 100,
                        position: "right"
                    });
                }

                $("#cboEmpresa").prop("disabled", false);
                $("#cboLocal").prop("disabled", false);

            },
            error: function (jqXHR, textStatus, errorThrown) {
                notif({
                    type: "error",
                    msg: jqXHR.responseText,
                    height: 100,
                    position: "right"
                });
            }
        });
    }

    const listarLocales = function (ruc) {
        $.ajax({
            url: urlLocal + "?ruc=" + ruc,
            type: "get",
            success: function (response) {

                if (response.Ok === true) {
                    cargarLocales(response.Locales);
                } else {
                    notif({
                        type: "error",
                        msg: response.Mensaje,
                        height: 100,
                        position: "right"
                    });
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                notif({
                    type: "error",
                    msg: jqXHR.responseText,
                    height: 100,
                    position: "right"
                });
            }
        });
    }

    const cargarEmpresas = function (empresas) {
        $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
        empresas.map(empresa => {
            $('#cboEmpresa').append($('<option>', { value: empresa.Ruc, text: empresa.Descripcion }));
        });

    }

    const cargarLocales = function (locales) {
        $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
        locales.map(local => {
            $('#cboLocal').append($('<option>', { value: local.Codigo, text: local.Nombre }));
        });

    }

    const validarLogin = function () {

        let ok = true;

        if (!estaAutenticado) {
            if ($("#txtUsuario").val().length === 0) {
                ok = false;
                $("#txtUsuario").addClass("is-invalid state-invalid");
            }

            if ($("#txtClave").val().length === 0) {
                ok = false;
                $("#txtClave").addClass("is-invalid state-invalid");
            }
        } else {
            if ($("#cboEmpresa").val().length === 0 || $("#cboLocal").val().length === 0) {
                ok = false;
                notif({
                    type: "error",
                    msg: "Debe seleccionar la empresa y local",
                    height: 100,
                    position: "right"
                });
            }
        }



        return ok;

    }

    const validarControl = function (control) {
        if (control.val().length > 0) {
            control.removeClass("is-invalid state-invalid");
        } else {
            control.addClass("is-invalid state-invalid");

        }
    }

    const guardarLocalSession = function () {
        $.ajax({
            url: urlGuardarLocalSession,
            type: "post",
            data: { codigoLocal: $("#cboLocal").val() },
            success: function (response) {
                if (response.Ok) {
                    document.location.href = home;
                    btnLoading($("#btnAutenticarse"), false);
                }
                else {
                    notif({
                        type: "error",
                        msg: response.Mensaje,
                        height: 100,
                        width: "all",
                        position: "center"
                    });
                    btnLoading($("#btnAutenticarse"), false);
                }
               
            },
            error: function (jqXHR, textStatus, errorThrown) {

                notif({
                    type: "error",
                    msg: jqXHR.responseText,
                    height: 100,
                    position: "right"
                });
            }
        });
    }


    return {
        init: function () {
            eventos();
        }
    }

}(jQuery);