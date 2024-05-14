using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Queries;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
    public class ObtenerMaeLocalQuery : IRequest<GenericResponseDTO<ObtenerMaeLocalDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
    }

    public class ObtenerMaeLocalHandler : IRequestHandler<ObtenerMaeLocalQuery, GenericResponseDTO<ObtenerMaeLocalDTO>>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeLocalDTO>> Handle(ObtenerMaeLocalQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeLocalDTO> { Ok = true };
            try
            {
                Mae_Local local = new Mae_Local();
                if (request.CodCadena == null)
                {
                    local = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodLocal == request.CodLocal).FirstOrDefaultAsync();
                }
                else
                {
                    local = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodCadena == request.CodCadena && s.CodRegion == request.CodRegion && s.CodZona == request.CodZona && s.CodLocal == request.CodLocal).FirstOrDefaultAsync();
                }

                if (local is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Local no existe";
                    return response;
                }

                var localDto = _mapper.Map<ObtenerMaeLocalDTO>(local);
                response.Data = localDto;
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
