using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.SerieProducto
{
    public class ListarMaeSeriesPorProductoDisponiblesQuery : IRequest<GenericResponseDTO<List<ListarMaeSerieProductoDto>>>
    {
        public string CodProducto { get; set; }
        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
    }

    public class ListarMaeSeriesPorProductoDisponiblesHandler : IRequestHandler<ListarMaeSeriesPorProductoDisponiblesQuery, GenericResponseDTO<List<ListarMaeSerieProductoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeSeriesPorProductoDisponiblesHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeSerieProductoDto>>> Handle(ListarMaeSeriesPorProductoDisponiblesQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeSerieProductoDto>>
            {
                Ok = true,
                Data = new List<ListarMaeSerieProductoDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMaeSerieProducto
                    .Obtener(x => x.CodEmpresa == request.CodEmpresaOrigen && x.CodLocal == request.CodLocalOrigen && x.CodProducto == request.CodProducto && x.IndEstado == "DISPONIBLE")
                    .OrderBy(x => x.NumSerie)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMaeSerieProductoDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de registros obtenido correctamente.";
            }
            catch (System.Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }

            return response;
        }
    }
}
