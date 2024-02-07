const urlListar = '/Seguridad/Sistema/Listar';
const urlGuardar = '/Seguridad/Sistema/CrearSistema';
const urlActualizar = '/Seguridad/Sistema/ActualizarSistema';

var dataTableListado = null;
const Sistema = function () {

    var eventos = function () {
        $('#btnNuevoSistema').click(function () {
            limpiarControles();
            $('#crearSistemaModal').modal('show');
            $('#tituloSistemaModalLabel').text('Crear Sistema');
            $('#codSistema').prop('disabled', false);
            $('#indActivo').prop('disabled', true);
        });

        $('#btnEditarSistema').click(function () {
            if (!validarSeleccion()) return;

            setearValoresDeControles();

            $('#crearSistemaModal').modal('show');
            $('#tituloSistemaModalLabel').text('Editar Sistema');
            $('#codSistema').prop('disabled', true);
            $('#indActivo').prop('disabled', false);
        });

        $('#btnGuardarSistema').click(function () {
            if (!validarValoresDeControles()) return;
            var sistema = obtenerValoresDeControles();
            console.log();
            if ($('#codSistema').prop('disabled'))
                actualizarSistema(sistema);
            else
                crearSistema(sistema);

            $('#crearSistemaModal').modal('hide');
        });

        $('#tableSistemas tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableListado.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    }

    const inicializarDataTableCajeros = function () {

        dataTableListado = $('#tableSistemas').DataTable({
            ajax: {
                url: urlListar,
                type: "post",
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal({
                        text: 'Error al listar los sistemas: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "CodSistema" },
                { data: "NomSistema" },
                { data: "Sigla" },
                { data: "IndActivo" },
                {
                    data: "FecCreacion",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        return convertirfecha(new Date(+(data.replace(/\D/g, ''))));
                    }
                },
                { data: "UsuCreacion" },
                {
                    data: "FecModifica",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }

                        if (data == null) return data;
                        return convertirfecha(new Date(+(data.replace(/\D/g, ''))));
                    }
                },
                { data: "UsuModifica" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            rowCallback: function (row, data, index) {
                if (data.IndActivo == "I") {
                    $("td", row).addClass("text-danger");
                }
            },
            bAutoWidth: false
        });

    }

    const crearSistema = function (sistema) {
        $.ajax({
            url: urlGuardar,
            type: 'POST',
            data: JSON.stringify(sistema),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (data.Ok) {
                    swal({
                        text: 'Sistema creado con éxito',
                        icon: "success"
                    });
                    dataTableListado.ajax.reload();
                    limpiarControles();
                } else {
                    swal({
                        text: 'Error al crear el sistema: ' + data.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: 'Error al crear el sistema: ' + jqXHR,
                    icon: "error"
                });
            }
        });
    }

    const actualizarSistema = function (sistema) {
        $.ajax({
            url: urlActualizar,
            type: 'POST',
            data: JSON.stringify(sistema),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (data.Ok) {
                    swal({
                        text: 'Sistema actualizado con éxito',
                        icon: "success"
                    });
                    dataTableListado.ajax.reload();
                    limpiarControles();
                } else {
                    swal({
                        text: 'Error al actualizar el sistema: ' + data.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: 'Error al actualizar el sistema: ' + jqXHR,
                    icon: "error"
                });
            }
        });
    }

    const setearValoresDeControles = function () {
        var fila = dataTableListado.row('.selected').data();
        console.log(fila);
        if (fila) {
            $('#codSistema').val(fila.CodSistema);
            $('#nomSistema').val(fila.NomSistema);
            $('#sigla').val(fila.Sigla);
            $('#indActivo').val(fila.IndActivo).trigger('change');
        }
    }

    const validarSeleccion = function () {
        var fila = dataTableListado.row('.selected').data();

        if (!fila) {
            swal({
                text: 'Por favor, selecciona un registro',
                icon: "warning"
            });
            return false;
        }

        return true;
    }


    const validarValoresDeControles = function () {
        var sistema = obtenerValoresDeControles();
        var mensajes = '';

        if (!sistema.CodSistema || sistema.CodSistema.trim() === '') {
            mensajes += 'El código del sistema no puede estar vacío\n';
        }

        if (!sistema.NomSistema || sistema.NomSistema.trim() === '') {
            mensajes += 'El nombre del sistema no puede estar vacío\n';
        }

        if (!sistema.Sigla || sistema.Sigla.trim() === '') {
            mensajes += 'La sigla no puede estar vacía\n';
        }

        if (!sistema.IndActivo || sistema.IndActivo.trim() === '') {
            mensajes += 'El estado no puede estar vacío\n';
        }

        if (mensajes !== '') {
            swal({
                text: mensajes,
                icon: "warning"
            });
            return false;
        }

        return true;
    }

    const obtenerValoresDeControles = function () {
        var sistema = {
            CodSistema: $('#codSistema').val(),
            NomSistema: $('#nomSistema').val(),
            Sigla: $('#sigla').val(),
            IndActivo: $('#indActivo').val()
        };
        return sistema;
    }

    const limpiarControles = function () {
        $('#codSistema').val('');
        $('#nomSistema').val('');
        $('#sigla').val('');
        $('#indActivo').val('A').trigger('change');
        $('#indActivo').prop('disabled', true);
        $('#codSistema').prop('disabled', false);

    }


    const convertirfecha = function (fecha) {

        if (fecha == null || fecha == "") return "";
        var day = fecha.getDate();       // yields date
        var month = fecha.getMonth() + 1;    // yields month (add one as '.getMonth()' is zero indexed)
        var year = fecha.getFullYear();  // yields year
        var hour = fecha.getHours();     // yields hours 
        var minute = fecha.getMinutes(); // yields minutes
        var second = fecha.getSeconds(); // yields seconds

        // After this construct a string with the above results as below
        return day + "/" + month + "/" + year + " " + hour + ':' + minute + ':' + second;
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