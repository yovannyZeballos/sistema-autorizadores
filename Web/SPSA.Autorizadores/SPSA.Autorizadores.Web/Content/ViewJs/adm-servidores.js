/* ==========================================
 * Inventario » Servidores (hosts + VMs)
 * Archivo: adm-servidores.js
 * ========================================== */

/* ---------- URLs ---------- */
var urlServListarPaginado = baseUrl + 'Inventario/Servidores/ListarPaginado';
var urlServListarTipos = baseUrl + 'Inventario/Servidores/ListarTipos';
var urlServListarSo = baseUrl + 'Inventario/Servidores/ListarSo';
var urlServCrear = baseUrl + 'Inventario/Servidores/Crear';
var urlServEditar = baseUrl + 'Inventario/Servidores/Editar';

var urlListarEmpresas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarLocalesPorEmp = baseUrl + 'Maestros/MaeLocal/ListarLocalPorEmpresa';

/* VMs */
var urlVmListarPorHost = baseUrl + 'Inventario/Servidores/ListarVmPorHost';
var urlVmCrear = baseUrl + 'Inventario/Servidores/CrearVm';
var urlVmEditar = baseUrl + 'Inventario/Servidores/EditarVm';
var urlVmListarPlat = baseUrl + 'Inventario/Servidores/ListarPlataformasVm';

var AdmServidores = (function ($) {

    /* ---------- Estado ---------- */
    var _dtServidores = null;
    var _dtVms = null;
    var _tiposCache = [];
    var _soCache = [];
    var _platCache = [];

    var _ctxServidor = { modo: 'crear', serieId: null, fila: null };
    var _ctxVm = { modo: 'crear', hostSerieId: null, vmId: null };

    /* ---------- Idioma común para DataTables ---------- */
    var idiomaDt = {
        lengthMenu: "Mostrar _MENU_ registros por pagina",
        zeroRecords: "No se encontraron resultados",
        info: "Mostrando pagina _PAGE_ de _PAGES_",
        infoEmpty: "No hay registros disponibles",
        infoFiltered: "(filtrado de _MAX_ registros totales)",
        search: "Buscar:",
        paginate: { previous: "Anterior", next: "Siguiente" }
    };

    /* ================================================================
     * API simples
     * ================================================================ */
    async function apiListarEmpresas() {
        return $.ajax({ url: urlListarEmpresas, type: 'POST', data: { request: { CodUsuario: '', Busqueda: '' } } });
    }
    async function apiListarLocales(codEmpresa) {
        if (!codEmpresa) return { Ok: true, Data: [] };
        return $.ajax({ url: urlListarLocalesPorEmp, type: 'POST', data: { request: { CodEmpresa: codEmpresa } } });
    }
    async function apiListarTiposServidor() { return $.ajax({ url: urlServListarTipos, type: 'GET' }); }
    async function apiListarSoServidor() { return $.ajax({ url: urlServListarSo, type: 'GET' }); }
    async function apiListarPlataformas() { return $.ajax({ url: urlVmListarPlat, type: 'GET' }); }

    /* ================================================================
     * Catálogos (caché)
     * ================================================================ */
    async function asegurarTipos() { if (_tiposCache.length) return; const r = await apiListarTiposServidor(); if (r.Ok) _tiposCache = r.Data || []; }
    async function asegurarSo() { if (_soCache.length) return; const r = await apiListarSoServidor(); if (r.Ok) _soCache = r.Data || []; }
    async function asegurarPlataformas() { if (_platCache.length) return; const r = await apiListarPlataformas(); if (r.Ok) _platCache = r.Data || []; }

    /* ================================================================
     * Filtros
     * ================================================================ */
    async function iniciarCombosFiltro() {
        $('#fEstadoSerie').select2({ width: '100%', placeholder: 'Todos', allowClear: true, minimumResultsForSearch: 0 });
        await cargarFiltroEmpresas();
        await cargarFiltroTipos();
        await cargarFiltroLocales();
    }
    async function cargarFiltroEmpresas() {
        try {
            const resp = await apiListarEmpresas();
            if (resp.Ok) {
                const data = resp.Data.map(x => ({ text: x.NomEmpresa, value: x.CodEmpresa }));
                await cargarCombo($('#fEmp'), data, { placeholder: 'Todos', todos: true });
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' }); }
    }
    async function cargarFiltroLocales() {
        var codEmp = $('#fEmp').val();
        if (!codEmp) {
            await cargarCombo($('#fLoc'), [], { placeholder: 'Todos', todos: true });
            $('#fLoc').prop('disabled', true);
            return;
        }
        try {
            const resp = await apiListarLocales(codEmp);
            if (resp.Ok) {
                const data = resp.Data.map(x => ({ text: x.NomLocal, value: x.CodLocal }));
                await cargarCombo($('#fLoc'), data, { placeholder: 'Todos', todos: true });
                $('#fLoc').prop('disabled', false);
            }
        } catch (err) { swal({ text: swalText(err, 'Error al listar locales'), icon: 'error' }); }
    }
    async function cargarFiltroTipos() {
        await asegurarTipos();
        const data = _tiposCache.map(x => ({ text: x.NomTipo, value: x.Id }));
        await cargarCombo($('#fTipo'), data, { placeholder: 'Todos', todos: true });
    }
    function parametrosFiltro(dtReq) {
        return {
            CodEmpresa: $('#fEmp').val() || null,
            CodLocal: $('#fLoc').val() || null,
            IndEstadoSerie: $('#fEstadoSerie').val() || null,
            TipoId: $('#fTipo').val() ? parseInt($('#fTipo').val(), 10) : null,
            Texto: (dtReq.search && dtReq.search.value) ? dtReq.search.value : ''
        };
    }

    /* ================================================================
     * Tabla de Servidores (hosts físicos)
     * ================================================================ */
    function iniciarTablaServidores() {
        _dtServidores = $('#tableServidores').DataTable({
            serverSide: true, processing: true, searching: true, ordering: false,
            ajax: function (data, callback) {
                var pagina = (data.start / data.length) + 1;
                var tamano = data.length;
                var filtros = parametrosFiltro(data);
                var params = Object.assign({ PageNumber: pagina, PageSize: tamano }, filtros);

                $.ajax({
                    url: urlServListarPaginado, type: 'GET', data: params, dataType: 'json',
                    success: function (resp) {
                        if (resp.Ok) {
                            var p = resp.Data;
                            callback({ draw: data.draw, recordsTotal: p.TotalRecords, recordsFiltered: p.TotalRecords, data: p.Items });
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
                {
                    data: null, title: 'Acciones', orderable: false, searchable: false, width: '115px',
                    render: function (data, type, row) {
                        var tieneCar = row && row.TipoId != null;
                        var tituloCar = tieneCar ? 'Editar caracter\u00EDsticas' : 'Agregar caracter\u00EDsticas';
                        var iconoCar = tieneCar ? 'fe-edit-2' : 'fe-plus-circle';
                        return (
                            '<div class="btn-group btn-group-sm" role="group">' +
                            '  <button class="btn btn-light js-host-editar" title="' + tituloCar + '"><i class="fe ' + iconoCar + '"></i></button>' +
                            '  <button class="btn btn-dark js-vm-listar" title="Ver VMs"><i class="fe fe-layers"></i></button>' +
                            '</div>'
                        );
                    }
                },
                { data: 'NumSerie', title: 'Serie', defaultContent: '' },
                { data: 'DesProducto', title: 'Producto', defaultContent: '' },
                { data: 'NomMarca', title: 'Marca', defaultContent: '' },
                { data: 'Modelo', title: 'Modelo', defaultContent: '' },
                { data: 'TipoServidor', title: 'Tipo', defaultContent: '' },
                { data: 'Hostname', title: 'Hostname', defaultContent: '' },
                { data: 'IpSo', title: 'IP SO', defaultContent: '' },
                { data: 'IpRemota', title: 'IP Remota', defaultContent: '' },
                { data: 'MacAddress', title: 'MAC', defaultContent: '' },
                { data: 'NomEmpresa', title: 'Empresa', defaultContent: '' },
                { data: 'NomLocal', title: 'Local', defaultContent: '' },
                {
                    data: 'IndEstadoSerie', title: 'Estado Serie',
                    render: function (data) {
                        if (!data) return '';
                        var cls = '';
                        switch (data) {
                            case 'DISPONIBLE': cls = 'dot-disponible'; break;
                            case 'EN_TRANSITO': cls = 'dot-transito'; break;
                            case 'DE_BAJA': cls = 'dot-baja'; break;
                            case 'EN_USO': cls = 'dot-uso'; break;
                        }
                        return '<span class="badge-dot"><i class="' + cls + '"></i>' + data + '</span>';
                    }
                },
                { data: 'RamGb', title: 'RAM (GB)', defaultContent: '' },
                { data: 'CpuSockets', title: 'Procesadores', defaultContent: '' },
                { data: 'CpuCores', title: 'Cores', defaultContent: '' },
                { data: 'HddTotal', title: 'HDD (GB)', defaultContent: '' },
                { data: 'NomSo', title: 'SO', defaultContent: '' },
                { data: 'FecIngreso', title: 'Fec. Ingreso', render: function (d) { return parseDotNetDate(d); } },
                { data: 'Antiguedad', title: 'Antig\u00FCedad', defaultContent: '' },
                { data: 'ConexionRemota', title: 'Conexi\u00F3n', defaultContent: '' }
            ],
            language: idiomaDt, // <<<<<<<<<<<<<<<<<<<<<<<
            initComplete: function () {
                $('#tableServidores_filter input')
                    .addClass('form-control-sm')
                    .attr('placeholder', 'hostname / serie / producto / marca / modelo');
                delegadosAccionesFila();
            },
            scrollY: '520px', scrollX: true, scrollCollapse: true, paging: true, lengthMenu: [10, 25, 50, 100],
            rowCallback: function (row) { $(row).css('cursor', 'pointer'); }
        });
    }

    function recargarTablaServidores() { if (_dtServidores) _dtServidores.ajax.reload(null, false); }

    /* ================================================================
     * Delegados por fila (tabla principal)
     * ================================================================ */
    function delegadosAccionesFila() {
        var $t = $('#tableServidores');

        $t.off('click', '.js-host-editar').on('click', '.js-host-editar', function () {
            var row = _dtServidores.row($(this).closest('tr')).data();
            if (row) abrirModalServidorEditar(row);
        });

        $t.off('click', '.js-vm-listar').on('click', '.js-vm-listar', function () {
            var row = _dtServidores.row($(this).closest('tr')).data();
            if (row) abrirModalListadoVms(row);
        });

        $t.off('click', 'tbody tr').on('click', 'tbody tr', function () {
            if ($(this).hasClass('selected')) $(this).removeClass('selected');
            else { $t.find('tbody tr.selected').removeClass('selected'); $(this).addClass('selected'); }
        });

        $t.off('dblclick', 'tbody tr').on('dblclick', 'tbody tr', function () {
            var row = _dtServidores.row(this).data();
            if (row) abrirModalServidorEditar(row);
        });
    }

    /* ================================================================
     * Modal Servidor (crear / editar características)
     * ================================================================ */
    function limpiarModalServidor() {
        $('#modalSrvSerie input, #modalSrvSerie textarea').val('');
        $('#mSerieId, #mTipoId, #mSoId').val('').trigger('change');
    }
    async function prepararCombosModalServidor() {
        await asegurarTipos(); await asegurarSo();
        var dataTipos = _tiposCache.map(x => ({ text: x.NomTipo, value: x.Id }));
        var dataSo = _soCache.map(x => ({ text: x.NomSo, value: x.Id }));
        await cargarCombo($('#mTipoId'), dataTipos, { placeholder: 'Seleccionar', dropdownParent: '#modalSrvSerie' });
        await cargarCombo($('#mSerieId'), [], { placeholder: 'Seleccionar', dropdownParent: '#modalSrvSerie' });
        await cargarCombo($('#mSoId'), dataSo, { placeholder: 'Seleccionar', dropdownParent: '#modalSrvSerie' });
    }
    async function abrirModalServidorCrear(fila) {
        _ctxServidor = { modo: 'crear', serieId: fila.SerieProductoId, fila };
        limpiarModalServidor(); await prepararCombosModalServidor();
        $('#mSerieId').prop('disabled', true)
            .append(new Option(fila.NumSerie || ('#' + fila.SerieProductoId), fila.SerieProductoId))
            .val(fila.SerieProductoId).trigger('change');
        $('#modalSrvSerieTitle').text('Crear caracter\u00EDsticas de Servidor');
        new bootstrap.Modal(document.getElementById('modalSrvSerie')).show();
    }
    async function abrirModalServidorEditar(fila) {
        if (!fila || fila.TipoId == null) return abrirModalServidorCrear(fila);
        _ctxServidor = { modo: 'editar', serieId: fila.SerieProductoId, fila };
        limpiarModalServidor(); await prepararCombosModalServidor();

        $('#mSerieId').prop('disabled', true)
            .append(new Option(fila.NumSerie || ('#' + fila.SerieProductoId), fila.SerieProductoId))
            .val(fila.SerieProductoId).trigger('change');

        $('#mTipoId').val(fila.TipoId).trigger('change');
        $('#mSoId').val(fila.SoId).trigger('change');

        $('#mHostname').val(fila.Hostname || '');
        $('#mIp').val(fila.IpSo || '');
        $('#mRam').val(fila.RamGb || '');
        $('#mCpuSockets').val(fila.CpuSockets || '');
        $('#mCpuCores').val(fila.CpuCores || '');
        $('#mHddGb').val(fila.HddTotal || '');
        $('#mMac').val((fila.MacAddress || '').toUpperCase());
        $('#mFecIngreso').val(toInputDate(fila.FecIngreso));
        $('#mAntiguedad').val(fila.Antiguedad || '');
        $('#mConexionRemota').val(fila.ConexionRemota || '');
        $('#mIpRemota').val(fila.IpRemota || '');

        $('#modalSrvSerieTitle').text('Caracter\u00EDsticas de Servidor');
        new bootstrap.Modal(document.getElementById('modalSrvSerie')).show();
    }
    function validarServidor() {
        var e = [];
        if (_ctxServidor.modo === 'crear' && !$('#mSerieId').val()) e.push('Seleccione la serie del producto.');
        if (!$('#mHostname').val().trim()) e.push('Hostname es obligatorio.');
        if (!$('#mTipoId').val()) e.push('Tipo de servidor es obligatorio.');
        var anti = $('#mAntiguedad').val(); if (anti && parseInt(anti, 10) < 0) e.push('Antig\u00FCedad no puede ser negativa.');
        if ($('#mConexionRemota').val().length > 20) e.push('Conexi\u00F3n remota excede 20 caracteres.');
        if (e.length) { swal({ text: e.join('\n'), icon: 'warning' }); return false; }
        return true;
    }
    function datosServidor() {
        return {
            SerieProductoId: _ctxServidor.serieId || parseInt($('#mSerieId').val(), 10),
            TipoId: $('#mTipoId').val() ? parseInt($('#mTipoId').val(), 10) : null,
            Hostname: $('#mHostname').val().trim(),
            IpSo: ($('#mIp').val() || '').trim(),
            Ip: ($('#mIp').val() || '').trim(),
            RamGb: $('#mRam').val() ? parseFloat($('#mRam').val()) : null,
            CpuSockets: $('#mCpuSockets').val() ? parseInt($('#mCpuSockets').val(), 10) : null,
            CpuCores: $('#mCpuCores').val() ? parseInt($('#mCpuCores').val(), 10) : null,
            HddTotal: ($('#mHddGb').val() || '').trim(),
            SoId: $('#mSoId').val() ? parseInt($('#mSoId').val(), 10) : null,
            MacAddress: ($('#mMac').val() || '').trim().toUpperCase(),
            FecIngreso: $('#mFecIngreso').val(),
            Antiguedad: $('#mAntiguedad').val() ? parseInt($('#mAntiguedad').val(), 10) : null,
            ConexionRemota: ($('#mConexionRemota').val() || '').trim(),
            IpRemota: ($('#mIpRemota').val() || '').trim()
        };
    }
    async function guardarServidor() {
        if (!validarServidor()) return;
        var datos = datosServidor();
        var esEdicion = (_ctxServidor.modo === 'editar');
        var url = esEdicion ? urlServEditar : urlServCrear;

        var $btn = $('#btnGuardar');
        var restaurar = setBtnBusy($btn, esEdicion ? 'Actualizando\u2026' : 'Creando\u2026');
        try {
            const resp = await $.ajax({ url, type: 'POST', contentType: 'application/json; charset=utf-8', data: JSON.stringify(datos), dataType: 'json' });
            if (resp.Ok) {
                swal({ text: resp.Mensaje || (esEdicion ? 'Servidor actualizado' : 'Servidor creado'), icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalSrvSerie')).hide();
                recargarTablaServidores();
            } else { swal({ text: swalText(resp, 'No fue posible guardar'), icon: 'warning' }); }
        } catch (err) { swal({ text: swalText(err, 'Error al guardar'), icon: 'error' }); }
        finally { restaurar(); }
    }

    /* ================================================================
     * VMs: listado (modal) + crear/editar (otro modal)
     * ================================================================ */
    function limpiarModalVm() {
        $('#vmHostSerieId').val('');
        $('#vmHostname,#vmIp,#vmRam,#vmVcores,#vmHddGb,#vmNotas').val('');
        $('#vmPlataformaId,#vmSoId').val('').trigger('change');
        $('#vmActivo').prop('checked', true);
    }
    async function prepararCombosModalVm() {
        await asegurarSo(); await asegurarPlataformas();
        var dataSo = _soCache.map(x => ({ text: x.NomSo, value: x.Id }));
        var dataPlat = _platCache.map(x => ({ text: x.NomPlataforma, value: x.Id }));
        await cargarCombo($('#vmSoId'), dataSo, { placeholder: 'Seleccionar', dropdownParent: '#modalVm' });
        await cargarCombo($('#vmPlataformaId'), dataPlat, { placeholder: 'Seleccionar', dropdownParent: '#modalVm' });
    }
    async function abrirModalVmCrearDesdeLista(hostSerieId, hostLabel) {
        _ctxVm = { modo: 'crear', hostSerieId: hostSerieId, vmId: null };
        limpiarModalVm(); await prepararCombosModalVm();
        $('#vmHostSerieId').val(hostSerieId);
        $('#modalVmTitle').text('Agregar VM a ' + (hostLabel || ('Host #' + hostSerieId)));
        const modalLista = bootstrap.Modal.getInstance(document.getElementById('modalVmList')); if (modalLista) modalLista.hide();
        new bootstrap.Modal(document.getElementById('modalVm')).show();
    }

    // Guardamos vm.Id en el contexto para enviarlo en la edición
    async function abrirModalVmEditar(vm) {
        _ctxVm = { modo: 'editar', hostSerieId: vm.HostSerieId, vmId: vm.Id };
        limpiarModalVm(); await prepararCombosModalVm();

        $('#vmHostSerieId').val(vm.HostSerieId);
        $('#vmHostname').val((vm.Hostname || '').toUpperCase());
        $('#vmIp').val(vm.Ip || '');
        $('#vmRam').val(vm.RamGb ?? '');
        $('#vmVcores').val(vm.VCores ?? '');
        $('#vmHddGb').val((vm.HddTotal || '').toUpperCase());
        if (vm.PlataformaId) $('#vmPlataformaId').val(vm.PlataformaId).trigger('change'); else $('#vmPlataformaId').val('').trigger('change');
        if (vm.SoId) $('#vmSoId').val(vm.SoId).trigger('change'); else $('#vmSoId').val('').trigger('change');
        $('#vmActivo').prop('checked', (vm.IndActivo || 'S') === 'S');
        $('#vmNotas').val(vm.Url || '');
        $('#modalVmTitle').text('Editar VM');

        const modalLista = bootstrap.Modal.getInstance(document.getElementById('modalVmList')); if (modalLista) modalLista.hide();
        new bootstrap.Modal(document.getElementById('modalVm')).show();
    }

    function validarVm() {
        var e = [];
        var hostSerieId = parseInt($('#vmHostSerieId').val() || '0', 10);
        var hostname = ($('#vmHostname').val() || '').trim();
        if (!hostSerieId) e.push('Host inv\u00E1lido.');
        if (!hostname) e.push('Hostname de VM es obligatorio.');
        if (e.length) { swal({ text: e.join('\n'), icon: 'warning' }); return false; }
        return true;
    }

    // Incluir Id cuando _ctxVm.modo === 'editar'
    function construirDatosVm(esEdicion) {
        var obj = {
            HostSerieId: parseInt($('#vmHostSerieId').val(), 10),
            Hostname: ($('#vmHostname').val() || '').trim(),
            Ip: ($('#vmIp').val() || '').trim(),
            RamGb: $('#vmRam').val() ? parseFloat($('#vmRam').val()) : null,
            VCores: $('#vmVcores').val() ? parseInt($('#vmVcores').val(), 10) : null,
            HddTotal: ($('#vmHddGb').val() || '').trim(),
            SoId: $('#vmSoId').val() ? parseInt($('#vmSoId').val(), 10) : null,
            PlataformaId: $('#vmPlataformaId').val() ? parseInt($('#vmPlataformaId').val(), 10) : null,
            IndActivo: $('#vmActivo').is(':checked') ? 'S' : 'N',
            Url: ($('#vmNotas').val() || '').trim()
        };
        if (esEdicion) obj.Id = _ctxVm.vmId;
        return obj;
    }

    async function guardarVm() {
        if (!validarVm()) return;

        var esEdicion = (_ctxVm.modo === 'editar');
        var datos = construirDatosVm(esEdicion);
        var url = esEdicion ? urlVmEditar : urlVmCrear;

        var $btn = $('#btnGuardarVm');
        var restaurar = setBtnBusy($btn, esEdicion ? 'Actualizando\u2026' : 'Creando\u2026');

        try {
            const resp = await $.ajax({
                url, type: 'POST', contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(datos), dataType: 'json'
            });
            if (resp.Ok) {
                swal({ text: resp.Mensaje || (esEdicion ? 'VM actualizada' : 'VM creada'), icon: 'success' });
                bootstrap.Modal.getInstance(document.getElementById('modalVm')).hide();
                if ($('#modalVmList').hasClass('show') && _dtVms) _dtVms.ajax.reload(null, false);
            } else {
                swal({ text: swalText(resp, 'No fue posible guardar la VM'), icon: 'warning' });
            }
        } catch (err) {
            swal({ text: swalText(err, 'Error al guardar VM'), icon: 'error' });
        } finally { restaurar(); }
    }

    function construirTablaVms(hostSerieId) {
        var $tbl = $('#tableVms');
        if (_dtVms) { _dtVms.destroy(); }

        $tbl.find('tbody').empty();

        _dtVms = $tbl.DataTable({
            serverSide: true, processing: true, searching: true, ordering: false,
            columnDefs: [
                { responsivePriority: 1, targets: 0 },    // Acciones
                { responsivePriority: 2, targets: 1 },    // Hostname
                { responsivePriority: 3, targets: 2 }     // IP
            ],
            ajax: function (data, callback) {
                var pagina = (data.start / data.length) + 1;
                var tamano = data.length;
                $.ajax({
                    url: urlVmListarPorHost, type: 'GET',
                    data: { HostSerieId: hostSerieId, PageNumber: pagina, PageSize: tamano, Texto: (data.search && data.search.value) || '' },
                    dataType: 'json',
                    success: function (resp) {
                        if (resp.Ok) {
                            var p = resp.Data;
                            callback({ draw: data.draw, recordsTotal: p.TotalRecords, recordsFiltered: p.TotalRecords, data: p.Items });
                        } else {
                            callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                        }
                    },
                    error: function (jq) {
                        swal({ text: swalText(jq, 'Error al listar VMs'), icon: 'error' });
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    }
                });
            },
            columns: [
                {
                    data: null, orderable: false, searchable: false,
                    render: function () { return '<div class="btn-group btn-group-sm" role="group"><button class="btn btn-light js-vm-editar" title="Editar VM"><i class="fe fe-edit-2"></i></button></div>'; }
                },
                { data: 'Hostname', title: 'Hostname' },
                { data: 'Ip', title: 'IP' },
                { data: 'RamGb', title: 'RAM(GB)' },
                { data: 'VCores', title: 'vCores' },
                { data: 'HddTotal', title: 'HDD' },
                { data: 'NomPlataforma', title: 'Plataforma' },
                { data: 'NomSo', title: 'SO' },
                {
                    data: 'IndActivo',
                    render: function (d) {
                        return d === 'S'
                            ? '<span class="badge bg-success">Sí</span>'
                            : '<span class="badge bg-secondary">No</span>';
                    }
                },
                { data: 'Url', title: 'Url' }
            ],
            language: idiomaDt,
            initComplete: function () {
                $('#tableVms_filter input').addClass('form-control-sm').attr('placeholder', 'Buscar VM\u2026');
                delegadosFilaVm();
            },
            lengthMenu: [10, 25, 50, 100]
        });
    }
    function delegadosFilaVm() {
        var $tbl = $('#tableVms');
        $tbl.off('click', '.js-vm-editar').on('click', '.js-vm-editar', async function () {
            var vm = _dtVms.row($(this).closest('tr')).data();
            if (!vm) return;
            await abrirModalVmEditar(vm);
        });
    }
    async function abrirModalListadoVms(filaHost) {
        var hostSerieId = filaHost.SerieProductoId;
        var hostLabel = filaHost.Hostname || filaHost.NumSerie || ('#' + filaHost.SerieProductoId);
        $('#vmListHostSerieId').val(hostSerieId);
        $('#modalVmListTitle').text('VMs del host ' + hostLabel);

        $('#btnVmNuevo').off('click').on('click', function () {
            abrirModalVmCrearDesdeLista(hostSerieId, hostLabel);
        });

        new bootstrap.Modal(document.getElementById('modalVmList')).show();
        construirTablaVms(hostSerieId);
    }

    /* ================================================================
     * Eventos globales
     * ================================================================ */
    function vincularEventos() {
        $('#fEmp').on('change', async function () { await cargarFiltroLocales(); recargarTablaServidores(); });
        $('#fLoc, #fEstadoSerie, #fTipo').on('change', function () { recargarTablaServidores(); });

        $('#btnGuardar').on('click', guardarServidor);
        $('#btnGuardarVm').on('click', guardarVm);

        $('#mMac').on('input', function () { this.value = (this.value || '').toUpperCase(); });
    }

    /* ================================================================
     * Arranque
     * ================================================================ */
    async function eventos() {
        try { vincularEventos(); await iniciarCombosFiltro(); iniciarTablaServidores(); }
        catch (e) { swal({ text: swalText(e, 'Error inicializando la p\u00E1gina'), icon: 'error' }); }
    }

    return {
        init: function () {
            checkSession(async function () {
                eventos();
            });
        }
    };

})(jQuery);
