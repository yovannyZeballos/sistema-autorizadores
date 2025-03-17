using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class CrearSolicitudUsuarioCommand : IRequest<RespuestaComunDTO>
    {
        public string CodLocalAlterno { get; set; }
        public string CodColaborador { get; set; }
        public string TipUsuario { get; set; }
        public string TipColaborador { get; set; }
        public string UsuSolicita { get; set; }
        public DateTime FecSolicita { get; set; } = DateTime.UtcNow;
        public string TipAccion { get; set; } = "C";
        public string IndAprobado { get; set; } = "S";
        public string Motivo { get; set; }
    }

    public class CrearSolicitudUsuarioHandler : IRequestHandler<CrearSolicitudUsuarioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearSolicitudUsuarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSolicitudUsuarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioSolicitudUsuarioASR.Existe(x => x.CodColaborador == request.CodColaborador && x.IndAprobado == "S");
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Ya existe una solicitud abierta de este usuario.";
                    return respuesta;
                }

                var solicitudUsuario = _mapper.Map<ASR_SolicitudUsuario>(request);
                _contexto.RepositorioSolicitudUsuarioASR.Agregar(solicitudUsuario);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Solicitud creado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;

                //if (ContainsConstraintViolation(ex, "idx_unique_nudoc_externals"))
                //{
                //    respuesta.Mensaje = "El número de documento ya se encuentra en uso.";
                //}
                //else
                //{
                //    respuesta.Mensaje = "Ocurrió un error al crear colaborador externo.";
                //}
                respuesta.Mensaje = "Ocurrió un error al crear solicitud de usuario.";
                _logger.Error(ex, "Ocurrió un error al crear solicitud de usuario");
            }
            return respuesta;
        }

        private bool ContainsConstraintViolation(Exception ex, string constraintName)
        {
            while (ex != null)
            {
                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Contains(constraintName))
                {
                    return true;
                }
                ex = ex.InnerException;
            }
            return false;
        }

    }
}
