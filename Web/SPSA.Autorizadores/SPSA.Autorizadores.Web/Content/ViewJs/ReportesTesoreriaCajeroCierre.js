var urlListarEmpresas = baseUrl + 'Monitor/ReporteTesoreria/ListarEmpresas';
var urlListarCadenas = baseUrl + 'Monitor/ReporteTesoreria/ListarCadenas';
var urlListarRegiones = baseUrl + 'Monitor/ReporteTesoreria/ListarRegiones';
var urlListarZonas = baseUrl + 'Monitor/ReporteTesoreria/ListarZonas';
var urlListarLocales = baseUrl + 'Monitor/ReporteTesoreria/ListarLocales';

var urlObtenerEmpresa = baseUrl + 'Maestros/Empresa/ObtenerEmpresa';

const urlReporteCierre = '/Monitor/ReporteCierre/ReporteCierre';
const urlReporteCierrePivot = '/Monitor/ReporteCierre/ReportePivotCierre';

var dataTableReporte = null;
var listadoReporte = [];

const ReportesTesoreriaCajeroCierre = function () {

    var eventos = function () {
        $("#cboEmpresa").on("change", async function () {
            await cargarComboCadenas();
        });

        $("#cboCadena").on("change", async function () {
            await cargarComboRegiones();
        });

        $("#cboRegion").on("change", async function () {
            await cargarComboZonas();
        });

        $("#cboRegion").on("change", async function () {
            //await cargarComboLocales();
        });

        ///////

        $("#btnConsultar").on('click', function () {
            if (!validar()) return;
            visualizarDataTableReporte();
            listarReporte();
        });

        $('#tableReportes').on('click', 'td', function () {
            var row = dataTableReporte.row(this).data();
            var column = dataTableReporte.column(this).header().innerText;

            let dato = [];
            dato = listadoReporte.filter(item => item.LOC_DESCRIPCION == row.Local && item.CIE_FCONTABLE_FORMAT == column);

            if (dato.length > 0) {

                var div = document.createElement("div");
                div.innerHTML = '<h1 class="text-success"><i class="feather feather-alert-circle"></i></h1>';
                div.innerHTML += `<p>Local: <b>${dato[0].LOC_DESCRIPCION}</b></p>`
                div.innerHTML += `<p>Venta del: <b>${dato[0].CIE_FCONTABLE}</b></p>`
                div.innerHTML += `<p>Cerrado el: <b>${dato[0].CIE_FCIERRE}</b></p>`
                swal({
                    content: div,
                    //text: dato[0].LOC_DESCRIPCION + '\n' + dato[0].CIE_FCONTABLE,
                });
            }

        })
    }

    function filtrarEmpresas(empresas) {
        return empresas.filter(function (empresa) {
            return empresa.IndAsociado === true;
        });
    }

    function filtrarCadenas(cadenas) {
        return cadenas.filter(function (cadena) {
            return cadena.IndAsociado === true;
        });
    }

    function filtrarRegiones(regiones) {
        return regiones.filter(function (region) {
            return region.IndAsociado === true;
        });
    }

    function filtrarZonas(zonas) {
        return zonas.filter(function (zona) {
            return zona.IndAsociado === true;
        });
    }

    function filtrarLocales(locales) {
        return locales.filter(function (local) {
            return local.IndAsociado === true;
        });
    }

    const listarEmpresas = function () {
        return new Promise((resolve, reject) => {

            const request = {
            };

            $.ajax({
                url: urlListarEmpresas,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });

    }

    const listarCadenas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa
            };

            $.ajax({
                url: urlListarCadenas,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const listarRegiones = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
            };

            $.ajax({
                url: urlListarRegiones,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const listarZonas = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();

            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion
            };

            $.ajax({
                url: urlListarZonas,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });

    }

    const listarLocales = function () {
        return new Promise((resolve, reject) => {
            const codEmpresa = $("#cboEmpresa").val();
            const codCadena = $("#cboCadena").val();
            const codRegion = $("#cboRegion").val();
            const codZona = $("#cboZona").val();

            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();
            if (!codZona) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion,
                CodZona: codZona
            };

            $.ajax({
                url: urlListarLocales,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });

    }

    const cargarComboEmpresa = async function () {

        try {
            const response = await listarEmpresas();
            var empresasConAsociado = filtrarEmpresas(response.Data);

            if (response.Ok) {
                $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
                empresasConAsociado.map(empresa => {
                    $('#cboEmpresa').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
                });
                //$('#cboEmpresa').val("001");
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const cargarComboCadenas = async function () {

        try {
            const response = await listarCadenas();

            var cadenasConAsociado = filtrarCadenas(response.Data);

            if (response === undefined) return;
            if (response.Ok) {
                $('#cboCadena').empty().append('<option label="Seleccionar"></option>');
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                cadenasConAsociado.map(cadena => {
                    $('#cboCadena').append($('<option>', { value: cadena.CodCadena, text: cadena.NomCadena }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const cargarComboRegiones = async function () {

        try {
            const response = await listarRegiones();
            var regionesConAsociado = filtrarRegiones(response.Data);

            if (response === undefined) return;
            if (response.Ok) {
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                regionesConAsociado.map(region => {
                    $('#cboRegion').append($('<option>', { value: region.CodRegion, text: region.NomRegion }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const cargarComboZonas = async function () {

        try {
            const response = await listarZonas();
            var zonasConAsociado = filtrarZonas(response.Data);

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                zonasConAsociado.map(region => {
                    $('#cboZona').append($('<option>', { value: region.CodZona, text: region.NomZona }));
                });
            } else {
                swal({
                    text: response.Mensaje,
                    icon: "error"
                });
                return;
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
        }
    }

    const cargarAños = function () {

        const date = new Date()
        const añoActual = date.getFullYear();
        const añoInical = 2018;
        const añoFinal = añoActual + 5;

        $('#cboAño').empty().append('<option label="Seleccionar"></option>');

        for (var i = añoInical; i <= añoFinal; i++) {
            $('#cboAño').append($('<option>', { value: i, text: i }));
        }

    }

    const cargarMeses = function () {

        $('#cboMes').empty().append('<option label="Seleccionar"></option>');
        $('#cboMes').append($('<option>', { value: 1, text: 'Enero' }));
        $('#cboMes').append($('<option>', { value: 2, text: 'Febrero' }));
        $('#cboMes').append($('<option>', { value: 3, text: 'Marzo' }));
        $('#cboMes').append($('<option>', { value: 4, text: 'Abril' }));
        $('#cboMes').append($('<option>', { value: 5, text: 'Mayo' }));
        $('#cboMes').append($('<option>', { value: 6, text: 'Junio' }));
        $('#cboMes').append($('<option>', { value: 7, text: 'Julio' }));
        $('#cboMes').append($('<option>', { value: 8, text: 'Agosto' }));
        $('#cboMes').append($('<option>', { value: 9, text: 'Setiembre' }));
        $('#cboMes').append($('<option>', { value: 10, text: 'Octubre' }));
        $('#cboMes').append($('<option>', { value: 11, text: 'Noviembre' }));
        $('#cboMes').append($('<option>', { value: 12, text: 'Diciembre' }));

    }

    const obtenerEmpresa = function (codEmpresa) {
        return new Promise((resolve, reject) => {
            if (!codEmpresa) return resolve();

            const request = {
                CodEmpresa: codEmpresa
            };

            $.ajax({
                url: urlObtenerEmpresa,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const visualizarDataTableReporte = async function () {

        const objEmpresa = await obtenerEmpresa($("#cboEmpresa").val());

        const request = {
            CodEmpresa: objEmpresa.Data.CodSociedad,
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val()
        };

        $.ajax({
            url: urlReporteCierrePivot,
            type: "post",
            data: { request },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    if (x != "Local")
                        columnas.push({
                            title: x.replace("_", " "),
                            data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                            className: "estado",
                        });
                    else
                        columnas.push({
                            title: x.replace("_", " "),
                            data: quitarTildes(x).replace(/ /g, "").replace(".", "")
                        });

                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                if (dataTableReporte != null) {
                    dataTableReporte.clear();
                    dataTableReporte.destroy();
                    dataTableReporte = null;
                }

                var tableReportesId = "#tableReportes";
                $(tableReportesId + " tbody").empty();
                $(tableReportesId + " thead").empty();

                const listaLocales = await listarLocales();
                var localesConAsociado = filtrarLocales(listaLocales.Data);
                /*console.log(localesConAsociado);*/

                var listaDatosFiltrada = response.Data.filter(function (localData) {
                    return localesConAsociado.some(function (local) {
                            return localData.Local.includes(local.CodLocal);
                        });
                });

                /*console.log(listaDatosFiltrada);*/


                dataTableReporte = $('#tableReportes').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    data: listaDatosFiltrada,
                    columns: columnas,
                    bAutoWidth: false,
                    rowCallback: function (row, data, i) {
                        var keys = Object.keys(data);
                        keys.sort().forEach((k, index) => {

                            if (data[k] === "A") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-danger");
                            }
                            else if (data[k] === "P") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-warning");
                            }
                            else if (data[k] === "C") {
                                $(`td:eq(${index + 1})`, row).addClass("bg-success");
                            }
                        });
                    },
                    buttons: [
                        {
                            extend: 'excel',
                            text: '<i class="fa fa-cloud-download"></i> Excel',
                            titleAttr: 'Descargar Excel',
                            className: 'btn-sm mb-1 ms-2',
                            exportOptions: {
                                modifier: { page: 'all' }
                            }
                        },
                    ],
                });

                dataTableReporte.buttons().container().prependTo($('#tableReportes_filter'));
                $('input[type="search"]').addClass("form-control-sm");
                $('#tableReportes_filter button').addClass("btn-sm");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const validar = function () {
        let mensajes = '';
        let ok = true;
        const codEmpresa = $("#cboEmpresa").val();
        const codCadena = $("#cboCadena").val();
        const codRegion = $("#cboRegion").val();
        const codZona = $("#cboZona").val();
        const año = $("#cboAño").val();
        const mes = $("#cboMes").val();

        if (codEmpresa === null || codEmpresa === '') {
            mensajes = mensajes + 'Debe seleccionar la empresa \n';
            ok = false;
        }

        if (codCadena === null || codCadena === '') {
            mensajes = mensajes + 'Debe seleccionar la empresa \n';
            ok = false;
        }

        if (codRegion === null || codRegion === '') {
            mensajes = mensajes + 'Debe seleccionar la empresa \n';
            ok = false;
        }

        if (codZona === null || codZona === '') {
            mensajes = mensajes + 'Debe seleccionar la empresa \n';
            ok = false;
        }

        if (año === '' || año === null) {
            mensajes = mensajes + 'Debe ingresar el Año \n';
            ok = false;
        }

        if (mes === '' || mes === null) {
            mensajes = mensajes + 'Debe ingresar el Mes \n';
            ok = false;
        }

        if (!ok)
            swal({
                text: mensajes,
                icon: "warning"
            });

        return ok;
    }



    //////////////

    

    const fechaActual = function () {
        let date = new Date()

        let month = `${(date.getMonth() + 1)}`;
        let year = date.getFullYear();

        $("#cboAño").val(year);
        $("#cboMes").val(month);
    }

    const listarReporte = function () {
        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            Año: $("#cboAño").val(),
            Mes: $("#cboMes").val()
        };

        $.ajax({
            url: urlReporteCierre,
            type: "post",
            data: { request },
            dataType: "json",
            success: function (response) {
                listadoReporte = response.Data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error", });
            }
        });
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                showLoading();
                cargarAños();
                cargarMeses();
                fechaActual();
                await cargarComboEmpresa();
                closeLoading();
              
            });
        }
    }

}(jQuery);