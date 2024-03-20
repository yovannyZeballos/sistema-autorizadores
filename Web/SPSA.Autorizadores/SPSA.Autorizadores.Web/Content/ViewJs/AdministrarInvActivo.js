var urlImportarInvActivo = baseUrl + 'Inventario/AdministrarInventario/ImportarExcelInventario';



var AdministrarInvActivo = function () {

    const eventos = function () {
        $("#btnAbrirModalInvActivo").click(function () {
            $("#modalImportarInvActivo").modal('show');
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



    return {
        init: function () {
            checkSession(function () {
                eventos();
                showLoading();
                //await cargarArbolEmpresas();
                closeLoading();
            });
        }
    }

}(jQuery);
