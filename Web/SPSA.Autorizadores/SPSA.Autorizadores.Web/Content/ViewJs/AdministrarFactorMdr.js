var urlListarEmpresasAsociadas      = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarFactoresMdr            = baseUrl + 'MdrBinesIzipay/FactoresMdr/ListarPaginado';
var urlListarOperadoresMdr          = baseUrl + 'MdrBinesIzipay/FactoresMdr/ListarOperador';
var urlListarClasificacionesMdr     = baseUrl + 'MdrBinesIzipay/FactoresMdr/ListarClasificacion';

var urlCrearFactorMdr               = baseUrl + 'MdrBinesIzipay/FactoresMdr/CrearFactorMdr';
var urlEliminarFactorMdr            = baseUrl + 'MdrBinesIzipay/FactoresMdr/EliminarFactorMdr';


var dataTableFactores = null;

var AdministrarFactorMdr = function () {

    const eventos = function () {

        //$('#tableFactores tbody').on('click', 'tr', function () {
        //    if ($(this).hasClass('selected')) {
        //        $(this).removeClass('selected');
        //    } else {
        //        $('#tableFactores').DataTable().$('tr.selected').removeClass('selected');
        //        $(this).addClass('selected');
        //    }
        //});

        $("#cboEmpresa, #cboNumAno, #cboClasificacion").on("change", function (e) {
            var table = $('#tableFactores').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });

        $("#cboOperador").on("change", async function () {
            const codOperador = $('#cboOperador').val();
            if (!codOperador) return resolve();

            $('#cboClasificacion').val('0');
            await cargarComboClasificaciones('#cboClasificacion', codOperador);
            $('#tableFactores').DataTable().ajax.reload();
        });

        $("#modalCboOperador").on("change", async function () {
            const codOperador = $('#modalCboOperador').val();
            if (!codOperador) return resolve();

            $('#modalCboClasificacion').val('0');
            await cargarComboClasificaciones('#modalCboClasificacion', codOperador);
        });

        $('#btnNuevoFactor').on('click', function () {
            // Resetear formulario del modal
            $('#formNuevoFactor')[0].reset();

            $('#modalCboEmpresa').val('0');
            $('#modalCboEmpresa').trigger('change');

            $('#modalCboNumAno').val((new Date()).getFullYear().toString());
            $('#modalCboNumAno').trigger('change');

            $('#modalCboOperador').val('0');
            $('#modalCboOperador').trigger('change');

            //$('#modalCboClasificacion').empty().append($('<option>', { value: '0', text: 'Seleccione clasificación' }));
            $('#modalInputFactor').val('0.00');

            // Abrir el modal
            var modal = new bootstrap.Modal(document.getElementById('modalNuevoFactor'));
            modal.show();
        });

        $('#btnGuardarModal').on('click', async function () {
            var codEmpresa = $('#modalCboEmpresa').val();
            var numAno = $('#modalCboNumAno').val();
            var codOperador = $('#modalCboOperador').val();
            var codClas = $('#modalCboClasificacion').val();
            var factor = $('#modalInputFactor').val();

            if (!codEmpresa || codEmpresa === '0') {
                swal({ text: "Seleccione una empresa.", icon: "warning" });
                return;
            }
            if (!numAno || numAno === '0') {
                swal({ text: "Seleccione un año.", icon: "warning" });
                return;
            }
            if (!codOperador || codOperador === '0') {
                swal({ text: "Seleccione un operador.", icon: "warning" });
                return;
            }
            if (!codClas || codClas === '0') {
                swal({ text: "Seleccione una clasificación.", icon: "warning" });
                return;
            }
            if (!factor || isNaN(parseFloat(factor))) {
                swal({ text: "Ingrese un valor numérico válido para Factor.", icon: "warning" });
                return;
            }

            var cmd = {
                CodEmpresa: codEmpresa,
                NumAno: numAno,
                CodOperador: codOperador,
                CodClasificacion: codClas,
                Factor: parseFloat(factor)
            };

            try {
                var response = await $.ajax({
                    url: urlCrearFactorMdr,
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(cmd),
                    dataType: 'json'
                });

                if (response.Ok) {
                    swal({ text: response.Mensaje, icon: "success" });

                    var modalEl = document.getElementById('modalNuevoFactor');
                    var modalObj = bootstrap.Modal.getInstance(modalEl);
                    modalObj.hide();

                    $('#tableFactores').DataTable().ajax.reload(null, false);
                } else {
                    swal({ text: response.Mensaje, icon: "warning" });
                }
            } catch (err) {
                swal({ text: err.responseText || err.statusText, icon: "error" });
            }
        });

        $('#btnEliminarFactor').on('click', async function () {
            var $checks = $('#tableFactores tbody .row-checkbox:checked');
            if ($checks.length === 0) {
                swal({ text: "Debe seleccionar al menos un registro para eliminar.", icon: "warning" });
                return;
            }

            var arrAEliminar = [];
            $checks.each(function () {
                var $ch = $(this);
                arrAEliminar.push({
                    CodEmpresa: $ch.data('empresa'),
                    NumAno: $ch.data('ano'),
                    CodOperador: $ch.data('operador'),
                    CodClasificacion: $ch.data('clasificacion')
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
                    Factores: arrAEliminar
                };

                try {
                    var response = await $.ajax({
                        url: urlEliminarFactorMdr,
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

                    $('#tableFactores').DataTable().ajax.reload(null, false);
                    $('#checkAllRows').prop('checked', false);

                } catch (err) {
                    swal({ text: err.responseText || err.statusText, icon: "error" });
                }
            });
        });

        

    };

    const listarEmpresasAsociadas = function () {
        return new Promise((resolve, reject) => {

            const request = {
                Busqueda: ''
            };

            $.ajax({
                url: urlListarEmpresasAsociadas,
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

    const listarOperadores = function () {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: urlListarOperadoresMdr,
                type: "post",
                data: JSON.stringify({}),
                success: function (response) {
                    resolve(response)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const listarClasificaciones = function (codOperador) {
        return new Promise((resolve, reject) => {
            const request = {
                CodOperador: codOperador
            };

            $.ajax({
                url: urlListarClasificacionesMdr,
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
            const response = await listarEmpresasAsociadas();

            if (response.Ok) {
                $('#cboEmpresa').empty().append($('<option>', { value: '0', text: 'Todos' }));
                $('#modalCboEmpresa').empty().append($('<option>', { value: '0', text: 'Todos' }));
                response.Data.map(empresa => {
                    $('#cboEmpresa').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
                    $('#modalCboEmpresa').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
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

    const cargarComboAnios = async function (selector) {
        const $sel = $(selector);
        $sel.empty();

        const yearStart = 2025;
        const yearNow = (new Date()).getFullYear();

        for (let año = yearNow; año >= yearStart; año--) {
            $sel.append($('<option>', {value: año.toString(),text: año.toString()}));
        }

        $sel.val(yearNow.toString());
    };

    const cargarComboOperadores = async function () {
        try {
            const response = await listarOperadores();

            if (response.Ok) {
                $('#cboOperador').empty().append($('<option>', { value: '0', text: 'Todos' }));
                $('#modalCboOperador').empty().append($('<option>', { value: '0', text: 'Todos' }));
                response.Data.map(operador => {
                    $('#cboOperador').append($('<option>', { value: operador.CodOperador, text: operador.NomOperador }));
                    $('#modalCboOperador').append($('<option>', { value: operador.CodOperador, text: operador.NomOperador }));
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

    const cargarComboClasificaciones = async function (cboClasificacion, codOperador) {
        try {
            const response = await listarClasificaciones(codOperador);

            if (response.Ok) {
                $(cboClasificacion).empty().append($('<option>', { value: '0', text: 'Todos' }));
                response.Data.map(clasificacion => {
                    $(cboClasificacion).append($('<option>', { value: clasificacion.CodClasificacion, text: clasificacion.NomClasificacion }));
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

    const visualizarDataTableFactores = function () {

        $('#tableFactores').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    CodEmpresa: $("#cboEmpresa").val(),
                    NumAno: $("#cboNumAno").val(),
                    CodOperador: $("#cboOperador").val(),
                    CodClasificacion: $("#cboClasificacion").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarFactoresMdr,
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
                            + 'data-empresa="' + row.CodEmpresa + '" '
                            + 'data-ano="' + row.NumAno + '" '
                            + 'data-operador="' + row.CodOperador + '" '
                            + 'data-clasificacion="' + row.CodClasificacion + '" '
                            + '/>';
                    }
                },
                { title: "Empresa", data: "NomEmpresa" },
                { title: "Operador", data: "NomOperador" },
                { title: "Clasificación", data: "NomClasificacion" },
                {
                    title: "Factor",
                    data: "Factor",
                    className: "text-end",
                    render: function (data, type, row) {
                        var num = parseFloat(data);
                        if (isNaN(num)) return "";
                        return num.toFixed(2) + " %";
                    }
                },
                {
                    title: "Activo",
                    data: "IndActivo",
                    className: 'text-center',
                    render: function (data, type, row) {
                        if (data === 'S') {
                            return '<i class="fe fe-check text-success"></i>';
                            //return '<span class="badge bg-success rounded-circle p-2"><i></i></span>';
                        } else {
                            return '<i class="fe fe-x text-danger"></i>';
                            //return '<span class="badge bg-danger rounded-circle p-2"><i></i></span>';
                        }
                    }
                },
                { title: "U. Creacion", data: "UsuCreacion" },
                { title: "F. Creacion", data: "FecCreacion",
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
                { title: "F. Modifica", data: "FecModifica",
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
            scrollY: '450px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });

        $('#checkAllRows').on('change', function () {
            var isChecked = $(this).is(':checked');
            $('#tableFactores tbody .row-checkbox').each(function () {
                $(this).prop('checked', isChecked);
            });
        });

        $('#tableFactores tbody').on('change', '.row-checkbox', function () {
            if (!$(this).is(':checked')) {
                $('#checkAllRows').prop('checked', false);
            }
        });

    };


    const validarSelecion = function (count) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar como minimo un registro",
                icon: "warning",
            });
            return false;
        }

        return true;
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                await cargarComboAnios('#cboNumAno');
                await cargarComboAnios('#modalCboNumAno');
                await cargarComboOperadores();
                visualizarDataTableFactores();
            });
        }
    }
}(jQuery);