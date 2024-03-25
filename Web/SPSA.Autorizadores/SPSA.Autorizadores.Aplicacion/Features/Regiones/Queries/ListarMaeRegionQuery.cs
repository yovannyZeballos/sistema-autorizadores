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

namespace SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries
{
    public class ListarMaeRegionQuery : IRequest<GenericResponseDTO<List<ListarMaeRegionDTO>>>
    {
		public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
    }

    public class ListarMaeRegionHandler : IRequestHandler<ListarMaeRegionQuery, GenericResponseDTO<List<ListarMaeRegionDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeRegionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeRegionDTO>>> Handle(ListarMaeRegionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeRegionDTO>> { Ok = true, Data = new List<ListarMaeRegionDTO>() };

            try
            {
                var regiones = await _contexto.RepositorioMaeRegion.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena)
                    .OrderBy(x => x.CodRegion)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeRegionDTO>>(regiones);
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
