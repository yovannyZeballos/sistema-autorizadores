var urlListarPeriodosMdr = baseUrl + 'MdrBinesIzipay/PeriodosMdr/ListarPaginado';

var urlCrearPeriodoMdr = baseUrl + 'MdrBinesIzipay/PeriodosMdr/Crear';
var urlEliminarPeriodoMdr = baseUrl + 'MdrBinesIzipay/PeriodosMdr/Eliminar';
var urlEditarPeriodoMdr = baseUrl + 'MdrBinesIzipay/PeriodosMdr/Editar';

var dataTablePeriodos = null;

var AdministrarPeriodoMdr = function () {

    const eventos = function () {


        $('#btnNuevoFactor').on('click', function () {
            $('#formNuevoPeriodo')[0].reset();

            $('#btnGuardarNuevoModal').show();
            $('#btnGuardarCambiosModal').hide();

            $('#modalInputCodPeriodo').val('');
            $('#modalInputDesPeriodo').val('');
            $('#modalChkActivo').prop('checked', true);

            $('#modalInputCodPeriodo').prop('disabled', true);

            var modal = new bootstrap.Modal(document.getElementById('modalNuevoPeriodo'));
            modal.show();
        });

        $('#btnEditarFactor').on('click', async function () {
            $('#formNuevoPeriodo')[0].reset();

            $('#btnGuardarNuevoModal').hide();
            $('#btnGuardarCambiosModal').show();

            var $checks = $('#tablePeriodos tbody .row-checkbox:checked');
            if ($checks.length !== 1) {
                return swal({
                    text: $checks.length === 0
                        ? "Seleccione un registro para modificar."
                        : "Seleccione sólo un registro.",
                    icon: "warning"
                });
            }

            var $tr = $checks.closest('tr');
            var data = $('#tablePeriodos').DataTable().row($tr).data();


            $('#modalInputCodPeriodo').val(data.CodPeriodo);
            $('#modalInputDesPeriodo').val(data.DesPeriodo);
            $('#modalChkActivo').prop('checked', data.IndActivo === 'S');

            $('#modalInputCodPeriodo').prop('disabled', true);

            var modal = new bootstrap.Modal(document.getElementById('modalNuevoPeriodo'));
            modal.show();
        });

        $('#btnGuardarNuevoModal').on('click', async function () {
            guardarFactor({ modo: 'crear' });
        });

        $('#btnGuardarCambiosModal').on('click', async function () {
            guardarFactor({ modo: 'editar' });
        });

        $('#btnEliminarFactor').on('click', async function () {
            var $checks = $('#tablePeriodos tbody .row-checkbox:checked');
            if ($checks.length === 0) {
                swal({ text: "Debe seleccionar al menos un registro para eliminar.", icon: "warning" });
                return;
            }

            var arrAEliminar = [];
            $checks.each(function () {
                var $ch = $(this);
                arrAEliminar.push({
                    CodPeriodo: $ch.data('periodo')
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
                    Periodos: arrAEliminar
                };

                try {
                    var response = await $.ajax({
                        url: urlEliminarPeriodoMdr,
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

                    $('#tablePeriodos').DataTable().ajax.reload(null, false);
                    $('#checkAllRows').prop('checked', false);

                } catch (err) {
                    swal({ text: err.responseText || err.statusText, icon: "error" });
                }
            });
        });

    };

    async function guardarFactor({ modo }) {
        var codPeriodo = $('#modalInputCodPeriodo').val().trim();
        var desPeriodo = $('#modalInputDesPeriodo').val().trim();
        var activo = $('#modalChkActivo').is(':checked') ? 'S' : 'N';


        if (desPeriodo === '') {
            swal({ text: "La descripción del período es obligatoria.", icon: "warning" });
            return;
        }

        const urlEleccion = (modo === 'editar') ? urlEditarPeriodoMdr
                                                : urlCrearPeriodoMdr;

        var payload = {
            CodPeriodo: codPeriodo,
            DesPeriodo: desPeriodo,
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

                var modalEl = document.getElementById('modalNuevoPeriodo');
                var modalObj = bootstrap.Modal.getInstance(modalEl);
                modalObj.hide();

                $('#tablePeriodos').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: response.Mensaje, icon: "warning" });
            }
        } catch (err) {
            swal({ text: err.responseText || err.statusText, icon: "error" });
        }
    }

    const visualizarDataTablePeriodos = function () {

        $('#tablePeriodos').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {};

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarPeriodosMdr,
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
                            + 'data-periodo="' + row.CodPeriodo + '" '
                            + '/>';
                    }
                },
                { title: "Código", data: "CodPeriodo" },
                { title: "Descripción", data: "DesPeriodo" },
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
            $('#tablePeriodos tbody .row-checkbox').each(function () {
                $(this).prop('checked', isChecked);
            });
        });

        $('#tablePeriodos tbody').on('change', '.row-checkbox', function () {
            if (!$(this).is(':checked')) {
                $('#checkAllRows').prop('checked', false);
            }
        });

    };

    return {
        init: function () {
            checkSession(async function () {
                eventos();

                visualizarDataTablePeriodos();
            });
        }
    }
}(jQuery);