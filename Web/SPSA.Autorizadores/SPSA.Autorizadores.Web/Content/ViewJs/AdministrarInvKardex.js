var urlListarInvKardex = baseUrl + 'Inventario/InventarioKardex/ListarInvKardex';

var urlModalCrearInvKardex = baseUrl + 'Inventario/InventarioKardex/CrearFormInvKardex';
var urlModalEditarInvKardex = baseUrl + 'Inventario/InventarioKardex/EditarFormInvKardex';

var dtListaKardex = null;
var AdministrarInvKardex = function () {

    const eventos = function () {

        $("#btnNuevoInvKardex").click(function () {

            if (validarNuevoInvActivo()) {
                abrirModalNuevoInvActivo();
            }
        });

        $("#btnEditarInvKardex").click(function () {
            abrirModalEditarInvActivo();
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

            console.log("child(1)" + NUM_CAJA);
            console.log("child(2)" + COD_ACTIVO);

            //abrirModalEditarInvCaja(codEmpresa, codCadena, codRegion, codZona, codLocal, NUM_CAJA, COD_ACTIVO);
        });

    }


    const validarFormInvActivo = function (invactivo) {
        let validate = true;

        if (invactivo.CodEmpresa === '' || invactivo.CodCadena === '' || invactivo.CodRegion === '' || invactivo.CodZona === '' || invactivo.CodLocal === ''
            || invactivo.CodActivo === '' || invactivo.CodModelo === '' || invactivo.NomMarca === '' || invactivo.CodSerie === '') {
            validate = false;
            $("#formInvActivo").addClass("was-validated");
            swal({ text: 'Faltan ingresar algunos campos obligatorios', icon: "warning", });
        }

        return validate;
    }

    const validarNuevoInvActivo = function () {
        let validate = true;

        if ($("#cboEmpresa").val() === '' || $("#cboCadena").val() === '' || $("#cboRegion").val() === '' || $("#cboZona").val() === '' || $("#cboLocal").val() === '') {
            validate = false;
            swal({ text: 'Debe seleccionar la empresa, cadena, region, zona y local.', icon: "warning", });
        }

        return validate;
    }

    const abrirModalEditarInvActivo = async function (codEmpresa, codCadena, codRegion, codZona, codLocal, codActivo, codModelo, nomMarca, codSerie) {
        $("#tituloModalInvKardex").html("Editar Activo");
        $("#btnActualizarInvActivo").show();
        $("#btnGuardarInvActivo").hide();

        const response = await obtenerInvActivo(codEmpresa, codCadena, codRegion, codZona, codLocal, codActivo, codModelo, nomMarca, codSerie);
        const model = response.Data;

        if (model.FecActualiza != "" && model.FecActualiza != null) {
            let timestamp = parseInt(model.FecActualiza.match(/\d+/)[0], 10);
            let date = new Date(timestamp);
            let formattedDate = date.toISOString().split('T')[0];
            model.FecActualiza = formattedDate;
        }

        if (model.FecSalida != "" && model.FecSalida != null) {
            let timestamp = parseInt(model.FecSalida.match(/\d+/)[0], 10);
            let date = new Date(timestamp);
            let formattedDate = date.toISOString().split('T')[0];
            model.FecSalida = formattedDate;
        }

        await cargarFormEditarInvActivo(model);
    }

    const abrirModalNuevoInvActivo = async function () {
        $("#tituloModalInvKardex").html("Ingresar Kardex");
        //$("#btnActualizarInvActivo").hide();
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

    const cargarFormEditarInvActivo = async function (model) {
        $.ajax({
            url: urlModalEditarInvActivo,
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
                $("#modalInvActivo").find(".modal-body").html(response);
                $("#modalInvActivo").modal('show');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({ text: jqXHR.responseText, icon: "error" });
            }
        });
    }

    const recargarDataTableKardex = function (request) {
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
            columns: [
                { data: "Id" },
                { data: "Kardex" },
                { data: "Fecha" },
                { data: "ActivoArea" },
                { data: "ActivoTipo" },
                { data: "Guia" },
                { data: "ActivoDescripcion" },
                { data: "ActivoModelo" },
                { data: "ActivoMarca" },
                { data: "Serie" },
                { data: "Origen" },
                { data: "Destino" },
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
            scrollY: '400px',
            scrollX: true,
            scrollCollapse: true,
            paging: true,
            rowCallback: function (row, data, index) {

            },
            bAutoWidth: false
        });
    }


    return {
        init: function () {
            checkSession(async function () {
                eventos();
                //await cargarComboEmpresa();
                recargarDataTableKardex();
            });
        }
    }

}(jQuery);
