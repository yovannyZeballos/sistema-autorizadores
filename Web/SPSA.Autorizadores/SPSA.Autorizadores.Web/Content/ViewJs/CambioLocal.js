var urlCambioLocal = baseUrl + 'Seguridad/CambioLocal/Index';
var urlEmpresas = baseUrl + 'Login/ListarEmpresas';
var urlLocal = baseUrl + 'Login/ListarLocales';
var urlGuardarLocalSession = baseUrl + 'Login/GuardarLocalSession';
var dataTableLocal = null;

var CambioLocal = function () {

    var eventos = function () {

        $("#btnGuardar").on("click", function () {

            const registrosSeleccionados = dataTableLocal.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }

            btnLoading($("#btnGuardar"), true);

            guardarLocalSession(registrosSeleccionados[0].Codigo);


        });

        $("#cboEmpresa").on("change", function () {
            listarLocales($(this).val());
        });


        $('#tableLocales tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableLocal.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    };

    const listarEmpresas = function () {
        $.ajax({
            url: urlEmpresas,
            type: "get",
            success: function (response) {

                if (response.Ok === true) {
                    cargarEmpresas(response.Empresas);
                    listarLocales(rucSession);
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

    const listarLocales = function (ruc) {
        $.ajax({
            url: urlLocal + "?ruc=" + ruc,
            type: "get",
            success: function (response) {
                cargarLocales(response.Locales)
                seleccionarLocal();
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
        $('#cboEmpresa').val(rucSession);


    }

    const cargarLocales = function (locales) {
        dataTableLocal.clear();
        dataTableLocal.rows.add(locales);
        dataTableLocal.draw();

    }

    const guardarLocalSession = function (codigo) {
        $.ajax({
            url: urlGuardarLocalSession,
            type: "post",
            data: { codigoLocal: codigo },
            success: function (response) {
                swal({
                    text: "Se cambio el local correctamente, se reinicara el sistema.",
                    icon: "success"
                }).then(() => {
                    document.location = home;
                    btnLoading($("#btnGuardar"), false);
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

    var visualizarDataTableLocales = function () {
        dataTableLocal = $('#tableLocales').DataTable({
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '180px',
            scrollCollapse: true,
            paging: false,
            "bAutoWidth": false,
            "columns": [
                { "data": "Codigo" },
                { "data": "Nombre" }
            ]
        });
    };

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

    const seleccionarLocal = function () {
        var i = 0;
        dataTableLocal.rows().data().toArray().map(row => {
            if (row.Codigo === codigoLocalSession) {
                dataTableLocal.row(i).nodes().to$().toggleClass('selected');
            }
            i++;
        });


    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                listarEmpresas();
                visualizarDataTableLocales();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }

}(jQuery);
