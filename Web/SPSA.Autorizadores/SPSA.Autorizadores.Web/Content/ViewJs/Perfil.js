const urlListar = '/Seguridad/Perfil/Listar';
const urlGuardar = '/Seguridad/Perfil/CrearPerfil';
const urlActualizar = '/Seguridad/Perfil/ActualizarPerfil';
const urlListarMenus = '/Seguridad/Menu/ListarMenus';
const urlListarSistemas = '/Seguridad/Sistema/ListarActivos';
const urlAsociarMenus = '/Seguridad/Perfil/AsociarMenus';
const urlListarMenusAsociados = '/Seguridad/Perfil/ListarMenus';

var dataTableListado = null;
var $treeview = $("#asociarMenuTree");
var menus = [{ "id": "0", "parent": "#", "text": "" }];
const Perfil = function () {

    var eventos = function () {
        $('#btnNuevoPerfil').click(function () {
            limpiarControles();
            $('#crearPerfilModal').modal('show');
            $('#tituloPerfilModalLabel').text('Crear Perfil');
            $('#codPerfil').prop('disabled', false);
            $('#indActivo').prop('disabled', true);
        });

        $('#btnEditarPerfil').click(function () {
            if (!validarSeleccion()) return;

            setearValoresDeControles();

            $('#crearPerfilModal').modal('show');
            $('#tituloPerfilModalLabel').text('Editar Perfil');
            $('#codPerfil').prop('disabled', true);
            $('#indActivo').prop('disabled', false);
        });

        $('#btnGuardarPerfil').click(function () {
            if (!validarValoresDeControles()) return;
            var perfil = obtenerValoresDeControles();
            if ($('#codPerfil').prop('disabled'))
                actualizarPerfil(perfil);
            else
                crearPerfil(perfil);

            $('#crearPerfilModal').modal('hide');
        });

        $('#tablePerfiles tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                dataTableListado.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });

        $('#btnAsociarMenus').click(function () {
            if (!validarSeleccion()) return;
            listarSistemas();
            $('#asociarMenusModal').modal('show');
            //listarMenus();
        });

        $("#cboSistema").on("change", function () {
            listarMenus();
        });

        $('#btnGuardarMenus').click(function () {
            let registros = $treeview.jstree("get_selected", true);
            console.log(registros);
            let menusAsociados = registros.map(item => {
                return {
                    CodMenu: item.id,
                    CodMenuPadre: item.parent
                }
            });

            let menus = {
                CodSistema: $("#cboSistema").val(),
                CodPerfil: dataTableListado.row('.selected').data().CodPerfil,
                Menus: menusAsociados
            };

            asociarMenus(menus);

        });

        $treeview.on('refresh.jstree', function () {
            $treeview.jstree('open_all');
        });


    }

    const inicializarDataTablePerfiles = function () {

        dataTableListado = $('#tablePerfiles').DataTable({
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
                        text: 'Error al listar los perfiles: ' + jqXHR,
                        icon: "error"
                    });
                }
            },
            columns: [
                { data: "CodPerfil" },
                { data: "NomPerfil" },
                { data: "TipPerfil" },
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

    const crearPerfil = function (perfil) {
        $.ajax({
            url: urlGuardar,
            type: 'POST',
            data: JSON.stringify(perfil),
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
                        text: 'Perfil creado con éxito',
                        icon: "success"
                    });
                    dataTableListado.ajax.reload();
                    limpiarControles();
                } else {
                    swal({
                        text: 'Error al crear el perfil: ' + data.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: 'Error al crear el perfil: ' + jqXHR,
                    icon: "error"
                });
            }
        });
    }

    const actualizarPerfil = function (perfil) {
        $.ajax({
            url: urlActualizar,
            type: 'POST',
            data: JSON.stringify(perfil),
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
                        text: 'Perfil actualizado con éxito',
                        icon: "success"
                    });
                    dataTableListado.ajax.reload();
                    limpiarControles();
                } else {
                    swal({
                        text: 'Error al actualizar el perfil: ' + data.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: 'Error al actualizar el perfil: ' + jqXHR,
                    icon: "error"
                });
            }
        });
    }

    const setearValoresDeControles = function () {
        var fila = dataTableListado.row('.selected').data();
        if (fila) {
            $('#codPerfil').val(fila.CodPerfil);
            $('#nomPerfil').val(fila.NomPerfil);
            $('#tipo').val(fila.TipPerfil);
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
        var perfil = obtenerValoresDeControles();
        var mensajes = '';

        if (!perfil.CodPerfil || perfil.CodPerfil.trim() === '') {
            mensajes += 'El código del perfil no puede estar vacío\n';
        }

        if (!perfil.NomPerfil || perfil.NomPerfil.trim() === '') {
            mensajes += 'El nombre del perfil no puede estar vacío\n';
        }

        if (!perfil.IndActivo || perfil.IndActivo.trim() === '') {
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
        var perfil = {
            CodPerfil: $('#codPerfil').val(),
            NomPerfil: $('#nomPerfil').val(),
            TipPerfil: $('#tipo').val(),
            Sigla: $('#sigla').val(),
            IndActivo: $('#indActivo').val()
        };
        return perfil;
    }

    const limpiarControles = function () {
        $('#codPerfil').val('');
        $('#nomPerfil').val('');
        $('#tipo').val('');
        $('#indActivo').val('A').trigger('change');
        $('#indActivo').prop('disabled', true);
        $('#codPerfil').prop('disabled', false);

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

    const listarMenus = function () {

        $treeview.jstree(true).deselect_all(true);

        let data = {
            CodSistema: $("#cboSistema").val(),
            CodPerfil: dataTableListado.row('.selected').data().CodPerfil
        }

        $.ajax({
            url: urlListarMenusAsociados,
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
                    mensajeError('Error al listar los menus: ' + data.Mensaje);
                    return;
                }

                menus = [{ "id": "0", "parent": "#", "text": "" }];

                data.Data.forEach(item => {
                    if (item.IndAsociado) {
                        menus.push({
                            id: item.CodMenu,
                            text: item.NomMenu,
                            parent: item.CodMenuPadre == "" || item.CodMenuPadre == null ? "0" : item.CodMenuPadre,
                            state: { selected: true },
                        });
                    } else {
                        menus.push({
                            id: item.CodMenu,
                            text: item.NomMenu,
                            parent: item.CodMenuPadre == "" || item.CodMenuPadre == null ? "0" : item.CodMenuPadre,
                            state: { selected: false },
                        });
                    }
                  
                });

                console.log(data);

                //$treeview.jstree(true).refresh();
                $treeview.jstree(true).load_node('#', (node,state) => {
                    $treeview.jstree('open_all');
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar los sistemas: ' + jqXHR);
            }
        });
    }


    const listarSistemas = function () {

        $.ajax({
            url: urlListarSistemas,
            type: 'POST',
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {
                if (!data.Ok) {
                    mensajeError('Error al listar los sistemas: ' + data.Mensaje);
                    return;
                }

                let results = data.Data.map(item => {
                    return {
                        id: item.CodSistema,
                        text: item.NomSistema
                    }
                });

                $("#cboSistema").empty();

                $("#cboSistema").select2({
                    data: results,
                    minimumResultsForSearch: '',
                    placeholder: "Seleccionar",
                    width: '100%',
                    dropdownParent: $('#asociarMenusModal .modal-content'),
                    language: {
                        noResults: function () {
                            return "No hay resultado";
                        },
                        searching: function () {
                            return "Buscando..";
                        }
                    }
                });

                $("#cboSistema").trigger('change');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar los sistemas: ' + jqXHR);
            }
        });
    }

    const inicializarMenus = function () {
        $treeview.jstree({
            checkbox: {
                keep_selected_style: false,

            },
            plugins: ["noclose", 'checkbox', 'json_data'],
            core: {
                'check_callback': true,
                'data': function (node, cb) {
                    cb.call(this, menus);
                }
            }
        });
    }

    const asociarMenus = function (menus) {
        $.ajax({
            url: urlAsociarMenus,
            type: 'POST',
            data: JSON.stringify(menus),
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
                        text: 'Menus asociados con éxito',
                        icon: "success"
                    });
                } else {
                    swal({
                        text: 'Error al asociar los menus: ' + data.Mensaje,
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: 'Error al asociar los menus: ' + jqXHR,
                    icon: "error"
                });
            }
        });
    }

    return {
        init: function () {
            checkSession(async function () {
                inicializarMenus();
                eventos();
                inicializarDataTablePerfiles();
            });
        }
    }

}(jQuery);