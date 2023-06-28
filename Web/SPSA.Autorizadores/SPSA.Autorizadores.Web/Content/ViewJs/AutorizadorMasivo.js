const urlEmpresas = baseUrl + 'Locales/AdministrarLocal/ListarEmpresas';
const urlFormatos = baseUrl + 'Locales/AdministrarLocal/ListarFormatos';
const urlLocales = baseUrl + 'Locales/AdministrarLocal/ListarLocales';
const urlAsignar = baseUrl + 'Autorizadores/AutorizadorMasivo/AsignarAutorizador';

var dataTableLocales = null;

const AutorizadorMasivo = function () {


    const eventos = function () {

        $("#cboEmpresa").on("change", function () {
            listarFormatos();
        });


        $("#cboFormato").on("change", function () {
            recargarDataTableLocales();

        });

        $("#btnAsignar").on("click", function () {
            asignarAutorizador();
        });


        $('#tableLocal tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#chkTodos").on("change", function () {
            chechTodos();
        })

    }

    const validarSelecion = function (count, unSoloRegistro = false) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar como mínimo un registro",
                icon: "warning",
            });
            return false;
        }

        if (unSoloRegistro && count > 1) {
            swal({
                text: "Solo debe seleccionar un registro",
                icon: "warning",
            });
            return false;
        }

        return true;

    }

    const asignarAutorizador = function () {

        const registrosSeleccionados = dataTableLocales.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let locales = [];

        registrosSeleccionados.map((item) => {
            locales.push({
                CodLocal: item.CodLocal,
                TipoSo: item.TIP_OS,
            });
        });

        $.ajax({
            url: urlAsignar,
            type: "post",
            data: {
                request: { Locales: locales }

            },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "error" });
                    return;
                }

                console.log(response);

                const linkSource = `data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                const fileName = response.NombreArchivo;
                downloadLink.href = linkSource;
                downloadLink.download = fileName;
                downloadLink.click();

                swal({
                    text: response.Mensaje,
                    icon: "success"
                });

                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnAsignar"), false);
            }
        });
    }

    const listarEmpresas = function () {
        $.ajax({
            url: urlEmpresas,
            type: "post",
            success: function (response) {

                if (response.Ok) {
                    $('#cboEmpresa').empty().append('<option label="Seleccionar"></option>');
                    response.Empresas.map(empresa => {
                        $('#cboEmpresa').append($('<option>', { value: empresa.Codigo, text: empresa.Descripcion }));
                    });
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });

    }

    const listarFormatos = function () {
        const codEmpresa = $("#cboEmpresa").val();
        if (!codEmpresa) return;

        const request = {
            CodEmpresa: codEmpresa
        };

        $.ajax({
            url: urlFormatos,
            type: "post",
            data: { request },
            success: function (response) {
                if (response === undefined) return;
                if (response.Ok) {
                    $('#cboFormato').empty().append('<option label="Seleccionar"></option>');
                    response.Formatos.map(formato => {
                        $('#cboFormato').append($('<option>', { value: formato.CodFormato, text: formato.Nombre }));
                    });
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                    return;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });

    }

    const visualizarDataTableLocales = function () {

        $.ajax({
            url: urlLocales,
            type: "post",
            data: { request: {} },
            dataType: "json",
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u")
                    });
                });

                dataTableLocales = $('#tableLocal').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '320px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    data: response.Locales,
                    bAutoWidth: false,
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

    const recargarDataTableLocales = function () {

        const request = {
            CodEmpresa: $("#cboEmpresa").val(),
            CodFormato: $("#cboFormato").val()
        };

        $.ajax({
            url: urlLocales,
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
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });

                    dataTableLocales.clear();
                    dataTableLocales.draw();
                    return;

                }

                dataTableLocales.clear();
                dataTableLocales.rows.add(response.Locales);
                dataTableLocales.draw();
                dataTableLocales.columns.adjust().draw();

                setTimeout(() => {
                    dataTableLocales.columns.adjust().draw();
                }, 1000);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const chechTodos = function () {

        if ($("#chkTodos").prop('checked')) {
            dataTableLocales.rows({ search: 'applied' }).nodes().each(function () {
                $(this).addClass('selected');
            });
        } else {
            dataTableLocales.rows({ search: 'applied' }).nodes().each(function () {
                $(this).removeClass('selected');
            });
        }


    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                listarEmpresas();
                visualizarDataTableLocales();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery);