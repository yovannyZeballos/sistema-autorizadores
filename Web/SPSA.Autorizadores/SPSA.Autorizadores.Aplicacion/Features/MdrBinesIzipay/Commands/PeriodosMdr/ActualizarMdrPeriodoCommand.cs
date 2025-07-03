using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.PeriodosMdr
{
    public class ActualizarMdrPeriodoCommand : IRequest<RespuestaComunDTO>
    {
        public long CodPeriodo { get; set; }
        public string DesPeriodo { get; set; }
        public string IndActivo { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMdrPeriodoHandler : IRequestHandler<ActualizarMdrPeriodoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMdrPeriodoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMdrPeriodoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var periodo = await _contexto.RepositorioMdrPeriodo
                    .Obtener(x =>
                        x.CodPeriodo == request.CodPeriodo)
                    .FirstOrDefaultAsync(); ;

                if (periodo is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra periodo con este ID.";
                    return respuesta;
                }

                periodo.DesPeriodo = request.DesPeriodo.ToUpper();
                periodo.UsuModifica = request.UsuModifica;
                periodo.FecModifica = DateTime.Now;
                periodo.IndActivo = request.IndActivo;

                _contexto.RepositorioMdrPeriodo.Actualizar(periodo);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Periodo actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
