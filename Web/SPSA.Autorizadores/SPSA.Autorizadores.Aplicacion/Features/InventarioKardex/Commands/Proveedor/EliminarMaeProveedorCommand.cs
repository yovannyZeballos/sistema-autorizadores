using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Proveedor;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Proveedor
{
    public class EliminarMaeProveedorCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeProveedorDto> Proveedores { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMaeProveedorHandler : IRequestHandler<EliminarMaeProveedorCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeProveedorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeProveedorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Proveedores == null || !request.Proveedores.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }


                foreach (var marcaEliminar in request.Proveedores)
                {
                    var proveedor = await _contexto.RepositorioMaeProveedor.Obtener(x => x.Ruc == marcaEliminar.Ruc)
                                                                        .FirstOrDefaultAsync();
                    if (proveedor is null)
                    {
                        mensajesError.Add($"No existe proveedor.");
                        continue;
                    }

                    if (string.Equals(proveedor.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        mensajesError.Add($"{proveedor.RazonSocial} ya está inactivo");
                        continue;
                    }

                    proveedor.IndActivo = "N";
                    proveedor.UsuElimina = request.UsuElimina;
                    proveedor.FecElimina = DateTime.Now;

                    _contexto.RepositorioMaeProveedor.Actualizar(proveedor);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ningun proveedor.";
                }

                if (mensajesError.Any())
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje += " Errores:\n" + string.Join("\n", mensajesError);
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error inesperado al eliminar registros.";
                _logger.Error(ex, "Error en EliminarMaeProveedorHandler");
            }
            return respuesta;
        }
    }
}
