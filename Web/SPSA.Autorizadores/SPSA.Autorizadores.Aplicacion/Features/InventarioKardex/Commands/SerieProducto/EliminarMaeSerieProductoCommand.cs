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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.SerieProducto
{
    public class EliminarMaeSerieProductoCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeSerieProductoDto> Series { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMaeSerieProductoHandler : IRequestHandler<EliminarMaeSerieProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeSerieProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeSerieProductoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Series == null || !request.Series.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }


                foreach (var serieEliminar in request.Series)
                {
                    var serie = await _contexto.RepositorioMaeSerieProducto.Obtener(x => x.Id == serieEliminar.Id)
                                                                        .FirstOrDefaultAsync();
                    if (serie is null)
                    {
                        mensajesError.Add($"No existe marca.");
                        continue;
                    }

                    //if (string.Equals(serie.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    mensajesError.Add($"Serie {serie.NumSerie} ya está inactivo");
                    //    continue;
                    //}

                    //serie.IndActivo = "N";
                    //serie.UsuElimina = request.UsuElimina;
                    //serie.FecElimina = DateTime.Now;

                    _contexto.RepositorioMaeSerieProducto.Actualizar(serie);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ninguna serie.";
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
                _logger.Error(ex, "Error en EliminarMaeSerieProductoHandler");
            }
            return respuesta;
        }
    }
}
