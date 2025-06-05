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
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.FactoresMdr
{
    public class EliminarMdrFactorIzipayCommand : IRequest<RespuestaComunDTO>
    {
        public List<MdrFactorDto> Factores { get; set; }
        public string UsuElimina { get; set; }
    }

    public class EliminarMdrFactorIzipayHandler : IRequestHandler<EliminarMdrFactorIzipayCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMdrFactorIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMdrFactorIzipayCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            var mensajesError = new List<string>();
            int contadorEliminados = 0;

            try
            {
                if (request.Factores == null || !request.Factores.Any())
                {
                    return new RespuestaComunDTO
                    {
                        Ok = false,
                        Mensaje = "No se enviaron registros para eliminar."
                    };
                }


                foreach (var factorEliminar in request.Factores)
                {
                    var factor = await _contexto.RepositorioMdrFactorIzipay.Obtener(x => x.CodEmpresa == factorEliminar.CodEmpresa
                                                                                    && x.NumAno == factorEliminar.NumAno
                                                                                    && x.CodOperador == factorEliminar.CodOperador
                                                                                    && x.CodClasificacion == factorEliminar.CodClasificacion)
                                                                        .FirstOrDefaultAsync();
                    if (factor is null)
                    {
                        mensajesError.Add($"No existe Factor.");
                        continue;
                    }

                    if (string.Equals(factor.IndActivo, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        mensajesError.Add($"El factor ya está inactivo: {factor.Operador.NomOperador}  - {factor.Clasificacion.NomClasificacion}");
                        continue;
                    }

                    factor.IndActivo = "N";
                    factor.UsuModifica = request.UsuElimina;
                    factor.FecModifica = DateTime.Now;

                    _contexto.RepositorioMdrFactorIzipay.Actualizar(factor);
                    contadorEliminados++;
                }
                await _contexto.GuardarCambiosAsync();

                if (contadorEliminados > 0)
                {
                    respuesta.Mensaje = $"Se desactivó {contadorEliminados} registro{(contadorEliminados > 1 ? "s" : "")} correctamente.";
                }
                else
                {
                    respuesta.Mensaje = "No se desactivó ningún factor.";
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
