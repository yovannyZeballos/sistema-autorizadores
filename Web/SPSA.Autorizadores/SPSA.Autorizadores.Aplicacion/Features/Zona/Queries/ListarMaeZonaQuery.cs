using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;
using System.Linq;
namespace SPSA.Autorizadores.Aplicacion.Features.Zona.Queries
{
    public class ListarMaeZonaQuery : IRequest<GenericResponseDTO<List<ListarMaeZonaDTO>>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
    }

    public class ListarMaeZonaHandler : IRequestHandler<ListarMaeZonaQuery, GenericResponseDTO<List<ListarMaeZonaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeZonaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeZonaDTO>>> Handle(ListarMaeZonaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeZonaDTO>> { Ok = true, Data = new List<ListarMaeZonaDTO>() };
            try
            {
                var regiones = await _contexto.RepositorioMaeZona.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion)
                    .OrderBy(x => x.CodZona)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeZonaDTO>>(regiones);
                response.Ok = true;
                response.Mensaje = "Se ha generado la lista correctamente";
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
