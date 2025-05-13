var urlListarSolicitudesCComCab = baseUrl + 'SolicitudCodComercio/AdministrarSolicitudComercio/ListarPaginado';

var solicitudesCComercioData = [];

var AdministrarSolicitudComercio = function () {
    const eventos = function () {
        $('#tableSolicitudesCab tbody').on('click', 'tr', function () {
            var tableCab = $('#tableSolicitudesCab').DataTable();
            var data = tableCab.row(this).data();

            if (data) {
                var detalles = data.Detalles || [];

                var tableDet = $('#tableSolicitudDet').DataTable();
                tableDet.clear().rows.add(detalles).draw();

                var tableCom = $('#tableLocalComercio').DataTable();
                tableCom.clear().draw(); // limpio comercios al cambiar de solicitud
            }
        });

        $('#tableSolicitudDet tbody').on('click', 'tr', function () {
            var tableDet = $('#tableSolicitudDet').DataTable();
            var data = tableDet.row(this).data();

            if (data) {
                var comercios = data.Comercios || [];

                var tableCom = $('#tableLocalComercio').DataTable();
                tableCom.clear().rows.add(comercios).draw();
            }
        });
    };

    const visualizarDataTableSolicitudesCab = function () {
        $('#tableSolicitudesCab').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    //CodLocalAlterno: $("#cboLocalBuscar").val(),
                    //CodigoOfisis: $("#txtCodOfisisBuscar").val(),
                    //NroDocIdent: $("#txtNroDocBuscar").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarSolicitudesCComCab,
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
                { targets: 0, visible: false }
            ],
            columns: [
                { data: "NroSolicitud", title: "Nro Solicitud" },
                { data: "TipEstado", title: "Estado" },
                //{ data: "CodigoOfisis", title: "Código" },
                //{ data: "ApelPaterno", title: "Ape. Paterno" },
                //{ data: "ApelMaterno", title: "Ape. Materno" },
                //{ data: "NombreTrabajador", title: "Nombre" },
                //{ data: "TipoDocIdent", title: "Tipo Doc" },
                //{ data: "NumDocIndent", title: "Nro Doc" },
                //{ data: "PuestoTrabajo", title: "Puesto" },
                {
                    data: "FecSolicitud",
                    title: "Fec. Solicitud",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                {
                    data: "FecRecepcion",
                    title: "Fec. Recepción",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                {
                    data: "FecRegistro",
                    title: "Fec. Registro",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                }
                //{ data: "TiSitu", title: "Estado" }
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

    const inicializarTablasSecundarias = function () {
        $('#tableSolicitudDet').DataTable({
            data: [],
            columns: [
                { data: 'CodLocalAlterno', title: 'Código Local' }
            ]
        });

        $('#tableLocalComercio').DataTable({
            data: [],
            columns: [
                { data: 'CodComercio', title: 'Código Comercio' },
                { data: 'NomCanalVta', title: 'Canal Venta' },
                { data: 'IndActiva', title: 'Estado' }
            ]
        });
    };

    return {
        init: function () {
            checkSession(async function () {
                inicializarTablasSecundarias();
                eventos();
                //await cargarComboEmpresa();
                visualizarDataTableSolicitudesCab();
            });
        }
    }
}(jQuery);