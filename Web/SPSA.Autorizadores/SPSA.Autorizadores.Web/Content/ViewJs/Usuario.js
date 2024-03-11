const urlListar = '/Seguridad/Usuario/Listar';
const urlListarEmpresas = '/Seguridad/Usuario/ListarEmpresas';
const urlGuardar = '/Seguridad/Usuario/CrearUsuario';
const urlActualizar = '/Seguridad/Usuario/ActualizarUsuario';
const urlAsociarEmpresa = '/Seguridad/Usuario/AsociarEmpresas';
const urlAsociarCadena = '/Seguridad/Usuario/AsociarCadenas';
const urlAsociarRegion = '/Seguridad/Usuario/AsociarRegiones';
const urlAsociarZona = '/Seguridad/Usuario/AsociarZonas';
const urlAsociarLocal = '/Seguridad/Usuario/AsociarLocales';
const urlAsociarPerfil = '/Seguridad/Usuario/AsociarPerfiles';
const urlListarEmpresasAsociadas = '/Empresa/ListarEmpresasAsociadas';
const urlListarCadenasAsociadas = '/Maestros/MaeCadena/ListarCadenasAsociadas';
const urlListarRegionesAsociadas = '/Maestros/MaeRegion/ListarRegionesAsociadas';
const urlListarZonasAsociadas = '/Maestros/MaeZona/ListarZonasAsociadas';
const urlListarCadenas = '/Seguridad/Usuario/ListarCadenas';
const urlListarRegiones = '/Seguridad/Usuario/ListarRegiones';
const urlListarZonas = '/Seguridad/Usuario/ListarZonas';
const urlListarLocales = '/Seguridad/Usuario/ListarLocales';
const urlListarPerfiles = '/Seguridad/Usuario/ListarPerfiles';

var dataTableListado = null;
var dataTableEmpresa = null;
var dataTableCadena = null;
var dataTableRegion = null;
var dataTableZona = null;
var dataTableLocal = null;
var dataTablePerfil = null;

var dataTableEmpresasInicializada = false;
var dataTableCadenasInicializada = false;
var dataTableRegionesInicializada = false;
var dataTableZonasInicializada = false;
var dataTableLocalesInicializada = false;
var dataTablePerfilesInicializada = false;
const Usuario = function () {

    var eventos = function () {
        $('#btnNuevoUsuario').on("click", function () {
            limpiarControles();
            $('#crearUsuarioModal').modal('show');
            $('#tituloUsuarioModalLabel').text('Crear Usuario');
            $('#codUsuario').prop('disabled', false);
            $('#indActivo').prop('disabled', true);
        });

        $('#btnEditarUsuario').on("click", function () {
            if (!validarSeleccion()) return;

            setearValoresDeControles();

            $('#crearUsuarioModal').modal('show');
            $('#tituloUsuarioModalLabel').text('Editar Usuario');
            $('#codUsuario').prop('disabled', true);
            $('#indActivo').prop('disabled', false);
        });

        $('#btnGuardarUsuario').on("click", function () {
            if (!validarValoresDeControles()) return;
            var usuario = obtenerValoresDeControles();
            console.log();
            if ($('#codUsuario').prop('disabled'))
                actualizarUsuario(usuario);
            else
                crearUsuario(usuario);

            $('#crearUsuarioModal').modal('hide');
        });

        $('#btnAsociarEmpresa').on("click", function () {
            if (!validarSeleccion()) return;
            var fila = obtenerFilaSeleccionada();
            $('#asociarEmpresaModalLabel').text('Asociar empresas para el usuario: ' + fila.CodUsuario);
            $('#asociarEmpresaModal').modal('show');

            if (!dataTableEmpresasInicializada)
                inicializarDataTableEmpresas();
            else
                dataTableEmpresa.ajax.reload();
        });

        $('#tableUsuarios tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableListado.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#btnGuardarEmpresa').on("click", function () {

            var usuario = obtenerFilaSeleccionada();
            if (usuario) {
                let codUsuario = usuario.CodUsuario;
                dataTableEmpresa.search('').draw();
                let empresasSeleccionadas = $("#tableEmpresa input:checkbox:checked").map(function () {
                    return $(this).attr("value");
                }).get();

                asociarEmpresas(codUsuario, empresasSeleccionadas || []);
            }
        });

        $("#chkTodos").on("change", function () {
            $('#tableEmpresa input[type="checkbox"]').prop('checked', $("#chkTodos").prop('checked'));
        })

        $('#btnAsociarCadena').on("click", function () {
            if (!validarSeleccion()) return;
            var fila = obtenerFilaSeleccionada();
            $('#asociarCadenaModalLabel').text('Asociar cadenas para el usuario: ' + fila.CodUsuario);
            $('#asociarCadenaModal').modal('show');
            $('#cboEmpresaCadena').val('').trigger('change');
        });

        $("#cboEmpresaCadena").on("change", function () {
            if (dataTableCadenasInicializada)
                dataTableCadena.ajax.reload();
            else
                inicializarDataTableCadenas();
        });

        $('#btnGuardarCadenas').on("click", function () {
            var usuario = obtenerFilaSeleccionada();
            if (usuario) {
                let codUsuario = usuario.CodUsuario;
                dataTableCadena.search('').draw();
                let cadenasSeleccionadas = $("#tableCadena input:checkbox:checked").map(function () {
                    return $(this).attr("value");
                }).get();

                asociarCadenas(codUsuario, cadenasSeleccionadas);
            }
        });

        $('#btnAsociarRegion').on("click", function () {
            console.log('asociar regiones')
            if (!validarSeleccion()) return;
            var fila = obtenerFilaSeleccionada();
            $('#asociarRegionModalLabel').text('Asociar regiones para el usuario: ' + fila.CodUsuario);
            $('#asociarRegionModal').modal('show');
            //$('#cboCadenaRegion').empty().trigger('change');
            listarEmpresasAsociadas('#cboEmpresaRegion', '#asociarRegionModal');
        });

        $("#cboCadenaRegion").on("change", function () {
            if (dataTableRegionesInicializada)
                dataTableRegion.ajax.reload();
            else
                inicializarDataTableRegion();
        });
        $("#cboEmpresaRegion").on("change", function () {
            if (dataTableRegion != null)
                dataTableRegion.clear().draw();

            listarCadenasAsociadas('#cboCadenaRegion', '#asociarRegionModal', '#cboEmpresaRegion');
        });

        $('#btnGuardarRegiones').on("click", function () {
            var usuario = obtenerFilaSeleccionada();
            if (usuario) {
                let codUsuario = usuario.CodUsuario;
                dataTableRegion.search('').draw();
                let regionesSeleccionadas = $("#tableRegion input:checkbox:checked").map(function () {
                    return $(this).attr("value");
                }).get();

                asociarRegiones(codUsuario, regionesSeleccionadas);
            }
        });

        $('#btnAsociarZona').on("click", function () {
            if (!validarSeleccion()) return;
            var fila = obtenerFilaSeleccionada();
            $('#asociarZonaModalLabel').text('Asociar zonas para el usuario: ' + fila.CodUsuario);
            $('#asociarZonaModal').modal('show');
            //$('#cboRegionZona').empty().trigger('change');
            listarEmpresasAsociadas('#cboEmpresaZona', '#asociarZonaModal');
        });

        $("#cboEmpresaZona").on("change", function () {
            listarCadenasAsociadas('#cboCadenaZona', '#asociarZonaModal', '#cboEmpresaZona');
        });

        $("#cboCadenaZona").on("change", function () {
            if (dataTableZona != null)
                dataTableZona.clear().draw();

            listarRegionesAsociadas('#cboRegionZona', '#asociarZonaModal', '#cboEmpresaZona', '#cboCadenaZona');
        });

        $("#cboRegionZona").on("change", function () {
            if (dataTableZonasInicializada)
                dataTableZona.ajax.reload();
            else
                inicializarDataTableZona();
        });

        $('#btnGuardarZonas').on("click", function () {
            var usuario = obtenerFilaSeleccionada();
            if (usuario) {
                let codUsuario = usuario.CodUsuario;
                dataTableZona.search('').draw();
                let zonasSeleccionadas = $("#tableZona input:checkbox:checked").map(function () {
                    return $(this).attr("value");
                }).get();

                asociarZonas(codUsuario, zonasSeleccionadas);
            }
        });

        $('#btnAsociarLocal').on("click", function () {
            if (!validarSeleccion()) return;
            var fila = obtenerFilaSeleccionada();
            $('#asociarLocalModalLabel').text('Asociar locales para el usuario: ' + fila.CodUsuario);
            $('#asociarLocalModal').modal('show');
            //$('#cboRegionZona').empty().trigger('change');
            listarEmpresasAsociadas('#cboEmpresaLocal', '#asociarLocalModal');
        });

        $("#cboEmpresaLocal").on("change", function () {
            listarCadenasAsociadas('#cboCadenaLocal', '#asociarLocalModal', '#cboEmpresaLocal');
        });

        $("#cboCadenaLocal").on("change", function () {
            listarRegionesAsociadas('#cboRegionLocal', '#asociarLocalModal', '#cboEmpresaLocal', '#cboCadenaLocal');
        });

        $("#cboRegionLocal").on("change", function () {
            listarZonasAsociadas('#cboZonaLocal', '#asociarLocalModal', '#cboEmpresaLocal', '#cboCadenaLocal', '#cboRegionLocal');
        });

        $("#cboZonaLocal").on("change", function () {

            if (dataTableLocal != null)
                dataTableLocal.clear().draw();

            if (dataTableLocalesInicializada)
                dataTableLocal.ajax.reload();
            else
                inicializarDataTableLocal();
        });

        $('#btnGuardarLocales').on("click", function () {
            var usuario = obtenerFilaSeleccionada();
            if (usuario) {
                let codUsuario = usuario.CodUsuario;
                dataTableLocal.search('').draw();
                let localesSeleccionados = $("#tableLocal input:checkbox:checked").map(function () {
                    return $(this).attr("value");
                }).get();

                asociarLocales(codUsuario, localesSeleccionados);
            }
        });

        $("#chkTodosLocal").on("change", function () {
            $('#tableLocal input[type="checkbox"]').prop('checked', $("#chkTodosLocal").prop('checked'));
        })

        $('#btnAsociarPerfil').on("click", function () {
            if (!validarSeleccion()) return;
            var fila = obtenerFilaSeleccionada();
            $('#asociarPerfilModalLabel').text('Asociar perfiles para el usuario: ' + fila.CodUsuario);
            $('#asociarPerfilModal').modal('show');

            if (!dataTablePerfilesInicializada)
                inicializarDataTablePerfiles();
            else
                dataTablePerfil.ajax.reload();
        });

        $("#chkTodosPerfil").on("change", function () {
            $('#tablePerfil input[type="checkbox"]').prop('checked', $("#chkTodosPerfil").prop('checked'));
        })

        $('#btnGuardarPerfil').on("click", function () {

            var usuario = obtenerFilaSeleccionada();
            if (usuario) {
                let codUsuario = usuario.CodUsuario;
                dataTablePerfil.search('').draw();
                let perfilesSeleccionadas = $("#tablePerfil input:checkbox:checked").map(function () {
                    return $(this).attr("value");
                }).get();

                asociarPerfil(codUsuario, perfilesSeleccionadas);
            }
        });
    }


    const inicializarDataTableUsuario = function () {

        dataTableListado = $('#tableUsuarios').DataTable({
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
                    mensajeError('Error al listar los usuarios: ' + jqXHR);
                }
            },
            columns: [
                { data: "CodUsuario" },
                { data: "CodColaborador" },
                { data: "TipUsuario" },
                { data: "DirEmail" },
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
                    data: "FecElimina",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }

                        if (data == null) return data;
                        return convertirfecha(new Date(+(data.replace(/\D/g, ''))));
                    }
                },
                { data: "UsuElimina" }
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

    const obtenerFilaSeleccionada = function () {
        return dataTableListado.row('.selected').data();
    }

    const crearUsuario = function (usuario) {
        $.ajax({
            url: urlGuardar,
            type: 'POST',
            data: JSON.stringify(usuario),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (data.Ok) {
                    mensajeExito('Usuario creado con éxito');
                    dataTableListado.ajax.reload();
                    limpiarControles();
                } else {
                    mensajeError('Error al crear el usuario: ' + data.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const actualizarUsuario = function (usuario) {
        $.ajax({
            url: urlActualizar,
            type: 'POST',
            data: JSON.stringify(usuario),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (data.Ok) {
                    mensajeExito('Usuario actualizado con éxito');
                    dataTableListado.ajax.reload();
                    limpiarControles();
                } else {
                    mensajeError('Error al actualizar el usuario: ' + data.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al actualizar el usuario: ' + jqXHR);
            }
        });
    }

    const setearValoresDeControles = function () {
        var fila = obtenerFilaSeleccionada();
        if (fila) {
            $('#codUsuario').val(fila.CodUsuario);
            $('#codColaborador').val(fila.CodColaborador);
            $('#tipUsuario').val(fila.TipUsuario).trigger('change');
            $('#dirEmail').val(fila.DirEmail);
            $('#indActivo').val(fila.IndActivo).trigger('change');
        }
    }

    const validarSeleccion = function () {
        var fila = obtenerFilaSeleccionada();

        if (!fila) {
            mensajeAdvertencia('Por favor, selecciona un registro');
            return false;
        }

        return true;
    }

    const validarValoresDeControles = function () {
        var usuario = obtenerValoresDeControles();
        var mensajes = '';

        if (!usuario.CodUsuario || usuario.CodUsuario.trim() === '') {
            mensajes += 'El código del usuario no puede estar vacío\n';
        }

        if (!usuario.CodColaborador || usuario.CodColaborador.trim() === '') {
            mensajes += 'El codigo del colaborador no puede estar vacío\n';
        }

        if (!usuario.IndActivo || usuario.IndActivo.trim() === '') {
            mensajes += 'El estado no puede estar vacío\n';
        }

        if (mensajes !== '') {
            mensajeAdvertencia(mensajes);
            return false;
        }

        return true;
    }

    const obtenerValoresDeControles = function () {
        var usuario = {
            CodUsuario: $('#codUsuario').val(),
            CodColaborador: $('#codColaborador').val(),
            TipUsuario: $('#tipUsuario').val(),
            DirEmail: $('#dirEmail').val(),
            IndActivo: $('#indActivo').val()
        };
        return usuario;
    }

    const limpiarControles = function () {
        $('#codUsuario').val('');
        $('#codColaborador').val('');
        $('#tipUsuario').val('AD').trigger('change');
        $('#dirEmail').val('');
        $('#indActivo').val('A').trigger('change');
        $('#indActivo').prop('disabled', true);
        $('#codUsuario').prop('disabled', false);

    }

    const inicializarDataTableEmpresas = function () {

        dataTableEmpresa = $('#tableEmpresa').DataTable({
            ajax: {
                url: urlListarEmpresas,
                type: "post",
                data: function (d) {
                    let usuario = obtenerFilaSeleccionada();
                    d.CodUsuario = usuario.CodUsuario;
                },
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    mensajeError('Error al listar las empresas: ' + jqXHR);
                }
            },
            columns: [
                {
                    data: "IndAsociado",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        let checked = data ? 'checked' : '';
                        return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(row.CodEmpresa).html() + '" ' + checked + '>';
                    }
                },
                { data: "CodEmpresa" },
                { data: "NomEmpresa" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '300px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            bAutoWidth: false,
            order: [[1, 'asc']]
        });

        dataTableEmpresasInicializada = true;
        $('#tableEmpresa_filter input').addClass('form-control-sm');
    }

    const asociarEmpresas = function (codUsuario, empresasAsociadas) {

        console.log(empresasAsociadas);
        $.ajax({
            url: urlAsociarEmpresa,
            type: 'POST',
            data: {
                CodUsuario: codUsuario,
                EmpresasAsociadas: empresasAsociadas
            },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (respuesta) {
                if (respuesta.Ok) {
                    mensajeExito('Empresas asociadas con éxito');
                } else {
                    mensajeError('Error al asociar empresas: ' + respuesta.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const listarEmpresasAsociadas = function (idControl, idModalContent) {

        let usuario = obtenerFilaSeleccionada();
        let data = { CodUsuario: usuario.CodUsuario, Busqueda: '' };

        $.ajax({
            url: urlListarEmpresasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al crear el usuario: ' + data.Mensaje);
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodEmpresa,
                        text: item.NomEmpresa
                    }
                });
                console.log(results);

                $(idControl).empty();

                $(idControl).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $(idModalContent + ' .modal-content'),
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idControl).trigger('change');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const listarCadenasAsociadas = function (idCombo, idModalContent, idComboEmpresa) {

        let usuario = obtenerFilaSeleccionada();
        let data = { CodUsuario: usuario.CodUsuario, CodEmpresa: $(idComboEmpresa).val(), Busqueda: '' };

        $.ajax({
            url: urlListarCadenasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las cadenas asociadas: ' + data.Mensaje);
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodCadena,
                        text: item.NomCadena
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $(idModalContent + ' .modal-content'),
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const listarRegionesAsociadas = function (idCombo, idModalContent, idComboEmpresa, idComboCadena) {

        let usuario = obtenerFilaSeleccionada();
        let data = {
            CodUsuario: usuario.CodUsuario,
            CodEmpresa: $(idComboEmpresa).val(),
            CodCadena: $(idComboCadena).val()
        };

        $.ajax({
            url: urlListarRegionesAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las regiones asociadas: ' + data.Mensaje);
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodRegion,
                        text: item.NomRegion
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $(idModalContent + ' .modal-content'),
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al crear el usuario: ' + jqXHR);
            }
        });
    }

    const inicializarSelect2 = function () {
        $('#cboEmpresaCadena').select2({
            ajax: {
                url: urlListarEmpresasAsociadas,
                dataType: 'json',
                type: "post",
                data: function (param) {
                    let usuario = obtenerFilaSeleccionada();
                    return { CodUsuario: usuario.CodUsuario, Busqueda: param.term };
                },
                processResults: function (data) {
                    let results = data.Data.map(item => {
                        return {
                            id: item.CodEmpresa,
                            text: item.NomEmpresa
                        }
                    });
                    return {
                        results: results
                    };
                }
            },
            minimumResultsForSearch: '',
            placeholder: "Seleccionar",
            width: '100%',
            dropdownParent: $('#asociarCadenaModal .modal-content'),
            language: {
                noResults: function () {
                    return "No hay resultado";
                },
                searching: function () {
                    return "Buscando..";
                }
            }
        });
    }

    const inicializarDataTableCadenas = function () {

        dataTableCadena = $('#tableCadena').DataTable({
            ajax: {
                url: urlListarCadenas,
                type: "post",
                data: function (d) {
                    let usuario = obtenerFilaSeleccionada();
                    d.CodUsuario = usuario.CodUsuario;
                    d.CodEmpresa = $('#cboEmpresaCadena').val();
                },
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    mensajeError('Error al listar las cadenas: ' + jqXHR);
                }
            },
            columns: [
                {
                    data: "IndAsociado",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        let checked = data ? 'checked' : '';
                        return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(row.CodCadena).html() + '" ' + checked + '>';
                    }
                },
                { data: "CodCadena" },
                { data: "NomCadena" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '300px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            bAutoWidth: false,
            order: [[1, 'asc']]
        });

        dataTableCadenasInicializada = true;
        $('#tableCadena_filter input').addClass('form-control-sm');
    }

    const asociarCadenas = function (codUsuario, cadenasAsociadas) {

        $.ajax({
            url: urlAsociarCadena,
            type: 'POST',
            data: {
                CodUsuario: codUsuario,
                CodEmpresa: $('#cboEmpresaCadena').val(),
                CadenasAsociadas: cadenasAsociadas
            },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (respuesta) {
                if (respuesta.Ok) {
                    mensajeExito('Cadenas asociadas con éxito');
                } else {
                    mensajeError('Error al asociar cadenas: ' + respuesta.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al asociar cadenas: ' + jqXHR);
            }
        });
    }

    const inicializarDataTableRegion = function () {

        dataTableRegion = $('#tableRegion').DataTable({
            ajax: {
                url: urlListarRegiones,
                type: "post",
                data: function (d) {
                    let usuario = obtenerFilaSeleccionada();
                    d.CodUsuario = usuario.CodUsuario;
                    d.CodEmpresa = $('#cboEmpresaRegion').val();
                    d.CodCadena = $('#cboCadenaRegion').val();
                },
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    mensajeError('Error al listar las regiones: ' + jqXHR);
                }
            },
            columns: [
                {
                    data: "IndAsociado",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        let checked = data ? 'checked' : '';
                        return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(row.CodRegion).html() + '" ' + checked + '>';
                    }
                },
                { data: "CodRegion" },
                { data: "NomRegion" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '300px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            bAutoWidth: false,
            order: [[1, 'asc']]
        });

        dataTableRegionesInicializada = true;
        $('#tableRegion_filter input').addClass('form-control-sm');
    }

    const asociarRegiones = function (codUsuario, regionesAsociadas) {

        $.ajax({
            url: urlAsociarRegion,
            type: 'POST',
            data: {
                CodUsuario: codUsuario,
                CodEmpresa: $('#cboEmpresaRegion').val(),
                CodCadena: $('#cboCadenaRegion').val(),
                RegionesAsociadas: regionesAsociadas
            },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (respuesta) {
                if (respuesta.Ok) {
                    mensajeExito('Regiones asociadas con éxito');
                } else {
                    mensajeError('Error al asociar regiones: ' + respuesta.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al asociar regiones: ' + jqXHR);
            }
        });
    }

    const inicializarDataTableZona = function () {

        dataTableZona = $('#tableZona').DataTable({
            ajax: {
                url: urlListarZonas,
                type: "post",
                data: function (d) {
                    let usuario = obtenerFilaSeleccionada();
                    d.CodUsuario = usuario.CodUsuario;
                    d.CodEmpresa = $('#cboEmpresaZona').val();
                    d.CodCadena = $('#cboCadenaZona').val();
                    d.CodRegion = $('#cboRegionZona').val();
                },
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    mensajeError('Error al listar las zonas: ' + jqXHR);
                }
            },
            columns: [
                {
                    data: "IndAsociado",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        let checked = data ? 'checked' : '';
                        return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(row.CodZona).html() + '" ' + checked + '>';
                    }
                },
                { data: "CodZona" },
                { data: "NomZona" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '300px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            bAutoWidth: false,
            order: [[1, 'asc']]
        });

        dataTableZonasInicializada = true;
        $('#tableZona_filter input').addClass('form-control-sm');
    }

    const asociarZonas = function (codUsuario, zonasAsociadas) {

        $.ajax({
            url: urlAsociarZona,
            type: 'POST',
            data: {
                CodUsuario: codUsuario,
                CodEmpresa: $('#cboEmpresaZona').val(),
                CodCadena: $('#cboCadenaZona').val(),
                CodRegion: $('#cboRegionZona').val(),
                ZonasAsociadas: zonasAsociadas
            },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (respuesta) {
                if (respuesta.Ok) {
                    mensajeExito('Zonas asociadas con éxito');
                } else {
                    mensajeError('Error al asociar zonas: ' + respuesta.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al asociar zonas: ' + jqXHR);
            }
        });
    }

    const listarZonasAsociadas = function (idCombo, idModalContent, idComboEmpresa, idComboCadena, idComboRegion) {

        let usuario = obtenerFilaSeleccionada();
        let data = {
            CodUsuario: usuario.CodUsuario,
            CodEmpresa: $(idComboEmpresa).val(),
            CodCadena: $(idComboCadena).val(),
            CodRegion: $(idComboRegion).val()
        };

        $.ajax({
            url: urlListarZonasAsociadas,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar las zonas asociadas: ' + data.Mensaje);
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodZona,
                        text: item.NomZona
                    }
                });

                $(idCombo).empty();

                $(idCombo).select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $(idModalContent + ' .modal-content'),
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $(idCombo).trigger('change');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar las zonas: ' + jqXHR);
            }
        });
    }

    const inicializarDataTableLocal = function () {

        dataTableLocal = $('#tableLocal').DataTable({
            ajax: {
                url: urlListarLocales,
                type: "post",
                data: function (d) {
                    let usuario = obtenerFilaSeleccionada();
                    d.CodUsuario = usuario.CodUsuario;
                    d.CodEmpresa = $('#cboEmpresaLocal').val();
                    d.CodCadena = $('#cboCadenaLocal').val();
                    d.CodRegion = $('#cboRegionLocal').val();
                    d.CodZona = $('#cboZonaLocal').val();
                },
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    mensajeError('Error al listar los locales: ' + jqXHR);
                }
            },
            columns: [
                {
                    data: "IndAsociado",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        let checked = data ? 'checked' : '';
                        return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(row.CodLocal).html() + '" ' + checked + '>';
                    }
                },
                { data: "CodLocal" },
                { data: "NomLocal" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '250px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            bAutoWidth: false,
            order: [[1, 'asc']]
        });

        dataTableLocalesInicializada = true;
        $('#tableLocal_filter input').addClass('form-control-sm');
    }

    const asociarLocales = function (codUsuario, localesAsociadas) {

        $.ajax({
            url: urlAsociarLocal,
            type: 'POST',
            data: {
                CodUsuario: codUsuario,
                CodEmpresa: $('#cboEmpresaLocal').val(),
                CodCadena: $('#cboCadenaLocal').val(),
                CodRegion: $('#cboRegionLocal').val(),
                CodZona: $('#cboZonaLocal').val(),
                LocalesAsociados: localesAsociadas
            },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (respuesta) {
                if (respuesta.Ok) {
                    mensajeExito('Locales asociadas con éxito');
                } else {
                    mensajeError('Error al asociar Locales: ' + respuesta.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al asociar Locales: ' + jqXHR);
            }
        });
    }

    const inicializarDataTablePerfiles = function () {

        dataTablePerfil = $('#tablePerfil').DataTable({
            ajax: {
                url: urlListarPerfiles,
                type: "post",
                data: function (d) {
                    let usuario = obtenerFilaSeleccionada();
                    d.CodUsuario = usuario.CodUsuario;
                },
                dataType: "JSON",
                dataSrc: "Data",
                beforeSend: function () {
                    showLoading();
                },
                complete: function () {
                    closeLoading();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    mensajeError('Error al listar los perfiles: ' + jqXHR);
                }
            },
            columns: [
                {
                    data: "IndAsociado",
                    render: function (data, type, row) {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        let checked = data ? 'checked' : '';
                        return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(row.CodPerfil).html() + '" ' + checked + '>';
                    }
                },
                { data: "CodPerfil" },
                { data: "NomPerfil" }
            ],
            language: {
                searchPlaceholder: 'Buscar...',
                sSearch: '',
            },
            scrollY: '300px',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            bAutoWidth: false,
            order: [[1, 'asc']]
        });

        dataTablePerfilesInicializada = true;
        $('#tablePerfil_filter input').addClass('form-control-sm');
    }

    const asociarPerfil = function (codUsuario, perfilesAsociadas) {

        $.ajax({
            url: urlAsociarPerfil,
            type: 'POST',
            data: {
                CodUsuario: codUsuario,
                PerfilesAsociadas: perfilesAsociadas
            },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (respuesta) {
                if (respuesta.Ok) {
                    mensajeExito('Perfiles asociadas con éxito');
                } else {
                    mensajeError('Error al asociar Perfiles: ' + respuesta.Mensaje);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al asociar Perfiles: ' + jqXHR);
            }
        });
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();
                inicializarDataTableUsuario();
                inicializarSelect2();
                $('#cboEmpresaRegion').select2();
                $('#cboCadenaRegion').select2();
                $('#cboEmpresaZona').select2();
                $('#cboCadenaZona').select2();
                $('#cboRegionZona').select2();
                $('#cboEmpresaLocal').select2();
                $('#cboCadenaLocal').select2();
                $('#cboRegionLocal').select2();
                $('#cboZonaLocal').select2();
            });
        }
    }

}(jQuery);