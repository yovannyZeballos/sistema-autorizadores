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

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class EliminarSolicitudUsuarioCommand : IRequest<RespuestaComunDTO>
    {
        public int NumSolicitud { get; set; }
        public string IndAprobado { get; set; } = "N";
        public string Motivo { get; set; } = "Anulado por el usuario";
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; } = DateTime.UtcNow;
    }

    public class EliminarSolicitudUsuarioHandler : IRequestHandler<EliminarSolicitudUsuarioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarSolicitudUsuarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarSolicitudUsuarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var solicitud = await _contexto.RepositorioSolicitudUsuarioASR.Obtener(x => x.NumSolicitud == request.NumSolicitud).FirstOrDefaultAsync();
                if (solicitud is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Numero de solicitud no existe";
                    return respuesta;
                }

                if (solicitud.IndAprobado != "S")
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Solo esta permitido eliminar solicitudes con estado 'Solicitado'";
                    return respuesta;
                }

                _mapper.Map(request, solicitud);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Solicitud eliminado exitosamente.";
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
