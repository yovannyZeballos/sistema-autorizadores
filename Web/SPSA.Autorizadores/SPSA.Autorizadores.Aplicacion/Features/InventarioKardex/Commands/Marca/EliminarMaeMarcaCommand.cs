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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Marca;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Marca
{
    public class EliminarMaeMarcaCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeMarcaDto> Marcas { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMaeMarcaHandler : IRequestHandler<EliminarMaeMarcaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeMarcaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeMarcaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Marcas == null || !request.Marcas.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }


                foreach (var marcaEliminar in request.Marcas)
                {
                    var marca = await _contexto.RepositorioMaeMarca.Obtener(x => x.Id == marcaEliminar.Id)
                                                                        .FirstOrDefaultAsync();
                    if (marca is null)
                    {
                        mensajesError.Add($"No existe marca.");
                        continue;
                    }

                    if (string.Equals(marca.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        mensajesError.Add($"Marca {marca.NomMarca} ya está inactivo");
                        continue;
                    }

                    marca.IndActivo = "N";
                    marca.UsuElimina = request.UsuElimina;
                    marca.FecElimina = DateTime.Now;

                    _contexto.RepositorioMaeMarca.Actualizar(marca);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ninguna marca.";
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
                _logger.Error(ex, "Error en EliminarMaeMarcaHandler");
            }
            return respuesta;
        }
    }
}
