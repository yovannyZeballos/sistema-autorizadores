const urlListarEmpresasAsociadas = baseUrl + 'Empresa/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = baseUrl + 'Maestros/MaeCadena/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = baseUrl + 'Maestros/MaeRegion/ListarRegionesAsociadas';
const urlListarZonasAsociadas = baseUrl + 'Maestros/MaeZona/ListarZonasAsociadas';
const urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadas';

const urlListarLocales = '/Local/ListarLocalesAsignados';
const urlListarCajeros = '/CajeroVolante/ListarCajerosVolanteOfiplan';
const urlListarCajerosAsginados = '/CajeroVolante/ListarCajerosVolante'; 
const urlCrearCajeroVolante = '/CajeroVolante/CrearCajerosVolante';
const urlEliminarCajeroVolante = '/CajeroVolante/EliminarCajerosVolante';

var dataTableCajero = null;
var dataTableCajeroVolante = null;
var dataTableLocal = null;

const CajeroVolante = function () {

    var eventos = function () {

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

        $("#btnAsignar").on('click', function () {
            asignar();
        });

        $("#bntEliminar").on('click', function () {
            eliminar();
        });

        $('#tableCajero tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $('#tableCajeroVolante tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $('#tableLocal tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#chkActivos,#chkInactivos").on("change", function () {

            if ($("#chkInactivos").prop('checked') && !$("#chkActivos").prop('checked')) {
                dataTableCajeroVolante.column(4).search('N').draw();
            }
            else if ($("#chkActivos").prop('checked') && !$("#chkInactivos").prop('checked')) {
                dataTableCajeroVolante.column(4).search('A').draw();
            }
            else {
                dataTableCajeroVolante.column(4).search('').draw();
            }

        });

        $("#chkSeleccionar").on("change", function () {
            chechTodos();
        })
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
                //seleccionarLocal();

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

    const chechTodos = function () {

        if ($("#chkSeleccionar").prop('checked')) {
            dataTableLocal.rows({ search: 'applied' }).nodes().each(function () {
                $(this).addClass('selected');
            });
        } else {
            dataTableLocal.rows({ search: 'applied' }).nodes().each(function () {
                $(this).removeClass('selected');
            });
        }
    }

    const visualizarDataTableCajero = function () {

        $.ajax({
            url: urlListarCajeros,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                        className: "pointer",
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableCajero != null) {
                    dataTableCajero.clear();
                    dataTableCajero.destroy();
                    dataTableCajero = null;
                }

                var tableId = "#tableCajero";
                $(tableId + " tbody").empty();
                $(tableId + " thead").empty();



                dataTableCajero = $('#tableCajero').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: true,
                    scrollY: '320px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
                    bAutoWidth: false,
                    language: {
                        searchPlaceholder: "Buscar Cajero",
                        sSearch: ''
                    }
                });

                $('input[type="search"]').addClass("form-control-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const visualizarDataTableLocal = function () {

        $.ajax({
            url: urlListarLocales,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                        className: "pointer"
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableLocal != null) {
                    dataTableLocal.clear();
                    dataTableLocal.destroy();
                    dataTableLocal = null;
                }

                var tableId = "#tableLocal";
                $(tableId + " tbody").empty();
                $(tableId + " thead").empty();



                dataTableLocal = $('#tableLocal').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: true,
                    scrollY: '320px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
                    bAutoWidth: false,
                    language: {
                        searchPlaceholder: "Buscar Local",
                        sSearch: ''
                    }
                });

                $('input[type="search"]').addClass("form-control-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const visualizarDataTableCajeroVolante = function () {

        $.ajax({
            url: urlListarCajerosAsginados,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x.replace("_", " "),
                        data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                        className: "pointer",
                    });
                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableCajeroVolante != null) {
                    dataTableCajeroVolante.clear();
                    dataTableCajeroVolante.destroy();
                    dataTableCajeroVolante = null;
                }

                var tableId = "#tableCajeroVolante";
                $(tableId + " tbody").empty();
                $(tableId + " thead").empty();



                dataTableCajeroVolante = $('#tableCajeroVolante').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    searching: true,
                    scrollY: '660px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: response.Data,
                    columns: columnas,
                    bAutoWidth: false,
                    rowCallback: function (row, data, index) {
                        if (data.ACT == "N") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    language: {
                        searchPlaceholder: "Buscar Volante",
                        sSearch: ''
                    }
                });

                $('input[type="search"]').addClass("form-control-sm");
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

        if (dataTableLocal != null) {
            dataTableLocal.clear();
            dataTableLocal.destroy();
            dataTableLocal = null;
        }

        //var tableId = "#tableLocal";
        //$(tableId + " tbody").empty();

        dataTableLocal = $('#tableLocal').DataTable({
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

    const cargarLocales = function (locales) {
        dataTableLocal.clear();
        dataTableLocal.rows.add(locales);
        dataTableLocal.draw();

    }

    const asignar = function () {
        const registrosSeleccionadosCajero = dataTableCajero.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionadosCajero.length, "Cajeros")) {
            return;
        }

        const registrosSeleccionadosLocal = dataTableLocal.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionadosLocal.length, "Locales")) {
            return;
        }

        let cajeros = [];
        let locales = [];

        registrosSeleccionadosLocal.map((item) => {
            locales.push(item.CodLocal);
        });

        registrosSeleccionadosCajero.map((item) => {
            cajeros.push({
                CodOfisis: item.Codigo,
                NumDocumento: item.NroDocu,
                CodSedeOrigen: item.C_LOCAL,
                LocalesAsignados: locales
            });
        });

       

        $.ajax({
            url: urlCrearCajeroVolante,
            type: "post",
            data: { request: cajeros },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                visualizarDataTableCajero();
                //visualizarDataTableLocal();
                visualizarDataTableLocales();
                listarLocalesAsociados('#cboLocal', '#cboEmpresa', '#cboCadena', '#cboRegion', '#cboZona');
                visualizarDataTableCajeroVolante();


                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }
               
                swal({
                    text: "Registros creados exitosamente.",
                    icon: "success"
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

    const eliminar = function () {
        const registrosSeleccionados = dataTableCajeroVolante.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length, "Cajero volante")) {
            return;
        }

        let cajeros = [];

        registrosSeleccionados.map((item) => {
            cajeros.push({
                CodOfisis: item.CODIGO,
                CodSede: item.DESOFIPLAN
            });
        });

        $.ajax({
            url: urlEliminarCajeroVolante,
            type: "post",
            data: { request: cajeros },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                visualizarDataTableCajero();
                //visualizarDataTableLocal();
                visualizarDataTableLocales();
                listarLocalesAsociados('#cboLocal', '#cboEmpresa', '#cboCadena', '#cboRegion', '#cboZona');
                visualizarDataTableCajeroVolante();

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                swal({
                    text: "Registros eliminados exitosamente.",
                    icon: "success"
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

    const validarSelecion = function (count, titulo) {
        if (count === 0) {
            swal({
                text: titulo + " - debe seleccionar como minimo un registro",
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

                listarEmpresasAsociadas('#cboEmpresa');

                visualizarDataTableCajero();
                //visualizarDataTableLocal();
                visualizarDataTableLocales();
                visualizarDataTableCajeroVolante();
                $('input[type="search"]').addClass("form-control-sm");


                $('#cboEmpresa').select2();
                $('#cboCadena').select2();
                $('#cboRegion').select2();
                $('#cboZona').select2();

            });
        }
    }

}(jQuery);