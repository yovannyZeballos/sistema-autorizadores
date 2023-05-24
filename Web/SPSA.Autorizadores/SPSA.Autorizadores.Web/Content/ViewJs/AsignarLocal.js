var urlLocalesAsignar = baseUrl + 'Autorizadores/AsignarLocal/ListarLocalesAsignar';
var urlAsignar = baseUrl + 'Autorizadores/AsignarLocal/Asignar';
var urlFormAsignarLocalPMM = baseUrl + 'Autorizadores/AsignarLocal/FormAsociarLocalPMM';
var urlAsociarLocalPMM = baseUrl + 'Autorizadores/AsignarLocal/AsociarLocalPMM';


var AsignarLocal = function () {

    var eventos = function () {

        $("#btnAsignar").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro asignar los locales?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        asignarLocal();
                    }
                });
        });

        $('#tableLocalesAsignar tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#cboEmpresa").on("change", function () {
            const valor = $(this).val();
            let regExSearch = '\\b' + valor + '\\b';

            if (valor === "0") {
                dataTableLocalesAsignar.column(1).search('').draw();

            }
            else if (valor === "4") {
                regExSearch = "^[4-8]$";
                dataTableLocalesAsignar.column(1).search(regExSearch, true, false).draw();
            }
            else {
                dataTableLocalesAsignar.column(1).search(regExSearch, true, false).draw();
            }
        });

        $("#cboInd").on("change", function () {
            const valor = $(this).val();
            let regExSearch = '\\b' + valor + '\\b';

            if (valor === "0") {
                dataTableLocalesAsignar.column(5).search('').draw();

            }
            else {
                dataTableLocalesAsignar.column(5).search(regExSearch, true, false).draw();
            }
        });

        $("#btnAsociarLocalPMM").on("click", function () {

            const registrosSeleccionados = dataTableLocalesAsignar.rows('.selected').data().toArray();

            if (!validarSelecion(registrosSeleccionados.length, true)) {
                return;
            }

            abrirModalAsignarLocalPMM(registrosSeleccionados[0]);
        });

        $("#btnGuardarLocalPMM").on("click", function () {

            const localSeleccionado = dataTableLocalesAsignar.rows('.selected').data().toArray()[0];

            var local = {
                CodCadenaCt2: $("#txtCadenaPMM").val(),
                CodLocalCt2: $("#txtLocalPMM").val(),
                CodEmpresa: localSeleccionado.COD_EMPRESA,
                CodSede: localSeleccionado.COD_LOC_OFI
            };

            if (validarLocalPMM(local))
                guardarLocalPMM(local);
        });

    }




    var visualizarDataTableLocal = function () {

        $.ajax({
            url: urlLocalesAsignar,
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
                    if (x === "COD_EMPRESA" || x === "COD_LOC_OFI") {
                        columnas.push({
                            title: x,
                            data: x.replace(" ", "").replace(".", ""),
                            visible: false
                        });
                    } else {
                        columnas.push({
                            title: x,
                            data: x.replace(" ", "").replace(".", ""),
                        });
                    }
                });

                dataTableLocalesAsignar = $('#tableLocalesAsignar').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '300px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    "data": response.Locales,
                    "bAutoWidth": false,
                    buttons: [
                        {
                            extend: 'excel',
                            text: 'Excel <i class="fa fa-cloud-download"></i>',
                            titleAttr: 'Descargar Excel',
                            className: 'btn-sm mb-1 ms-2',
                            exportOptions: {
                                modifier: { page: 'all' }
                            }
                        },
                    ]
                });

                dataTableLocalesAsignar.buttons().container().prependTo($('#tableLocalesAsignar_filter'));

                $('input[type="search"]').addClass("form-control-sm");
                closeLoading();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });


    };

    const cargarLocales = function () {
        $.ajax({
            url: urlLocalesAsignar,
            type: "post",
            dataType: "json",
            success: function (response) {
                dataTableLocalesAsignar.clear();
                dataTableLocalesAsignar.rows.add(response.Locales);
                dataTableLocalesAsignar.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const asignarLocal = function () {

        const registrosSeleccionados = dataTableLocalesAsignar.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        showLoading();

        let data = registrosSeleccionados.map(x => {
            return {
                CodCadena: x.Cadena,
                CodLocal: x.Local,
            }
        });


        $.ajax({
            url: urlAsignar,
            type: "post",
            data: {
                request: data
            },
            dataType: "json",
            success: function (response) {
                closeLoading();

                if (response.Ok) {
                    swal({
                        text: "Proceso exitoso",
                        icon: "success"
                    });
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                }

                cargarLocales();

            },
            error: function (jqXHR, textStatus, errorThrown) {
                closeLoading();
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
            }
        });
    }

    const validarSelecion = function (count, unRegistro = false) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar como minimo un registro",
                icon: "warning",
            });
            return false;
        }

        if (unRegistro && count > 1 ) {
            swal({
                text: "Debe seleccionar solo un registro",
                icon: "warning",
            });
            return false;
        }

        return true;
    }

    const abrirModalAsignarLocalPMM = function (registro) {

        const request = {
            CodEmpresa: registro.COD_EMPRESA,
            CodSede: registro.COD_LOC_OFI
        };

        $.ajax({
            url: urlFormAsignarLocalPMM,
            type: "post",
            data: { request },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalLocalPMM").find(".modal-body").html(response);
                $("#modalLocalPMM").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const validarLocalPMM = function (local) {
        let validate = true;

        if (local.CodCadenaCt2 === '' || local.CodLocalCt2 === '' ) {
            validate = false;
            $("#formLocalPMM").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const guardarLocalPMM = function (local) {

        $.ajax({
            url: urlAsociarLocalPMM,
            type: "post",
            data: { request: local },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                cargarLocales();
                $("#modalLocalPMM").modal('hide');
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
                visualizarDataTableLocal();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
