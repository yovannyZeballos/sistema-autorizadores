var urlDescargarKardex = baseUrl + 'Inventario/Kardex/DescargarPorFechas';

var dataTableProveedores = null;

var MovimientoKardex = function () {

    const eventos = function () {

        $("#btnDescargarKardex").on("click", function () {
            var fDesde = $("#fDesde").val();
            var fHasta = $("#fHasta").val();

            if (!fDesde || !fHasta) {
                swal("Aviso", "Debe seleccionar las fechas Desde y Hasta.", "warning");
                return;
            }
            if (fDesde > fHasta) {
                swal("Aviso", "La fecha Desde no puede ser mayor que la fecha Hasta.", "warning");
                return;
            }

            // navega directo (descarga el archivo)
            window.location = urlDescargarKardex
                + "?FechaInicio=" + encodeURIComponent(fDesde)
                + "&FechaFin=" + encodeURIComponent(fHasta);
        });


    };

    async function guardarProveedor({ modo }) {
        var ruc = $('#modalInputRuc').val().trim();
        var razonSocial = $('#modalInputRazonSocial').val().trim();
        var nomComercial = $('#modalInputNomComercial').val().trim();
        var dirFiscal = $('#modalInputDirFiscal').val().trim();
        var contacto = $('#modalInputContacto').val().trim();
        var telefono = $('#modalInputTelefono').val().trim();
        var email = $('#modalInputEmail').val().trim();
        var activo = $('#modalChkActivo').is(':checked') ? 'S' : 'N';

        if (ruc === '') {
            swal({ text: "Ruc es obligatoria.", icon: "warning" });
            return;
        }

        if (razonSocial === '') {
            swal({ text: "Razón social es obligatoria.", icon: "warning" });
            return;
        }

        if (nomComercial === '') {
            swal({ text: "Nombre Comercial es obligatoria.", icon: "warning" });
            return;
        }

        if (dirFiscal === '') {
            swal({ text: "Dirección Fiscal es obligatoria.", icon: "warning" });
            return;
        }

        const urlEleccion = (modo === 'editar') ? urlEditarProveedor
                                                : urlCrearProveedor;

        var payload = {
            Ruc: ruc,
            RazonSocial: razonSocial,
            NomComercial: nomComercial,
            DirFiscal: dirFiscal,
            Contacto: contacto,
            Telefono: telefono,
            Email: email,
            IndActivo: activo
        };

        try {
            var response = await $.ajax({
                url: urlEleccion,
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(payload),
                dataType: 'json'
            });

            if (response.Ok) {
                swal({ text: response.Mensaje, icon: "success" });

                var modalEl = document.getElementById('modalNuevoProveedor');
                var modalObj = bootstrap.Modal.getInstance(modalEl);
                modalObj.hide();

                $('#tableProveedores').DataTable().ajax.reload(null, false);
            } else {
                swal({ text: response.Mensaje, icon: "warning" });
            }
        } catch (err) {
            swal({ text: err.responseText || err.statusText, icon: "error" });
        }
    }

    const visualizarDataTableProveedores = function () {

        $('#tableProveedores').DataTable({
            searching: true,
            processing: true,
            serverSide: true,
            ordering: false,
            ajax: function (data, callback, settings) {
                var pageNumber = (data.start / data.length) + 1;
                var pageSize = data.length;

                var filtros = {
                    IndActivo: $("#cboIndActivoBuscar").val(),
                    FiltroVarios: (data.search.value || '').toUpperCase()
                };

                var params = Object.assign({ PageNumber: pageNumber, PageSize: pageSize }, filtros);

                $.ajax({
                    url: urlListarProveedores,
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
            columns: [
                {
                    data: null,
                    orderable: false,
                    className: 'text-center',
                    render: function (data, type, row) {
                        return ''
                            + '<input type="checkbox" class="row-checkbox" '
                            + 'data-proveedor="' + row.Ruc + '" '
                            + '/>';
                    }
                },
                { title: "Ruc", data: "Ruc" },
                { title: "Razón Social", data: "RazonSocial" },
                {
                    title: "Activo",
                    data: "IndActivo",
                    className: 'text-center',
                    render: function (data, type, row) {
                        if (data === 'S') {
                            return '<i class="fe fe-check text-success fs-6"></i>';
                        } else {
                            return '<i class="fe fe-x text-danger fs-6"></i>';
                        }
                    }
                },
                { title: "U. Creacion", data: "UsuCreacion" },
                {
                    title: "F. Creacion", data: "FecCreacion",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { title: "U. Modifica", data: "UsuModifica" },
                {
                    title: "F. Modifica", data: "FecModifica",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
                { title: "U. Elimina", data: "UsuElimina" },
                {
                    title: "F. Elimina", data: "FecElimina",
                    render: function (data, type, row) {
                        if (data) {
                            var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                            var date = new Date(timestamp);
                            return isNaN(date.getTime()) ? "" : date.toLocaleDateString('es-PE');
                        }
                        return "";
                    }
                },
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron resultados",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "No hay registros disponibles",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                search: "Buscar Por:"
            },
            initComplete: function () {
                $('#tableProveedores_filter input').addClass('form-control-sm').attr('placeholder', 'Buscar...');
            },
            scrollY: '500px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            lengthMenu: [10, 25, 50, 100],
        });

        $('#checkAllRows').on('change', function () {
            var isChecked = $(this).is(':checked');
            $('#tableProveedores tbody .row-checkbox').each(function () {
                $(this).prop('checked', isChecked);
            });
        });

        $('#tableProveedores tbody').on('change', '.row-checkbox', function () {
            if (!$(this).is(':checked')) {
                $('#checkAllRows').prop('checked', false);
            }
        });

    };

    return {
        init: function () {
            checkSession(async function () {
                eventos();
                //visualizarDataTableProveedores();
            });
        }
    }
}(jQuery);