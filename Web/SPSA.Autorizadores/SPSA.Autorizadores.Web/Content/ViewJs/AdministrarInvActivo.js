var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresa';
var urlListarCadenas = baseUrl + 'Maestros/MaeCadena/ListarCadena';
var urlListarRegiones = baseUrl + 'Maestros/MaeRegion/ListarRegion';
var urlListarZonas = baseUrl + 'Maestros/MaeZona/ListarZona';
var urlListarLocales = baseUrl + 'Maestros/MaeLocal/ListarLocal';
var urlListarActivos = baseUrl + 'Inventario/InventarioActivo/ListarLocal';


var urlImportarInvActivo = baseUrl + 'Inventario/InventarioActivo/ImportarExcelInventario';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var AdministrarInvActivo = function () {

    const eventos = function () {

        $("#cboEmpresa").on("change", async function () {
            await cargarComboCadenas();
        });

        $("#cboCadena").on("change", async function () {
            await cargarComboRegiones();
        });

        $("#cboRegion").on("change", async function () {
            await cargarComboZonas();
        });

        $("#cboZona").on("change", async function () {
            await cargarComboLocales();
        });


        $("#btnCargarArchivo").click(function () {
            $("#modalImportarInvActivo").modal('show');
        });

        $("#btnPlantillaArchivo").click(function () {
            descargarPlantillas("Plantilla_InvActivo");
        });

        $("#btnCargarExcelInvActivo").on("click", function () {
            var inputFile = document.getElementById('archivoExcelInvActivo');
            var archivoSeleccionado = inputFile.files.length > 0;

            if (archivoSeleccionado) {
                swal({
                    title: "¿Está seguro importar el archivo?",
                    text: " Sí el código de activo existe, este sera actualizado con los nuevos datos recibidos.",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        importarExcelInvActivo();
                    }
                });
            } else {
                alert('Por favor, seleccione un archivo antes de continuar.');
            }
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

            if (response.Ok) {
                $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(empresa => {
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

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboCadena').empty().append('<option label="Seleccionar"></option>');
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(cadena => {
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
            if (response === undefined) return;
            if (response.Ok) {
                $('#cboRegion').empty().append('<option label="Seleccionar"></option>');
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
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

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboZona').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
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

    const cargarComboLocales = async function () {

        try {
            const response = await listarLocales();

            if (response === undefined) return;

            if (response.Ok) {
                $('#cboLocal').empty().append('<option label="Seleccionar"></option>');
                response.Data.map(region => {
                    $('#cboLocal').append($('<option>', { value: region.CodLocal, text: region.NomLocal }));
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


    const importarExcelInvActivo = function () {
        var archivo = document.getElementById('archivoExcelInvActivo').files[0];
        var formData = new FormData();
        formData.append('archivoExcel', archivo);

        $.ajax({
            url: urlImportarInvActivo,
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
                $("#archivoExcelInvActivo").val(null);
                $("#modalImportarInvActivo").modal('hide');
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

    const descargarPlantillas = function (nombreCarpeta) {
        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
            data: { nombreCarpeta: nombreCarpeta },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                const linkSource = `data:application/zip;base64,` + response.Archivo + '\n';
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


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                //showLoading();
                //closeLoading();
            });
        }
    }

}(jQuery);
