// ===================== Endpoints =====================
var urlListarColaboradoresExt = baseUrl + 'Maestros/MaeColaboradorExt/ListarPaginado';
var urlObtenerColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Obtener';
var urlCrearColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Crear';
var urlModificarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Editar';
var urlImportarColabExt = baseUrl + 'Maestros/MaeColaboradorExt/Importar';
var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

// ===================== Estado local =====================
var _modoColabExt = 'crear';          // 'crear' | 'editar'
var _usuarioActual = null;

// ===================== Módulo =====================
var AdministrarColaboradorExt = (function ($) {

    function initSelect2(target, opts) {
        var $els = (target && target.jquery) ? target : $(target);
        if (!$els.length || !$.fn.select2) return;

        $els.each(function () {
            var $s = $(this);

            // usar la primera opción como placeholder si existe
            var $opt0 = $s.find('option').first();
            var placeholder = $opt0.attr('label') || $opt0.text() || 'Seleccionar…';
            if ($opt0.length && !$opt0.attr('value')) $opt0.attr('value', '');

            // si ya está inicializado, reiniciar
            if ($s.hasClass('select2-hidden-accessible')) {
                $s.select2('destroy');
            }

            $s.select2($.extend(true, {
                width: '100%',
                placeholder: placeholder,
                allowClear: true,
                minimumResultsForSearch: 0
            }, (opts || {})));
        });
    }

    function eventos() {
        // Buscar
        $("#btnBuscarColabExt").on("click", function (e) {
            e.preventDefault();
            $('#tableColaboradoresExt').DataTable().ajax.reload();
        });

        // Selección de fila
        $('#tableColaboradoresExt tbody').on('click', 'tr', function () {
            $('#tableColaboradoresExt tbody tr').removeClass('selected');
            $(this).addClass('selected');
        });

        // Nuevo
        $("#btnModalNuevoColabExt").on("click", async function () {
            await abrirModalColabExtNuevo();
        });

        // Editar
        $("#btnModalEditar").on("click", async function () {
            const filas = document.querySelectorAll("#tableColaboradoresExt tbody tr.selected");
            if (!validarSelecion(filas.length)) return;

            const table = $('#tableColaboradoresExt').DataTable();
            const row = table.row(filas[0]).data();

            // No permitir edición si está INACTIVO
            if ((row.IndActivo || '').toUpperCase() === 'N') {
                swal({ text: "El colaborador está INACTIVO y no puede editarse.", icon: "warning" });
                return;
            }

            await abrirModalColabExtEditar(row);
        });

        // Guardar
        $(document).on("click", "#btnGuardarColabExt", async function () {
            const model = colectarModeloColabExtDesdeModal();
            if (!validarFormularioColabExt(model)) return;

            const url = (_modoColabExt === 'editar') ? urlModificarColabExt : urlCrearColabExt;

            await guardarColabExt(model, url);
        });

        // Importar
        $("#btnImportarColabExt").on("click", function () {
            $("#excelImportar").trigger("click");
        });
        $("#excelImportar").on("change", function () {
            importarExcelMaeColabExt();
        });

        // Descargar plantilla
        $("#btnDescargarPlantilla").click(function () {
            descargarPlantilla("Plantilla_ColaboradorExt");
        });

    }

    // ===== DataTable =====
    function visualizarDataTableColaboradores() {
        $('#tableColaboradoresExt').DataTable({
            searching: false,
            processing: true,
            serverSide: true,
            ajax: function (data, callback) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    TipoUsuario: $("#cboTipoUsuarioBuscar").val(),
                    IndActivo: $("#cboIndActivoBuscar").val(),
                    FiltroVarios: $("#txtFiltroBuscar").val()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarColaboradoresExt,
                    type: "GET",
                    data: params,
                    dataType: "json",
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
                        swal({ text: jqXHR.responseText, icon: "error" });
                        callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    }
                });
            },
            columns: [
                { data: "NomLocal", title: "Local" },
                { data: "CodigoOfisis", title: "Código" },
                { data: "ApelPaterno", title: "Ape. Paterno" },
                { data: "ApelMaterno", title: "Ape. Materno" },
                { data: "NombreTrabajador", title: "Nombre" },
                { data: "TipoDocIdent", title: "Tipo Doc" },
                { data: "NumDocIndent", title: "Nro Doc" },
                { data: "PuestoTrabajo", title: "Puesto" },
                {
                    data: "FechaIngresoEmpresa",
                    title: "Fec. Ingreso",
                    render: function (data) {
                        if (!data) return "";
                        var m = /\/Date\((\d+)\)\//.exec(data + '');
                        if (m) {
                            var d = new Date(parseInt(m[1], 10));
                            return isNaN(d.getTime()) ? "" : d.toLocaleDateString('es-PE');
                        }
                        return (data + '').substring(0, 10);
                    }
                },
                {
                    data: "FechaCeseTrabajador",
                    title: "Fec. Cese",
                    render: function (data) {
                        if (!data) return "";
                        var m = /\/Date\((\d+)\)\//.exec(data + '');
                        if (m) {
                            var d = new Date(parseInt(m[1], 10));
                            return isNaN(d.getTime()) ? "" : d.toLocaleDateString('es-PE');
                        }
                        return (data + '').substring(0, 10);
                    }
                },
                {
                    data: "IndActivo",
                    title: "Estado",
                    render: function (data) {
                        return (data === 'S') ? 'ACTIVO' : 'INACTIVO';
                    }
                }
            ],
            rowCallback: function (row, data) {
                if ((data.IndActivo || '').toUpperCase() === 'N') {
                    $(row).addClass('row-inactivo');
                } else {
                    $(row).removeClass('row-inactivo');
                }
            },
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
    }

    function setCamposIdentificacionBloqueados(bloquear) {
        $('#cboTipoDoc, #txtNroDoc, #txtFecIngreso')
            .prop('disabled', bloquear);
    }

    async function abrirModalColabExtNuevo() {
        _modoColabExt = 'crear';
        _usuarioActual = $("#txtUsuario").val() || '';
        limpiarFormColabExt();

        setCamposIdentificacionBloqueados(false);

        $('#txtUsuCreacion').val(_usuarioActual);
        $('#txtFecCreacion').val(new Date().toISOString().slice(0, 10));
        $('#txtCodOfisis').val('9000000000');
        $('input[name="IndicadorPersonal"][value="S"]').prop('checked', true);
        $('input[name="TipoColaborador"][value="C"]').prop('checked', true);

        $("#tituloModalColabExt").text("Nuevo Colaborador Externo");
        $("#modalColabExt").modal('show');
    }

    async function abrirModalColabExtEditar(m) {
        // validacion extra
        if ((m.IndActivo || '').toUpperCase() === 'N') {
            swal({ text: "El colaborador está INACTIVO y no puede editarse.", icon: "warning" });
            return;
        }

        _modoColabExt = 'editar';
        _usuarioActual = $("#txtUsuario").val() || '';

        limpiarFormColabExt();

        try {
            // Bloquear campos solicitados en modo editar
            setCamposIdentificacionBloqueados(true);

            $('input[name="IndicadorPersonal"][value="' + (m.IndPersonal || 'S') + '"]').prop('checked', true);
            $('input[name="TipoColaborador"][value="' + (m.TipoUsuario || 'C') + '"]').prop('checked', true);
            $('#txtUsuCreacion').val(m.UsuCreacion || '');
            $('#txtFecCreacion').val(convertToISODate(m.FecCreacion));
            $('#txtCodOfisis').val(m.CodigoOfisis || '');
            $('#txtApPaterno').val(m.ApelPaterno || '');
            $('#txtApMaterno').val(m.ApelMaterno || '');
            $('#txtNomTrabajador').val(m.NombreTrabajador || '');
            $('#cboTipoDoc').val(m.TipoDocIdent || 'DNI');
            $('#txtNroDoc').val(m.NumDocIndent || '');
            $('#txtPuesto').val(m.PuestoTrabajo || '');
            $('#txtFecIngreso').val(convertToISODate(m.FechaIngresoEmpresa));
            $('#txtFecCese').val(convertToISODate(m.FechaCeseTrabajador));
            $("#tituloModalColabExt").text("Editar Colaborador Externo");
            $("#modalColabExt").modal('show');
        } catch (err) {
            swal({ text: (err && err.responseText) || err, icon: 'error' });
        }
    }

    function limpiarFormColabExt() {
        const form = $('#formColabExt')[0];
        if (form) form.reset();
        $('#formColabExt').removeClass('was-validated');
    }

    // Guardar
    async function guardarColabExt(model, url) {
        try {
            const resp = await $.ajax({
                url: url,
                type: "POST",
                data: { command: model },
                dataType: "json",
                beforeSend: showLoading,
                complete: closeLoading
            });

            $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);

            if (!resp || !resp.Ok) {
                swal({ text: (resp && resp.Mensaje) || 'No se pudo guardar.', icon: "warning" });
                return;
            }

            swal({ text: resp.Mensaje || 'Guardado correctamente.', icon: "success" });
            $("#modalColabExt").modal('hide');

        } catch (jqXHR) {
            $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);
            swal({ text: jqXHR.responseText || 'Error al guardar.', icon: "error" });
        }
    }

    // Importar / Plantilla
    function importarExcelMaeColabExt() {
        var formData = new FormData();
        var uploadFiles = $('#excelImportar').prop('files');
        formData.append("excelImportar", uploadFiles[0]);

        $.ajax({
            url: urlImportarColabExt,
            type: "POST",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            beforeSend: showLoading,
            complete: function () {
                closeLoading();
                $("#excelImportar").val(null);
            },
            success: function (response) {
                $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" }).then(() => {
                        if (response.Errores && response.Errores.length > 0) {
                            let html = response.Errores.map(e => `<tr><td>${e.Fila}</td><td>${e.Mensaje}</td></tr>`).join('');
                            $('#tbodyErrores').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success" });
            },
            error: function (jqXHR) {
                $('#tableColaboradoresExt').DataTable().ajax.reload(null, false);
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    function descargarPlantilla(nombreCarpeta) {
        $.ajax({
            url: urlDescargarPlantilla,
            type: "POST",
            data: { nombreCarpeta },
            dataType: "json",
            success: function (response) {
                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }
                const linkSource = `data:application/zip;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                downloadLink.href = linkSource;
                downloadLink.download = response.NombreArchivo;
                downloadLink.click();
            },
            error: function (jqXHR) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    // ===== Validaciones =====
    function validarSelecion(count) {
        if (count === 0) {
            swal({ text: "Debe seleccionar como mínimo un registro", icon: "warning" });
            return false;
        }
        return true;
    }

    function colectarModeloColabExtDesdeModal() {
        const esEditar = (_modoColabExt === 'editar');

        return {
            IndPersonal: $('input[name="IndicadorPersonal"]:checked').val(),
            TipoUsuario: $('input[name="TipoColaborador"]:checked').val(),
            CodigoOfisis: esEditar ? ($("#txtCodOfisis").val() || '') : "0",
            ApelPaterno: $("#txtApPaterno").val(),
            ApelMaterno: $("#txtApMaterno").val(),
            NombreTrabajador: $("#txtNomTrabajador").val(),
            TipoDocIdent: $("#cboTipoDoc").val(),
            NumDocIndent: $("#txtNroDoc").val(),
            PuestoTrabajo: $("#txtPuesto").val(),
            FechaIngresoEmpresa: $("#txtFecIngreso").val(),
            FechaCeseTrabajador: $("#txtFecCese").val(),
            UsuCreacion: esEditar ? null : ($("#txtUsuario").val() || ''),
            UsuModifica: esEditar ? ($("#txtUsuario").val() || '') : null
        };
    }

    function validarFormularioColabExt(model) {
        let ok = true;

        // Requeridos
        const req = [
            model.ApelPaterno, model.ApelMaterno, model.NombreTrabajador,
            model.TipoDocIdent, model.NumDocIndent, model.PuestoTrabajo,
            model.FechaIngresoEmpresa
        ];
        if (req.some(v => !v || (v + '').trim() === '')) ok = false;

        // Reglas por tipo de doc
        if (model.TipoDocIdent === "DNI") {
            if (!/^\d{8}$/.test(model.NumDocIndent || '')) ok = false;
        } else if (model.TipoDocIdent === "CEX") {
            if ((model.NumDocIndent || '').length < 9) ok = false;
        }

        // Si es personal 'S', el documento no puede ser '00000000'
        if ((model.IndPersonal || '').toUpperCase() === 'S' && (model.NumDocIndent || '') === '00000000') {
            ok = false;
        }

        if (!ok) {
            $("#formColabExt").addClass("was-validated");
            let msg = 'Faltan campos obligatorios o hay datos con formato inválido.';
            if ((model.IndPersonal || '').toUpperCase() === 'S' && (model.NumDocIndent || '') === '00000000') {
                msg = "Para personal 'S', el número de documento no puede ser 00000000.";
            }
            swal({ text: msg, icon: "warning" });
        }
        return ok;
    }

    // Utilidad fecha
    function convertToISODate(dateStr, preserveUTC = false) {
        if (!dateStr) return "";

        // ya ISO
        if (/^\d{4}-\d{2}-\d{2}$/.test(dateStr)) return dateStr;

        const m = /\/Date\((-?\d+)\)\//.exec(String(dateStr));
        let d = m ? new Date(parseInt(m[1], 10)) : new Date(String(dateStr));
        if (isNaN(d)) return "";

        // Si el backend guarda el "día" en UTC, usa getters UTC
        const y = preserveUTC ? d.getUTCFullYear() : d.getFullYear();
        const mo = String((preserveUTC ? d.getUTCMonth() : d.getMonth()) + 1).padStart(2, '0');
        const da = String(preserveUTC ? d.getUTCDate() : d.getDate()).padStart(2, '0');
        return `${y}-${mo}-${da}`;
    }

    // Init
    return {
        init: function () {
            checkSession(async function () {
                eventos();
                initSelect2('#cboTipoUsuarioBuscar, #cboIndActivoBuscar');
                visualizarDataTableColaboradores();
            });
        }
    };

})(jQuery);
