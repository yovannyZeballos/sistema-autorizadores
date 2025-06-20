var urlListarSolicitudesCComCab = baseUrl + 'SolicitudCodComercio/AdministrarSolicitudComercio/ListarPaginado';
var urlImportarSolicitudesCCom = baseUrl + 'SolicitudCodComercio/AdministrarSolicitudComercio/ImportarSolicitudes';
var urlImportarMaeLocalComercios = baseUrl + 'SolicitudCodComercio/AdministrarSolicitudComercio/ImportarComercios';
var urlCrearMaeCodComercio = baseUrl + 'SolicitudCodComercio/AdministrarSolicitudComercio/CrearEditarMaeCodComercio';

var modoEdicion = false;

var AdministrarSolicitudComercio = function () {
    const eventos = function () {
        $('#tableLocalComercio tbody').on('click', 'tr', seleccionarFila);
        $('#btnNuevoComercio').on('click', () => iniciarEdicion(false));
        $('#btnEditarComercio').on('click', () => iniciarEdicion(true));
        $('#btnGuardarNuevoComercio').on('click', guardarComercioDesdeFila);

        $('#tableSolicitudesCab tbody').on('click', 'tr', cargarDetallesSolicitud);
        $('#tableSolicitudDet tbody').on('click', 'tr', cargarComerciosLocal);

        $('#btnImportarSolicitudCab').on('click', () => $('#excelImportar').trigger('click'));
        $('#excelImportar').on('change', importarExcelSolicitud);

        $('#btnImportarMaeLocalComercio').on('click', () => $('#excelImportarComercios').trigger('click'));
        $('#excelImportarComercios').on('change', importarExcelComercios);
    };

    const seleccionarFila = function () {
        $(this).addClass('selected').siblings().removeClass('selected');
    };

    const iniciarEdicion = function (editar) {
        modoEdicion = editar;

        if (editar) {
            const tableCom = $('#tableLocalComercio').DataTable();
            if (tableCom.rows('.editing').any()) {
                swal("Advertencia", "Ya tienes una fila en edición.", "warning");
                return;
            }

            const row = $('#tableLocalComercio tbody tr.selected');
            if (!row.length) return swal('Advertencia', 'Debe seleccionar un comercio.', 'warning');

            const data = tableCom.row(row).data();
            if (!data) return;

            const inputs = generarInputs(data, true);
            row.html(inputs);
            row.addClass('editing');
        } else {
            agregarFilaEditableComercio();
        }

        toggleBotonesEdicion(true);
    };

    const cargarDetallesSolicitud = function () {
        $('#tableSolicitudesCab tbody tr').removeClass('selected');
        $(this).addClass('selected');

        const data = $('#tableSolicitudesCab').DataTable().row(this).data();
        if (!data) return;

        const detalles = data.Detalles || [];
        const tableDet = $('#tableSolicitudDet').DataTable();
        tableDet.clear().rows.add(detalles).draw();

        $('#modalDetalleLabel').text(`Detalle de Solicitud N° ${data.NroSolicitud}`);
        $('#modalDetalleComercio').modal('show');

        if (detalles.length) {
            $('#tableSolicitudDet tbody tr:eq(0)').addClass('selected');

            const firstRowData = tableDet.row(0).data();
            if (firstRowData) cargarComerciosLocalPorData(firstRowData);
        }
        else {
            $('#tableLocalComercio').DataTable().clear().draw();
        }
    };

    const cargarComerciosLocal = function () {
        $('#tableSolicitudDet tbody tr').removeClass('selected');
        $(this).addClass('selected');

        const data = $('#tableSolicitudDet').DataTable().row(this).data();
        if (data) cargarComerciosLocalPorData(data);
    };

    const cargarComerciosLocalPorData = function (data) {
        const tableCom = $('#tableLocalComercio').DataTable();
        tableCom.clear().rows.add(data.Comercios || []).draw();

        $('#modalComerciosLabel').text(`Comercios de Local: ${data.NomLocal}`);
        toggleBotonesEdicion(false);
    };

    const generarInputs = function (data, isEdit) {
        var readonly = isEdit ? 'readonly' : '';

        var codComercio = `<input type="text" class="form-control form-control-sm inputCodComercio" value="${data.CodComercio || ''}" ${readonly} />`;
        var nomCanalVta = `<input type="text" class="form-control form-control-sm inputNomCanalVta" value="${data.NomCanalVta || ''}" />`;

        var desOperador = `
        <select class="form-select form-select-sm inputDesOperador">
            <option value="IZIPAY" ${data.DesOperador === 'IZIPAY' ? 'selected' : ''}>IZIPAY</option>
            <option value="NIUBIZ" ${data.DesOperador === 'NIUBIZ' ? 'selected' : ''}>NIUBIZ</option>
            <option value="DINNERS" ${data.DesOperador === 'DINNERS' ? 'selected' : ''}>DINNERS</option>
        </select>`;

        var indActiva = `
        <select class="form-select form-select-sm inputIndActiva">
            <option value="S" ${data.IndActiva === 'S' ? 'selected' : ''}>Sí</option>
            <option value="N" ${data.IndActiva === 'N' ? 'selected' : ''}>No</option>
        </select>`;

        return `<td>${codComercio}</td><td>${nomCanalVta}</td><td>${desOperador}</td><td>${indActiva}</td>`;
    };

    const toggleBotonesEdicion = function (editing) {
        $('#btnGuardarNuevoComercio').prop('disabled', !editing);
        $('#btnNuevoComercio').prop('disabled', editing);
        $('#btnEditarComercio').prop('disabled', editing);
        $('#btnEliminarComercio').prop('disabled', editing);
    };

    const agregarFilaEditableComercio = function () {
        const tableCab = $('#tableSolicitudesCab').DataTable();
        const tableDet = $('#tableSolicitudDet').DataTable();

        const cabData = tableCab.row('#tableSolicitudesCab tbody tr.selected').data();
        const detData = tableDet.row('#tableSolicitudDet tbody tr.selected').data();

        if (!cabData || !detData) {
            swal("Advertencia", "Debe seleccionar una solicitud y un local", "warning");
            return;
        }

        const tableCom = $('#tableLocalComercio').DataTable();

        if (tableCom.rows('.editing').any()) {
            swal("Advertencia", "Ya tienes una fila en edición.", "warning");
            return;
        }

        const newRowData = {
            CodComercio: `<input type="text" class="form-control form-control-sm inputCodComercio" maxlength="12" oninput="this.value = this.value.replace(/[^0-9]/g, '')" />`,
            NomCanalVta: `<input type="text" class="form-control form-control-sm inputNomCanalVta" />`,
            DesOperador: `<select class="form-select form-select-sm inputDesOperador">
                <option value="IZIPAY">IZIPAY</option>
                <option value="NIUBIZ">NIUBIZ</option>
                <option value="DINNERS">DINNERS</option>
            </select>`,
            IndActiva: `<select class="form-select form-select-sm inputIndActiva">
                <option value="S">Sí</option>
                <option value="N">No</option>
            </select>`,
            NroSolicitud: cabData.NroSolicitud,
            CodLocalAlterno: detData.CodLocalAlterno
        };

        tableCom.row.add(newRowData).draw(false);
        $('#tableLocalComercio tbody tr:last').addClass('editing');
    };

    const guardarComercioDesdeFila = function () {
        var tableCom = $('#tableLocalComercio').DataTable();
        var fila = tableCom.row('.editing');

        if (!fila.any()) {
            swal("Advertencia", "No hay fila en edición.", "warning");
            return;
        }

        var $tr = $(fila.node());
        var filaData = fila.data(); // aquí está NroSolicitud y CodLocalAlterno

        var dataForm = {
            NroSolicitud: filaData.NroSolicitud,
            CodLocalAlterno: filaData.CodLocalAlterno,
            CodComercio: $tr.find('.inputCodComercio').val(),
            NomCanalVta: $tr.find('.inputNomCanalVta').val(),
            DesOperador: $tr.find('.inputDesOperador').val(),
            IndActiva: $tr.find('.inputIndActiva').val(),
            EsEdicion: modoEdicion
        };

        if (!dataForm.CodComercio) {
            swal("Error", "Debe ingresar código comercio", "error");
            return;
        }

        $.ajax({
            url: urlCrearMaeCodComercio,
            type: 'POST',
            data: { command: dataForm },
            dataType: 'json',
            beforeSend: function () { showLoading(); },
            complete: function () { closeLoading(); },
            success: function (response) {
                if (!response.Ok) {
                    return swal("Error", response.Mensaje, "error");
                }
                swal("Correcto", response.Mensaje, "success");

                tableCom.row($tr).remove();

                tableCom.row.add({
                    CodComercio: dataForm.CodComercio,
                    NomCanalVta: dataForm.NomCanalVta,
                    DesOperador: dataForm.DesOperador,
                    IndActiva: dataForm.IndActiva
                }).draw(false);

                $('#btnGuardarNuevoComercio').prop('disabled', true);
                $('#btnNuevoComercio').prop('disabled', false);
                $('#btnEliminarComercio').prop('disabled', false);
                $('#btnEditarComercio').prop('disabled', false);

                recargarSolicitudesCabManteniendoSeleccion(dataForm.NroSolicitud);


            },
            error: function () {
                swal("Error", "Ocurrió un error al guardar.", "error");
            }
        });
    };

    const visualizarDataTableSolicitudesCab = function () {
        $('#tableSolicitudesCab').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
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
                //{ targets: 0, visible: false }
            ],
            columns: [
                { data: "NroSolicitud", title: "Nro Solicitud" },
                { data: "RznSocial", title: "Razón Social" },
                {
                    data: 'TipEstado',
                    title: 'Estado',
                    render: function (data, type, row) {
                        if (data === 'S') return 'Solicitado';
                        if (data === 'R') return 'Recibido';
                        return data || '';
                    }
                },
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
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });
    };

    const inicializarTablasSecundarias = function () {
        $('#tableSolicitudDet').DataTable({
            data: [],
            ordering: false,
            columns: [
                { data: 'NomEmpresa', title: 'Empresa' },
                //{ data: 'CodLocalAlterno', title: 'Código Local' },
                { data: 'NomLocal', title: 'Local' },
                {
                    data: 'TipEstado',
                    title: 'Estado',
                    render: function (data, type, row) {
                        if (data === 'P') return 'Pendiente';
                        if (data === 'R') return 'Recibido';
                        return data || '';
                    }
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
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });

        $('#tableLocalComercio').DataTable({
            data: [],
            ordering: false,
            columns: [
                { data: 'CodComercio', title: 'Cod Comercio' },
                { data: 'NomCanalVta', title: 'Canal Venta' },
                { data: 'DesOperador', title: 'Operador' },
                { data: 'IndActiva', title: 'Activo' }
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
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });
    };

    const importarExcelSolicitud = function () {
        var formData = new FormData();
        var uploadFiles = $('#excelImportar').prop('files');
        formData.append("excelImportar", uploadFiles[0]);

        $.ajax({
            url: urlImportarSolicitudesCCom,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#excelImportar").val(null);
            },
            success: function (response) {
                if (!response.Ok) {
                    $('#tableSolicitudesCab').DataTable().ajax.reload(null, false);

                    if (response.Errores.length > 0) {
                        let mensajeErrores = response.Errores
                            .map(error => `Fila ${error.Fila}: ${error.Mensaje}`)
                            .join('\n');

                        swal({
                            title: response.Mensaje,
                            text: mensajeErrores,
                            icon: "warning",
                            buttons: true,
                        });
                    } else {
                        swal({
                            text: response.Mensaje,
                            icon: "warning"
                        });
                    }

                    return;
                }

                $('#tableSolicitudesCab').DataTable().ajax.reload(null, false);

                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const importarExcelComercios = function () {
        var formData = new FormData();
        var uploadFiles = $('#excelImportarComercios').prop('files');
        formData.append("excelImportarComercios", uploadFiles[0]);

        const solicitudCab = $('#tableSolicitudesCab').DataTable().row('#tableSolicitudesCab tbody tr.selected').data();
        if (!solicitudCab) {
            swal("Advertencia", "Debe seleccionar una solicitud.", "warning");
            return;
        }

        formData.append("nroSolicitud", solicitudCab.NroSolicitud.toString());
        const locales = solicitudCab.Detalles || [];
        formData.append("localesJson", JSON.stringify(locales));

        $.ajax({
            url: urlImportarMaeLocalComercios,
            type: "post",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
                $("#excelImportarComercios").val(null);
                recargarSolicitudesCabManteniendoSeleccion(solicitudCab.NroSolicitud);
            },
            success: function (response) {

                if (!response.Ok) {

                    if (response.Errores.length > 0) {
                        let mensajeErrores = response.Errores
                            .map(error => `Fila ${error.Fila}: ${error.Mensaje}`)
                            .join('\n');

                        swal({
                            title: response.Mensaje,
                            text: mensajeErrores,
                            icon: "warning",
                            buttons: true,
                        });
                    } else {
                        swal({
                            text: response.Mensaje,
                            icon: "warning"
                        });
                    }

                     return;
                }

                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarSolicitudesCabManteniendoSeleccion = function (nroSolicitud) {
        const tableCab = $('#tableSolicitudesCab').DataTable();
        const tableDet = $('#tableSolicitudDet').DataTable();
        const tableCom = $('#tableLocalComercio').DataTable();

        const info = tableCab.page.info();
        if (info.page !== 0) {
            // No estaba en la pág.1: voy a la 1 y redibujo
            tableCab.page(0).draw(false);
        } else {
            // Ya estaba en la pág.1: recargo directamente
            tableCab.ajax.reload(null, false);
        }

        // Una sola vez, tras el próximo draw, pinto detalle → comercio
        tableCab.one('draw', () => {
            const data = tableCab.rows({ page: 'current' }).data().toArray();
            const idx = data.findIndex(d => d.NroSolicitud === nroSolicitud);
            if (idx < 0) return;

            const rowNode = tableCab.row(idx).node();
            $('#tableSolicitudesCab tbody tr').removeClass('selected');
            $(rowNode).addClass('selected');

            const detalles = data[idx].Detalles || [];
            tableDet.clear().rows.add(detalles).draw(false);

            if (detalles.length) {
                $('#tableSolicitudDet tbody tr').removeClass('selected');
                $('#tableSolicitudDet tbody tr:eq(0)').addClass('selected');
                const comercios = detalles[0].Comercios || [];
                tableCom.clear().rows.add(comercios).draw(false);
            } else {
                tableCom.clear().draw(false);
            }
        });
    };


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                inicializarTablasSecundarias();
                visualizarDataTableSolicitudesCab();
            });
        }
    }
}(jQuery);