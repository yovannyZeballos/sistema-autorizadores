const urlListarCajeros = '/Cajeros/AdministrarCajero/ListarCajeros';
const urlListarColaboradores = '/Cajeros/AdministrarCajero/ListarColaboradores';
const urlAsignarCajero = '/Cajeros/AdministrarCajero/AsignarCajero';
const urlEliminarCajero = '/Cajeros/AdministrarCajero/EliminarCajero';
const urlDescargar = '/Cajeros/AdministrarCajero/DescargarMaestro';
const urlGenerarArchivo = '/Cajeros/AdministrarCajero/GenerarArchivo';

var dataTableCajeros = null;
var dataTableColaboradores = null;

const AdministrarCajero = function () {

    var inicalizarTablaColaboradores = true;

    var eventos = function () {

        $("#btnNuevoCajero").on('click', function () {
            $('#modalColaboradores').modal('show');
            if (inicalizarTablaColaboradores) {
                inicializarDataTableColaboradores();
                inicalizarTablaColaboradores = false;
            }
            else {
                recargarDataTableColaboradores();
            }

            $("#btnAsignar").show();

        });

        $("#btnAsignar").on('click', function () {
            asignarCajero();
        });

        $("#btnEliminarCajero").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar a los cajeros?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        eliminarCajero();
                    }
                });


        });

        $("#btnGenerarArchivo").on('click', function () {
            generarArchivo();
        });

        $("#btnBuscarColaborador").on('click', function () {
            $('#modalColaboradores').modal('show');

            if (inicalizarTablaColaboradores) {
                inicializarDataTableColaboradores(true);
                inicalizarTablaColaboradores = false;
            }
            else {
                recargarDataTableColaboradores(true);
            }

            $("#btnAsignar").hide();

        });

        $("#btnDescargarMaestro").on('click', function () {
            descargarMaestro();
        });

        $('#tableColaboradores tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $('#tableCajeros tbody').on('click', 'tr', function () {
            $(this).toggleClass('selected');
        });

        $("#chkActivosCajeros,#chkInactivosCajeros").on("change", function () {

            if ($("#chkInactivosCajeros").prop('checked') && !$("#chkActivosCajeros").prop('checked')) {
                dataTableCajeros.column(0).search('2').draw();
            }
            else if ($("#chkActivosCajeros").prop('checked') && !$("#chkInactivosCajeros").prop('checked')) {
                var regExSearch = "^[3]|[1]$";
                dataTableCajeros.column(0).search(regExSearch, true, false).draw();
            }
            else {
                dataTableCajeros.column(0).search('').draw();
            }

        });

        $("#chkActivosColaborador,#chkAnuladosColaborador").on("change", function () {

            if ($("#chkAnuladosColaborador").prop('checked') && !$("#chkActivosColaborador").prop('checked')) {
                dataTableColaboradores.column(6).search('ANU').draw();
            }
            else if ($("#chkActivosColaborador").prop('checked') && !$("#chkAnuladosColaborador").prop('checked')) {
                dataTableColaboradores.column(6).search('ACT').draw();
            }
            else {
                dataTableColaboradores.column(6).search('').draw();
            }

        });

        $("#chkTodos").on("change", function () {
            chechTodos();
        })
    }

    const chechTodos = function () {

        if ($("#chkTodos").prop('checked')) {
            dataTableCajeros.rows({ search: 'applied' }).nodes().each(function () {
                $(this).addClass('selected');
            });
        } else {
            dataTableCajeros.rows({ search: 'applied' }).nodes().each(function () {
                $(this).removeClass('selected');
            });
        }


    }

    const inicializarDataTableCajeros = function () {

        $.ajax({
            url: urlListarCajeros,
            type: "post",
            data: { request: {} },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    if (x === "CAJ_ESTADO") {
                        columnas.push({
                            title: x,
                            data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                            visible: false
                        });
                    } else {
                        columnas.push({
                            title: x.replace("_", " "),
                            data: quitarTildes(x).replace(/ /g, "").replace(".", "")
                        });
                    }

                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                dataTableCajeros = $('#tableCajeros').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    rowCallback: function (row, data, index) {
                        if (data.CAJ_ESTADO == "2") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    columns: columnas,
                    data: response.Cajeros,
                    bAutoWidth: false
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const recargarDataTableCajeros = function () {

        $.ajax({
            url: urlListarCajeros,
            type: "post",
            data: { request: {} },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });

                    dataTableCajeros.clear();
                    dataTableCajeros.draw();
                    return;

                }
                dataTableCajeros.clear();
                dataTableCajeros.rows.add(response.Cajeros);
                dataTableCajeros.draw();
                dataTableCajeros.columns.adjust().draw();

                setTimeout(() => {
                    dataTableCajeros.columns.adjust().draw();
                }, 500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error", });
            }
        });
    }

    const inicializarDataTableColaboradores = function (todos = false) {

        $.ajax({
            url: urlListarColaboradores,
            type: "post",
            data: { todos },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    if (x === "CAJ_TIPO" || x === "CAJ_TIPO_DOCID" || x === 'CAJ_TIPO_CONTRATO') {
                        columnas.push({
                            title: x,
                            data: quitarTildes(x).replace(/ /g, "").replace(".", ""),
                            visible: false
                        });
                    } else {
                        columnas.push({
                            title: x.replace("_", " "),
                            data: quitarTildes(x).replace(/ /g, "").replace(".", "")
                        });
                    }

                });

                if (!response.Ok) {
                    swal({ text: response.Mensaje, icon: "warning" });
                    return;
                }

                dataTableColaboradores = $('#tableColaboradores').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '300px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    rowCallback: function (row, data, index) {
                        if (data.Estado == "ANU") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    columns: columnas,
                    data: response.Data,
                    bAutoWidth: false
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

    }

    const recargarDataTableColaboradores = function (todos = false) {

        $.ajax({
            url: urlListarColaboradores,
            type: "post",
            data: { todos },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (!response.Ok) {
                    swal({
                        text: response.Mensaje,
                        icon: "warning"
                    });

                    dataTableColaboradores.clear();
                    dataTableColaboradores.draw();
                    return;

                }
                dataTableColaboradores.clear();
                dataTableColaboradores.rows.add(response.Data);
                dataTableColaboradores.draw();
                dataTableColaboradores.columns.adjust().draw();

                setTimeout(() => {
                    dataTableColaboradores.columns.adjust().draw();
                }, 500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error", });
            }
        });
    }

    const asignarCajero = function () {

        const registrosSeleccionados = dataTableColaboradores.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let cajeros = [];

        registrosSeleccionados.map((item) => {
            cajeros.push({
                CodLocal: item.LocalCT2,
                Nombres: item.Nombre,
                Apellidos: item.Apellidos,
                Tipo: item.CAJ_TIPO,
                TipoContrato: item.CAJ_TIPO_CONTRATO,
                Rut: item.NroDocu,
                TipoDocIdentidad: item.CAJ_TIPO_DOCID,
                CodigoEmpleado: item.Codigo
            });
        });

        $.ajax({
            url: urlAsignarCajero,
            type: "post",
            data: { cajeros },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                swal({
                    text: response.Mensaje,
                    icon: "success"
                });

                $('#modalColaboradores').modal('hide');

                recargarDataTableCajeros();

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const eliminarCajero = function () {

        const registrosSeleccionados = dataTableCajeros.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let cajeros = [];

        registrosSeleccionados.map((item) => {
            cajeros.push(item.DocIdentidad);
        });

        $.ajax({
            url: urlEliminarCajero,
            type: "post",
            data: { nroDocumentos: cajeros },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                swal({
                    text: response.Mensaje,
                    icon: "success"
                });

                recargarDataTableCajeros();

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const validarSelecion = function (count) {
        if (count === 0) {
            swal({
                text: "Debe seleccionar como minimo un registro",
                icon: "warning",
            });
            return false;
        }

        return true;
    }

    const descargarMaestro = function () {

        $.ajax({
            url: urlDescargar,
            type: "post",
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

                const linkSource = `data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,` + response.Archivo + '\n';
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

    const generarArchivo = function () {

        const registrosSeleccionados = dataTableCajeros.rows('.selected').data().toArray();
        debugger;
        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let cajeros = [];

        registrosSeleccionados.map((item) => {
            cajeros.push(item.CodCajero);
        });

        $.ajax({
            url: urlGenerarArchivo,
            data: { cajeros },
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                swal({ text: response.Mensaje, icon: response.Ok ? "success" : "error" });
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
                inicializarDataTableCajeros();
            });
        }
    }

}(jQuery);