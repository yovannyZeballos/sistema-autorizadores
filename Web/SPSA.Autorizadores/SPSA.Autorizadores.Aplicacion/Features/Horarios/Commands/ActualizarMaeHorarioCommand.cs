using System;
using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands
{
    public class ActualizarMaeHorarioCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumDia { get; set; }
        public string CodDia { get; set; }
        public string HorOpen { get; set; }
        public string HorClose { get; set; }
        public string MinLmt { get; set; }
        public string UsuModifica { get; set; }
        public DateTime FecModifica { get; set; }
    }

    public class ActualizarMaeHorarioHandler : IRequestHandler<ActualizarMaeHorarioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeHorarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeHorarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var horario = await _contexto.RepositorioMaeHorario.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion
                                                                        && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal && x.NumDia == request.NumDia).FirstOrDefaultAsync();
                if (horario is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Horario no existe";
                    return respuesta;
                }

                _mapper.Map(request, horario);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Horario actualizado exitosamente.";
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
