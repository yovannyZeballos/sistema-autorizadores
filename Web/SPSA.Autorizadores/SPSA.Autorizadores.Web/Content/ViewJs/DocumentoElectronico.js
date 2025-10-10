var urlListarDocumentosElectronicos = baseUrl + 'Operaciones/Reportes/ListarDocumentos';
var urlListarLocalesAsociados = baseUrl + 'Operaciones/Reportes/ListarLocalesAsociadasPorEmpresa';

var dataTableDocumentosElectronicos = null;

var DocumentoElectronico = function () {

    const eventos = function () {

        $('#btnLimpiar').on('click', function () {
            limpiarCampos();
        });


        $('#btnBuscar').on('click', function () {
            if (validarFechas()) {
                visualizarDataTableDocumentos();
            }
        });

        // Evento para el botón de acción
        $('#tableDocumentosElectronicos').on('click', '.btn-accion', function () {
            const nroDocumento = $(this).data('nro-documento');
            const tipoDocumento = $(this).data('tipo-documento');
            verPdf(nroDocumento, tipoDocumento);
        });

        $('#txtCajero, #txtNroTransaccion, #txtCaja').on('input', function () {
            // Solo números
            this.value = this.value.replace(/[^0-9]/g, '');

            // Limitar longitud solo para txtCajero
            if (this.id === 'txtCajero') {
                this.value = this.value.slice(0, 8);
            }
        });
    };

    const verPdf = function (nroDocumento, tipoDocumento) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', baseUrl + 'Operaciones/Reportes/VerPdf?numero=' + nroDocumento + '&tipo=' + tipoDocumento, true);
        xhr.responseType = 'blob';

        xhr.onload = function () {
            if (xhr.status === 200 && xhr.getResponseHeader('Content-Type') === 'application/pdf') {
                var blob = xhr.response;
                var url = URL.createObjectURL(blob);
                window.open(url, '_blank');
            } else {
                var reader = new FileReader();
                reader.onload = function () {
                    var respuesta = {};
                    try {
                        respuesta = JSON.parse(reader.result);
                    } catch (e) { }
                    swal({
                        text: respuesta.Mensaje || 'No se pudo mostrar el documento.',
                        icon: "error"
                    });
                };
                reader.readAsText(xhr.response);
            }
        };

        xhr.send();
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
                    TipoDocumento: $("#cboTipoDocumento").val(),
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
                { title: "Fecha", data: "Fecha" },
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
                                        data-nro-documento="${row.DocElectronico}"
                                        data-tipo-documento="${row.TipoDocElectronico}">
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

    const listarLocales = function () {

        $.ajax({
            url: urlListarLocalesAsociados,
            type: "post",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (response.Ok === true) {
                    cargarLocales(response.Data);
                } else {
                    swal({
                        text: response.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
            }
        });
    }

    const cargarLocales = function (locales) {
        $('#cboLocal').append($('<option>', { value: '0', text: 'Todos' }));
        locales.map(local => {
            $('#cboLocal').append($('<option>', { value: local.CodLocal, text: local.NomLocal }));
        });
        $('#cboLocal').val('0');
    }

    const inicializarDatePicker = function () {
        $('.fc-datepicker').datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            closeText: 'Cerrar',
            prevText: '<Ant',
            nextText: 'Sig>',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'dd/mm/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: '',
            changeMonth: true,
            changeYear: true
        });
    }

    const validarFechas = function () {
        const fechaInicio = $('#txtFechaInicio').val();
        const fechaFin = $('#txtFechaFin').val();

        console.log({ fechaInicio, fechaFin });

        // Si ambas fechas están vacías, no validar
        if (!fechaInicio || !fechaFin) {
            swal({
                title: "Error de validación",
                text: "La fecha de inicio y fin son obligatorios",
                icon: "warning"
            });
            return false;
        }

        // Convertir fechas del formato dd/mm/yyyy a Date
        const fechaInicioObj = convertirFecha(fechaInicio);
        const fechaFinObj = convertirFecha(fechaFin);

        // Validar que la fecha de inicio no sea superior a la fecha fin
        if (fechaInicioObj > fechaFinObj) {
            swal({
                title: "Error de validación",
                text: "La fecha de inicio no puede ser superior a la fecha fin.",
                icon: "warning"
            });
            return false;
        }

        // Calcular la diferencia en días
        const diferenciaTiempo = fechaFinObj.getTime() - fechaInicioObj.getTime();
        const diferenciaDias = Math.ceil(diferenciaTiempo / (1000 * 3600 * 24));

        // Validar que la diferencia no supere los 7 días
        if (diferenciaDias > 7) {
            swal({
                title: "Error de validación",
                text: "La diferencia entre la fecha de inicio y fin no puede superar los 7 días.",
                icon: "warning"
            });
            return false;
        }

        return true;
    };

    const convertirFecha = function (fechaString) {
        // Convertir fecha del formato dd/mm/yyyy a Date
        const partes = fechaString.split('/');
        const dia = parseInt(partes[0], 10);
        const mes = parseInt(partes[1], 10) - 1; // Los meses en JavaScript van de 0 a 11
        const año = parseInt(partes[2], 10);
        return new Date(año, mes, dia);
    };

    const limpiarCampos = function () {
        $('#cboLocal').val('0').trigger('change');
        $('#txtFechaInicio').val('');
        $('#txtFechaFin').val('');
        $('#txtCajero').val('0');
        $('#txtCaja').val('0');
        $('#txtNroTransaccion').val('0');
        $('#cboTipoDocumento').val('0').trigger('change');
        $('#cboTipoDocCliente').val('0').trigger('change');
        $('#txtNroDocCliente').val('');
    };

    return {
        init: function () {
            checkSession(function () {
                eventos();
                inicializarDatePicker();
                listarLocales();
            });
        }
    }
}(jQuery);