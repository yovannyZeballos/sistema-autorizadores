var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarPuestos = baseUrl + 'Maestros/MaePuesto/ListarPaginado';

var urlModalNuevoPuesto = baseUrl + 'Maestros/MaePuesto/NuevoForm';
var urlModalModificarPuesto = baseUrl + 'Maestros/MaePuesto/ModificarForm';

var urlObtenerPuesto = baseUrl + 'Maestros/MaePuesto/Obtener';
var urlCrearPuesto = baseUrl + 'Maestros/MaePuesto/CrearPuesto';
var urlModificarPuesto = baseUrl + 'Maestros/MaePuesto/ModificarPuesto';
var urlImportarPuesto = baseUrl + 'Maestros/MaePuesto/Importar';


var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var codLocalAlternoAnterior = "";
var dataTablePuestos = null;

var AdministrarPuesto = function () {
    const eventos = function () {

        $('#tablePuestos').on('change', '.chk-indicador', function () {
            var indicador = $(this).data("indicador");
            var coEmpr = $(this).data("co-empr");
            var codPuesto = $(this).data("co-pues-trab");
            var nuevoValor = $(this).is(":checked") ? "S" : "N";

            var table = $('#tablePuestos').DataTable();
            var row = $(this).closest('tr');
            var rowData = table.row(row).data();

            var command = {
                CodEmpresa: coEmpr,
                CodPuesto: codPuesto,
                DesPuesto: rowData.DesPuesto,
                IndAutAut: rowData.IndAutAut,
                IndAutOpe: rowData.IndAutOpe,
                IndManAut: rowData.IndManAut,
                IndManOpe: rowData.IndManOpe,
                FecAsigna: null,
                UsuAsigna: $("#txtUsuario").val(),
                FecElimina: null,
                UsuElimina: $("#txtUsuario").val()
            };

            // Según el indicador, asigna el nuevo valor en la propiedad correspondiente.
            switch (indicador) {
                case "IndAutAut":
                    command.IndAutAut = nuevoValor;
                    break;
                case "IndAutOpe":
                    command.IndAutOpe = nuevoValor;
                    break;
                case "IndManAut":
                    command.IndManAut = nuevoValor;
                    break;
                case "IndManOpe":
                    command.IndManOpe = nuevoValor;
                    break;
            }

            $.ajax({
                url: urlModificarPuesto,
                type: "POST",
                data: command,
                dataType: "json",
                success: function (response) {
                    if (response.Ok) {
                        //swal("Actualizado", "El puesto se actualizó correctamente.", "success");
                        $('#tablePuestos').DataTable().ajax.reload(null, false);
                    } else {
                        swal("Error", response.Mensaje, "error");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal("Error", jqXHR.responseText, "error");
                }
            });
        });

        $("#btnBuscarPuesto").on("click", function (e) {
            var table = $('#tablePuestos').DataTable();
            e.preventDefault();
            table.ajax.reload();
        });
    }

    const listarEmpresasAsociadas = function () {
        return new Promise((resolve, reject) => {

            const codUsuario = $("#txtUsuario").val();

            const request = {
                CodUsuario: codUsuario,
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

    const cargarComboEmpresa = async function () {
        try {
            const response = await listarEmpresasAsociadas();

            if (response.Ok) {
                $('#cboEmpresaBuscar').empty().append('<option label="Todos"></option>');
                //$('#cboLocalBuscar').empty().append('<option label="Todos"></option>');
                response.Data.map(empresa => {
                    $('#cboEmpresaBuscar').append($('<option>', { value: empresa.CodEmpresa, text: empresa.NomEmpresa }));
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


    const visualizarDataTablePuestos = function () {
        $('#tablePuestos').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    CodEmpresa: $("#cboEmpresaBuscar").val(),
                    CodPuesto: $("#txtCodPuestoBuscar").val(),
                    DesPuesto: $("#txtDesPuestoBuscar").val()
                };

                // Combinar los parámetros de paginación con los filtros
                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);


                $.ajax({
                    url: urlListarPuestos,
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
            columnDefs: [
                { targets: 0, visible: false }  // Oculta la primera columna "COD. EMPRESA"
            ],
            columns: [
                { data: "CodEmpresa", title: "Empresa" },
                { data: "NomEmpresa", title: "Empresa" },
                { data: "CodPuesto", title: "Código Puesto" },
                { data: "DesPuesto", title: "Descripción Puesto" },
                {
                    data: "IndAutAut",
                    title: "Autorizador Automático",
                    render: function (data, type, row) {
                        var checked = data === "S" ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndAutAut" data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    },
                    orderable: false,
                    searchable: false
                },
                {
                    data: "IndAutOpe",
                    title: "Operador Automático",
                    render: function (data, type, row) {
                        var checked = data === "S" ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndAutOpe" data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    },
                    orderable: false,
                    searchable: false
                },
                {
                    data: "IndManAut",
                    title: "Autorizador Manual",
                    render: function (data, type, row) {
                        var checked = data === "S" ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndManAut" data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    },
                    orderable: false,
                    searchable: false
                },
                {
                    data: "IndManOpe",
                    title: "Operador Manual",
                    render: function (data, type, row) {
                        var checked = data === "S" ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndManOpe" data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    },
                    orderable: false,
                    searchable: false
                }
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
    };

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                visualizarDataTablePuestos();
            });
        }
    }
}(jQuery);