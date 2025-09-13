var urlListarDocumentosElectronicos = baseUrl + 'Operaciones/Reportes/ListarDocumentos';

var dataTableDocumentosElectronicos = null;

var DocumentoElectronico = function () {

    const eventos = function () {
        // Evento para el botón de acción
        $('#tableDocumentosElectronicos').on('click', '.btn-accion', function () {
            const docElectronico = $(this).data('doc-electronico');
            const nroDocumento = $(this).data('nro-documento');
            const local = $(this).data('local');

            // Llamar al método que ejecutará la acción
            ejecutarAccion(docElectronico, nroDocumento, local);
        });
    };

    const ejecutarAccion = function (docElectronico, nroDocumento, local) {
        // Aquí defines qué acción realizar cuando se hace clic en el botón
        swal({
            title: "Detalle del Documento",
            text: `Documento Electrónico: ${docElectronico}\nNúmero: ${nroDocumento}\nLocal: ${local}`,
            icon: "info"
        });

        // Ejemplo de otras acciones que podrías realizar:
        // - Abrir un modal con más detalles
        // - Hacer una llamada AJAX para obtener información adicional
        // - Redirigir a otra página
        // - Descargar un archivo
    };

    const visualizarDataTableDocumentos = function () {

        if (dataTableDocumentosElectronicos != null) {
            dataTableDocumentosElectronicos.destroy();
        }

        dataTableDocumentosElectronicos = $('#tableDocumentosElectronicos').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback, settings) {
                // Recoger los filtros de la página
                var request = {
                    CodLocal: $("#cboLocal").val(),
                    FechaInicio: $("#txtFechaInicio").val(),
                    FechaFin: $("#txtFechaFin").val(),
                    TipoDocCliente: $("#cboTipoDocCliente").val(),
                    NroDocCliente: $("#txtNroDocCliente").val(),
                    Cajero: $("#txtCajero").val(),
                    Caja: $("#txtCaja").val(),
                    NroTransaccion: $("#txtNroTransaccion").val(),
                    NumeroPagina: data.start / data.length + 1,
                    TamañoPagina: data.length,
                    Busqueda: $("#txtFiltro").val(),
                };

                $.ajax({
                    url: urlListarDocumentosElectronicos,
                    type: "POST",
                    data: request,
                    dataType: "json",
                    beforeSend: function () {
                        showLoading();
                    },
                    complete: function () {
                        closeLoading();
                    },
                    success: function (response) {
                        if (response.Ok) {
                            var pagedData = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pagedData.TotalRecords,
                                recordsFiltered: pagedData.TotalRecords,
                                data: pagedData.Items
                            });
                            $('#txtFiltro').focus();
                        } else {
                            swal({ text: response.Mensaje, icon: "error" });
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
                    data: "Fecha",
                    title: "Fecha",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { title: "Caja", data: "Caja" },
                { title: "Importe", data: "Importe" },
                { title: "Doc. Electrónico", data: "DocElectronico" },
                { title: "Medio Pago", data: "MedioPago" },
                { title: "Cajero", data: "Cajero" },
                { title: "Tip. Doc.", data: "TipoDocumento" },
                { title: "Num. Doc.", data: "NroDocumento" },
                {
                    title: "Acción",
                    data: null,
                    orderable: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return ` <a class="text-primary btn-accion" href="#" 
                        data-doc-electronico="${row.DocElectronico}" 
                                        data-nro-documento="${row.NroDocumento}"
                                        data-local="${row.Local}">
                                  <i class="fa fa-print "></i> Imprimir
                                </a>`;
                    }
                },
                { title: "Local", data: "Local" }
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
            eventos();
            visualizarDataTableDocumentos();
        }
    }
}(jQuery);