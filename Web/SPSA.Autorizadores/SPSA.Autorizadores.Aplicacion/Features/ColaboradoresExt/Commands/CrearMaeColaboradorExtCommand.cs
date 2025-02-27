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

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands
{
    public class CrearMaeColaboradorExtCommand : IRequest<RespuestaComunDTO>
    {
        public string CodLocalAlterno { get; set; }
        public string CodigoOfisis { get; set; }
        public string ApelPaterno { get; set; }
        public string ApelMaterno { get; set; }
        public string NombreTrabajador { get; set; }
        public string TipoDocIdent { get; set; }
        public string NumDocIndent { get; set; }
        public DateTime FechaIngresoEmpresa { get; set; }
        public DateTime FechaCeseTrabajador { get; set; }
        public string TiSitu { get; set; }
        public string PuestoTrabajo { get; set; }
        public string MotiSepa { get; set; }
        public string IndPersonal { get; set; }
        public string TipoUsuario { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; } = DateTime.UtcNow;
    }

    public class CrearMaeColaboradorExtHandler : IRequestHandler<CrearMaeColaboradorExtCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                //bool existe = await _contexto.RepositorioMaeColaboradorExt.Existe(x => x.CodLocalAlterno == request.CodLocalAlterno && x.CodigoOfisis == request.CodigoOfisis);
                //if (existe)
                //{
                //    respuesta.Ok = false;
                //    respuesta.Mensaje = "Colaborador externo ya existe";
                //    return respuesta;
                //}

                var maeColaboradorExt = _mapper.Map<Mae_ColaboradorExt>(request);
                _contexto.RepositorioMaeColaboradorExt.Agregar(maeColaboradorExt);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Colaborador externo creado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;

                if (ContainsConstraintViolation(ex, "idx_unique_nudoc_externals"))
                {
                    respuesta.Mensaje = "El número de documento ya se encuentra en uso.";
                }
                else
                {
                    respuesta.Mensaje = "Ocurrió un error al crear colaborador externo.";
                }

                _logger.Error(ex, "Ocurrió un error al crear colaborador externo");
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
