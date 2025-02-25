using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Queries
{
    public class ObtenerMaeColaboradorExtQuery : IRequest<GenericResponseDTO<ObtenerMaeColaboradorExtDTO>>
    {
        public string CodLocalAlterno { get; set; }
        public string CodigoOfisis { get; set; }
    }

    public class ObtenerMaeColaboradorExtHandler : IRequestHandler<ObtenerMaeColaboradorExtQuery, GenericResponseDTO<ObtenerMaeColaboradorExtDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeColaboradorExtDTO>> Handle(ObtenerMaeColaboradorExtQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeColaboradorExtDTO> { Ok = true };
            try
            {
                Mae_ColaboradorExt maeColaboradorExt = new Mae_ColaboradorExt();

                maeColaboradorExt = await _contexto.RepositorioMaeColaboradorExt.Obtener(s => s.CodLocalAlterno == request.CodLocalAlterno && s.CodigoOfisis == request.CodigoOfisis).FirstOrDefaultAsync();

                if (maeColaboradorExt is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Colaborador externo no existe";
                    return response;
                }

                var maeColaboradorExtDto = _mapper.Map<ObtenerMaeColaboradorExtDTO>(maeColaboradorExt);

                Mae_Local maeLocal = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodLocalAlterno == request.CodLocalAlterno).FirstOrDefaultAsync();
                maeColaboradorExtDto.CodEmpresa = maeLocal.CodEmpresa;
                maeColaboradorExtDto.CodLocalAlterno = maeLocal.CodLocalAlterno;

                response.Data = maeColaboradorExtDto;
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }
            return response;
        }
    }
}
