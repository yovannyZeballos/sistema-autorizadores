var urlListarProveedores = baseUrl + 'Inventario/Proveedores/ListarPaginado';

var urlCrearProveedor = baseUrl + 'Inventario/Proveedores/Crear';
var urlEliminarProveedor = baseUrl + 'Inventario/Proveedores/Eliminar';
var urlEditarProveedor = baseUrl + 'Inventario/Proveedores/Editar';

var dataTableProveedores = null;

var AdministrarProveedor = function () {

    const eventos = function () {

        $("#btnBuscar").on("click", function (e) {
            var table = $('#tableProveedores').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $('#btnNuevoProveedor').on('click', function () {
            $('#formNuevoProveedor')[0].reset();

            $('#btnGuardarNuevoModal').show();
            $('#btnGuardarCambiosModal').hide();

            $('#modalInputRuc').val('');
            $('#modalInputRazonSocial').val('');
            $('#modalInputNomComercial').val('');
            $('#modalInputDirFiscal').val('');
            $('#modalInputContacto').val('');
            $('#modalInputTelefono').val('');
            $('#modalInputEmail').val('');
            $('#modalChkActivo').prop('checked', true);

            $('#modalInputRuc').prop('disabled', false);

            var modal = new bootstrap.Modal(document.getElementById('modalNuevoProveedor'));
            modal.show();
        });

        $('#btnEditarProveedor').on('click', async function () {
            $('#formNuevoProveedor')[0].reset();
            document.getElementById('modalTituloProveedorLabel').textContent = 'Editar Proveedor';
            $('#btnGuardarNuevoModal').hide();
            $('#btnGuardarCambiosModal').show();

            var $checks = $('#tableProveedores tbody .row-checkbox:checked');
            if ($checks.length !== 1) {
                return swal({
                    text: $checks.length === 0
                        ? "Seleccione un registro para modificar."
                        : "Seleccione sólo un registro.",
                    icon: "warning"
                });
            }

            var $tr = $checks.closest('tr');
            var data = $('#tableProveedores').DataTable().row($tr).data();


            $('#modalInputRuc').val(data.Ruc);
            $('#modalInputRazonSocial').val(data.RazonSocial);
            $('#modalInputNomComercial').val(data.NomComercial);
            $('#modalInputDirFiscal').val(data.DirFiscal);
            $('#modalInputContacto').val(data.Contacto);
            $('#modalInputTelefono').val(data.Telefono);
            $('#modalInputEmail').val(data.Email);
            $('#modalChkActivo').prop('checked', data.IndActivo === 'S');

            $('#modalInputRuc').prop('disabled', true);

            var modal = new bootstrap.Modal(document.getElementById('modalNuevoProveedor'));
            modal.show();
        });

        $('#btnGuardarNuevoModal').on('click', async function () {
            guardarProveedor({ modo: 'crear' });
        });

        $('#btnGuardarCambiosModal').on('click', async function () {
            guardarProveedor({ modo: 'editar' });
        });

        $('#btnEliminarProveedor').on('click', async function () {
            var $checks = $('#tableProveedores tbody .row-checkbox:checked');
            if ($checks.length === 0) {
                swal({ text: "Debe seleccionar al menos un registro para eliminar.", icon: "warning" });
                return;
            }

            var arrAEliminar = [];
            $checks.each(function () {
                var $ch = $(this);
                arrAEliminar.push({
                    Ruc: $ch.data('proveedor')
                });
            });

            swal({
                text: "¿Está seguro que desea eliminar los registros seleccionados?",
                icon: "warning",
                buttons: ["Cancelar", "Eliminar"],
                dangerMode: true
            }).then(async (confirmar) => {
                if (!confirmar) return;

                var payload = {
                    Proveedores: arrAEliminar
                };

                try {
                    var response = await $.ajax({
                        url: urlEliminarProveedor,
                        type: "POST",
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify(payload),
                        dataType: 'json'
                    });

                    if (response.Ok) {
                        swal({ text: response.Mensaje, icon: "success" });
                    } else {
                        swal({ text: response.Mensaje, icon: "warning" });
                    }

                    $('#tableProveedores').DataTable().ajax.reload(null, false);
                    $('#checkAllRows').prop('checked', false);

                } catch (err) {
                    swal({ text: err.responseText || err.statusText, icon: "error" });
                }
            });
        });

    };

    async function guardarProveedor({ modo }) {
        var ruc = $('#modalInputRuc').val().trim();
        var razonSocial = $('#modalInputRazonSocial').val().trim();
        var nomComercial = $('#modalInputNomComercial').val().trim();
        var dirFiscal = $('#modalInputDirFiscal').val().trim();
        var contacto = $('#modalInputContacto').val().trim();
        var telefono = $('#modalInputTelefono').val().trim();
        var email = $('#modalInputEmail').val().trim();
        var activo = $('#modalChkActivo').is(':checked') ? 'S' : 'N';

        if (ruc === '') {
            swal({ text: "Ruc es obligatoria.", icon: "warning" });
            return;
        }

        if (razonSocial === '') {
            swal({ text: "Razón social es obligatoria.", icon: "warning" });
            return;
        }

        if (nomComercial === '') {
            swal({ text: "Nombre Comercial es obligatoria.", icon: "warning" });
            return;
        }

        if (dirFiscal === '') {
            swal({ text: "Dirección Fiscal es obligatoria.", icon: "warning" });
            return;
        }

        const urlEleccion = (modo === 'editar') ? urlEditarProveedor
                                                : urlCrearProveedor;

        var payload = {
            Ruc: ruc,
            RazonSocial: razonSocial,
            NomComercial: nomComercial,
            DirFiscal: dirFiscal,
            Contacto: contacto,
            Telefono: telefono,
            Email: email,
            IndActivo: activo
        };

        try {
            var response = await $.ajax({
                url: urlEleccion,
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (response.Ok) {
                swal({ text: response.Mensaje, icon: "success" });

                var modalEl = document.getElementById('modalNuevoProveedor');
                var modalObj = bootstrap.Modal.getInstance(modalEl);
                modalObj.hide();

                $('#tableProveedores').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: response.Mensaje, icon: "warning" });
            }
        } catch (err) {
            swal({ text: err.responseText || err.statusText, icon: "error" });
        }
    }

    const visualizarDataTableProveedores = function () {

        $('#tableProveedores').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    IndActivo: $("#cboIndActivoBuscar").val(),
                    FiltroVarios: $("#txtFiltroVariosBuscar").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarProveedores,
                    type: "GET",
                    data: params,
                    dataType: "json",
                    success: function (response) {
                        if (response.Ok) {
                            var pagedData = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pagedData.TotalRecords,
                                recordsFiltered: pagedData.TotalRecords,
                                data: pagedData.Items
                            });
                        } else {
                            callback({
                                draw: data.draw,
                                recordsTotal: 0,
                                recordsFiltered: 0,
                                data: []
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        swal({
                            text: jqXHR.responseText,
                            icon: "error",
                        });
                        callback({
                            draw: data.draw,
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            data: []
                        });
                    }
                });
            },
            columns: [
                {
                    data: null,
                    orderable: false,
                    className: 'text-center',
                    render: function (data, type, row) {
                        return ''
                            + '<input type="checkbox" class="row-checkbox" '
                            + 'data-proveedor="' + row.Ruc + '" '
                            + '/>';
                    }
                },
                { title: "Código", data: "Ruc" },
                { title: "Razón Social", data: "RazonSocial" },
                {
                    title: "Activo",
                    data: "IndActivo",
                    className: 'text-center',
                    render: function (data, type, row) {
                        if (data === 'S') {
                            return '<i class="fe fe-check text-success fs-6"></i>';
                        } else {
                            return '<i class="fe fe-x text-danger fs-6"></i>';
                        }
                    }
                },
                { title: "U. Creacion", data: "UsuCreacion" },
                {
                    title: "F. Creacion", data: "FecCreacion",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { title: "U. Modifica", data: "UsuModifica" },
                {
                    title: "F. Modifica", data: "FecModifica",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { title: "U. Elimina", data: "UsuElimina" },
                {
                    title: "F. Elimina", data: "FecElimina",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)"
            },
            scrollY: '500px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });

        $('#checkAllRows').on('change', function () {
            var isChecked = $(this).is(':checked');
            $('#tableProveedores tbody .row-checkbox').each(function () {
                $(this).prop('checked', isChecked);
            });
        });

        $('#tableProveedores tbody').on('change', '.row-checkbox', function () {
            if (!$(this).is(':checked')) {
                $('#checkAllRows').prop('checked', false);
            }
        });

    };

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                visualizarDataTableProveedores();
            });
        }
    }
}(jQuery);