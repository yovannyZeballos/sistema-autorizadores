var urlPuestos = baseUrl + 'Autorizadores/Puesto/Listar';
var urlActualizarPuestos = baseUrl + 'Autorizadores/Puesto/Actualizar';
var urlEmpresas = baseUrl + 'Empresa/ListarEmpresas';

var Puesto = function () {

    var eventos = function () {
        $("#btnAsignar").on('click', function () {
            actualizar();
        });

        $("#cboEmpresa").on("change", function () {
            cargarRoles();
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
                    scrollY: '300px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    //"data": response.Puestos,
                    "bAutoWidth": false,
                    'columnDefs': [{
                        'targets': 3,
                        'searchable': false,
                        'orderable': false,
                        'render': function (data, type, full, meta) {
                            if (data === 'S')
                                return '<input type="checkbox" checked name="id[]" value="' + data + '">';
                            else
                                return '<input type="checkbox" name="id[]" value="' + data + '">';
                        }
                    }],
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
                dataTableRoles.buttons().container().prependTo($('#tableRoles_filter'));
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

        showLoading();

        const puestos = dataTableRoles.rows().data().toArray();
        let puestosActualizar = [];
        dataTableRoles.$('input[type="checkbox"]').each(function (index, value) {
            const puestoActualizar = puestos[index];
            puestosActualizar.push({
                CodEmpresa: puestoActualizar.CO_EMPR,
                CodPuesto: puestoActualizar.Codigo,
                Select: this.checked ? 'S' : 'N'
            });
        });


        $.ajax({
            url: urlActualizarPuestos,
            type: "post",
            dataType: "json",
            data: { puestos: puestosActualizar },
            success: function (response) {
                closeLoading();

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
                        icon: "warning"
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
