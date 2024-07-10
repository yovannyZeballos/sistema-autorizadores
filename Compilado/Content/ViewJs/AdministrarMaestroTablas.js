var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresa';
var urlListarCadenas = baseUrl + 'Maestros/MaeCadena/ListarCadena';
var urlListarRegiones = baseUrl + 'Maestros/MaeRegion/ListarRegion';
var urlListarZonas = baseUrl + 'Maestros/MaeZona/ListarZona';
var urlListarLocales = baseUrl + 'Maestros/MaeLocal/ListarLocal';

var urlObtenerEmpresa = baseUrl + 'Maestros/MaeEmpresa/ObtenerEmpresa';
var urlObtenerCadena = baseUrl + 'Maestros/MaeCadena/ObtenerCadena';
var urlObtenerRegion = baseUrl + 'Maestros/MaeRegion/ObtenerRegion';
var urlObtenerZona = baseUrl + 'Maestros/MaeZona/ObtenerZona';
var urlObtenerLocal = baseUrl + 'Maestros/MaeLocal/ObtenerLocal';

var urlCrearEmpresa = baseUrl + 'Maestros/MaeEmpresa/CrearEmpresa';
var urlCrearCadena = baseUrl + 'Maestros/MaeCadena/CrearCadena';
var urlCrearRegion = baseUrl + 'Maestros/MaeRegion/CrearRegion';
var urlCrearZona = baseUrl + 'Maestros/MaeZona/CrearZona';
var urlCrearLocal = baseUrl + 'Maestros/MaeLocal/CrearLocal';

var urlActualizarEmpresa = baseUrl + 'Maestros/MaeEmpresa/ActualizarEmpresa';
var urlActualizarCadena = baseUrl + 'Maestros/MaeCadena/ActualizarCadena';
var urlActualizarRegion = baseUrl + 'Maestros/MaeRegion/ActualizarRegion';
var urlActualizarZona = baseUrl + 'Maestros/MaeZona/ActualizarZona';
var urlActualizarLocal = baseUrl + 'Maestros/MaeLocal/ActualizarLocal';

var urlImportarEmpresa = baseUrl + 'Maestros/MaeMaeEmpresa/ImportarExcelEpresa';
var urlImportarCadena = baseUrl + 'Maestros/MaeCadena/ImportarExcelCadena';
var urlImportarRegion = baseUrl + 'Maestros/MaeRegion/ImportarExcelRegion';
var urlImportarZona = baseUrl + 'Maestros/MaeZona/ImportarExcelZona';
var urlImportarLocal = baseUrl + 'Maestros/MaeLocal/ImportarExcelLocal';

var urlModalCrearEditarEmpresa = baseUrl + 'Maestros/MaeTablas/CrearEditarEmpresa';
var urlModalCrearEditarCadena = baseUrl + 'Maestros/MaeTablas/CrearEditarCadena';
var urlModalCrearEditarRegion = baseUrl + 'Maestros/MaeTablas/CrearEditarRegion';
var urlModalCrearEditarZona = baseUrl + 'Maestros/MaeTablas/CrearEditarZona';
var urlModalCrearEditarLocal = baseUrl + 'Maestros/MaeTablas/CrearEditarLocal';

var urlDescargarLocalPorEmpresa = baseUrl + 'Maestros/MaeLocal/DescargarLocalPorEmpresa';

var AdministrarMaestroTablas = function () {

    const eventos = function () {

        $("#btnAbrirModalEmpresa").click(function () {
            $("#modalImportarEmpresa").modal('show');
        });

        $("#btnAbrirModalCadena").click(function () {
            $("#modalImportarCadena").modal('show');
        });

        $("#btnAbrirModalRegion").click(function () {
            $("#modalImportarRegion").modal('show');
        });

        $("#btnAbrirModalZona").click(function () {
            $("#modalImportarZona").modal('show');
        });


        $('.tree').on('click', '.empresa, .cadena, .region, .zona, .crear-cadena-btn, .editar-empresa-btn, .editar-cadena-btn', function () {
            $('.tree li').removeClass('selected');
            $(this).addClass('selected').parents('li').addClass('selected');
        });

        $('.tree').on('click', '.empresa', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent().data('codigo');
            var ulCadenas = $(this).parent('ul');
            cargarArbolCadenas(codEmpresa, ulCadenas);
        });

        $('.tree').on('click', '.cadena', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).data('empresa');
            var codCadena = $(this).data('cadena');
            var ulCadenas = $(this).closest('ul');

            cargarArbolRegiones(codEmpresa, codCadena, ulCadenas);
        });

        $('.tree').on('click', '.region', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).data('empresa');
            var codCadena = $(this).data('cadena');
            var codRegion = $(this).data('region');
            var ulRegiones = $(this).closest('ul');

            cargarArbolZonas(codEmpresa, codCadena, codRegion, ulRegiones);
        });


        $('.crear-empresa-btn').click(function (event) {
            event.stopPropagation();
            abrirModalNuevaEmpresa();
        });

        $('.tree').on('click', '.crear-cadena-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.empresa').data('empresa');
            abrirModalNuevaCadena(codEmpresa);
        });

        $('.tree').on('click', '.crear-region-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.cadena').data('empresa');
            var codCadena = $(this).parent('.cadena').data('cadena');

            abrirModalNuevaRegion(codEmpresa, codCadena);
        });

        $('.tree').on('click', '.crear-zona-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.region').data('empresa');
            var codCadena = $(this).parent('.region').data('cadena');
            var codRegion = $(this).parent('.region').data('region');

            abrirModalNuevaZona(codEmpresa, codCadena, codRegion);

        });


        $('.tree').on('click', '.editar-empresa-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.empresa').data('empresa');
            abrirModalEditarEmpresa(codEmpresa);
        });

        $('.tree').on('click', '.editar-cadena-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.cadena').data('empresa');
            var codCadena = $(this).parent('.cadena').data('cadena');

            abrirModalEditarCadena(codEmpresa, codCadena);

            $("#txtCodCadena").prop("disabled", true);
        });

        $('.tree').on('click', '.editar-region-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.region').data('empresa');
            var codCadena = $(this).parent('.region').data('cadena');
            var codRegion = $(this).parent('.region').data('region');

            abrirModalEditarRegion(codEmpresa, codCadena, codRegion);
        });

        $('.tree').on('click', '.editar-zona-btn', function (event) {
            event.stopPropagation();

            var codEmpresa = $(this).parent('.zona').data('empresa');
            var codCadena = $(this).parent('.zona').data('cadena');
            var codRegion = $(this).parent('.zona').data('region');
            var codZona = $(this).parent('.zona').data('zona');

            abrirModalEditarZona(codEmpresa, codCadena, codRegion, codZona);
        });


        $('.tree').on('click', '.exportar-locales-btn', function (event) {
            event.stopPropagation();
            var codEmpresa = $(this).parent('.empresa').data('empresa');

            swal({
                title: "¿Está seguro exportar el archivo?",
                text: "Puede demorar unos minutos la exportación de locales por empresa...",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    descargarLocalesPorEmpresa(codEmpresa);
                }
            });
        });


        $("#btnCargarExcelEmpresa").on("click", function () {
            var inputFile = document.getElementById('archivoExcelEmpresa');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de empresa existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelEmpresa();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });

        $("#btnCargarExcelCadena").on("click", function () {
            var inputFile = document.getElementById('archivoExcelCadena');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de cadena existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelCadena();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });

        $("#btnCargarExcelRegion").on("click", function () {
            var inputFile = document.getElementById('archivoExcelRegion');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de region existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelRegion();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });

        $("#btnCargarExcelZona").on("click", function () {
            var inputFile = document.getElementById('archivoExcelZona');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de zona existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelZona();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
        });


        $("#btnGuardarEmpresa").on("click", async function () {
            var empresa = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                NomEmpresa: $("#txtNomEmpresa").val(),
                Ruc: $("#txtRuc").val(),
                CodSociedad: $("#txtCodSociedad").val(),
                CodEmpresaOfi: $("#txtCodEmpresaOfi").val()
            };

            if (validarEmpresa(empresa))
                await guardarEmpresa(empresa, urlCrearEmpresa);
        });

        $("#btnActualizarEmpresa").on("click", async function () {
            var empresa = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                NomEmpresa: $("#txtNomEmpresa").val(),
                Ruc: $("#txtRuc").val(),
                CodSociedad: $("#txtCodSociedad").val(),
                CodEmpresaOfi: $("#txtCodEmpresaOfi").val()
            };

            if (validarEmpresa(empresa))
                await guardarEmpresa(empresa, urlActualizarEmpresa);
        });

        $("#btnGuardarCadena").on("click", async function () {

            var cadena = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                CodCadena: $("#txtCodCadena").val(),
                NomCadena: $("#txtNomCadena").val(),
                CadNumero: $("#txtCadNumero").val()
            };

            if (validarCadena(cadena))
                await guardarCadena(cadena, urlCrearCadena);
        });

        $("#btnActualizarCadena").on("click", async function () {

            var cadena = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                CodCadena: $("#txtCodCadena").val(),
                NomCadena: $("#txtNomCadena").val(),
                CadNumero: $("#txtCadNumero").val()
            };

            if (validarCadena(cadena))
                await guardarCadena(cadena, urlActualizarCadena);
        });

        $("#btnGuardarRegion").on("click", async function () {
            var region = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                CodCadena: $("#txtCodCadena").val(),
                CodRegion: $("#txtCodRegion").val(),
                NomRegion: $("#txtNomRegion").val(),
                CodRegional: $("#txtCodRegional").val()
            };

            //console.log(region);

            if (validarRegion(region))
                await guardarRegion(region, urlCrearRegion);
        });

        $("#btnActualizarRegion").on("click", async function () {
            var region = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                CodCadena: $("#txtCodCadena").val(),
                CodRegion: $("#txtCodRegion").val(),
                NomRegion: $("#txtNomRegion").val(),
                CodRegional: $("#txtCodRegional").val()
            };

            if (validarRegion(region))
                await guardarRegion(region, urlActualizarRegion);
        });

        $("#btnGuardarZona").on("click", async function () {
            var zona = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                CodCadena: $("#txtCodCadena").val(),
                CodRegion: $("#txtCodRegion").val(),
                CodZona: $("#txtCodZona").val(),
                NomZona: $("#txtNomZona").val(),
                CodCordina: $("#txtCodCordina").val()
            };

            console.log(zona);

            if (validarZona(zona))
                await guardarZona(zona, urlCrearZona);
        });

        $("#btnActualizarZona").on("click", async function () {
            var zona = {
                CodEmpresa: $("#txtCodEmpresa").val(),
                CodCadena: $("#txtCodCadena").val(),
                CodRegion: $("#txtCodRegion").val(),
                CodZona: $("#txtCodZona").val(),
                NomZona: $("#txtNomZona").val(),
                CodCordina: $("#txtCodCordina").val()
            };

            if (validarZona(zona))
                await guardarZona(zona, urlActualizarZona);
        });

    };


    const descargarLocalesPorEmpresa = function (codEmpresa) {

        const request = {
            CodEmpresa: codEmpresa,
        };

        $.ajax({
            url: urlDescargarLocalPorEmpresa,
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

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                const linkSource = `data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                const fileName = response.NombreArchivo;
                downloadLink.href = linkSource;
                downloadLink.download = fileName;
                downloadLink.click();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }




    const listarEmpresas = function () {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: urlListarEmpresas,
                type: "post",
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const listarCadenas = function (codEmpresa) {
        return new Promise((resolve, reject) => {
            //const codEmpresa = $("#cboEmpresa").val();
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

    const listarRegiones = function (codEmpresa, codCadena) {
        return new Promise((resolve, reject) => {

            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();

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

    const listarZonas = function (codEmpresa, codCadena, codRegion) {
        return new Promise((resolve, reject) => {

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

    const obtenerCadena = function (codEmpresa, codCadena) {
        return new Promise((resolve, reject) => {
            //const codEmpresa = $("#cboEmpresa").val();
            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena
            };

            $.ajax({
                url: urlObtenerCadena,
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

    const obtenerRegion = function (codEmpresa, codCadena, codRegion) {
        return new Promise((resolve, reject) => {
            if (!codEmpresa) return resolve();
            if (!codCadena) return resolve();
            if (!codRegion) return resolve();

            const request = {
                CodEmpresa: codEmpresa,
                CodCadena: codCadena,
                CodRegion: codRegion
            };

            $.ajax({
                url: urlObtenerRegion,
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

    const obtenerZona = function (codEmpresa, codCadena, codRegion, codZona) {
        return new Promise((resolve, reject) => {
            //const codEmpresa = $("#cboEmpresa").val();
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
                url: urlObtenerZona,
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


    const validarEmpresa = function (empresa) {
        let validate = true;

        if (empresa.CodEmpresa === '' || empresa.NomEmpresa === '' || empresa.Ruc === '' || empresa.CodSociedad === '' || empresa.CodEmpresaOfi === '') {
            validate = false;
            $("#formEmpresa").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarCadena = function (cadena) {
        let validate = true;

        if (cadena.CodEmpresa === '' || cadena.CodCadena === '' || cadena.NomCadena === '' || cadena.CadNumero === '') {
            validate = false;
            $("#formCadena").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarRegion = function (region) {
        let validate = true;

        if (region.CodEmpresa === '' || region.CodCadena === '' || region.CodRegion === '' || region.NomRegion === '' || region.CodRegional === '') {
            validate = false;
            $("#formRegion").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarZona = function (zona) {
        let validate = true;

        if (zona.CodEmpresa === '' || zona.CodCadena === '' || zona.CodRegion === '' || zona.CodZona === '' || zona.NomZona === '' || zona.CodCordina === '') {
            validate = false;
            $("#formZona").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }



    const guardarEmpresa = function (empresa, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: empresa },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                await cargarArbolEmpresas();
                $("#modalEmpresa").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarCadena = function (cadena, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: cadena },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                await cargarArbolEmpresas();
                $("#modalCadena").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarRegion = function (region, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: region },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                await cargarArbolEmpresas();
                $("#modalRegion").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarZona = function (zona, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: zona },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                await cargarArbolEmpresas();
                $("#modalZona").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }


    const cargarArbolEmpresas = async function () {

        try {
            const response = await listarEmpresas();

            if (response.Ok) {

                var treeRoot = $('#tree-root');
                treeRoot.empty();
                response.Data.forEach(function (empresa) {
                    var empresaNode = $('<ul><li class="empresa" data-empresa="' + empresa.CodEmpresa + '"><b>' + empresa.CodEmpresa + ' | ' + empresa.Ruc + ' | ' + empresa.NomEmpresa + '</b> | ' + empresa.CodSociedad + ' | ' + empresa.CodEmpresaOfi + '<span class="editar-empresa-btn">Editar</span><span class="crear-cadena-btn">(+)Cadena</span><span class="exportar-locales-btn">(+)Exportar Locales</span></li></ul>');
                    empresaNode.data('codigo', empresa.CodEmpresa);
                    treeRoot.append(empresaNode);
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

    const cargarArbolCadenas = async function (codEmpresa, ulCadenas) {
        try {
            if (!ulCadenas.children('ul').hasClass('loaded-cadena')) {
                const response = await listarCadenas(codEmpresa);
                if (response.Ok) {
                    var cadenas = response.Data;
                    if (cadenas.length > 0) {
                        var cadenasNode = $('<ul></ul>');
                        cadenas.forEach(function (cadena) {
                            var cadenaItem = $('<li class="cadena" data-empresa="' + cadena.CodEmpresa + '" data-cadena="' + cadena.CodCadena + '"><b>' + cadena.CodCadena + ' | ' + cadena.NomCadena + '</b> | ' + cadena.CadNumero + '<span class="editar-cadena-btn">Editar</span><span class="crear-region-btn">(+)Región</span></li>');
                            cadenasNode.append(cadenaItem);
                        });
                        ulCadenas.append(cadenasNode);
                        ulCadenas.children('ul').toggleClass('loaded-cadena', true);
                    } else {
                        swal({
                            text: 'No se ha encontrado listado de cadenas para esta empresa ',
                            icon: "warning"
                        });
                    }

                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            }
            else {
                ulCadenas.children('ul').empty().toggleClass('loaded-cadena', false);
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
            return;
        }
    }

    const cargarArbolRegiones = async function (codEmpresa, codCadena, ulCadenas) {
        try {
            var clickedCadena = ulCadenas.find('.cadena[data-cadena="' + codCadena + '"]');

            if (!clickedCadena.hasClass('loaded-region')) {
                const response = await listarRegiones(codEmpresa, codCadena);

                if (response.Ok) {
                    var regiones = response.Data;

                    if (regiones.length > 0) {
                        var regionesNode = $('<ul></ul>');

                        regiones.forEach(function (region) {
                            var regionItem = $('<li class="region" data-empresa="' + region.CodEmpresa + '" data-cadena="' + region.CodCadena + '" data-region="' + region.CodRegion + '"><b>' + region.CodRegion + ' | ' + region.NomRegion + '</b><span class="editar-region-btn">Editar</span><span class="crear-zona-btn">(+)Zona</span></li>');
                            regionesNode.append(regionItem);
                        });

                        clickedCadena.after(regionesNode);
                        clickedCadena.addClass('loaded-region');
                    } else {
                        swal({
                            text: 'No se ha encontrado listado de regiones para esta cadena',
                            icon: "warning"
                        });
                    }
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            } else {
                clickedCadena.next('ul').empty().remove();
                clickedCadena.removeClass('loaded-region');
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
            return;
        }
    }

    const cargarArbolZonas = async function (codEmpresa, codCadena, codRegion, ulRegiones) {
        try {
            var clickedRegion = ulRegiones.find('.region[data-region="' + codRegion + '"]');

            if (!clickedRegion.hasClass('loaded-zona')) {
                const response = await listarZonas(codEmpresa, codCadena, codRegion);

                if (response.Ok) {
                    var zonas = response.Data;

                    if (zonas.length > 0) {
                        var zonasNode = $('<ul></ul>');

                        zonas.forEach(function (zona) {
                            var zonaItem = $('<li class="zona" data-empresa="' + zona.CodEmpresa + '" data-cadena="' + zona.CodCadena + '" data-region="' + zona.CodRegion + '" data-zona="' + zona.CodZona + '"><b>' + zona.CodZona + ' | ' + zona.NomZona + '</b><span class="editar-zona-btn">Editar</span></li>');
                            zonasNode.append(zonaItem);
                        });

                        clickedRegion.after(zonasNode);
                        clickedRegion.addClass('loaded-zona');
                    } else {
                        swal({
                            text: 'No se ha encontrado listado de regiones para esta cadena',
                            icon: "warning"
                        });
                    }
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            } else {
                clickedRegion.next('ul').empty().remove();
                clickedRegion.removeClass('loaded-zona');
            }
        } catch (error) {
            swal({
                text: error,
                icon: "error"
            });
            return;
        }
    }


    const abrirModalNuevaEmpresa = async function () {
        $("#tituloModalEmpresa").html("Nueva Empresa");
        $("#btnActualizarEmpresa").hide();
        $("#btnGuardarEmpresa").show();
        const model = {};

        await cargarFormEmpresa(model, false);
    }

    const abrirModalNuevaCadena = async function (codEmpresa) {
        $("#tituloModalCadena").html("Nueva Cadena");
        $("#btnActualizarCadena").hide();
        $("#btnGuardarCadena").show();

        const model = {};
        model.CodEmpresa = codEmpresa;

        await cargarFormCadena(model, false);
    }

    const abrirModalNuevaRegion = async function (codEmpresa, codCadena) {
        $("#tituloModalRegion").html("Nueva Región");
        $("#btnActualizarRegion").hide();
        $("#btnGuardarRegion").show();

        const model = {};
        model.CodEmpresa = codEmpresa;
        model.CodCadena = codCadena;

        await cargarFormRegion(model, false);
    }

    const abrirModalNuevaZona = async function (codEmpresa, codCadena, codRegion) {
        $("#tituloModalZona").html("Nueva Zona");
        $("#btnActualizarZona").hide();
        $("#btnGuardarZona").show();

        const model = {};
        model.CodEmpresa = codEmpresa;
        model.CodCadena = codCadena;
        model.CodRegion = codRegion;

        await cargarFormZona(model, false);
    }


    const abrirModalEditarEmpresa = async function (codEmpresa) {
        $("#tituloModalEmpresa").html("Editar Empresa");
        $("#btnActualizarEmpresa").show();
        $("#btnGuardarEmpresa").hide();

        const response = await obtenerEmpresa(codEmpresa);
        const model = response.Data;

        await cargarFormEmpresa(model, true);
    }

    const abrirModalEditarCadena = async function (codEmpresa, codCadena) {
        $("#tituloModalCadena").html("Editar Cadena");
        $("#btnActualizarCadena").show();
        $("#btnGuardarCadena").hide();

        const response = await obtenerCadena(codEmpresa, codCadena);
        const model = response.Data;

        await cargarFormCadena(model, true);
    }

    const abrirModalEditarRegion = async function (codEmpresa, codCadena, codRegion) {
        $("#tituloModalRegion").html("Editar Región");
        $("#btnActualizarRegion").show();
        $("#btnGuardarRegion").hide();

        const response = await obtenerRegion(codEmpresa, codCadena, codRegion);
        const model = response.Data;

        await cargarFormRegion(model, true);
    }

    const abrirModalEditarZona = async function (codEmpresa, codCadena, codRegion, codZona) {
        $("#tituloModalZona").html("Editar Zona");
        $("#btnActualizarZona").show();
        $("#btnGuardarZona").hide();

        const response = await obtenerZona(codEmpresa, codCadena, codRegion, codZona);
        const model = response.Data;

        await cargarFormZona(model, true);
    }


    const cargarFormEmpresa = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarEmpresa,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalEmpresa").find(".modal-body").html(response);
                $("#modalEmpresa").modal('show');

                $("#txtCodEmpresa").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormCadena = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarCadena,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalCadena").find(".modal-body").html(response);
                $("#modalCadena").modal('show');

                $("#txtCodEmpresa").prop("disabled", deshabilitar);
                $("#txtCodCadena").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormRegion = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarRegion,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalRegion").find(".modal-body").html(response);
                $("#modalRegion").modal('show');

                $("#txtCodEmpresa").prop("disabled", deshabilitar);
                $("#txtCodCadena").prop("disabled", deshabilitar);
                $("#txtCodRegion").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormZona = async function (model, deshabilitar) {
        $.ajax({
            url: urlModalCrearEditarZona,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalZona").find(".modal-body").html(response);
                $("#modalZona").modal('show');

                $("#txtCodEmpresa").prop("disabled", deshabilitar);
                $("#txtCodCadena").prop("disabled", deshabilitar);
                $("#txtCodRegion").prop("disabled", deshabilitar);
                $("#txtCodZona").prop("disabled", deshabilitar);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }


    const importarExcelEmpresa = function () {
        var archivo = document.getElementById('archivoExcelEmpresa').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarEmpresa,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#archivoExcelEmpresa").val(null);
                $("#modalImportarEmpresa").modal('hide');
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErroresCaja').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarExcelCadena = function () {
        var archivo = document.getElementById('archivoExcelCadena').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarCadena,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#archivoExcelCadena").val(null);
                $("#modalImportarCadena").modal('hide');
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErroresCaja').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarExcelRegion = function () {
        var archivo = document.getElementById('archivoExcelRegion').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarRegion,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#archivoExcelRegion").val(null);
                $("#modalImportarRegion").modal('hide');
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErroresCaja').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarExcelZona = function () {
        var archivo = document.getElementById('archivoExcelZona').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarZona,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#archivoExcelZona").val(null);
                $("#modalImportarZOna").modal('hide');
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErroresCaja').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                showLoading();
                await cargarArbolEmpresas();
                closeLoading();
            });
        }
    }

}(jQuery);
