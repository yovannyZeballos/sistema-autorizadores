﻿var urlListarInvKardex = baseUrl + 'Inventario/InventarioKardex/ListarInvKardex';

var urlModalCrearInvKardex = baseUrl + 'Inventario/InventarioKardex/CrearFormInvKardex';
var urlModalEditarInvKardex = baseUrl + 'Inventario/InventarioKardex/EditarFormInvKardex';

var urlCrearInvKardex = baseUrl + 'Inventario/InventarioKardex/CrearInvKardex';
var urlActualizarInvKardex = baseUrl + 'Inventario/InventarioKardex/ActualizarInvKardex';
var urlEliminarInvKardex = baseUrl + 'Inventario/InventarioKardex/EliminarInvKardex';
var urlObtenerInvKardex = baseUrl + 'Inventario/InventarioKardex/ObtenerInvKardex';
var urlImportarInvKardex = baseUrl + 'Inventario/InventarioKardex/ImportarExcelInvKardex';

var urlDescargarPlantilla = baseUrl + 'Maestros/MaeTablas/DescargarPlantillas';

var dtListaKardex = null;
var AdministrarInvKardex = function () {

    const eventos = function () {

        $("#btnIrActivos").click(function () {
            window.location.href = '/Inventario/InventarioKardexActivo'; // Reemplaza con la ruta a la vista

        });

        $("#btnIrLocales").click(function () {
            window.location.href = '/Inventario/InventarioKardexLocal'; // Reemplaza con la ruta a la vista

        });

        $("#btnProcesar").click(function () {
            recargarDataTableKardex();
        });

        $("#btnNuevoInvKardex").click(function () {
            abrirModalNuevoInvKardex();
        });

        $("#btnEditarInvKardex").click(async function () {

            var filasSeleccionada = document.querySelectorAll("#tableKardex tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }
            const kardex_id = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const objKardex = await obtenerKardex(kardex_id);

            if (objKardex === undefined) return;

            abrirModalEditarInvKardex(objKardex);
        });

        $("#btnGuardarInvKardex").on("click", async function () {
            var inv_kardex = {
                ActivoId: $("#cboActivoId").val(),
                Kardex: $("#cboIndKardex").val(),
                Fecha: $("#txtFecha").val(),
                Guia: $("#txtGuia").val(),
                Serie: $("#txtSerie").val(),
                OrigenId: $("#cboOrigen").val(),
                DestinoId: $("#cboDestino").val(),
                Tk: $("#txtTk").val(),
                Cantidad: $("#txtCantidad").val(),
                TipoStock: $("#cboTipoStock").val(),
                Oc: $("#txtOc").val(),
                Sociedad: $("#txtSociedad").val()
            };

            if (validarFormInvKardex(inv_kardex))
                await guardarInvKardex(inv_kardex, urlCrearInvKardex);
        });

        $("#btnActualizarInvKardex").on("click", async function () {
            var inv_kardex = {
                Id: $("#txtId").val(),
                ActivoId: $("#cboActivoId").val(),
                Kardex: $("#cboIndKardex").val(),
                Fecha: $("#txtFecha").val(),
                Guia: $("#txtGuia").val(),
                Serie: $("#txtSerie").val(),
                OrigenId: $("#cboOrigen").val(),
                DestinoId: $("#cboDestino").val(),
                Tk: $("#txtTk").val(),
                Cantidad: $("#txtCantidad").val(),
                TipoStock: $("#cboTipoStock").val(),
                Oc: $("#txtOc").val(),
                Sociedad: $("#txtSociedad").val()
            };

            if (validarFormInvKardex(inv_kardex))
                await guardarInvKardex(inv_kardex, urlActualizarInvKardex);
        });

        $("#btnEliminarInvKardex").on("click", function () {
            var filasSeleccionada = document.querySelectorAll("#tableKardex tbody tr.selected");
            if (!validarSelecion(filasSeleccionada.length)) {
                return;
            }

            //const kardex_id = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;

            const request = {
                Id: filasSeleccionada[0].querySelector('td:nth-child(1)').textContent
            };

            swal({
                title: "¿Estás seguro?",
                text: "No podrás revertir esto",
                icon: "warning",
                buttons: {
                    cancel: {
                        text: "Cancelar",
                        value: null,
                        visible: true,
                        className: "",
                        closeModal: true,
                    },
                    confirm: {
                        text: "Sí, eliminarlo",
                        value: true,
                        visible: true,
                        className: "",
                        closeModal: true
                    }
                }
            }).then(async (isConfirm) => {
                if (isConfirm) {
                    await eliminarInvKardex(request);
                }
            });
        });

        $('#tableKardex tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dtListaKardex.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#tableKardex tbody').on('dblclick', 'tr', function () {

            var filasSeleccionada = $(this);

            const NUM_CAJA = filasSeleccionada[0].querySelector('td:nth-child(1)').textContent;
            const COD_ACTIVO = filasSeleccionada[0].querySelector('td:nth-child(2)').textContent;

            //console.log("child(1)" + NUM_CAJA);
            //console.log("child(2)" + COD_ACTIVO);

            //abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
        });

        $("#btnImportarInvKardex").on("click", function () { 
            swal({
                title: "¿Está seguro importar el archivo?",
                text: "Asegurese que cumple con el formato y nombre columnas requeridos.",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    $("#excelInventario").trigger("click");
                }
            });
        });

        $('#excelInventario').change(function (e) {
            importarExcelInvKardex();
        });

        $("#btnDescargarPlantillas").click(function () {
            descargarPlantillas("Plantilla_InvKardex");
        });
    }


    const validarFormInvKardex = function (invKardex) {
        let validate = true;

        if (invKardex.ActivoId === '' || invKardex.Kardex === '' || invKardex.Fecha === '' || invKardex.Guia === '' || invKardex.Serie === ''
            || invKardex.Origen === '' || invKardex.Destino === '') {
            validate = false;
            $("#formInvKardex").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarSelecion = function (count) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar un registro",
                icon: "warning",
            });
            return false;
        }
        return true;
    }

    const abrirModalEditarInvKardex = async function (objKardex) {
        $("#tituloModalInvKardex").html("Editar Activo");
        $("#btnActualizarInvKardex").show();
        $("#btnGuardarInvKardex").hide();

        if (objKardex.Fecha != "" && objKardex.Fecha != null) {
            let timestamp = parseInt(objKardex.Fecha.match(/\d+/)[0], 10);
            let date = new Date(timestamp);
            let formattedDate = date.toISOString().split('T')[0];
            objKardex.Fecha = formattedDate;
        }

        await cargarFormEditarInvKardex(objKardex);
    }

    const abrirModalNuevoInvKardex = async function () {
        $("#tituloModalInvKardex").html("Ingresar Kardex");
        $("#btnActualizarInvKardex").hide();
        $("#btnGuardarInvKardex").show();

        const model = {};

        await cargarFormCrearInvKardex(model);
    }

    const cargarFormCrearInvKardex = async function (model) {
        $.ajax({
            url: urlModalCrearInvKardex,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalInvKardex").find(".modal-body").html(response);
                $("#modalInvKardex").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const cargarFormEditarInvKardex = async function (model) {
        $.ajax({
            url: urlModalEditarInvKardex,
            type: "post",
            data: { model },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                $("#modalInvKardex").find(".modal-body").html(response);
                $("#modalInvKardex").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const guardarInvKardex = function (invKardex, url) {
        $.ajax({
            url: url,
            type: "post",
            data: { command: invKardex },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                swal({ text: response.Mensaje, icon: "success", });
                recargarDataTableKardex();
                $("#modalInvKardex").modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const obtenerKardex = function (kardexId) {
        return new Promise((resolve, reject) => {
            const request = {
                Id: kardexId
            };

            $.ajax({
                url: urlObtenerInvKardex,
                type: "post",
                data: { request },
                success: function (response) {
                    resolve(response.Data)
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(jqXHR.responseText)
                }
            });
        });
    }

    const eliminarInvKardex = async function (request) {
        $.ajax({
            url: urlEliminarInvKardex,
            type: "post",
            data: { request },
            dataType: "html",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: async function (response) {
                swal("Eliminado!", "Registro eliminado exitosamente.", "success");
                recargarDataTableKardex(request);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarDataTableKardex = function () {

        const request = {
            Kardex: $("#cboFiltroKardex").val(),
            FechaInicio: $("#txtFechaInicio").val(),
            FechaFin: $("#txtFechaFin").val()
        }

        if ($.fn.DataTable.isDataTable('#tableKardex')) {
            $('#tableKardex').DataTable().clear().draw();
            $('#tableKardex').DataTable().destroy();
        }
        dtListaKardex = $('#tableKardex').DataTable({
            ajax: {
                url: urlListarInvKardex,
                type: "post",
                dataType: "JSON",
                dataSrc: "Data",
                data: { request },
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal({
                        text: 'Error al listar kardex: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            //responsive: true, // Habilitar el modo responsivo
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            columns: [
                { data: "Id" },
                { data: "Kardex" },
                {
                    data: "Fecha",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.match(/\d+/)[0]);
                            var fecha = new Date(timestamp);
                            return fecha.toLocaleDateString();
                        }
                        return '';
                    }
                },
                { data: "ActivoArea" },
                { data: "ActivoTipo" },
                { data: "Guia" },
                { data: "ActivoDescripcion" },
                { data: "ActivoModelo" },
                { data: "ActivoMarca" },
                { data: "Serie" },
                { data: "OrigenLocal" },
                { data: "DestinoLocal" },
                { data: "Tk" },
                { data: "Cantidad" },
                { data: "TipoStock" },
                { data: "Oc" },
                { data: "Sociedad" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            rowCallback: function (row, data, index) {
            },
            bAutoWidth: false,
            buttons: [
                {
                    extend: 'excel',
                    text: 'Exportar excel',
                    titleAttr: 'Exportar Excel',
                    className: 'btn btn-primary btn-block btn-sm',
                    exportOptions: {
                        modifier: { page: 'all' }
                    },
                    filename: function () {
                        const fecha = $("#txtFechaFin").val().replace('/', '');
                        return `INV_KARDEX_${fecha}`;
                    },
                    action: function (e, dt, node, config) {

                        if (!this.data().count()) {
                            swal({
                                text: "No hay información disponible para Exportar.",
                                icon: "warning"
                            });
                            return;
                        }
                        $.fn.dataTable.ext.buttons.excelHtml5.action.call(this, e, dt, node, config);
                    }
                },
            ],
            order: [],
            dom: 'Bfrtip' // Asegúrate de que los botones se rendericen correctamente
        });

        //$("#container-btn-exportar").append(dtListaKardex.buttons().container());
        // Asegúrate de que los botones se agreguen al DOM después de inicializar el DataTable
        dtListaKardex.buttons().container().appendTo('#container-btn-exportar');
    }

    const fechaActual = function () {
        let date = new Date()

        let day = `${(date.getDate())}`.padStart(2, '0');
        let month = `${(date.getMonth() + 1)}`.padStart(2, '0');
        let year = date.getFullYear();

        $("#cboFiltroKardex").val("TODOS").trigger("change");
        $("#txtFechaInicio").val(`${day}/${month}/${year}`);
        $("#txtFechaFin").val(`${day}/${month}/${year}`);
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

    const importarExcelInvKardex = function () {
        var formData = new FormData();
        var uploadFiles = $('#excelInventario').prop('files');
        formData.append("excelInventario", uploadFiles[0]);

        $.ajax({
            url: urlImportarInvKardex,
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
                $("#excelInventario").val(null);
            },
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", }).then(() => {

                        if (response.Errores.length > 0) {
                            let html = "";
                            response.Errores.map((error) => {
                                html += `<tr><td>${error.Fila}</td><td>${error.Mensaje}</td></tr>`
                            });
                            $('#tbodyErroresCaja').html(html);
                            $('#modalErroresImportacionExcel').modal("show");
                        }
                    });
                    return;
                }
                swal({ text: response.Mensaje, icon: "success", });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const descargarPlantillas = function (nombreCarpeta) {
        $.ajax({
            url: urlDescargarPlantilla,
            type: "post",
            data: { nombreCarpeta: nombreCarpeta },
            dataType: "json",
            success: function (response) {

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning", });
                    return;
                }

                const linkSource = `data:application/zip;base64,` + response.Archivo + '\n';
                const downloadLink = document.createElement("a");
                const fileName = response.NombreArchivo;
                downloadLink.href = linkSource;
                downloadLink.download = fileName;
                downloadLink.click();

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                inicializarDatePicker();
                fechaActual();
                //await cargarComboEmpresa();
                recargarDataTableKardex();
            });
        }
    }

}(jQuery);
