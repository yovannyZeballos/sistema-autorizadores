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

namespace SPSA.Autorizadores.Aplicacion.Features.Zona.Queries
{
    public class ObtenerMaeZonaQuery : IRequest<GenericResponseDTO<ObtenerMaeZonaDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
    }

    public class ObtenerMaeZonaHandler : IRequestHandler<ObtenerMaeZonaQuery, GenericResponseDTO<ObtenerMaeZonaDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeZonaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeZonaDTO>> Handle(ObtenerMaeZonaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeZonaDTO> { Ok = true };
            try
            {
                var zona = await _contexto.RepositorioMaeZona.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodCadena == request.CodCadena && s.CodRegion == request.CodRegion && s.CodZona == request.CodZona).FirstOrDefaultAsync();
                if (zona is null)
                {
                    response.Ok = false;
                    response.Mensaje = "La zona no existe";
                    return response;
                }

                var zonaDto = _mapper.Map<ObtenerMaeZonaDTO>(zona);
                response.Data = zonaDto;
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
