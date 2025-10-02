// ======================== Utils ========================
function swalText(err, fallback) {
    if (!err) return fallback || '';
    if (typeof err === 'string') return err;
    if (err.responseText) return err.responseText;
    if (err.statusText) return err.statusText;
    if (err.Mensaje) return err.Mensaje;
    try { return JSON.stringify(err); } catch { return fallback || ''; }
}

// Inicializar combos con Select2
async function cargarCombo($select, data, { placeholder = 'Seleccionar�', todos = false, dropdownParent = null } = {}) {
    if (!$select || !$select.length) return;

    // Destruye select2 previo
    if ($.fn.select2 && $select.hasClass('select2-hidden-accessible')) {
        $select.select2('destroy');
    }

    $select.empty();

    // Opci�n inicial
    if (todos) $select.append(new Option('Todos', ''));
    else $select.append(new Option('', ''));

    // Poblar opciones
    if (Array.isArray(data) && data.length) {
        data.forEach(d => {
            var opt = new Option(d.text, d.value, false, false);
            if (d.attrs) { Object.entries(d.attrs).forEach(([k, v]) => $(opt).attr(k, v)); }
            $select.append(opt);
        });
    }

    // Padre correcto del dropdown
    var parentEl = dropdownParent
        ? $(dropdownParent)
        : ($select.closest('.modal').length ? $select.closest('.modal') : $(document.body));

    // Inicializa select2
    if ($.fn.select2) {
        $select.select2({
            width: '100%',
            placeholder: placeholder,
            allowClear: true,
            minimumResultsForSearch: 0,
            dropdownParent: parentEl
        });
    }

    $select.val('').trigger('change');
}

// Helper gen�rico para DataTables con paginaci�n
function buildDataTable($el, { url, filtrosFn, columns }) {
    return $el.DataTable({
        serverSide: true,
        processing: true,
        searching: true,
        ordering: false,
        ajax: function (data, callback) {
            var pageNumber = (data.start / data.length) + 1;
            var pageSize = data.length;
            var filtros = filtrosFn ? filtrosFn(data) : {};
            var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

            $.ajax({
                url, type: 'GET', data: params, dataType: 'json',
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
                    }
                },
                error: function (jqXHR) {
                    swal({ text: swalText(jqXHR, 'Error al listar'), icon: 'error' });
                    callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                }
            });
        },
        columns,
        language: {
            searchPlaceholder: 'Buscar...',
            sSearch: '',
            lengthMenu: "Mostrar _MENU_ registros por pagina",
            zeroRecords: "No se encontraron resultados",
            info: "Mostrando pagina _PAGE_ de _PAGES_",
            infoEmpty: "No hay registros disponibles",
            infoFiltered: "(filtrado de _MAX_ registros totales)"
        },
        initComplete: function () {
            var $input = $el.closest('.dataTables_wrapper').find('.dataTables_filter input');
            $input.addClass('form-control-sm').attr('placeholder', 'Buscar...');
        },
        scrollY: '500px',
        scrollX: true,
        scrollCollapse: true,
        paging: true,
        lengthMenu: [10, 25, 50, 100],
    });
}

function parseDotNetDate(value) {
    if (!value) return '';
    var m = /\/Date\((\d+)\)\//.exec(value + '');
    if (m) {
        var d = new Date(parseInt(m[1], 10));
        return d.toLocaleDateString('es-PE');
    }
    return (value + '').substring(0, 10);
}

function setBtnBusy($btn, busyText) {
    var old = { html: $btn.html(), disabled: $btn.prop('disabled') };
    $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>' + (busyText || 'Procesando�'));
    return function restore() { $btn.prop('disabled', old.disabled).html(old.html); };
}
