using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.Queries
{
    public class ObtenerMaeColaboradorIntQuery : IRequest<GenericResponseDTO<ObtenerMaeColaboradorIntDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodigoOfisis { get; set; }
    }

    public class ObtenerMaeColaboradorIntHandler : IRequestHandler<ObtenerMaeColaboradorIntQuery, GenericResponseDTO<ObtenerMaeColaboradorIntDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeColaboradorIntHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeColaboradorIntDTO>> Handle(ObtenerMaeColaboradorIntQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeColaboradorIntDTO> { Ok = true };
            try
            {
                Mae_ColaboradorInt maeColaboradorInt = new Mae_ColaboradorInt();

                maeColaboradorInt = await _contexto.RepositorioMaeColaboradorInt.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodLocal == request.CodLocal && s.CodigoOfisis == request.CodigoOfisis).FirstOrDefaultAsync();

                if (maeColaboradorInt is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Colaborador interno no existe";
                    return response;
                }

                var maeColaboradorIntDto = _mapper.Map<ObtenerMaeColaboradorIntDTO>(maeColaboradorInt);

                //Mae_Local maeLocal = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodLocalAlterno == request.CodLocalAlterno).FirstOrDefaultAsync();
                //maeColaboradorIntDto.CodEmpresa = maeLocal.CodEmpresa;
                //maeColaboradorIntDto.CodLocalAlterno = maeLocal.CodLocalAlterno;

                response.Data = maeColaboradorIntDto;
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
