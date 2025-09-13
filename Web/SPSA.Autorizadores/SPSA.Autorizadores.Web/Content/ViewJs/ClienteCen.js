var urlConsultaCliente = baseUrl + 'Operaciones/Gestion/ConsultarClienteCen';
var urlInsertarCliente = baseUrl + 'Operaciones/Gestion/InsertarClienteCen';


var ClienteCen = function () {

    const eventos = function () {
        $("#cboTipoDocumento").on('change', function () {
            validarCamposBusqueda();
            validarNumeroDocumento();
        });

        $("#txtNroDocumento").on('blur', function () {
            validarNumeroDocumento();
        });

        $("#txtNroDocumento").on('input', function () {
            if (!$('#lblErrorNumeroDocumento').hasClass('d-none')) {
                validarNumeroDocumento();
            }
        });


        $("#btnBuscar").on('click', function () {
            if (!validarCamposBusqueda() || !validarNumeroDocumento()) {
                swal({
                    text: "Por favor, ingrese los datos de busqueda correctamente",
                    icon: "warning"
                });
                return;
            }
            consultaCliente();
        });

        // Eventos del formulario de registro
        $("#cboTipoDocumentoRegistro").on('change', function () {
            mostrarCamposSegunTipoDocumento();
            validarFormularioRegistro();
            validarNumeroDocumentoRegistro();
        });

        $("#btnGuardarCliente").on('click', function () {
            if (validarFormularioRegistro() & validarNumeroDocumentoRegistro()) {
                insertarCliente();
            } else {
                swal({
                    text: "Por favor, complete todos los campos requeridos",
                    icon: "warning"
                });
            }
        });

        $("#btnCancelarRegistro").on('click', function () {
            ocultarFormularioRegistro();
        });

        // Validaciones en tiempo real para el formulario de registro
        $("#txtNombresRegistro, #txtApellidosRegistro, #txtRazonSocialRegistro").on('blur', function () {
            validarFormularioRegistro();
        });

    };

    const consultaCliente = function () {
        limpiarTabla();
        ocultarFormularioRegistro();

        const request = {
            TipoDocumento: $('#cboTipoDocumento').val(),
            NumeroDocumento: $('#txtNroDocumento').val()
        }

        $.ajax({
            url: urlConsultaCliente,
            type: "post",
            dataType: "json",
            data: { request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (!response.Ok) {
                    $('#mensajeBusqueda').removeClass('d-none').text(response.Mensaje)
                    mostrarFormularioRegistro();
                    return;
                }

                let cliente = response.Data;

                // Ocultar mensaje de error y mostrar tabla
                $('#mensajeBusqueda').addClass('d-none');
                mostrarClienteEnTabla(cliente);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText,
                    icon: "error",
                });
                limpiarTabla();
                ocultarFormularioRegistro();
            }
        });
    }

    const insertarCliente = function () {
        const tipoDocumento = $('#cboTipoDocumentoRegistro').val();

        const request = {
            TipoDocumento: tipoDocumento,
            NumeroDocumento: $('#txtNroDocumentoRegistro').val(),
            Nombres: tipoDocumento === 'DNI' ? $('#txtNombresRegistro').val() : '',
            Apellidos: tipoDocumento === 'DNI' ? $('#txtApellidosRegistro').val() : '',
            RazonSocial: tipoDocumento === 'RUC' ? $('#txtRazonSocialRegistro').val() : ''
        };

        $.ajax({
            url: urlInsertarCliente,
            type: "post",
            dataType: "json",
            data: { request },
            beforeSend: function () {
                showLoading();
            },
            complete: function () {
                closeLoading();
            },
            success: function (response) {
                if (response.Ok) {
                    swal({
                        text: "Cliente registrado exitosamente",
                        icon: "success"
                    }).then(() => {
                        ocultarFormularioRegistro();
                        //consultaCliente();
                    });
                } else {
                    swal({
                        text: response.Mensaje || "Error al registrar el cliente",
                        icon: "error"
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal({
                    text: jqXHR.responseText || "Error al registrar el cliente",
                    icon: "error"
                });
            }
        });
    }

    const mostrarFormularioRegistro = function () {
        // Setear valores desde la búsqueda
        $('#cboTipoDocumentoRegistro').val($('#cboTipoDocumento').val()).trigger('change');
        $('#txtNroDocumentoRegistro').val($('#txtNroDocumento').val());

        // Mostrar campos según tipo de documento
        mostrarCamposSegunTipoDocumento();

        // Mostrar formulario
        $('#contenedorFormularioRegistro').removeClass('d-none');
    }

    const ocultarFormularioRegistro = function () {
        $('#mensajeBusqueda').addClass('d-none');
        limpiarFormularioRegistro();
        $('#contenedorFormularioRegistro').addClass('d-none');
    }

    const mostrarCamposSegunTipoDocumento = function () {
        const tipoDocumento = $('#cboTipoDocumentoRegistro').val();

        if (tipoDocumento === 'DNI') {
            $('#camposPersonaNatural').removeClass('d-none');
            $('#camposPersonaJuridica').addClass('d-none');
            // Limpiar campos de persona jurídica
            $('#txtRazonSocialRegistro').val('');
        } else if (tipoDocumento === 'RUC') {
            $('#camposPersonaNatural').addClass('d-none');
            $('#camposPersonaJuridica').removeClass('d-none');
            // Limpiar campos de persona natural
            $('#txtNombresRegistro').val('');
            $('#txtApellidosRegistro').val('');
        } else {
            $('#camposPersonaNatural').addClass('d-none');
            $('#camposPersonaJuridica').addClass('d-none');
        }

        // Limpiar mensajes de error
        limpiarErroresFormularioRegistro();
    }

    const validarFormularioRegistro = function () {
        let esValido = true;
        const tipoDocumento = $('#cboTipoDocumentoRegistro').val().trim();
        const numeroDocumento = $('#txtNroDocumentoRegistro').val().trim();

        // Validar tipo de documento
        if (tipoDocumento === '') {
            $('#lblErrorTipoDocumentoRegistro').removeClass('d-none').text('Debe seleccionar el tipo de documento');
            esValido = false;
        } else {
            $('#lblErrorTipoDocumentoRegistro').addClass('d-none');
        }

        // Validar número de documento
        if (numeroDocumento === '') {
            $('#lblErrorNumeroDocumentoRegistro').removeClass('d-none').text('Debe ingresar el número de documento');
            esValido = false;
        } else {
            $('#lblErrorNumeroDocumentoRegistro').addClass('d-none');
        }

        // Validar campos específicos según tipo de documento
        if (tipoDocumento === 'DNI') {
            const nombres = $('#txtNombresRegistro').val().trim();
            const apellidos = $('#txtApellidosRegistro').val().trim();

            if (nombres === '') {
                $('#lblErrorNombresRegistro').removeClass('d-none').text('Debe ingresar los nombres');
                esValido = false;
            } else {
                $('#lblErrorNombresRegistro').addClass('d-none');
            }

            if (apellidos === '') {
                $('#lblErrorApellidosRegistro').removeClass('d-none').text('Debe ingresar los apellidos');
                esValido = false;
            } else {
                $('#lblErrorApellidosRegistro').addClass('d-none');
            }
        } else if (tipoDocumento === 'RUC') {
            const razonSocial = $('#txtRazonSocialRegistro').val().trim();

            if (razonSocial === '') {
                $('#lblErrorRazonSocialRegistro').removeClass('d-none').text('Debe ingresar la razón social');
                esValido = false;
            } else {
                $('#lblErrorRazonSocialRegistro').addClass('d-none');
            }
        }

        return esValido;
    }

    const limpiarFormularioRegistro = function () {
        $('#cboTipoDocumentoRegistro').val('');
        $('#txtNroDocumentoRegistro').val('');
        $('#txtNombresRegistro').val('');
        $('#txtApellidosRegistro').val('');
        $('#txtRazonSocialRegistro').val('');

        $('#formRegistroCliente')[0].reset();

        $('#camposPersonaNatural').addClass('d-none');
        $('#camposPersonaJuridica').addClass('d-none');

        limpiarErroresFormularioRegistro();
    }

    const limpiarErroresFormularioRegistro = function () {
        $('#lblErrorTipoDocumentoRegistro').addClass('d-none');
        $('#lblErrorNumeroDocumentoRegistro').addClass('d-none');
        $('#lblErrorNombresRegistro').addClass('d-none');
        $('#lblErrorApellidosRegistro').addClass('d-none');
        $('#lblErrorRazonSocialRegistro').addClass('d-none');
    }



    const mostrarClienteEnTabla = function (cliente) {
        const tbody = $('#tablaClienteBody');

        // Limpiar tabla antes de agregar nuevo contenido
        tbody.empty();

        // Crear fila con los datos del cliente
        const fila = `
            <tr>
                <td>${cliente.TipoDocumento || ''}</td>
                <td>${cliente.NumeroDocumento || ''}</td>
                <td>${cliente.Nombres || ''}</td>
                <td>${cliente.Apellidos || ''}</td>
                <td>${cliente.RazonSocial || ''}</td>
            </tr>
        `;

        tbody.append(fila);

        // Mostrar la tabla
        $('#contenedorTablaCliente').removeClass('d-none');
    }

    const validarNumeroDocumento = function() {
        const tipoDocumento = $('#cboTipoDocumento').val().trim();
        const numeroDocumento = $('#txtNroDocumento').val().trim();

        const esValido = (tipoDocumento === 'DNI' && /^\d{8}$/.test(numeroDocumento)) ||
            (tipoDocumento === 'RUC' && /^\d{11}$/.test(numeroDocumento));

        if (numeroDocumento && !esValido) {
            $('#lblErrorNumeroDocumento').removeClass('d-none').text('Verifique longitud: DNI=8, RUC=11.');
        } else {
            $('#lblErrorNumeroDocumento').addClass('d-none');
        }
        return esValido;
    }

    const validarNumeroDocumentoRegistro = function () {
        const tipoDocumento = $('#cboTipoDocumentoRegistro').val().trim();
        const numeroDocumento = $('#txtNroDocumentoRegistro').val().trim();

        const esValido = (tipoDocumento === 'DNI' && /^\d{8}$/.test(numeroDocumento)) ||
            (tipoDocumento === 'RUC' && /^\d{11}$/.test(numeroDocumento));

        if (numeroDocumento && !esValido) {
            $('#lblErrorNumeroDocumentoRegistro').removeClass('d-none').text('Verifique longitud: DNI=8, RUC=11.');
        } else {
            $('#lblErrorNumeroDocumentoRegistro').addClass('d-none');
        }
        return esValido;
    }

    const validarCamposBusqueda = function () {
        const tipoDocumento = $('#cboTipoDocumento').val().trim();
        const numeroDocumento = $('#txtNroDocumento').val().trim();
        const esValido = tipoDocumento !== '' && numeroDocumento !== '';

        console.log({ tipoDocumento , numeroDocumento, esValido})

        if (tipoDocumento === '') {
            $('#lblErrorTipoDocumento').removeClass('d-none').text('Debe ingresar el tipo de documento');
        } else {
            $('#lblErrorTipoDocumento').addClass('d-none');
        }

        if (numeroDocumento === '') {
            $('#lblErrorNumeroDocumento').removeClass('d-none').text('Debe ingresar el numero de documento');
        } else {
            $('#lblErrorNumeroDocumento').addClass('d-none');
        }

        return esValido;
    }

    const limpiarTabla = function () {
        $('#tablaClienteBody').empty();
        $('#contenedorTablaCliente').addClass('d-none');
    }

    // Función global para limpiar formulario (llamada desde el botón Limpiar)
    window.resetForm = function () {
        $('#cboTipoDocumento').val('');
        $('#txtNroDocumento').val('');
        $('#lblErrorTipoDocumento').addClass('d-none');
        $('#lblErrorNumeroDocumento').addClass('d-none');
        $('#mensajeBusqueda').addClass('d-none');
        limpiarTabla();
        ocultarFormularioRegistro();
    }

    return {
        init: function () {
            eventos();
        }
    }
}(jQuery);