using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using AutoMapper;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
    public class ListarMaeLocalQuery : IRequest<GenericResponseDTO<List<ListarMaeLocalDTO>>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
    }

    public class ListarMaeLocalHandler : IRequestHandler<ListarMaeLocalQuery, GenericResponseDTO<List<ListarMaeLocalDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeLocalDTO>>> Handle(ListarMaeLocalQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeLocalDTO>> { Ok = true, Data = new List<ListarMaeLocalDTO>() };
            try
            {
                var locales = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona).ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeLocalDTO>>(locales);
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
