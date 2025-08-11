using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Producto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Producto
{
    public class EliminarMaeProductoCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeProductoDto> Productos { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMaeProductoHandler : IRequestHandler<EliminarMaeProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeProductoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Productos == null || !request.Productos.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }


                foreach (var productoEliminar in request.Productos)
                {
                    var producto = await _contexto.RepositorioMaeProducto.Obtener(x => x.CodProducto == productoEliminar.CodProducto)
                                                                        .FirstOrDefaultAsync();
                    if (producto is null)
                    {
                        mensajesError.Add($"No existe marca.");
                        continue;
                    }

                    if (string.Equals(producto.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        mensajesError.Add($"Producto {producto.DesProducto} ya está inactivo");
                        continue;
                    }

                    producto.IndActivo = "N";
                    producto.UsuElimina = request.UsuElimina;
                    producto.FecElimina = DateTime.Now;

                    _contexto.RepositorioMaeProducto.Actualizar(producto);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ningun producto.";
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
                _logger.Error(ex, "Error en EliminarMaeProductoHandler");
            }
            return respuesta;
        }
    }
}
