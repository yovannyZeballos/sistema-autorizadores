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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.AreaGestion;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.AreaGestion
{
    public class EliminarMaeAreaGestionCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeAreaGestionDto> Areas { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMaeAreaGestionHandler : IRequestHandler<EliminarMaeAreaGestionCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeAreaGestionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeAreaGestionCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Areas == null || !request.Areas.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }

                foreach (var marcaEliminar in request.Areas)
                {
                    var area = await _contexto.RepositorioMaeAreaGestion.Obtener(x => x.Id == marcaEliminar.Id)
                                                                        .FirstOrDefaultAsync();
                    if (area is null)
                    {
                        mensajesError.Add($"No existe área gestión.");
                        continue;
                    }

                    if (string.Equals(area.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        mensajesError.Add($"Área {area.NomAreaGestion} ya está inactivo");
                        continue;
                    }

                    area.IndActivo = "N";
                    area.UsuElimina = request.UsuElimina;
                    area.FecElimina = DateTime.Now;

                    _contexto.RepositorioMaeAreaGestion.Actualizar(area);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ninguna área.";
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
                _logger.Error(ex, "Error en EliminarMaeAreaGestionHandler");
            }
            return respuesta;
        }
    }
}
