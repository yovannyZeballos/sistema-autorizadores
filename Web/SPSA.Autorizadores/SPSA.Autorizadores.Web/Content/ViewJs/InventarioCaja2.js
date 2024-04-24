var urlImportarInventario = baseUrl + 'Inventario/InventarioCaja/Importar';
var urlDescargarPlantilla = baseUrl + 'Inventario/InventarioCaja/DescargarPlantillas';

const InventarioCaja = function () {


    const eventos = function () {

        $("#btnImportar").on("click", function () {
            $("#excelInventario").trigger("click");
        });

        $('#excelInventario').change(function (e) {
            importarExcelInventario();
        });

        $("#btnDescargarPlantillas").on("click", function () {
            descargarPlantillas();
        });
    }

    const importarExcelInventario = function () {

        var formData = new FormData();
        var uploadFiles = $('#excelInventario').prop('files');
        formData.append("excelInventario", uploadFiles[0]);

        $.ajax({
            url: urlImportarInventario,
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
                $("#excelInventario").val(null);
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
                            $('#modalErroresImportacionCaja').modal("show");
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

    const descargarPlantillas = function () {

        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
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
            });
        }
    }
}(jQuery);