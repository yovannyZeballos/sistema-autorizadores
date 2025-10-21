/* URLs */
const urlListarSolicitudesCComCab = `${baseUrl}SolicitudCodComercio/AdministrarSolicitudComercio/ListarPaginado`;
const urlImportarSolicitudesCCom = `${baseUrl}SolicitudCodComercio/AdministrarSolicitudComercio/ImportarSolicitudes`;
const urlImportarMaeLocalComercios = `${baseUrl}SolicitudCodComercio/AdministrarSolicitudComercio/ImportarComercios`;
const urlCrearMaeCodComercio = `${baseUrl}SolicitudCodComercio/AdministrarSolicitudComercio/CrearEditarMaeCodComercio`;

const AdministrarSolicitudComercio = (function ($) {
    let dtCab, dtDet, dtCom;

    const EstadoCabMapper = { S: 'Solicitado', R: 'Recibido' };
    const EstadoDetMapper = { P: 'Pendiente', R: 'Recibido' };

    // ---------- Helpers ----------
    const htmlEscape = (v) => (v == null ? '' : String(v).replace(/[&<>"'`=\/]/g, s =>
        ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;', '`': '&#x60;', '=': '&#x3D;', '/': '&#x2F;' }[s])));

    const parseDotNetDate = (s) => {
        if (!s) return '';
        const m = /\/Date\((\d+)\)\//.exec(s);
        if (!m) return '';
        const d = new Date(parseInt(m[1], 10));
        return isNaN(d) ? '' : d.toLocaleDateString('es-PE');
    };

    const showError = (msg, title = 'Error') => swal({ title, text: msg || 'Ocurrió un error', icon: 'error' });

    const isEditingRow = () => $('#tableLocalComercio tbody tr.editing').length > 0;

    // Toolbar de comercios
    const setCommerceToolbarState = () => {
        const hayEdicion = isEditingRow();
        const hayDetSel = $('#tableSolicitudDet tbody tr.selected').length > 0;

        $('#btnNuevoComercio').prop('disabled', hayEdicion || !hayDetSel);
        const hayComSel = $('#tableLocalComercio tbody tr.selected').length > 0;
        $('#btnEditarComercio').prop('disabled', hayEdicion || !hayComSel);
    };

    const bindNumericCodComercio = ($row) => {
        $row.find('.inputCodComercio')
            .attr('maxlength', 12)
            .on('input', function () {
                this.value = this.value.replace(/\D/g, '').slice(0, 12);
            });
    };

    // ---------- DataTables ----------
    const visualizarDataTableSolicitudesCab = function () {
        dtCab = $('#tableSolicitudesCab').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback) {
                const pageNumber = (data.start / data.length) + 1;
                const pageSize = data.length;
                const params = { PageNumber: pageNumber, PageSize: pageSize };

                $.ajax({
                    url: urlListarSolicitudesCComCab,
                    type: 'GET',
                    data: params,
                    dataType: 'json',
                    success: function (response) {
                        if (response && response.Ok && response.Data) {
                            const pg = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: pg.TotalRecords,
                                recordsFiltered: pg.TotalRecords,
                                data: pg.Items
                            });
                        } else {
                            callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                        }
                    },
                    error: function (jqXHR) {
                        showError(jqXHR.responseText);
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    }
                });
            },
            columns: [
                { data: 'NroSolicitud', title: 'Nro Solicitud' },
                { data: 'RznSocial', title: 'Razón Social' },
                { data: 'TipEstado', title: 'Estado', render: (d) => EstadoCabMapper[d] || (d || '') },
                { data: 'FecSolicitud', title: 'Fec. Solicitud', render: parseDotNetDate },
                { data: 'FecRecepcion', title: 'Fec. Recepción', render: parseDotNetDate },
                { data: 'FecRegistro', title: 'Fec. Registro', render: parseDotNetDate }
            ],
            language: {
                searchPlaceholder: 'Buscar...', sSearch: '',
                lengthMenu: 'Mostrar _MENU_ registros por página',
                zeroRecords: 'No se encontraron resultados',
                info: 'Mostrando página _PAGE_ de _PAGES_',
                infoEmpty: 'No hay registros disponibles',
                infoFiltered: '(filtrado de _MAX_ registros totales)'
            },
            scrollY: '400px', scrollX: true, scrollCollapse: true,
            paging: true, lengthMenu: [10, 25, 50, 100]
        });
    };

    const inicializarTablasSecundarias = function () {
        dtDet = $('#tableSolicitudDet').DataTable({
            data: [], ordering: false,
            columns: [
                { data: 'NomLocal', title: 'Local' },
                { data: 'TipEstado', title: 'Estado', render: (d) => EstadoDetMapper[d] || (d || '') }
            ],
            language: {
                searchPlaceholder: 'Buscar...', sSearch: '',
                lengthMenu: 'Mostrar _MENU_ registros por página',
                zeroRecords: 'No se encontraron resultados',
                info: 'Mostrando página _PAGE_ de _PAGES_',
                infoEmpty: 'No hay registros disponibles',
                infoFiltered: '(filtrado de _MAX_ registros totales)'
            },
            paging: true, lengthMenu: [10, 25, 50, 100]
        });

        dtCom = $('#tableLocalComercio').DataTable({
            data: [], ordering: false,
            columns: [
                { data: 'CodComercio', title: 'Cod Comercio' },
                { data: 'NomCanalVta', title: 'Canal Venta' },
                { data: 'DesOperador', title: 'Operador' },
                //{
                //    data: null, title: 'Activo',
                //    render: (d, t, row) => {
                //        const v = row.IndEstado ?? row.IndActiva ?? '';
                //        if (t !== 'display') return v;
                //        return v === 'S' ? 'Sí' : (v === 'N' ? 'No' : '');
                //    },
                //    defaultContent: ''
                //},
                // === Activo (acepta HTML en edición o S/N en lectura) ===
                {
                    data: 'IndEstado',            // <- usamos SIEMPRE esta propiedad
                    title: 'Activo',
                    render: (d, t, row) => {
                        // Si estamos en edición, d es el <select> y lo mostramos tal cual
                        if (typeof d === 'string' && d.indexOf('<select') !== -1) return d;
                        // Para export/cálculo
                        if (t !== 'display') return d ?? '';
                        // Para pantalla: mapea S/N a Sí/No
                        return d === 'S' ? 'Sí' : (d === 'N' ? 'No' : '');
                    },
                    defaultContent: ''
                },
                { data: 'Acciones', title: 'Acciones', orderable: false, searchable: false, defaultContent: '' }
            ],
            language: {
                searchPlaceholder: 'Buscar...', sSearch: '',
                lengthMenu: 'Mostrar _MENU_ registros por página',
                zeroRecords: 'No se encontraron resultados',
                info: 'Mostrando página _PAGE_ de _PAGES_',
                infoEmpty: 'No hay registros disponibles',
                infoFiltered: '(filtrado de _MAX_ registros totales)'
            },
            paging: true, lengthMenu: [10, 25, 50, 100]
        });
    };

    // ---------- Maestro → Detalle → Comercios ----------
    const cargarDetallesSolicitud = function () {
        $('#tableSolicitudesCab tbody tr').removeClass('selected');
        $(this).addClass('selected');

        const data = dtCab.row(this).data();
        if (!data) return;

        const detalles = data.Detalles || [];
        dtDet.clear().rows.add(detalles).draw();

        $('#lblSolicitudHeader').text(`Solicitud N° ${data.NroSolicitud} — ${data.RznSocial}`);
        $('#modalDetalleComercio').modal('show');

        if (detalles.length) {
            $('#tableSolicitudDet tbody tr:eq(0)').addClass('selected');
            cargarComerciosLocalPorData(detalles[0]);
        } else {
            dtCom.clear().draw();
            setCommerceToolbarState();
        }
    };

    const cargarComerciosLocal = function () {
        if (isEditingRow()) return swal('Advertencia', 'Termina la edición actual primero.', 'warning');

        $('#tableSolicitudDet tbody tr').removeClass('selected');
        $(this).addClass('selected');

        const data = dtDet.row(this).data();
        if (data) cargarComerciosLocalPorData(data);
    };

    const cargarComerciosLocalPorData = function (det) {
        const cab = dtCab.row('#tableSolicitudesCab tbody tr.selected').data() || {};
        const comercios = (det.Comercios || []).map(c => ({
            ...c,
            NroSolicitud: cab.NroSolicitud,
            CodEmpresa: det.CodEmpresa,
            CodLocal: det.CodLocal
        }));

        dtCom.clear().rows.add(comercios).draw();
        $('#lblLocalHeader').text(det.NomLocal || '');
        setCommerceToolbarState();
    };

    // ---------- Edición en-fila ----------
    const makeRowEditable = ($tr, isNew, snapshotObj) => {
        const row = dtCom.row($tr);
        const data = row.data() || {};
        const snap = snapshotObj || JSON.parse(JSON.stringify(data));

        const editObj = {
            CodComercio: `<input type="text" class="form-control form-control-sm inputCodComercio" value="${htmlEscape(data.CodComercio || '')}" maxlength="12" />`,
            NomCanalVta: `<input type="text" class="form-control form-control-sm inputNomCanalVta" value="${htmlEscape(data.NomCanalVta || '')}" />`,
            DesOperador: `<select class="form-select form-select-sm inputDesOperador">
                      <option value="VISA-IZIPAY"  ${data.DesOperador === 'VISA-IZIPAY' ? 'selected' : ''}>VISA-IZIPAY</option>
                      <option value="NIUBIZ"  ${data.DesOperador === 'NIUBIZ' ? 'selected' : ''}>NIUBIZ</option>
                      <option value="DINNERS" ${data.DesOperador === 'DINNERS' ? 'selected' : ''}>DINNERS</option>
                    </select>`,
            IndEstado: `<select class="form-select form-select-sm inputIndEstado">
                      <option value="S" ${(data.IndEstado ?? data.IndActiva) === 'S' ? 'selected' : ''}>Sí</option>
                      <option value="N" ${(data.IndEstado ?? data.IndActiva) === 'N' ? 'selected' : ''}>No</option>
                    </select>`,
            Acciones: `
        <div class="d-flex gap-2">
          <button type="button" class="btn btn-sm btn-success btn-save-row"><i class="fe fe-save"></i> Guardar</button>
          <button type="button" class="btn btn-sm btn-secondary btn-cancel-row"><i class="fe fe-x"></i> Cancelar</button>
        </div>`,

            // metas + snapshot
            NroSolicitud: data.NroSolicitud,
            CodEmpresa: data.CodEmpresa,
            CodLocal: data.CodLocal,
            __snapshot: snap,
            __isNew: !!isNew
        };

        row.data(editObj).draw(false);
        const $new = $(row.node()).addClass('editing');
        bindNumericCodComercio($new);
        setCommerceToolbarState();
    };

    const salirDeEdicionConDatos = ($tr, plainObj) => {
        const row = dtCom.row($tr);
        const prev = row.data() || {};
        const displayObj = {
            CodComercio: plainObj.CodComercio,
            NomCanalVta: plainObj.NomCanalVta,
            DesOperador: plainObj.DesOperador,
            IndEstado: plainObj.IndEstado,
            Acciones: '',
            NroSolicitud: prev.NroSolicitud,
            CodEmpresa: prev.CodEmpresa,
            CodLocal: prev.CodLocal
        };
        row.data(displayObj).draw(false);
        $(row.node()).removeClass('editing');
        setCommerceToolbarState();
    };

    // Nuevo
    const agregarFilaEditableComercio = function () {
        if (isEditingRow()) return swal('Advertencia', 'Ya tienes una fila en edición.', 'warning');

        const cabData = dtCab.row('#tableSolicitudesCab tbody tr.selected').data();
        const detData = dtDet.row('#tableSolicitudDet tbody tr.selected').data();
        if (!cabData || !detData) return swal('Advertencia', 'Debe seleccionar una solicitud y un local.', 'warning');

        const nueva = {
            CodComercio: '', NomCanalVta: '', DesOperador: '', IndEstado: '', Acciones: '',
            NroSolicitud: cabData.NroSolicitud, CodEmpresa: detData.CodEmpresa, CodLocal: detData.CodLocal
        };
        const row = dtCom.row.add(nueva).draw(false);
        makeRowEditable($(row.node()), true);
    };

    // Editar
    const iniciarEdicionExistente = function () {
        if (isEditingRow()) return swal('Advertencia', 'Ya tienes una fila en edición.', 'warning');
        const $sel = $('#tableLocalComercio tbody tr.selected');
        if (!$sel.length) return swal('Advertencia', 'Debe seleccionar un comercio.', 'warning');

        const snap = JSON.parse(JSON.stringify(dtCom.row($sel).data() || {}));
        makeRowEditable($sel, false, snap);
    };

    // Guardar (en-fila)
    const onSaveRow = function () {
        const $tr = $(this).closest('tr');
        if (!$tr.hasClass('editing')) return;

        const row = dtCom.row($tr);
        const rowData = row.data() || {};
        const codComercio = ($tr.find('.inputCodComercio').val() || '').trim();
        const nomCanalVta = ($tr.find('.inputNomCanalVta').val() || '').trim();
        const desOperador = $tr.find('.inputDesOperador').val();
        const indEstado = $tr.find('.inputIndEstado').val();

        if (!/^\d{1,12}$/.test(codComercio)) return showError('El Código de Comercio debe ser numérico (máx. 12 dígitos).');

        const { NroSolicitud, CodEmpresa, CodLocal, __isNew } = rowData || {};
        if (!NroSolicitud || !CodEmpresa || !CodLocal) return showError('Faltan datos de solicitud/local para guardar.');

        const payload = {
            NroSolicitud, CodEmpresa, CodLocal,
            CodComercio: codComercio,
            NomCanalVta: nomCanalVta,
            DesOperador: desOperador,
            IndEstado: indEstado,  // si tu API espera IndActiva, cámbialo aquí
            EsEdicion: !__isNew
        };

        $.ajax({
            url: urlCrearMaeCodComercio,
            type: 'POST',
            data: { command: payload },
            dataType: 'json',
            beforeSend: showLoading,
            complete: closeLoading,
            success: function (response) {
                if (!response || !response.Ok) return showError((response && response.Mensaje) || 'No se pudo guardar.');
                swal('Correcto', response.Mensaje, 'success');

                salirDeEdicionConDatos($tr, {
                    CodComercio: codComercio,
                    NomCanalVta: nomCanalVta,
                    DesOperador: desOperador,
                    IndEstado: indEstado
                });

                // refresco liviano conservando selección
                recargarSolicitudesCabManteniendoSeleccion(NroSolicitud);
            },
            error: function (jqXHR) { showError(jqXHR.responseText); }
        });
    };

    // Cancelar (en-fila)
    const onCancelRow = function () {
        const $tr = $(this).closest('tr');
        if (!$tr.hasClass('editing')) return;

        const row = dtCom.row($tr);
        const data = row.data() || {};
        if (data.__isNew) {
            row.remove().draw(false);
            setCommerceToolbarState();
            return;
        }

        const snap = data.__snapshot;
        if (snap) {
            row.data(snap).draw(false);
            $(row.node()).removeClass('editing');
            setCommerceToolbarState();
        } else {
            // fallback
            salirDeEdicionConDatos($tr, {
                CodComercio: htmlEscape(data.CodComercio || ''),
                NomCanalVta: htmlEscape(data.NomCanalVta || ''),
                DesOperador: htmlEscape(data.DesOperador || ''),
                IndEstado: htmlEscape((data.IndEstado ?? data.IndActiva) || '')
            });
        }
    };

    // ---------- Importaciones ----------
    const importarExcelSolicitud = function () {
        const f = $('#excelImportar').prop('files')[0];
        if (!f) return swal('Advertencia', 'Seleccione un archivo Excel.', 'warning');

        const formData = new FormData();
        formData.append('excelImportar', f);

        $.ajax({
            url: urlImportarSolicitudesCCom,
            type: 'POST',
            data: formData, dataType: 'json',
            contentType: false, processData: false,
            beforeSend: showLoading,
            complete: function () { closeLoading(); $('#excelImportar').val(null); },
            success: function (response) {
                dtCab.ajax.reload(null, false);
                if (!response || !response.Ok) {
                    if (response && Array.isArray(response.Errores) && response.Errores.length) {
                        const msg = response.Errores.map(e => `Fila ${e.Fila}: ${e.Mensaje}`).join('\n');
                        swal({ title: response.Mensaje || 'Advertencia', text: msg, icon: 'warning', buttons: true });
                    } else {
                        swal({ text: (response && response.Mensaje) || 'Error al importar', icon: 'warning' });
                    }
                    return;
                }
                swal({ text: response.Mensaje, icon: 'success' });
            },
            error: function (jqXHR) { showError(jqXHR.responseText); }
        });
    };

    const importarExcelComercios = function () {
        const f = $('#excelImportarComercios').prop('files')[0];
        if (!f) return swal('Advertencia', 'Seleccione un archivo Excel.', 'warning');

        const solicitudCab = dtCab.row('#tableSolicitudesCab tbody tr.selected').data();
        if (!solicitudCab) return swal('Advertencia', 'Debe seleccionar una solicitud.', 'warning');

        const formData = new FormData();
        formData.append('excelImportarComercios', f);
        formData.append('nroSolicitud', String(solicitudCab.NroSolicitud));
        formData.append('localesJson', JSON.stringify(solicitudCab.Detalles || []));

        $.ajax({
            url: urlImportarMaeLocalComercios,
            type: 'POST',
            data: formData, dataType: 'json',
            contentType: false, processData: false,
            beforeSend: showLoading,
            complete: function () {
                closeLoading();
                $('#excelImportarComercios').val(null);
                recargarSolicitudesCabManteniendoSeleccion(solicitudCab.NroSolicitud);
            },
            success: function (response) {
                if (!response || !response.Ok) {
                    if (response && Array.isArray(response.Errores) && response.Errores.length) {
                        const msg = response.Errores.map(e => `Fila ${e.Fila}: ${e.Mensaje}`).join('\n');
                        swal({ title: response.Mensaje || 'Advertencia', text: msg, icon: 'warning', buttons: true });
                    } else {
                        swal({ text: (response && response.Mensaje) || 'Error al importar comercios', icon: 'warning' });
                    }
                    return;
                }
                swal({ text: response.Mensaje, icon: 'success' });
            },
            error: function (jqXHR) { showError(jqXHR.responseText); }
        });
    };

    // Mantiene selección al recargar cabecera
    const recargarSolicitudesCabManteniendoSeleccion = function (nroSolicitud) {
        const info = dtCab.page.info();
        if (info.page !== 0) dtCab.page(0).draw(false); else dtCab.ajax.reload(null, false);

        dtCab.one('draw', () => {
            const data = dtCab.rows({ page: 'current' }).data().toArray();
            const idx = data.findIndex(d => d.NroSolicitud === nroSolicitud);
            if (idx < 0) return;

            const rowNode = dtCab.row(idx).node();
            $('#tableSolicitudesCab tbody tr').removeClass('selected');
            $(rowNode).addClass('selected');

            const detalles = data[idx].Detalles || [];
            dtDet.clear().rows.add(detalles).draw(false);

            if (detalles.length) {
                $('#tableSolicitudDet tbody tr').removeClass('selected');
                $('#tableSolicitudDet tbody tr:eq(0)').addClass('selected');
                const comercios = detalles[0].Comercios || [];
                dtCom.clear().rows.add(comercios).draw(false);
            } else {
                dtCom.clear().draw(false);
            }
            setCommerceToolbarState();
        });
    };

    // ---------- Eventos ----------
    const eventos = function () {
        // Selección cabecera
        $('#tableSolicitudesCab tbody').on('click', 'tr', function () {
            if (isEditingRow()) return swal('Advertencia', 'Termina la edición actual primero.', 'warning');
            $('#tableSolicitudesCab tbody tr').removeClass('selected');
            $(this).addClass('selected');
            cargarDetallesSolicitud.call(this);
        });

        // Selección detalle
        $('#tableSolicitudDet tbody').on('click', 'tr', function () {
            if (isEditingRow()) return swal('Advertencia', 'Termina la edición actual primero.', 'warning');
            $('#tableSolicitudDet tbody tr').removeClass('selected');
            $(this).addClass('selected');
            cargarComerciosLocal.call(this);
        });

        // Selección comercio (sólo para habilitar Editar)
        $('#tableLocalComercio tbody').on('click', 'tr', function () {
            if (!$(this).hasClass('editing')) {
                $(this).addClass('selected').siblings().removeClass('selected');
                setCommerceToolbarState();
            }
        });

        // Botones toolbar de comercios (dentro del body de “comercios”)
        $('#btnNuevoComercio').on('click', agregarFilaEditableComercio);
        $('#btnEditarComercio').on('click', iniciarEdicionExistente);

        // Acciones en-fila
        $('#tableLocalComercio tbody')
            .on('click', '.btn-save-row', onSaveRow)
            .on('click', '.btn-cancel-row', onCancelRow);

        // Importaciones
        $('#btnImportarSolicitudCab').on('click', () => $('#excelImportar').trigger('click'));
        $('#excelImportar').on('change', importarExcelSolicitud);

        $('#btnImportarMaeLocalComercio').on('click', () => $('#excelImportarComercios').trigger('click'));
        $('#excelImportarComercios').on('change', importarExcelComercios);

        // Si cierran el modal, cancela edición si la hubiera
        $('#modalDetalleComercio').on('hidden.bs.modal', function () {
            const $ed = $('#tableLocalComercio tbody tr.editing');
            if ($ed.length) onCancelRow.call($ed.find('.btn-cancel-row')[0] || $ed[0]);
        });
    };

    return {
        init: function () {
            checkSession(function () {
                eventos();
                inicializarTablasSecundarias();
                visualizarDataTableSolicitudesCab();
            });
        }
    };
}(jQuery));
