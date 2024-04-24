const urlLogin = baseUrl + 'Login/Index';
const urlListarEmpresasAsociadas = baseUrl + 'Empresa/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = baseUrl + 'Maestros/MaeCadena/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = baseUrl + 'Maestros/MaeRegion/ListarRegionesAsociadas';
const urlListarZonasAsociadas = baseUrl + 'Maestros/MaeZona/ListarZonasAsociadas';
const urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadas';
const urlLocal = baseUrl + 'Login/ListarLocales';
const urlGuardarLocalSession = baseUrl + 'Login/GuardarLocalSession';
const urlObtenerUsuario = baseUrl + 'Seguridad/Usuario/ObtenerUsuario';
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
            $("#cboCadena").prop("disabled", true);
            $("#cboRegion").prop("disabled", true);
            $("#cboZona").prop("disabled", true);
            $("#cboLocal").prop("disabled", true);
            $("#txtUsuario").prop("disabled", false);
            $("#txtClave").prop("disabled", false);
            estaAutenticado = false;
        });


        //$("#cboEmpresa").on("change", function () {
        //    listarLocales($(this).val());
        //});

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

        $("#cboEmpresa").on("change", function () {
            listarCadenasAsociadas('#cboCadena', '#cboEmpresa');
        });

        $("#cboCadena").on("change", function () {
            listarRegionesAsociadas('#cboRegion', '#cboEmpresa', '#cboCadena');
        });

        $("#cboRegion").on("change", function () {
            listarZonasAsociadas('#cboZona', '#cboEmpresa', '#cboCadena', '#cboRegion');
        });

        $("#cboZona").on("change", function () {
            listarLocalesAsociados('#cboLocal', '#cboEmpresa', '#cboCadena', '#cboRegion', '#cboZona');
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
                    $("#txtUsuario").prop("disabled", true);
                    $("#txtClave").prop("disabled", true);


                    obtenerUsuario();

                    //$("#cboEmpresa").prop("disabled", false);
                    //$("#cboCadena").prop("disabled", false);
                    //$("#cboRegion").prop("disabled", false);
                    //$("#cboZona").prop("disabled", false);
                    //$("#cboLocal").prop("disabled", false);

                    //listarEmpresasAsociadas('#cboEmpresa');
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


    const obtenerUsuario = function () {

        const data = {
            CodUsuario: $("#txtUsuario").val()
        };

        $.ajax({
            url: urlObtenerUsuario,
            type: "post",
            data: data,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (response.Ok === true) {
                    $("#cboTipoUsuario").val(response.Data.TipUsuario).trigger('change');

                    validarTipoUsuario(response.Data.TipUsuario);

                    listarEmpresasAsociadas('#cboEmpresa');
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


    const validarTipoUsuario = function (tipo) {

        switch (tipo) {
            case "AD", "SG":
                $("#cboEmpresa").prop("disabled", false);
                break;
            case "US":
                $("#cboEmpresa").prop("disabled", false);
                $("#cboCadena").prop("disabled", false);
                $("#cboRegion").prop("disabled", false);
                $("#cboZona").prop("disabled", false);
                $("#cboLocal").prop("disabled", false);
                break;

        }

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

        const data = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodCadena: $("#cboCadena").val(),
            CodRegion: $("#cboRegion").val(),
            CodZona: $("#cboZona").val(),
            CodLocal: $("#cboLocal").val()
        }

        $.ajax({
            url: urlGuardarLocalSession,
            type: "post",
            data: data,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
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


    const listarEmpresasAsociadas = function (idControl) {

        let data = { CodUsuario: $('#txtUsuario').val(), Busqueda: '' };

        $.ajax({
            url: urlListarEmpresasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    notif({
                        type: "error",
                        msg: data.Mensaje,
                        height: 100,
                        position: "right"
                    });

                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodEmpresa,
                        text: item.NomEmpresa
                    }
                });
                console.log(results);

                $(idControl).empty();

                $(idControl).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idControl).trigger('change');
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

    const listarCadenasAsociadas = function (idCombo, idComboEmpresa) {

        let data = { CodUsuario: $('#txtUsuario').val(), CodEmpresa: $(idComboEmpresa).val(), Busqueda: '' };

        $.ajax({
            url: urlListarCadenasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    notif({
                        type: "error",
                        msg: data.Mensaje,
                        height: 100,
                        position: "right"
                    });
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodCadena,
                        text: item.NomCadena
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

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

    const listarRegionesAsociadas = function (idCombo, idComboEmpresa, idComboCadena) {

        let data = {
            CodUsuario: $('#txtUsuario').val(),
            CodEmpresa: $(idComboEmpresa).val(),
            CodCadena: $(idComboCadena).val()
        };

        $.ajax({
            url: urlListarRegionesAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    notif({
                        type: "error",
                        msg: data.Mensaje,
                        height: 100,
                        position: "right"
                    });
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodRegion,
                        text: item.NomRegion
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

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

    const listarZonasAsociadas = function (idCombo, idComboEmpresa, idComboCadena, idComboRegion) {

        let data = {
            CodUsuario: $('#txtUsuario').val(),
            CodEmpresa: $(idComboEmpresa).val(),
            CodCadena: $(idComboCadena).val(),
            CodRegion: $(idComboRegion).val()
        };

        $.ajax({
            url: urlListarZonasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    notif({
                        type: "error",
                        msg: data.Mensaje,
                        height: 100,
                        position: "right"
                    });
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodZona,
                        text: item.NomZona
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

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

    const listarLocalesAsociados = function (idCombo, idComboEmpresa, idComboCadena, idComboRegion, idComboZona) {

        let data = {
            CodUsuario: $('#txtUsuario').val(),
            CodEmpresa: $(idComboEmpresa).val(),
            CodCadena: $(idComboCadena).val(),
            CodRegion: $(idComboRegion).val(),
            CodZona: $(idComboZona).val()
        };

        $.ajax({
            url: urlListarLocalesAsociados,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    notif({
                        type: "error",
                        msg: data.Mensaje,
                        height: 100,
                        position: "right"
                    });
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodLocal,
                        text: item.NomLocal
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

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
            $('#cboEmpresa').select2();
            $('#cboCadena').select2();
            $('#cboRegion').select2();
            $('#cboZona').select2();
            $('#cboLocal').select2();
            $('#cboTipoUsuario').select2();
        }
    }

}(jQuery);