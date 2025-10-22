const urlLogin = baseUrl + 'Login/Index';

var Login = (function ($) {

    function setBtnLoading(loading) {
        var $btn = $("#btnAutenticarse");
        var $spin = $("#btnSpinner");
        if (loading) {
            $btn.prop("disabled", true).addClass("disabled");
            $spin.removeClass("d-none");
        } else {
            $btn.prop("disabled", false).removeClass("disabled");
            $spin.addClass("d-none");
        }
    }

    function bloquearCampos(bloquear) {
        $("#txtUsuario, #txtClave").prop("disabled", bloquear);
    }

    function autenticar() {
        var user = $("#txtUsuario").val().trim();
        var pass = $("#txtClave").val().trim();

        if (!user || !pass) {
            notif({ type: "error", msg: "Ingrese usuario y contraseña", position: "right" });
            return;
        }

        $.ajax({
            url: urlLogin,
            type: 'POST',
            data: { Usuario: user, Password: pass },
            beforeSend: function () {
                // Muestra spinner del botón y (si quieres) el modal global
                setBtnLoading(true);
                bloquearCampos(true);
                if (typeof showLoading === 'function') showLoading();
            },
            success: function (r) {
                if (r && r.Ok) {
                    // Mantén el spinner visible hasta redirigir (mejor UX, sin parpadeo)
                    window.location = r.NextUrl || (baseUrl + 'Seleccion/Ubicacion');
                } else {
                    notif({ type: "error", msg: (r && r.Mensaje) || "Error en login", position: "right" });
                }
            },
            error: function (jqXHR) {
                notif({ type: "error", msg: jqXHR.responseText || "Error de red", position: "right" });
            },
            complete: function () {
                // Si hubo error, se verá este estado; si hay éxito, la navegación cancela el repintado.
                if (typeof closeLoading === 'function') closeLoading();
                setBtnLoading(false);
                bloquearCampos(false);
            }
        });
    }

    function eventos() {
        $("#btnAutenticarse").on("click", autenticar);
        $("#txtUsuario, #txtClave").on("keypress", function (e) {
            if (e.which === 13) autenticar();
        });
    }

    return { init: eventos };

})(jQuery);

$(function () { Login.init(); });
