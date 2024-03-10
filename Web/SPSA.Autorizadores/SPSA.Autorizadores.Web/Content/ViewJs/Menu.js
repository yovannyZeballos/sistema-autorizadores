const urlListarSistemas = '/Seguridad/Sistema/ListarActivos';
const urlListarMenus = '/Seguridad/Menu/ListarMenus';
const urlCrearMenu = '/Seguridad/Menu/CrearMenu';
const urlEditarMenu = '/Seguridad/Menu/ActualizarMenu';
const urlEliminarMenu = '/Seguridad/Menu/EliminarMenu';
const urlObtenerMenu = '/Seguridad/Menu/ObtenerMenu';

var $treeview = $("#jstree");
var menus = [{ "id": "0", "parent": "#", "text": "" }];
var accion = "";
const Menu = function () {

    var eventos = function () {
        $('#btnCrear').on("click", function () {
            $('#crearMenuModalLabel').text('Crear menu');
            $('#crearMenulModal').modal('show');
            accion = "NUEVO";
        });

        $('#btnGuardar').on("click", function () {
            let registros = $treeview.jstree("get_selected", true);
            let codMenuPadre = registros.length > 0 ? registros[0].id : "";

            let menu = {
                NomMenu: $('#nomMenu').val(),
                UrlMenu: $('#urlMenu').val(),
                IconoMenu: $('#iconoMenu').val(),
                CodMenuPadre: codMenuPadre,
                CodSistema: $("#cboSistema").val(),
                CodMenu: accion === "EDITAR" ? registros[0].id : ""
            }
            if (accion === "NUEVO")
                guardarMenu(menu);
            else if (accion === "EDITAR")
                editarMenu(menu);
        });

        $('#btnEditar').on("click", function () {

            let registros = $treeview.jstree("get_selected", true);
            if (!validarSeleccion(registros)) return;

            $('#crearMenuModalLabel').text('Editar menu');
            $('#crearMenulModal').modal('show');

            obtenerMenu(registros[0].id);

        });

        $treeview.on('refresh.jstree', function () {
            $treeview.jstree('open_all');
        });

        $("#cboSistema").on("change", function () {
            listarMenus();
        });

        $('#btnEliminar').on("click", function () {

            let registros = $treeview.jstree("get_selected", true);
            if (!validarSeleccion(registros)) return;

            swal({
                title: "Confirmar!",
                text: "¿Está seguro eliminar el menu?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            }).then((willDelete) => {
                if (willDelete) {
                    if (!validarEliminacion(registros[0])) return;

                    let menu = {
                        CodMenu: registros[0].id
                    }
                    eliminarMenu(menu);
                }
            });
        });
    }

    const validarEliminacion = function (menu) {
        if (menu.children.length > 0) {
            mensajeAdvertencia('No se puede eliminar un menu que tiene submenus');
            return false;
        }
        return true;
    }

    const validarSeleccion = function (registros = []) {
        if (registros.length == 0) {
            mensajeAdvertencia('Por favor, seleccione un menu');
            return false;
        }
        return true;
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
                mensajeError('Error al listar los sistemas: ' + jqXHR.responseText);
            }
        });
    }

    const obtenerMenu = function (codMenu) {

        const data = {
            CodMenu: codMenu
        };

        console.log(data);

        $.ajax({
            url: urlObtenerMenu,
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
                    mensajeError('Error al obtener el menu: ' + data.Mensaje);
                    return;
                }

                const menu = data.Data;
                $('#nomMenu').val(menu.NomMenu);
                $('#urlMenu').val(menu.UrlMenu);
                $('#iconoMenu').val(menu.IconoMenu);

                accion = "EDITAR";

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar los sistemas: ' + jqXHR.responseText);
            }
        });
    }

    const listarMenus = function () {

        $treeview.jstree(true).deselect_all(true);

        $.ajax({
            url: urlListarMenus,
            type: 'POST',
            data: JSON.stringify({ codSistema: $("#cboSistema").val() }),
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
                    menus.push({
                        id: item.CodMenu,
                        text: item.NomMenu,
                        parent: item.CodMenuPadre == "" || item.CodMenuPadre == null ? "0" : item.CodMenuPadre
                    });
                });

                $treeview.jstree(true).refresh();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al listar los sistemas: ' + jqXHR.responseText);
            }
        });
    }

    const inicializarMenus = function () {

        $treeview.jstree({
            plugins: ["noclose"],
            core: {
                'check_callback': true,
                'data': function (node, cb) {
                    cb.call(this, menus);
                }
            }
        });
    }

    const guardarMenu = function (menu) {
        $.ajax({
            url: urlCrearMenu,
            type: 'POST',
            data: JSON.stringify(menu),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {

                if (data.Ok) {
                    mensajeExito('Menu creado con éxito');
                    cerrarModal();
                    limpiarFormulario();
                    listarMenus();
                } else {
                    mensajeError('Error al crear el menu: ' + data.Mensaje);
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al guardar el menu: ' + jqXHR.responseText);
            }
        });
    }

    const limpiarFormulario = function () {
        $('#nomMenu').val('');
        $('#urlMenu').val('');
        $('#iconoMenu').val('');
    }

    const cerrarModal = function () {
        $('#crearMenulModal').modal('hide');
    }

    const editarMenu = function (menu) {
        $.ajax({
            url: urlEditarMenu,
            type: 'POST',
            data: JSON.stringify(menu),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {

                if (data.Ok) {
                    mensajeExito('Menu editado con éxito');
                    cerrarModal();
                    limpiarFormulario();
                    listarMenus();
                } else {
                    mensajeError('Error al editar el menu: ' + data.Mensaje);
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al editar el menu: ' + jqXHR.responseText);
            }
        });
    }

    const eliminarMenu = function (menu) {
        $.ajax({
            url: urlEliminarMenu,
            type: 'POST',
            data: JSON.stringify(menu),
            contentType: 'application/json',
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (data) {

                if (data.Ok) {
                    mensajeExito('Menu eliminado con éxito');
                    listarMenus();
                } else {
                    mensajeError('Error al eliminar el menu: ' + data.Mensaje);
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                mensajeError('Error al eliminar el menu: ' + jqXHR.responseText);
            }
        });
    }

    return {
        init: function () {
            checkSession(function () {
                inicializarMenus();
                eventos();
                listarSistemas();
            });
        }
    }

}(jQuery);