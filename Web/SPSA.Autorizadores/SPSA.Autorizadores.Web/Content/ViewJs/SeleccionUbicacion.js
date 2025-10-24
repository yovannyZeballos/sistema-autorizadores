var SeleccionUbicacion = (function ($) {

    function s2($el, placeholder) {
        if ($.fn.select2 && !$el.data('select2')) {
            $el.select2({
                width: '100%',
                placeholder: placeholder || 'Seleccionar…',
                allowClear: true,
                minimumResultsForSearch: 0,
                dropdownParent: $(document.body)
            });
        }
    }


    function setOptions($el, items) {
        $el.empty();
        $el.append(new Option('', ''));
        if (items && items.length) {
            for (var i = 0; i < items.length; i++) {
                $el.append(new Option(items[i].text, items[i].id));
            }
        }
        $el.val('').trigger('change');
        if (items && items.length === 1) {
            $el.val(items[0].id).trigger('change');
        }
    }



    function ajaxPost(url, data, onOk) {
        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data || {}),
            beforeSend: function () { if (typeof showLoading === 'function') showLoading(); },
            complete: function () { if (typeof closeLoading === 'function') closeLoading(); },
            success: function (r) {
                if (!r || r.Ok === false) {
                    notif({ type: "error", msg: (r && r.Mensaje) ? r.Mensaje : 'Error', position: 'right' });
                    return;
                }
                if (onOk) onOk(r);
            },
            error: function (jqXHR) {
                notif({ type: "error", msg: jqXHR.responseText, position: 'right' });
            }
        });
    }

    // --- Spinner y bloqueo de botón Ingresar ---
    function setBtnLoadingUbic(loading) {
        var $btn = $("#btnIngresar");
        var $spin = $("#btnSpinnerUbic");
        if (loading) {
            $btn.prop("disabled", true).addClass("disabled");
            $spin.removeClass("d-none");
        } else {
            $btn.prop("disabled", false).removeClass("disabled");
            $spin.addClass("d-none");
        }
    }

    //function bloquearCombos(bloquear) {
    //    $('#cboEmpresa,#cboLocal').prop('disabled', bloquear);
    //    if ($.fn.select2) {
    //        $('#cboEmpresa,#cboLocal').each(function () {
    //            $(this).select2(); // refresco de estado visual
    //        });
    //    }
    //}
    function bloquearCombos(bloquear) {
        $('#cboEmpresa,#cboLocal').prop('disabled', bloquear)
            .trigger('change.select2'); // solo notifica disabled, no reinit
    }

    // -------------------------------------------

    function cargarEmpresas() {
        ajaxPost(urlListarEmpresasAsociadas, { CodUsuario: usuarioLogueado, Busqueda: '' }, function (r) {
            var items = [];
            if (r.Data && r.Data.length) {
                for (var i = 0; i < r.Data.length; i++) {
                    items.push({ id: r.Data[i].CodEmpresa, text: r.Data[i].NomEmpresa });
                }
            }
            setOptions($('#cboEmpresa'), items);
        });
    }

    function cargarLocales() {
        var emp = $('#cboEmpresa').val() || '';
        if (!emp) {
            setOptions($('#cboLocal'), []);
            return;
        }
        ajaxPost(urlListarLocalesAsociados, { CodUsuario: usuarioLogueado, CodEmpresa: emp }, function (r) {
            var items = [];
            if (r.Data && r.Data.length) {
                for (var i = 0; i < r.Data.length; i++) {
                    items.push({ id: r.Data[i].CodLocal, text: r.Data[i].NomLocal });
                }
            }
            setOptions($('#cboLocal'), items);
        });
    }

    function guardarSesion() {
        var emp = $('#cboEmpresa').val();
        var loc = $('#cboLocal').val();

        if (!emp || !loc) {
            notif({ type: "error", msg: "Seleccione al menos Empresa y Local", position: "right" });
            return;
        }

        $.ajax({
            url: urlGuardarLocalSession,
            type: 'POST',
            data: { CodEmpresa: emp, CodLocal: loc },
            beforeSend: function () {
                if (typeof showLoading === 'function') showLoading();
                setBtnLoadingUbic(true);
                bloquearCombos(true);
            },
            complete: function () {
                if (typeof closeLoading === 'function') closeLoading();
                // Si hubo error, re-habilitamos UI; si hay éxito, la navegación ocurrirá antes de ver el “off”.
                setBtnLoadingUbic(false);
                bloquearCombos(false);
            },
            success: function (r) {
                if (r.Ok) {
                    window.location = baseUrl + 'Home/Index';
                } else {
                    notif({ type: "error", msg: r.Mensaje || 'No se pudo guardar la sesión', position: "right" });
                }
            },
            error: function (jqXHR) {
                notif({ type: "error", msg: jqXHR.responseText, position: "right" });
            }
        });
    }

    function init() {
        s2($('#cboEmpresa'), 'Empresa');
        s2($('#cboLocal'), 'Local');    

        $('#cboEmpresa').on('change', cargarLocales);
        $('#btnIngresar').on('click', guardarSesion);

        cargarEmpresas();
      
    }

    return { init: init };

})(jQuery);

$(function () { SeleccionUbicacion.init(); });
