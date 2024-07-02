var urlCambioLocal = baseUrl + 'Seguridad/CambioLocal/Index';
var urlEmpresas = baseUrl + 'Login/ListarEmpresas';
var urlLocal = baseUrl + 'Login/ListarLocales';
var urlGuardarLocalSession = baseUrl + 'Login/GuardarLocalSession';

const urlListarEmpresasAsociadas = baseUrl + 'Empresa/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = baseUrl + 'Maestros/MaeCadena/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = baseUrl + 'Maestros/MaeRegion/ListarRegionesAsociadas';
const urlListarZonasAsociadas = baseUrl + 'Maestros/MaeZona/ListarZonasAsociadas';
const urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadas';

var urlListarLocalesCambioPrecio = baseUrl + 'Reportes/AdministrarReportes/ListarLocalesCambioPrecio';
var urlListarLocalesNotaCredito = baseUrl + 'Reportes/AdministrarReportes/ListarLocalesNotaCredito';

var dataTableRegistros = null;

var AdministrarReportes = function () {

    var eventos = function () {

        $("#btnConsultar").on('click', function () {
            var selectedReport = $('#cboTipoReporte').val();

            switch (selectedReport) {
                case '01':
                    visualizarDataTable(urlListarLocalesCambioPrecio);
                    break;
                case '02':
                    visualizarDataTable(urlListarLocalesNotaCredito);
                    break;
                default:
                    alert('Por favor, seleccione un tipo de reporte válido.');
            }

            //visualizarDataTable();
        });

        $('#btnExportar').click(function () {
            // Convertir la tabla HTML a una cadena CSV
            var csv = [];
            var rows = document.querySelectorAll("#tableReportes tr");

            for (var i = 0; i < rows.length; i++) {
                var row = [], cols = rows[i].querySelectorAll("td, th");

                for (var j = 0; j < cols.length; j++) {
                    row.push(cols[j].innerText);
                }

                csv.push(row.join("|"));
            }

            // Crear un Blob a partir de la cadena CSV
            var csvFile = new Blob([csv.join("\n")], { type: "text/csv" });

            // Crear un enlace de descarga para el archivo CSV
            var downloadLink = document.createElement("a");
            downloadLink.download = "Reporte.csv";
            downloadLink.href = window.URL.createObjectURL(csvFile);
            downloadLink.style.display = "none";

            // Agregar el enlace al documento y hacer clic en él
            document.body.appendChild(downloadLink);
            downloadLink.click();
            document.body.removeChild(downloadLink);
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

        $('#tableReportes tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableRegistros.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    };

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

                $(idCombo).val(codLocalSession);

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

    var visualizarDataTable = function (dtUrl) {

        const request = {
            CodLocal: $("#cboLocal").val(),
            FechaInicio: $("#txtFechaInicio").val(),
            FechaFin: $("#txtFechaFin").val(),
        };

        $.ajax({
            url: dtUrl,
            type: "post",
            data: { request },
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
                        title: x,
                        data: x.replace(" ", "").replace(".", ""),
                        defaultContent: "",
                    });
                });

                if (dataTableRegistros != null) {
                    dataTableRegistros.clear();
                    dataTableRegistros.destroy();
                    dataTableRegistros = null;
                }

                var dataTableRegistrosId = "#tableReportes";
                $(dataTableRegistrosId + " tbody").empty();
                $(dataTableRegistrosId + " thead").empty();

                if (columnas.length == 0) return;

                dataTableRegistros = $(dataTableRegistrosId).DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    "data": response.Data,
                    "bAutoWidth": false
                });

                //dataTableRegistros.buttons().container().prependTo($('#tableColaboradorCesado_filter'));
                $('input[type="search"]').addClass("form-control-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
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

    const inicializarDatePicker = function () {
        $('.fc-datepicker').datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            closeText: 'Cerrar',
            prevText: '<Ant',
            nextText: 'Sig>',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'dd/mm/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: '',
            changeMonth: true,
            changeYear: true
        });
    }

    const fechaActual = function () {
        let date = new Date()

        let day = `${(date.getDate())}`.padStart(2, '0');
        let month = `${(date.getMonth() + 1)}`.padStart(2, '0');
        let year = date.getFullYear();

        $("#txtFechaInicio").val(`${day}/${month}/${year}`);
        $("#txtFechaFin").val(`${day}/${month}/${year}`);
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                $('input[type="search"]').addClass("form-control-sm");
                inicializarDatePicker();
                fechaActual();
                listarEmpresasAsociadas('#cboEmpresa');
                $('#cboEmpresa').select2();
                $('#cboCadena').select2();
                $('#cboRegion').select2();
                $('#cboZona').select2();
                $('#cboLocal').select2();
                $('#cboTipoReporte').select2();
            });
        }
    }

}(jQuery);
