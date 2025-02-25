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

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands
{
    public class EliminarMaeColaboradorExtCommand : IRequest<RespuestaComunDTO>
    {
        public string CodLocalAlterno { get; set; }
        public string CodigoOfisis { get; set; }
        public string TipoDocIdent { get; set; }
        public string NumDocIndent { get; set; }
    }
    public class EliminarMaeColaboradorExtHandler : IRequestHandler<EliminarMaeColaboradorExtCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var existeColaboradorExt = await _contexto.RepositorioMaeColaboradorExt.Existe(x => x.CodLocalAlterno == request.CodLocalAlterno && x.CodigoOfisis == request.CodigoOfisis);
                if (!existeColaboradorExt)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Colaborador externo no existe";
                    return respuesta;
                }

                var invColaboradorTemp = await _contexto.RepositorioMaeColaboradorExt.Obtener(x => x.CodLocalAlterno == request.CodLocalAlterno && x.CodigoOfisis == request.CodigoOfisis).FirstOrDefaultAsync();
                _contexto.RepositorioMaeColaboradorExt.Eliminar(invColaboradorTemp);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Colaborador externo elimindo exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al eliminar el colaborador externo";
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
