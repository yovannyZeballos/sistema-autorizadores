using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries
{
    public class ObtenerMaeRegionQuery : IRequest<GenericResponseDTO<ObtenerMaeRegionDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
    }
    public class ObtenerMaeRegionHandler : IRequestHandler<ObtenerMaeRegionQuery, GenericResponseDTO<ObtenerMaeRegionDTO>>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeRegionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeRegionDTO>> Handle(ObtenerMaeRegionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeRegionDTO> { Ok = true };
            try
            {
                var region = await _contexto.RepositorioMaeRegion.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodCadena == request.CodCadena && s.CodRegion == request.CodRegion).FirstOrDefaultAsync();
                if (region is null)
                {
                    response.Ok = false;
                    response.Mensaje = "La region no existe";
                    return response;
                }

                var regionDto = _mapper.Map<ObtenerMaeRegionDTO>(region);
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
