var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarFactoresMdr = baseUrl + 'MdrBinesIzipay/FactoresMdr/ListarPaginado';
var urlListarOperadoresMdr = baseUrl + 'MdrBinesIzipay/FactoresMdr/ListarOperador';
var urlListarClasificacionesMdr = baseUrl + 'MdrBinesIzipay/FactoresMdr/ListarClasificacion';

var urlCrearFactorMdr = baseUrl + 'MdrBinesIzipay/FactoresMdr/CrearFactorMdr';
var urlEliminarFactorMdr = baseUrl + 'MdrBinesIzipay/FactoresMdr/EliminarFactorMdr';
var urlImportarExcel = baseUrl + 'MdrBinesIzipay/Bines/DesdeExcel';


var dataTableFactores = null;

var AdministrarFactorMdr = function () {

    const eventos = function () {

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
            $('#formNuevoFactor')[0].reset();

            $('#modalCboEmpresa').val('0');
            $('#modalCboEmpresa').trigger('change');

            $('#modalCboNumAno').val((new Date()).getFullYear().toString());
            $('#modalCboNumAno').trigger('change');

            $('#modalCboOperador').val('0');
            $('#modalCboOperador').trigger('change');

            $('#modalInputFactor').val('0.00');

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

        $('#btnDescargarMaestro').on('click', function () {
            var empresa = $('#cboEmpresa').val();
            var anio = $('#cboNumAno').val();

            if (!empresa || empresa === '0' || !anio) {
                swal({ text: "Debe seleccionar Empresa y Año.", icon: "warning" });
                return;
            }

            swal({
                text: "¿Deseas descargar el archivo CSV del consolidado?",
                icon: "warning",
                buttons: {
                    cancel: {
                        text: "No",
                        value: false,
                        visible: true,
                        className: "btn btn-secondary",
                        closeModal: true
                    },
                    confirm: {
                        text: "Sí",
                        value: true,
                        visible: true,
                        className: "btn btn-primary",
                        closeModal: true
                    }
                }
            }).then((confirmar) => {
                if (!confirmar) {
                    return;
                }

                var url = baseUrl + 'MdrBinesIzipay/Bines/DescargarCsvStreaming'
                    + '?codEmpresa=' + encodeURIComponent(empresa)
                    + '&numAno=' + encodeURIComponent(anio);

                showLoading();

                $.ajax({
                    url: url,
                    method: 'GET',
                    xhrFields: {
                        responseType: 'blob'
                    },
                    success: function (data, textStatus, jqXHR) {

                        closeLoading();

                        var contentType = jqXHR.getResponseHeader('Content-Type') || '';

                        if (contentType.indexOf('application/json') !== -1) {
                            var reader = new FileReader();
                            reader.onload = function () {
                                try {
                                    var obj = JSON.parse(reader.result);
                                    if (obj.ok === false) {
                                        swal({ text: obj.mensaje || 'Error desconocido.', icon: "error" });
                                    } else {
                                        swal({ text: 'Ocurrió un error inesperado.', icon: "error" });
                                    }
                                } catch (parseErr) {
                                    swal({ text: "Error al procesar la respuesta del servidor.", icon: "error" });
                                }
                            };
                            reader.readAsText(data);
                            return;
                        }

                        var disposition = jqXHR.getResponseHeader('Content-Disposition');
                        var fileName = "Consolidado.csv";
                        if (disposition && disposition.indexOf('filename=') !== -1) {
                            fileName = disposition
                                .split('filename=')[1]
                                .trim()
                                .replace(/["']/g, '');
                        }

                        var blob = new Blob([data], { type: 'text/csv;charset=utf-8;' });
                        var urlBlob = URL.createObjectURL(blob);
                        var a = document.createElement('a');
                        a.href = urlBlob;
                        a.download = fileName;
                        document.body.appendChild(a);
                        a.click();
                        document.body.removeChild(a);
                        URL.revokeObjectURL(urlBlob);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        closeLoading();
                        swal({
                            text: "Error al descargar CSV: " + (errorThrown || jqXHR.statusText),
                            icon: "error"
                        });
                    }
                });
            });
        });

        $('#btnCargarBines').on('click', function () {
            $('#excelBinesTmp').click();
        });

        $('#excelBinesTmp').on('change', function () {
            var fileInput = this;
            if (!fileInput.files || fileInput.files.length === 0) return;

            swal({
                text: "Al importar, se limpiará la tabla temporal y se cargarán nuevos registros. ¿Deseas continuar?",
                icon: "warning",
                buttons: {
                    cancel: {
                        text: "No",
                        value: false,
                        visible: true,
                        className: "btn btn-secondary",
                        closeModal: true
                    },
                    confirm: {
                        text: "Sí",
                        value: true,
                        visible: true,
                        className: "btn btn-primary",
                        closeModal: true
                    }
                }
            }).then(function (confirmar) {
                if (!confirmar) {
                    $(fileInput).val('');
                    return;
                }

                showLoading();

                var formData = new FormData();
                formData.append('archivoExcel', fileInput.files[0]);

                $.ajax({
                    url: urlImportarExcel,
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false
                })
                .done(function (response) {
                    closeLoading();
                    $(fileInput).val('');

                    if (response.ok) {
                        swal({ text: response.mensaje, icon: "success" });
                    } else {
                        swal({ text: response.mensaje, icon: "warning" });
                    }
                })
                .fail(function (xhr) {
                    closeLoading();
                    $(fileInput).val('');
                    swal({ text: "Error al subir el archivo: " + (xhr.responseText || xhr.statusText), icon: "error" });
                });
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
            $sel.append($('<option>', { value: año.toString(), text: año.toString() }));
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