using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Colaborador.Queries
{
    public class ObtenerMaeColaboradorQuery : IRequest<GenericResponseDTO<ObtenerMaeColaboradorDTO>>
    {
        public string CoEmpr { get; set; }
        public string CodigoOfisis { get; set; }
    }
    public class ObtenerMaeColaboradorHandler : IRequestHandler<ObtenerMaeColaboradorQuery, GenericResponseDTO<ObtenerMaeColaboradorDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeColaboradorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeColaboradorDTO>> Handle(ObtenerMaeColaboradorQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeColaboradorDTO> { Ok = true };
            try
            {
                var colaborador = await _contexto.RepositorioMaeColaborador.Obtener(s => s.CoEmpr == request.CoEmpr && s.CodigoOfisis == request.CodigoOfisis).FirstOrDefaultAsync();
                if (colaborador is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Colaborador no existe";
                    return response;
                }

                var regionDto = _mapper.Map<ObtenerMaeColaboradorDTO>(colaborador);
                response.Data = regionDto;
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
