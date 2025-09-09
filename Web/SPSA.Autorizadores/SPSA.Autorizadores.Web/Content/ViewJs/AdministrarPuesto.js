// ===================== Endpoints =====================
var urlListarEmpresasAsociadas = baseUrl + 'Maestros/MaeEmpresa/ListarEmpresasAsociadas';
var urlListarPuestos = baseUrl + 'Maestros/MaePuesto/ListarPaginado';
var urlModificarPuesto = baseUrl + 'Maestros/MaePuesto/ModificarPuesto';
var urlImportarPuesto = baseUrl + 'Maestros/MaePuesto/Importar';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

// ===================== Utiles / helpers =====================
function swalText(err, fallback) {
    if (!err) return fallback || '';
    if (typeof err === 'string') return err;
    if (err.responseText) return err.responseText;
    if (err.statusText) return err.statusText;
    if (err.Mensaje) return err.Mensaje;
    try { return JSON.stringify(err); } catch { return fallback || ''; }
}

function debounce(fn, ms) {
    let t;
    return function () { clearTimeout(t); t = setTimeout(() => fn.apply(this, arguments), ms); };
}

// ===================== Módulo =====================
var AdministrarPuesto = (function ($) {

    // ==== AJAX base =====
    function listarEmpresasAsociadas() {
        const request = { CodUsuario: $("#txtUsuario").val() || '', Busqueda: '' };
        return $.ajax({ url: urlListarEmpresasAsociadas, type: 'POST', data: { request } });
    }

    // ==== UI init ====
    function initSelect2(target) {
        const $el = (target && target.jquery) ? target : $(target);
        if (!$el.length) return;
        // asegure placeholder desde la 1ra opción
        const $opt0 = $el.find('option').first();
        if ($opt0.length && !$opt0.attr('value')) $opt0.attr('value', '');
        $el.select2({
            width: '100%',
            placeholder: $opt0.text() || 'Todos',
            allowClear: true,
            minimumResultsForSearch: 0
        });
    }

    async function cargarComboEmpresa() {
        try {
            const resp = await listarEmpresasAsociadas();
            const $cbo = $('#cboEmpresaBuscar');
            $cbo.empty().append('<option value="">Todos</option>');
            if (resp.Ok) {
                resp.Data.forEach(e => $cbo.append(new Option(e.NomEmpresa, e.CodEmpresa)));
            } else {
                swal({ text: swalText(resp, 'No fue posible listar empresas'), icon: 'error' });
            }
            initSelect2($cbo);
        } catch (err) {
            swal({ text: swalText(err, 'Error al listar empresas'), icon: 'error' });
        }
    }

    // ==== Eventos ====
    function eventos() {
        // Buscar
        $("#btnBuscarPuesto").off('click.pue').on('click.pue', function (e) {
            e.preventDefault();
            if ($.fn.DataTable.isDataTable('#tablePuestos')) $('#tablePuestos').DataTable().ajax.reload();
        });

        // Buscar cuando se presione Enter en la descripción
        $("#txtDesPuestoBuscar").off('keyup.pue').on('keyup.pue', debounce(function (e) {
            if (e.key === 'Enter') $("#btnBuscarPuesto").trigger('click');
        }, 250));

        // Toggle de indicadores (delegado, robusto ante redraw)
        $(document).off('change.pue', '#tablePuestos .chk-indicador')
            .on('change.pue', '#tablePuestos .chk-indicador', async function () {
                const $chk = $(this);
                const indicador = $chk.data("indicador");
                const codEmpresa = $chk.data("co-empr");
                const codPuesto = $chk.data("co-pues-trab");
                const nuevoValor = $chk.is(":checked") ? "S" : "N";

                const dt = $('#tablePuestos').DataTable();
                const row = $chk.closest('tr');
                const data = dt.row(row).data();
                if (!data) return;

                // Evitar clicks repetidos mientras actualiza
                $chk.prop('disabled', true);

                // payload (mantén campos que espera tu backend)
                const command = {
                    CodEmpresa: codEmpresa,
                    CodPuesto: codPuesto,
                    DesPuesto: data.DesPuesto,
                    IndAutAut: data.IndAutAut,
                    IndAutOpe: data.IndAutOpe,
                    IndManAut: data.IndManAut,
                    IndManOpe: data.IndManOpe,
                    FecAsigna: null,
                    UsuAsigna: $("#txtUsuario").val() || '',
                    FecElimina: null,
                    UsuElimina: $("#txtUsuario").val() || ''
                };

                // Aplica el cambio al campo correcto
                command[indicador] = nuevoValor;

                try {
                    const resp = await $.ajax({
                        url: urlModificarPuesto,
                        type: 'POST',
                        data: command,
                        dataType: 'json'
                    });

                    if (!resp || !resp.Ok) {
                        // revertir estado visual si falla
                        $chk.prop('checked', !$chk.is(':checked'));
                        swal({ text: swalText(resp, 'No se pudo actualizar el puesto.'), icon: 'warning' });
                    } else {
                        // Actualiza el dato en la fila sin recargar toda la tabla
                        // (evita flicker; si necesitas recálculo del backend, descomenta reload)
                        data[indicador] = nuevoValor;
                        dt.row(row).data(data).invalidate();
                        // dt.ajax.reload(null, false);
                    }
                } catch (err) {
                    // revertir estado visual si falla
                    $chk.prop('checked', !$chk.is(':checked'));
                    swal({ text: swalText(err, 'Error al actualizar el puesto'), icon: 'error' });
                } finally {
                    $chk.prop('disabled', false);
                }
            });
    }

    // ==== DataTable ====
    function visualizarDataTablePuestos() {
        $('#tablePuestos').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                // Mejora: si el checkbox no está marcado => no filtra (usa null)
                const valOrNull = (sel) => $(sel).is(':checked') ? 'S' : null;

                var filtros = {
                    CodEmpresa: $("#cboEmpresaBuscar").val() || null,
                    DesPuesto: ($("#txtDesPuestoBuscar").val() || '').trim(),
                    IndAutAut: valOrNull("#checkAutorizadorAutomatico"),
                    IndAutOpe: valOrNull("#checkOperadorAutomatico"),
                    IndManAut: valOrNull("#checkAutorizadorManual"),
                    IndManOpe: valOrNull("#checkOperadorManual")
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarPuestos,
                    type: "GET",
                    data: params,
                    dataType: "json",
                    success: function (response) {
                        if (response.Ok) {
                            var p = response.Data;
                            callback({
                                draw: data.draw,
                                recordsTotal: p.TotalRecords,
                                recordsFiltered: p.TotalRecords,
                                data: p.Items
                            });
                        } else {
                            callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                        }
                    },
                    error: function (jqXHR) {
                        swal({ text: swalText(jqXHR, 'Error al listar puestos'), icon: "error" });
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    }
                });
            },
            columnDefs: [
                { targets: 0, visible: false } // oculta CodEmpresa si no necesitas verla
            ],
            columns: [
                { data: "CodEmpresa", title: "COD. EMPRESA" },
                { data: "NomEmpresa", title: "NOM. EMPRESA", defaultContent: '' },
                { data: "CodPuesto", title: "COD. PUESTO", defaultContent: '' },
                { data: "DesPuesto", title: "DES. PUESTO", defaultContent: '' },
                {
                    data: "IndAutAut", title: "A. AUTOMÁTICO",
                    render: function (data, type, row) {
                        var checked = (data === "S") ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndAutAut" ' +
                            'data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    }, orderable: false, searchable: false
                },
                {
                    data: "IndAutOpe", title: "O. AUTOMÁTICO",
                    render: function (data, type, row) {
                        var checked = (data === "S") ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndAutOpe" ' +
                            'data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    }, orderable: false, searchable: false
                },
                {
                    data: "IndManAut", title: "A. MANUAL",
                    render: function (data, type, row) {
                        var checked = (data === "S") ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndManAut" ' +
                            'data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    }, orderable: false, searchable: false
                },
                {
                    data: "IndManOpe", title: "O. MANUAL",
                    render: function (data, type, row) {
                        var checked = (data === "S") ? "checked" : "";
                        return '<input type="checkbox" class="chk-indicador" data-indicador="IndManOpe" ' +
                            'data-co-empr="' + row.CodEmpresa + '" data-co-pues-trab="' + row.CodPuesto + '" ' + checked + ' />';
                    }, orderable: false, searchable: false
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
            lengthMenu: [10, 25, 50, 100]
        });
    }

    // ==== Importar / Plantilla (si en el futuro lo agregas a puestos) ====
    // (los dejé listos por si más adelante extiendes el módulo con importación)

    // ==== API público ====
    return {
        init: function () {
            checkSession(async function () {
                eventos();
                await cargarComboEmpresa();
                visualizarDataTablePuestos();
            });
        }
    };

})(jQuery);
