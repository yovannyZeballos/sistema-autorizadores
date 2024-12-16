var urlCambioLocal = baseUrl + 'Seguridad/CambioLocal/Index';
var urlEmpresas = baseUrl + 'Login/ListarEmpresas';
var urlLocal = baseUrl + 'Login/ListarLocales';
var urlGuardarLocalSession = baseUrl + 'Login/GuardarLocalSession';

const urlListarEmpresasAsociadas = baseUrl + 'Empresa/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = baseUrl + 'Maestros/MaeCadena/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = baseUrl + 'Maestros/MaeRegion/ListarRegionesAsociadas';
const urlListarZonasAsociadas = baseUrl + 'Maestros/MaeZona/ListarZonasAsociadas';
const urlListarLocalesAsociados = baseUrl + 'Local/ListarLocalesAsociadas';

var urlListarValesRedimidos = baseUrl + 'Reportes/ValesRedimidos/Descargar';
var urlListarValesRedimidosPaginado = baseUrl + 'Reportes/ValesRedimidos/ListarPaginado';

var dataTableRegistros = null;

var ValesRedimidos = function () {

    var eventos = function () {

        $("#btnConsultar").on('click', function () {
            visualizarDataTable(urlListarValesRedimidosPaginado);
        });

        $('#btnDescargar').click(function () {
            descargarExcel();
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

        var dataTableRegistrosId = "#tableReportes";

        if ($.fn.DataTable.isDataTable(dataTableRegistrosId)) {
            $(dataTableRegistrosId).DataTable().clear().destroy();
            $(dataTableRegistrosId + " tbody").empty();
            $(dataTableRegistrosId + " thead").empty();
        }

        showLoading();

        $.ajax({
            url: dtUrl,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                CodLocal: request.CodLocal,
                FechaInicio: request.FechaInicio,
                FechaFin: request.FechaFin,
                draw: 1,
                startRow: 1,
                pageSize: 50
            }),
            success: function (response) {
                closeLoading();

                if (!response.Ok) {
                    swal({
                        text: "Ocurrió un error: " + response.Mensaje,
                        icon: "error",
                    });
                    return;
                }

                const columnas = [];
                const totalColumnas = response.Columnas.length;
                if (response.Columnas && response.Columnas.length > 0) {
                    response.Columnas.forEach((col, index) => {
                        columnas.push({
                            title: col,
                            data: col.replace(" ", "").replace(".", ""),
                            defaultContent: "",
                            orderable: false,
                            visible: index < totalColumnas - 2
                        });
                    });
                }

                dataTableRegistros = $(dataTableRegistrosId).DataTable({
                    pageLength: 50,
                    searching: false,
                    data: response.Data,
                    columns: columnas,
                    serverSide: true,
                    processing: true,
                    paging: true,
                    scrollY: "400px",
                    scrollX: true,
                    ajax: {
                        url: dtUrl,
                        type: "POST",
                        contentType: "application/json",
                        data: function (d) {
                            return JSON.stringify({
                                Draw: d.draw,
                                StartRow: d.start + 1,
                                PageSize: d.length,
                                CodLocal: request.CodLocal,
                                FechaInicio: request.FechaInicio,
                                FechaFin: request.FechaFin
                            });
                        },
                        dataSrc: function (json) {
                            if (!json || !json.Columnas || !json.Data) {
                                swal({
                                    text: "La respuesta del servidor no contiene las propiedades esperadas.",
                                    icon: "error",
                                });
                                return [];
                            }
                            return json.Data;
                        }
                    },
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                        lengthMenu: "Mostrar _MENU_ registros por página",
                        zeroRecords: "No se encontraron resultados",
                        info: "Mostrando página _PAGE_ de _PAGES_",
                        infoEmpty: "No hay registros disponibles",
                        infoFiltered: "(filtrado de _MAX_ registros totales)"
                    },
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                closeLoading();
                swal({
                    text: "Error de conexión: " + textStatus,
                    icon: "error",
                });
            }
        });
    };

    function descargarExcel() {

        showLoading();

        fetch(urlListarValesRedimidos, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "X-CSRF-TOKEN": $('input[name="__RequestVerificationToken"]').val() || ""
            },

            body: JSON.stringify({
                CodLocal: $("#cboLocal").val(),
                FechaInicio: $("#txtFechaInicio").val(),
                FechaFin: $("#txtFechaFin").val()
            })
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(err => {
                        throw new Error(err.mensaje || "Error desconocido en la descarga");
                    });
                }
                return response.blob(); // Convertir la respuesta a Blob
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url;
                a.download = "ValesRedimidos.xlsx";
                document.body.appendChild(a);
                a.click();
                a.remove();

                window.URL.revokeObjectURL(url);
            })
            .catch(error => {
                swal({
                    text: "Ocurrió un error: " + error.message,
                    icon: "error",
                });
            })
            .finally(() => {
                closeLoading();
            });
    }

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
