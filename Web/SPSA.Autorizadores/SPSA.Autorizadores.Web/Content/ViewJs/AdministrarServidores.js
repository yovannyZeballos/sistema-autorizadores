// ======================== URLs ========================
var urlServListarPaginado = baseUrl + 'Inventario/Servidores/ListarPaginado';
var urlServListarTipos = baseUrl + 'Inventario/Servidores/ListarTipos';
var urlServListarSo = baseUrl + 'Inventario/Servidores/ListarSo';
var urlServCrear = baseUrl + 'Inventario/Servidores/Crear';
var urlServEditar = baseUrl + 'Inventario/Servidores/Editar';
var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmp = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';

var AdministrarServidores = (function ($) {

    // ======================== Estado ========================
    var dt = null;
    var _tiposCache = [];
    var _soCache = [];
    var _editContext = { mode: 'create', currentId: null, currentRowData: null };

    // ======================== Cargar combos ========================
    async function listarEmpresas() {
        return $.ajax({ url: urlListarEmpresas, type: 'POST', data: { request: { CodUsuario: '', Busqueda: '' } } });
    }

    async function listarLocales(codEmpresa) {
        if (!codEmpresa) return { Ok: true, Data: [] };
        return $.ajax({ url: urlListarLocalesPorEmp, type: 'POST', data: { request: { CodEmpresa: codEmpresa } } });
    }

    async function listarTiposServidor() {
        return $.ajax({ url: urlServListarTipos, type: 'GET' });
    }

    async function listarSoServidor() {
        return $.ajax({ url: urlServListarSo, type: 'GET' });
    }

    async function cargarFiltroEmpresas() {
        try {
            const resp = await listarEmpresas();
            var $emp = $('#fEmp');
            $emp.empty().append('<option value="">Todos</option>');
            if (resp.Ok) {
                resp.Data.forEach(e => $emp.append(new Option(e.NomEmpresa, e.CodEmpresa)));
            }
            initSelect2($emp, { placeholder: 'Todos' });
        } catch (err) { swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' }); }
    }

    async function cargarFiltroLocales() {
        var codEmp = $('#fEmp').val();
        var $loc = $('#fLoc');
        $loc.empty().append('<option value="">Todos</option>');
        if (!codEmp) {
            initSelect2($loc, { placeholder: 'Todos' });
            $loc.prop('disabled', true).trigger('change.select2');
            return;
        }
        try {
            const resp = await listarLocales(codEmp);
            if (resp.Ok) {
                resp.Data.forEach(l => $loc.append(new Option(l.NomLocal, l.CodLocal)));
            }
            initSelect2($loc, { placeholder: 'Todos' });
            $loc.prop('disabled', false).trigger('change.select2');
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' });
        }
    }

    async function ensureTipos() {
        if (_tiposCache.length) return;
        const resp = await listarTiposServidor();
        if (resp.Ok) _tiposCache = resp.Data || [];
    }

    async function ensureSo() {
        if (_soCache.length) return;
        const resp = await listarSoServidor();
        if (resp.Ok) _soCache = resp.Data || [];
    }

    async function cargarFiltroEmpresas() {
        const resp = await listarEmpresas();
        if (resp.Ok) {
            let data = resp.Data.map(x => ({ text: x.NomEmpresa, value: x.CodEmpresa }));
            await cargarCombo($('#fEmp'), data, { placeholder: 'Todos', todos: true });
        }
    }

    async function cargarFiltroLocales() {
        var codEmp = $('#fEmp').val();
        if (!codEmp) {
            await cargarCombo($('#fLoc'), [], { placeholder: 'Todos', todos: true });
            $('#fLoc').prop('disabled', true);
            return;
        }
        const resp = await listarLocales(codEmp);
        if (resp.Ok) {
            let data = resp.Data.map(x => ({ text: x.NomLocal, value: x.CodLocal }));
            await cargarCombo($('#fLoc'), data, { placeholder: 'Todos', todos: true });
            $('#fLoc').prop('disabled', false);
        }
    }

    async function cargarFiltroTipos() {
        await ensureTipos();
        let data = _tiposCache.map(x => ({ text: x.NomTipo, value: x.Id }));
        await cargarCombo($('#fTipo'), data, { placeholder: 'Todos', todos: true });
    }

    function initCombosFijos() {
        //cargarCombo($('#fEstadoSerie'), [], { placeholder: 'Todos', todos: true });

        $('#fEstadoSerie').select2({
            width: '100%',
            placeholder: 'Todos',
            allowClear: true,
            minimumResultsForSearch: 0
        });
    }

    // ======================== DataTable ========================
    function visualizarDataTable() {
        dt = $('#tableServidores').DataTable({
            serverSide: true,
            processing: true,
            searching: true,
            ordering: false,
            ajax: function (data, callback) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    CodEmpresa: $('#fEmp').val() || null,
                    CodLocal: $('#fLoc').val() || null,
                    IndEstadoSerie: $('#fEstadoSerie').val() || null,
                    TipoId: $('#fTipo').val() ? parseInt($('#fTipo').val(), 10) : null,
                    Texto: (data.search && data.search.value) ? data.search.value : ''
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlServListarPaginado,
                    type: 'GET',
                    data: params,
                    dataType: 'json',
                    success: function (resp) {
                        if (resp.Ok) {
                            var p = resp.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: p.TotalRecords,
                                recordsFiltered: p.TotalRecords,
                                data: p.Items
                            });
                        } else {
                            callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                            swal({ text: swalText(resp, 'No se pudo listar'), icon: 'warning' });
                        }
                    },
                    error: function (jq) {
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                        swal({ text: swalText(jq, 'Error al listar'), icon: 'error' });
                    }
                });
            },
            columns: [
                { data: 'NumSerie', title: 'Serie', defaultContent: '' },
                { data: 'DesProducto', title: 'Producto', defaultContent: '' },
                { data: 'NomMarca', title: 'Marca', defaultContent: '' },
                { data: 'Modelo', title: 'Modelo', defaultContent: '' },
                { data: 'TipoServidor', title: 'Tipo', defaultContent: '' },
                { data: 'Hostname', title: 'Hostname', defaultContent: '' },
                { data: 'Ip', title: 'IP', defaultContent: '' },
                { data: 'NomEmpresa', title: 'Empresa', defaultContent: '' },
                { data: 'NomLocal', title: 'Local', defaultContent: '' },
                //{ data: 'IndEstadoSerie', title: 'Estado Serie', defaultContent: '' },
                {
                    data: 'IndEstadoSerie',
                    title: 'Estado Serie',
                    render: function (data) {
                        if (!data) return '';
                        let cls = '';
                        switch (data) {
                            case 'DISPONIBLE': cls = 'dot-disponible'; break;
                            case 'EN_TRANSITO': cls = 'dot-transito'; break;
                            case 'DE_BAJA': cls = 'dot-baja'; break;
                        }
                        return `<span class="badge-dot"><i class="${cls}"></i>${data}</span>`;
                    }
                },
                { data: 'RamGb', title: 'RAM (GB)', defaultContent: '' },
                { data: 'CpuSockets', title: 'Procesadores', defaultContent: '' },
                { data: 'CpuCores', title: 'Cores', defaultContent: '' },
                { data: 'HddTotal', title: 'HDD (Total)', defaultContent: '' },
                { data: 'NomSo', title: 'SO', defaultContent: '' },
                { data: 'IpIdracIlo', title: 'iDRAC / iLO', defaultContent: '' },
                { data: 'HostnameUnicola', title: 'Unicola', defaultContent: '' },
                { data: 'HostnameBranch', title: 'Branch', defaultContent: '' },
                { data: 'HostnameFileServer', title: 'FileServer', defaultContent: '' },
                { data: 'EnlaceUrl', title: 'Enlace', defaultContent: '' }
            ],
            language: {
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                search: "Buscar:"
            },
            initComplete: function () {
                $('#tableServidores_filter input')
                    .addClass('form-control-sm')
                    .attr('placeholder', 'hostname / serie / producto / marca / modelo');
            },
            scrollY: '520px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
            // Selección por fila
            rowCallback: function (row) {
                $(row).css('cursor', 'pointer');
            },
            drawCallback: function () {
                // Limpia click handlers previos
                $('#tableServidores tbody tr').off('click').on('click', function () {
                    // toggle selección única
                    if ($(this).hasClass('selected')) {
                        $(this).removeClass('selected');
                    } else {
                        $('#tableServidores tbody tr.selected').removeClass('selected');
                        $(this).addClass('selected');
                    }
                });

                // Doble click abre modal
                $('#tableServidores tbody tr').off('dblclick').on('dblclick', function () {
                    var row = dt.row(this).data();
                    if (row) abrirModalEditar(row);
                });
            }
        });
    }

    // ======================== Modal Crear/Editar ========================
    function limpiarModal() {

        $('#modalSrvSerie input, #modalSrvSerie textarea').val('');
        $('#mSerieId, #mTipoId, #mSoId').val('').trigger('change');
    }

    async function prepararCombosModal() {
        await ensureTipos();
        await ensureSo();

        let dataTipos = _tiposCache.map(x => ({ text: x.NomTipo, value: x.Id }));
        let dataSo = _soCache.map(x => ({ text: x.NomSo, value: x.Id }));

        await cargarCombo($('#mTipoId'), dataTipos, { placeholder: 'Seleccionar', dropdownParent: '#modalSrvSerie' });
        await cargarCombo($('#mSerieId'), [], { placeholder: 'Seleccionar', dropdownParent: '#modalSrvSerie' });
        await cargarCombo($('#mSoId'), dataSo, { placeholder: 'Seleccionar', dropdownParent: '#modalSrvSerie' });
    }

    async function abrirModalCrearConSerie(row) {
        _editContext = { mode: 'create', currentId: row.SerieProductoId, currentRowData: row };
        limpiarModal();
        await prepararCombosModal();

        $('#mSerieId').prop('disabled', true)
            .append(new Option(row.NumSerie || ('#' + row.SerieProductoId), row.SerieProductoId))
            .val(row.SerieProductoId).trigger('change');

        $('#modalSrvSerieTitle').text('Crear características de Servidor');
        new bootstrap.Modal(document.getElementById('modalSrvSerie')).show();
    }


    async function abrirModalEditar(row) {
        if (!row || row.TipoId == null) {
            return abrirModalCrearConSerie(row);
        }

        _editContext = { mode: 'edit', currentId: row.SerieProductoId, currentRowData: row };
        limpiarModal();
        await prepararCombosModal();

        $('#mSerieId').prop('disabled', true)
            .append(new Option(row.NumSerie || ('#' + row.SerieProductoId), row.SerieProductoId))
            .val(row.SerieProductoId).trigger('change');

        $('#mTipoId').val(row.TipoId).trigger('change');
        $('#mSoId').val(row.SoId).trigger('change');

        $('#mHostname').val(row.Hostname || '');
        $('#mIp').val(row.Ip || '');
        $('#mRam').val(row.RamGb || '');
        $('#mCpuSockets').val(row.CpuSockets || '');
        $('#mCpuCores').val(row.CpuCores || '');
        $('#mHddGb').val(row.HddTotal || '');
        $('#mHostBranch').val(row.HostnameBranch || '');
        $('#mIpBranch').val(row.IpBranch || '');
        $('#mHostFs').val(row.HostnameFileServer || '');
        $('#mIpFs').val(row.IpFileServer || '');
        $('#mHostUnicola').val(row.HostnameUnicola || '');
        $('#mIpUnicola').val(row.IpUnicola || '');
        $('#mEnlaceUrl').val(row.EnlaceUrl || '');
        $('#mIpIdracIlo').val(row.IpIdracIlo || '');

        $('#modalSrvSerieTitle').text('Detalle de Servidor');
        new bootstrap.Modal(document.getElementById('modalSrvSerie')).show();
    }

    
    function validarModal() {
        var errores = [];
        if (_editContext.mode === 'create' && !$('#mSerieId').val()) errores.push('Seleccione la serie del producto.');
        if (!$('#mHostname').val().trim()) errores.push('Hostname es obligatorio.');
        if (!$('#mTipoId').val()) errores.push('Tipo de servidor es obligatorio.');

        if (errores.length) {
            swal({ text: errores.join('\n'), icon: 'warning' });
            return false;
        }
        return true;
    }

    async function guardarModal() {
        if (!validarModal()) return;

        // Payload
        var payload = {
            SerieProductoId: _editContext.currentId || parseInt($('#mSerieId').val(), 10),
            TipoId: $('#mTipoId').val() ? parseInt($('#mTipoId').val(), 10) : null,
            Hostname: $('#mHostname').val().trim(),
            Ip: $('#mIp').val().trim(),
            RamGb: $('#mRam').val() ? parseFloat($('#mRam').val()) : null,
            CpuSockets: $('#mCpuSockets').val() ? parseInt($('#mCpuSockets').val(), 10) : null,
            CpuCores: $('#mCpuCores').val() ? parseInt($('#mCpuCores').val(), 10) : null,
            HddTotal: $('#mHddGb').val().trim(),
            SoId: $('#mSoId').val() ? parseInt($('#mSoId').val(), 10) : null,
            HostnameBranch: $('#mHostBranch').val().trim(),
            IpBranch: $('#mIpBranch').val().trim(),
            HostnameFileServer: $('#mHostFs').val().trim(),
            IpFileServer: $('#mIpFs').val().trim(),
            HostnameUnicola: $('#mHostUnicola').val().trim(),
            IpUnicola: $('#mIpUnicola').val().trim(),
            EnlaceUrl: $('#mEnlaceUrl').val().trim(),
            IpIdracIlo: $('#mIpIdracIlo').val().trim(),
        };

        var esEdicion = _editContext.mode === 'edit';
        var url = esEdicion ? urlServEditar : urlServCrear;
        var $btn = $('#btnGuardar');
        var restore = setBtnBusy($btn, esEdicion ? 'Actualizando…' : 'Creando…');

        try {
            const resp = await $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });
            if (resp.Ok) {
                swal({ text: resp.Mensaje || (esEdicion ? 'Actualizado' : 'Creado') + ' correctamente', icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalSrvSerie')).hide();
                if (dt) dt.ajax.reload(null, false);
            } else {
                swal({ text: swalText(resp, 'No fue posible guardar'), icon: 'warning' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al guardar'), icon: 'error' });
        } finally {
            restore();
        }
    }


    // ======================== Eventos ========================
    function eventos() {
        // filtros
        $('#fEmp').on('change', cargarFiltroLocales);
        $('#fLoc, #fEstadoSerie, #fTipo').on('change', function () {
            if (dt) dt.ajax.reload();
        });

        // botón detalle -> usa la fila seleccionada
        $('#btnDetalle').on('click', function () {
            var $row = $('#tableServidores tbody tr.selected');
            if ($row.length !== 1) {
                swal({ text: 'Selecciona una fila para ver el detalle.', icon: 'warning' });
                return;
            }
            var data = dt.row($row).data();
            abrirModalEditar(data);
        });

        // guardar modal
        $('#btnGuardar').on('click', guardarModal);

    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                initCombosFijos();
                await cargarFiltroEmpresas();
                await cargarFiltroTipos();
                await cargarFiltroLocales();
                visualizarDataTable();
            });
        }
    };

})(jQuery);
