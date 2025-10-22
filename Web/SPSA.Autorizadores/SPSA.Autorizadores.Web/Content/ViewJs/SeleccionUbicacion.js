var SeleccionUbicacion = (function ($) {

    function s2($el, placeholder) {
        if ($.fn.select2) {
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

    function bloquearCombos(bloquear) {
        $('#cboEmpresa,#cboCadena,#cboRegion,#cboZona,#cboLocal').prop('disabled', bloquear);
        if ($.fn.select2) {
            $('#cboEmpresa,#cboCadena,#cboRegion,#cboZona,#cboLocal').each(function () {
                $(this).select2(); // refresco de estado visual
            });
        }
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

    function cargarCadenas() {
        var emp = $('#cboEmpresa').val() || '';
        if (!emp) {
            setOptions($('#cboCadena'), []); setOptions($('#cboRegion'), []);
            setOptions($('#cboZona'), []); setOptions($('#cboLocal'), []);
            return;
        }
        ajaxPost(urlListarCadenasAsociadas, { CodUsuario: usuarioLogueado, CodEmpresa: emp, Busqueda: '' }, function (r) {
            var items = [];
            if (r.Data && r.Data.length) {
                for (var i = 0; i < r.Data.length; i++) {
                    items.push({ id: r.Data[i].CodCadena, text: r.Data[i].NomCadena });
                }
            }
            setOptions($('#cboCadena'), items);
        });
    }

    function cargarRegiones() {
        var emp = $('#cboEmpresa').val() || '';
        var cad = $('#cboCadena').val() || '';
        if (!emp || !cad) {
            setOptions($('#cboRegion'), []); setOptions($('#cboZona'), []); setOptions($('#cboLocal'), []);
            return;
        }
        ajaxPost(urlListarRegionesAsociadas, { CodUsuario: usuarioLogueado, CodEmpresa: emp, CodCadena: cad }, function (r) {
            var items = [];
            if (r.Data && r.Data.length) {
                for (var i = 0; i < r.Data.length; i++) {
                    items.push({ id: r.Data[i].CodRegion, text: r.Data[i].NomRegion });
                }
            }
            setOptions($('#cboRegion'), items);
        });
    }

    function cargarZonas() {
        var emp = $('#cboEmpresa').val() || '';
        var cad = $('#cboCadena').val() || '';
        var reg = $('#cboRegion').val() || '';
        if (!emp || !cad || !reg) {
            setOptions($('#cboZona'), []); setOptions($('#cboLocal'), []);
            return;
        }
        ajaxPost(urlListarZonasAsociadas, { CodUsuario: usuarioLogueado, CodEmpresa: emp, CodCadena: cad, CodRegion: reg }, function (r) {
            var items = [];
            if (r.Data && r.Data.length) {
                for (var i = 0; i < r.Data.length; i++) {
                    items.push({ id: r.Data[i].CodZona, text: r.Data[i].NomZona });
                }
            }
            setOptions($('#cboZona'), items);
        });
    }

    function cargarLocales() {
        var emp = $('#cboEmpresa').val() || '';
        var cad = $('#cboCadena').val() || '';
        var reg = $('#cboRegion').val() || '';
        var zon = $('#cboZona').val() || '';
        if (!emp || !cad || !reg || !zon) {
            setOptions($('#cboLocal'), []);
            return;
        }
        ajaxPost(urlListarLocalesAsociados, { CodUsuario: usuarioLogueado, CodEmpresa: emp, CodCadena: cad, CodRegion: reg, CodZona: zon }, function (r) {
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
        var cad = $('#cboCadena').val();
        var reg = $('#cboRegion').val();
        var zon = $('#cboZona').val();
        var loc = $('#cboLocal').val();

        if (!emp || !loc) {
            notif({ type: "error", msg: "Seleccione al menos Empresa y Local", position: "right" });
            return;
        }

        $.ajax({
            url: urlGuardarLocalSession,
            type: 'POST',
            data: { CodEmpresa: emp, CodCadena: cad, CodRegion: reg, CodZona: zon, CodLocal: loc },
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
        s2($('#cboCadena'), 'Cadena');
        s2($('#cboRegion'), 'Región');
        s2($('#cboZona'), 'Zona');
        s2($('#cboLocal'), 'Local');

        $('#cboEmpresa').on('change', cargarCadenas);
        $('#cboCadena').on('change', cargarRegiones);
        $('#cboRegion').on('change', cargarZonas);
        $('#cboZona').on('change', cargarLocales);
        $('#btnIngresar').on('click', guardarSesion);

        cargarEmpresas();
    }

    return { init: init };

})(jQuery);

$(function () { SeleccionUbicacion.init(); });
