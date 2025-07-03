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
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.PeriodosMdr
{
    public class EliminarMdrPeriodoCommand : IRequest<RespuestaComunDTO>
    {
        public List<MdrPeriodoDto> Periodos { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMdrPeriodoHandler : IRequestHandler<EliminarMdrPeriodoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMdrPeriodoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMdrPeriodoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Periodos == null || !request.Periodos.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }


                foreach (var periodoEliminar in request.Periodos)
                {
                    var periodo = await _contexto.RepositorioMdrPeriodo.Obtener(x => x.CodPeriodo == periodoEliminar.CodPeriodo)
                                                                        .FirstOrDefaultAsync();
                    if (periodo is null)
                    {
                        mensajesError.Add($"No existe periodo.");
                        continue;
                    }

                    if (string.Equals(periodo.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        mensajesError.Add($"Periodo {periodo.DesPeriodo} ya está inactivo");
                        continue;
                    }

                    periodo.IndActivo = "N";
                    periodo.UsuElimina = request.UsuElimina;
                    periodo.FecElimina = DateTime.Now;

                    _contexto.RepositorioMdrPeriodo.Actualizar(periodo);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ningún periodo.";
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
                respuesta.Mensaje = "Ocurrió un error inesperado al eliminar los factores.";
                _logger.Error(ex, "Error en EliminarMdrFactorIzipayHandler");
            }
            return respuesta;
        }
    }
}
