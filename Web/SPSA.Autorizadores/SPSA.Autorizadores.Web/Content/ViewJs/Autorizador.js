var urlColaborador = baseUrl + 'Autorizadores/Autorizador/ListarColaborador';
var urlTodosColaborador = baseUrl + 'Autorizadores/Autorizador/ListarTodosColaborador';
var urlAutorizador = baseUrl + 'Autorizadores/Autorizador/ListarAutorizador';
var urlAsignarAutorizador = baseUrl + 'Autorizadores/Autorizador/AsignarAutorizador';
var urlEliminarAutorizador = baseUrl + 'Autorizadores/Autorizador/EliminarAutorizador';
var urlActualizarEstadoArchivoAutorizador = baseUrl + 'Autorizadores/Autorizador/ActualizarEstadoArchivoAutorizador';
var urlImprimir = baseUrl + 'Autorizadores/Autorizador/Imprimir';
var urlReimprimir = baseUrl + 'Autorizadores/Autorizador/Reimprimir';


var Autorizador = function () {

    var eventos = function () {
        $("#btnAsignar").on('click', function () {
            asignarAutorizador();
        });

        $("#btnGenerar").on('click', function () {
            generacionArchivo();
        });

        $("#btnEliminar").on('click', function () {

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar a los autorizadores?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        eliminarAutorizador();
                    }
                });



        });

        $("#btnBuscarColaborador").on('click', function () {
            $('#modalBusqueda').modal('show');
            cargarTodosColaboradores();
        });


        $("#bntColaboradoresActivos").on('click', function () {
            $('#modalColaboradoresActivos').modal('show');
            cargarColaboradores();
        });

        $("#btnAsignarAutorizadorBusqueda").on('click', function () {
            asignarAutorizadorBusqueda();
        });

        $("#chkAnulados,#chkActivos").on("change", function () {
            checkActivoAnulado();
        })

        $("#chkAnuladosTodos,#chkActivosTodos").on("change", function () {
            checkActivoAnuladoTodos();
        })

        $("#chkInactivosAutorizadores,#chkActivosAutorizadores").on("change", function () {
            checkActivoAnuladoAutorizadores();
        })

        $("#chkTodos").on("change", function () {
            chechTodos();
        })

        $("#btnImprimir").on('click', function () {
            imprimir();
        });

        $("#btnReimprimir").on('click', function () {
            reimprimir();
        });

    }

    $('#tableColaborador tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $('#tableAutorizador tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $('#tableBusquedaColaborador tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    var visualizarDataTableColaborador = function () {


        $.ajax({
            url: urlColaborador,
            type: "post",
            dataType: "json",
            success: function (response) {

                var columnas = [];

                response.Columnas.forEach((x) => {
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
                        defaultContent: "",
                    });
                });

                dataTableColaborador = $('#tableColaborador').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '350px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    //data: response.Colaboradores,
                    bAutoWidth: false,
                    rowCallback: function (row, data, index) {
                        if (data.Estado == "ANU") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    buttons: [
                        {
                            extend: 'excel',
                            text: 'Excel <i class="fa fa-cloud-download"></i>',
                            titleAttr: 'Descargar Excel',
                            className: 'btn-sm mb-1 ms-2',
                            exportOptions: {
                                modifier: { page: 'all' }
                            }
                        },
                    ],
                });

                dataTableColaborador.buttons().container().prependTo($('#tableColaborador_filter'));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    };

    var visualizarDataTableAutorizador = function () {

        $.ajax({
            url: urlAutorizador,
            type: "post",
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
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", ""),
                        defaultContent: "",
                    });
                });

                dataTableAutorizador = $('#tableAutorizador').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '400px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    "columns": columnas,
                    "data": response.Autorizadores,
                    "bAutoWidth": false,
                    rowCallback: function (row, data, index) {
                        if (data.Estado == "ELI") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    buttons: [
                        {
                            extend: 'excel',
                            text: 'Excel <i class="fa fa-cloud-download"></i>',
                            titleAttr: 'Descargar Excel',
                            className: 'btn-sm mb-1 ms-2',
                            exportOptions: {
                                modifier: { page: 'all' }
                            }
                        },
                    ],
                });

                dataTableAutorizador.buttons().container().prependTo($('#tableAutorizador_filter'));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });


    };

    var visualizarDataTableBusquedaColaborador = function () {

        $.ajax({
            url: urlTodosColaborador,
            type: "post",
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
                    columnas.push({
                        title: x,
                        data: x.replace(" ", "").replace(".", "").replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u"),
                    });
                });

                dataTableBusquedaColaborador = $('#tableBusquedaColaborador').DataTable({
                    language: {
                        searchPlaceholder: 'Buscar...',
                        sSearch: '',
                    },
                    scrollY: '300px',
                    scrollX: true,
                    scrollCollapse: true,
                    paging: false,
                    columns: columnas,
                    data: response.Colaboradores,
                    bAutoWidth: false,
                    rowCallback: function (row, data, index) {
                        if (data.Estado == "ANU") {
                            $("td", row).addClass("text-danger");
                        }
                    },
                    buttons: [
                        {
                            extend: 'excel',
                            text: 'Excel <i class="fa fa-cloud-download"></i>',
                            titleAttr: 'Descargar Excel',
                            className: 'btn-sm mb-1 ms-2',
                            exportOptions: {
                                modifier: { page: 'all' }
                            }
                        },
                    ],
                });

                dataTableBusquedaColaborador.buttons().container().prependTo($('#tableBusquedaColaborador_filter'));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });

        //dataTableBusquedaColaborador = $('#tableBusquedaColaborador').DataTable({
        //    language: {
        //        searchPlaceholder: 'Buscar...',
        //        sSearch: '',
        //    },
        //    scrollY: '180px',
        //    scrollX: true,
        //    scrollCollapse: true,
        //    paging: false,
        //    "bAutoWidth": false,
        //    "columns": [
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.Codigo;
        //                else
        //                    return '<span class="text-danger">' + obj.Codigo + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.ApellidoPaterno;
        //                else
        //                    return '<span class="text-danger">' + obj.ApellidoPaterno + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.ApellidoMaterno;
        //                else
        //                    return '<span class="text-danger">' + obj.ApellidoMaterno + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.Nombres;
        //                else
        //                    return '<span class="text-danger">' + obj.Nombres + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.FechaIngreso;
        //                else
        //                    return '<span class="text-danger">' + obj.FechaIngreso + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return 'ACTIVO';
        //                else
        //                    return '<span class="text-danger">ANULADO</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.NumeroDocumento;
        //                else
        //                    return '<span class="text-danger">' + obj.NumeroDocumento + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.CodigoLocal;
        //                else
        //                    return '<span class="text-danger">' + obj.CodigoLocal + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.DescLocal;
        //                else
        //                    return '<span class="text-danger">' + obj.DescLocal + '</span>';
        //            }
        //        },
        //        {
        //            "data": function (obj) {
        //                if (obj.Estado == 'ACT')
        //                    return obj.DescPuesto;
        //                else
        //                    return '<span class="text-danger">' + obj.DescPuesto + '</span>';
        //            }
        //        }
        //    ]
        //});
    };

    const asignarAutorizador = function () {

        const registrosSeleccionados = dataTableColaborador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        btnLoading($("#btnAsignar"), true);

        let autorizadores = [];

        registrosSeleccionados.map((item) => {
            autorizadores.push({
                Codigo: item.Codigo,
                Nombres: item.Nombre,
                ApellidoPaterno: item.APaterno,
                ApellidoMaterno: item.AMaterno,
                NumeroDocumento: item.Documento
            });
        });

        $.ajax({
            url: urlAsignarAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                cargarColaboradores();
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });
                btnLoading($("#btnAsignar"), false);
                $('#modalColaboradoresActivos').modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnAsignar"), false);
            }
        });
    }

    const asignarAutorizadorBusqueda = function () {

        const registrosSeleccionados = dataTableBusquedaColaborador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        btnLoading($("#btnAsignarAutorizadorBusqueda"), true);

        let autorizadores = [];

        registrosSeleccionados.map((item) => {
            autorizadores.push({
                Codigo: item.Codigo,
                Nombres: item.Nombre,
                ApellidoPaterno: item.APaterno,
                ApellidoMaterno: item.AMaterno,
                NumeroDocumento: item.Documento
            });
        });


        $.ajax({
            url: urlAsignarAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                cargarColaboradores();
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });

                btnLoading($("#btnAsignarAutorizadorBusqueda"), false);

                $('#modalBusqueda').modal('hide');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnAsignarAutorizadorBusqueda"), false);
            }
        });
    }

    const eliminarAutorizador = function () {

        const registrosSeleccionados = dataTableAutorizador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }


        let autorizadores = registrosSeleccionados.map(x => {
            return {
                Codigo: x.Codigo,
                CodigoAutorizador: x.Autorizador,
                UsuarioCreacion: x.UCreación
            }
        });



        btnLoading($("#btnEliminar"), true);

        $.ajax({
            url: urlEliminarAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                cargarAutorizadores();
                swal({
                    text: response.Mensaje,
                    icon: "success"
                });
                btnLoading($("#btnEliminar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error"
                });
                btnLoading($("#btnEliminar"), false);
            }
        });
    }

    const generacionArchivo = function () {

        const registrosSeleccionados = dataTableAutorizador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        btnLoading($("#btnGenerar"), true);

        let autorizadores = registrosSeleccionados.map(x => {
            return {
                Codigo: x.Codigo,
                CodigoAutorizador: x.Autorizador,
                CodLocal: x.LOCAL,
                NumeroTarjeta: x.Tarjeta
            }
        });

        $.ajax({
            url: urlActualizarEstadoArchivoAutorizador,
            type: "post",
            data: { autorizadores: autorizadores },
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
                btnLoading($("#btnGenerar"), false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                btnLoading($("#btnGenerar"), false);
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

    const cargarTodosColaboradores = function () {
        $.ajax({
            url: urlTodosColaborador,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                dataTableBusquedaColaborador.clear();
                dataTableBusquedaColaborador.rows.add(response.Colaboradores);
                dataTableBusquedaColaborador.draw();

                setTimeout(() => {
                    dataTableBusquedaColaborador.columns.adjust().draw();
                }, 500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const cargarAutorizadores = function () {
        $.ajax({
            url: urlAutorizador,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                dataTableAutorizador.clear();
                dataTableAutorizador.rows.add(response.Autorizadores);
                dataTableAutorizador.draw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const cargarColaboradores = function () {
        $.ajax({
            url: urlColaborador,
            type: "post",
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                dataTableColaborador.clear();
                dataTableColaborador.rows.add(response.Colaboradores);
                dataTableColaborador.draw();

                setTimeout(() => {
                    dataTableColaborador.columns.adjust().draw();
                }, 500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const checkActivoAnulado = function () {

        if ($("#chkAnulados").prop('checked') && !$("#chkActivos").prop('checked')) {
            dataTableColaborador.column(5).search('ANU').draw();
        }
        else if ($("#chkActivos").prop('checked') && !$("#chkAnulados").prop('checked')) {
            dataTableColaborador.column(5).search('ACT').draw();
        }
        else {
            dataTableColaborador.column(5).search('').draw();
        }
    }

    const checkActivoAnuladoAutorizadores = function () {

        if ($("#chkInactivosAutorizadores").prop('checked') && !$("#chkActivosAutorizadores").prop('checked')) {
            dataTableAutorizador.column(8).search('ELI').draw();
        }
        else if ($("#chkActivosAutorizadores").prop('checked') && !$("#chkInactivosAutorizadores").prop('checked')) {
            var regExSearch = '\\b' + 'ACT' + '\\b';
            dataTableAutorizador.column(8).search(regExSearch, true, false).draw();
        }
        else {
            dataTableAutorizador.column(8).search('').draw();
        }
    }
    const chechTodos = function () {

        if ($("#chkTodos").prop('checked')) {
            dataTableAutorizador.rows({ search: 'applied' }).nodes().each(function () {
                $(this).addClass('selected');
            });
        } else {
            dataTableAutorizador.rows({ search: 'applied' }).nodes().each(function () {
                $(this).removeClass('selected');
            });
        }


    }

    const checkActivoAnuladoTodos = function () {

        if ($("#chkAnuladosTodos").prop('checked') && !$("#chkActivosTodos").prop('checked')) {
            dataTableBusquedaColaborador.column(5).search('ANU').draw();
        }
        else if ($("#chkActivosTodos").prop('checked') && !$("#chkAnuladosTodos").prop('checked')) {
            dataTableBusquedaColaborador.column(5).search('ACT').draw();
        }
        else {
            dataTableBusquedaColaborador.column(5).search('').draw();
        }
    }

    const imprimir = function () {
        const registrosSeleccionados = dataTableAutorizador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let autorizadores = registrosSeleccionados.map(x => {
            return {
                CodColaborador: x.Codigo,
                CodAutorizador: x.Autorizador,
                CodLocal: x.LOCAL,
                NomAutorizador: x.Nombre,
                Cargo: x.Puesto
            }
        });

        $.ajax({
            url: urlImprimir,
            type: "post",
            data: { command: { Autorizadores: autorizadores } },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                let icon = "success";

                if (!response.Ok) {
                    icon = "warning";
                }

                mandarImpresora(response.Contenido);

                swal({ text: response.Mensaje, icon: icon });

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    const reimprimir = function () {

        let motivo = $("#txtMotivo").val();

        if (motivo === "") {
            swal({
                text: "Debe ingresar el motivo",
                icon: "warning",
            });
            return;
        }

        const registrosSeleccionados = dataTableAutorizador.rows('.selected').data().toArray();

        if (!validarSelecion(registrosSeleccionados.length)) {
            return;
        }

        let autorizadores = registrosSeleccionados.map(x => {
            return {
                CodColaborador: x.Codigo,
                CodAutorizador: x.Autorizador,
                CodLocal: x.LOCAL,
                NomAutorizador: x.Nombre,
                Cargo: x.Puesto
            }
        });

        $.ajax({
            url: urlReimprimir,
            type: "post",
            data: { command: { Autorizadores: autorizadores, Motivo: motivo } },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {

                $('#modalMotivo').modal('hide');

                let icon = "success";

                if (!response.Ok) {
                    icon = "warning";
                }

                mandarImpresora(response.Contenido);

                swal({ text: response.Mensaje, icon: icon });

            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
            }
        });
    }

    // Función para convertir Base64 a Blob
    const b64toBlob = (b64Data, contentType = '', sliceSize = 512) => {
        const byteCharacters = atob(b64Data);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    };

    const mandarImpresora = function (contenido) {
        let iframe = document.createElement('iframe');
        iframe.id = 'iframePDF';
        iframe.style.display = 'none';
        document.body.appendChild(iframe);

        // Convertir la cadena Base64 a Blob y crear una URL para el Blob
        let blob = b64toBlob(contenido, 'application/pdf');
        let blobUrl = URL.createObjectURL(blob);

        // Cargar el PDF en el iframe y mandar a imprimir
        //let iframe = document.getElementById('iframePDF') || document.createElement('iframe');
        iframe.style.display = 'none';

        if (!iframe.id) {
            iframe.id = 'iframePDF';
            document.body.appendChild(iframe);
        }

        iframe.src = blobUrl;

        iframe.onload = function () {
            setTimeout(function () { // Dar tiempo para que el PDF se cargue completamente
                iframe.contentWindow.print();
            }, 500);
        };
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                visualizarDataTableColaborador();
                visualizarDataTableAutorizador();
                visualizarDataTableBusquedaColaborador();
                $('input[type="search"]').addClass("form-control-sm");
            });
        }
    }
}(jQuery)
