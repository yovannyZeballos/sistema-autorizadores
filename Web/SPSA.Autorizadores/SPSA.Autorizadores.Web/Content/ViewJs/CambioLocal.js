var urlCambioLocal = baseUrl + 'Seguridad/CambioLocal/Index';
var urlEmpresas = baseUrl + 'Login/ListarEmpresas';
var urlLocal = baseUrl + 'Login/ListarLocales';
var urlGuardarLocalSession = baseUrl + 'Login/GuardarLocalSession';

const urlListarEmpresasAsociadas = baseUrl + 'Empresa/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = baseUrl + 'Maestros/MaeCadena/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = baseUrl + 'Maestros/MaeRegion/ListarRegionesAsociadas';
const urlListarZonasAsociadas = baseUrl + 'Maestros/MaeZona/ListarZonasAsociadas';
const urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadas';

var dataTableLocal = null;

var CambioLocal = function () {

    var eventos = function () {

        $("#btnGuardar").on("click", function () {

            const registrosSeleccionados = dataTableLocal.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length)) {
                return;
            }

            btnLoading($("#btnGuardar"), true);
            guardarLocalSession(registrosSeleccionados[0].CodLocal);

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

                $(idControl).val(codEmpresaSession);

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

                $(idCombo).val(codCadenaSession);

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

                $(idCombo).val(codRegionSession);

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

                $(idCombo).val(codZonaSession);

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

                cargarLocales(data.Data);
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

        let datos = {
            /*CodUsuario: $('#txtUsuario').val(),*/
            CodEmpresa: $('#cboEmpresa').val(),
            CodCadena: $('#cboCadena').val(),
            CodRegion: $('#cboRegion').val(),
            CodZona: $('#cboZona').val(),
            CodLocal: codigo
        };


        $.ajax({
            url: urlGuardarLocalSession,
            type: "post",
            data: { query: datos },
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
                { "data": "CodLocal" },
                { "data": "NomLocal" }
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
        dataTableLocal.rows().every(function () {
            var rowData = this.data();
            if (rowData.CodLocal === codLocalSession) {
                $(this.node()).toggleClass('selected');
            }
            i++;
        });
    };

    return {
        init: function () {
            checkSession(function () {
                eventos();
                /*listarEmpresas();*/
                listarEmpresasAsociadas('#cboEmpresa');
                
                visualizarDataTableLocales();
                $('input[type="search"]').addClass("form-control-sm");

                $('#cboEmpresa').select2();
                $('#cboCadena').select2();
                $('#cboRegion').select2();
                $('#cboZona').select2();
            });
        }
    }

}(jQuery);
