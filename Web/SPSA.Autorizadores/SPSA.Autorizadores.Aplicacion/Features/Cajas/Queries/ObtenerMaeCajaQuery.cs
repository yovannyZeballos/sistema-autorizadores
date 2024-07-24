using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Queries
{
    public class ObtenerMaeCajaQuery : IRequest<GenericResponseDTO<ObtenerMaeCajaDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumCaja { get; set; }
    }
    public class ObtenerMaeCajaHandler : IRequestHandler<ObtenerMaeCajaQuery, GenericResponseDTO<ObtenerMaeCajaDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeCajaDTO>> Handle(ObtenerMaeCajaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeCajaDTO> { Ok = true };
            try
            {
                var caja = await _contexto.RepositorioMaeCaja.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodCadena == request.CodCadena && s.CodRegion == request.CodRegion && s.CodZona == request.CodZona && s.CodLocal == request.CodLocal && s.NumCaja == request.NumCaja).FirstOrDefaultAsync();
                if (caja is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Local no existe";
                    return response;
                }

                var cajaDto = _mapper.Map<ObtenerMaeCajaDTO>(caja);
                response.Data = cajaDto;
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
