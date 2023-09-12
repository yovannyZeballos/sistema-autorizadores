var urlPuestos = baseUrl + 'Autorizadores/Puesto/Listar';
var urlActualizarPuestos = baseUrl + 'Autorizadores/Puesto/Actualizar';
var urlEmpresas = baseUrl + 'Empresa/ListarEmpresas';
var urlDescargar = baseUrl + 'Autorizadores/Puesto/DescargarMaestro';


var Puesto = function () {

    var eventos = function () {
        $("#btnAsignar").on('click', function () {
            actualizar();
        });

        $("#cboEmpresa").on("change", function () {
            cargarRoles();
        });

        $("#btnDescargarMaestro").on('click', function () {
            descargarMaestro();
        });

        $(document).on('change', '[name="id[]"]', function () {
            var checkbox = $(this);

            if (checkbox.is(':checked')) {
                checkbox.val('S');
            } else {
                checkbox.val('N');
            }
        });


    }

    const listarEmpresas = function () {
        $.ajax({
            url: urlEmpresas,
            type: "get",
            success: function (response) {

                if (response.Ok === true) {
                    cargarEmpresas(response.Empresas);
                    cargarRoles();
                } else {
                    notif({
                        type: "error",
                        msg: response.Mensaje,
                        height: 100,
                        position: "right"
                    });
                }
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

    const cargarRoles = () => {
        const data = {
            CodEmpresa: $('#cboEmpresa').val()
        };


        $.ajax({
            url: urlPuestos,
            type: "post",
            data: data,
            success: function (response) {

                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                    return;
                }

                if (dataTableRoles) {
                    dataTableRoles.clear();
                    dataTableRoles.rows.add(response.Puestos);
                    dataTableRoles.draw();
                }


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


    var inicializarDataTableRoles = function () {

        const data = {
            CodEmpresa: $('#cboEmpresa').val()
        };


        $.ajax({
            url: urlPuestos,
            type: "post",
            data: data,
            success: function (response) {

                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });
                    return;
                }

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", ""),
                        defaultContent: "",
                    });
                });

                dataTableRoles = $('#tableRoles').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '420px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    //"data": response.Puestos,
                    "bAutoWidth": false,
                    'columnDefs': [{
                        'targets': [3, 4, 5, 6],
                        'searchable': false,
                        'orderable': false,
                        'render': function (data, type, full, meta) {
                            if (data === 'S')
                                return '<input type="checkbox" checked class="checkbox" name="id[]" value="' + data + '">';
                            else
                                return '<input type="checkbox" class="checkbox" name="id[]" value="' + data + '">';
                        }
                    }],
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


    };

    const actualizar = function () {

        let puestosActualizar = [];

        $("#tableRoles tbody tr").each(function (index) {

            let codEmpresa, codPuesto, indAutorizador, indCajero, indAutoCajero, indAutoAutorizador;

            $(this).children("td").each(function (index2) {
                switch (index2) {
                    case 0:
                        codEmpresa = $(this).text();
                        break;
                    case 1:
                        codPuesto = $(this).text();
                        break;
                    case 3:
                        indAutorizador =$(this).find('input[type=checkbox]').val();
                        break;
                    case 4:
                        indCajero = $(this).find('input[type=checkbox]').val();
                        break;
                    case 5:
                        indAutoAutorizador = $(this).find('input[type=checkbox]').val();
                        break;
                    case 6:
                        indAutoCajero = $(this).find('input[type=checkbox]').val();
                        break;
                    default:
                }
            });

            puestosActualizar.push({
                CodEmpresa: codEmpresa,
                CodPuesto: codPuesto,
                IndAutorizador: indAutorizador,
                IndCajero: indCajero,
                IndAutoCajero: indAutoCajero,
                IndAutoAutorizador: indAutoAutorizador
            });
        });

        $.ajax({
            url: urlActualizarPuestos,
            type: "post",
            dataType: "json",
            data: { puestos: puestosActualizar },
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
                    return;
                }

                if (response.Mensaje.length > 0) {
                    swal({
                        text: response.Mensaje,
                        icon: "success"
                    });
                    return;
                }

                cargarRoles();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const cargarEmpresas = function (empresas) {

        empresas.map((empresa) => {
            $('#cboEmpresa').append($('<option>', { value: empresa.Ruc, text: empresa.Descripcion }));
        });
        $('#cboEmpresa').val($('#cboEmpresa option:last-child').val()).trigger('change');
    }

    const descargarMaestro = function () {

        const data = {
            CodEmpresa: $('#cboEmpresa').val()
        };

        $.ajax({
            url: urlDescargar,
            type: "post",
            data: data,
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

    return {
        init: function () {
            checkSession(function () {
                eventos();
                listarEmpresas();
                inicializarDataTableRoles();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
