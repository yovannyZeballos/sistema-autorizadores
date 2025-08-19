using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.SerieProducto
{
    public class ListarPaginadoMaeSerieProductoQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeSerieProductoDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string CodProducto { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarPaginadoMaeSerieProductoHandler : IRequestHandler<ListarPaginadoMaeSerieProductoQuery, GenericResponseDTO<PagedResult<ListarMaeSerieProductoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMaeSerieProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeSerieProductoDto>>> Handle(ListarPaginadoMaeSerieProductoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeSerieProductoDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeSerieProductoDto>()
            };

            try
            {
                // Rechaza scan si no envían producto
                if (string.IsNullOrWhiteSpace(request.CodProducto))
                {
                    response.Data = new PagedResult<ListarMaeSerieProductoDto>
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0,
                        Items = new List<ListarMaeSerieProductoDto>()
                    };
                    return response;
                }

                Expression<Func<Mae_SerieProducto, bool>> predicate = x => x.CodProducto == request.CodProducto;

                var pagedRegistros = await _contexto.RepositorioMaeSerieProducto.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.NumSerie },
                    ascending: true
                );

                var dtoItems = new List<ListarMaeSerieProductoDto>(pagedRegistros.Items.Count);
                foreach (var k in pagedRegistros.Items)
                {
                    var nomLocalActual = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == k.CodEmpresa && x.CodLocal == k.CodLocal)
                                                                            .Select(x => x.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                    //var nomLocalDes = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == k.CodEmpresaDestino && x.CodLocal == k.CodLocalDestino)
                    //                                                        .Select(x => x.NomLocal)
                    //                                                        .FirstOrDefaultAsync() ?? string.Empty;

                    dtoItems.Add(new ListarMaeSerieProductoDto
                    {
                        CodProducto = k.CodProducto,
                        NumSerie = k.NumSerie,
                        IndEstado = k.IndEstado,
                        StkActual = k.StkActual,
                        LocalActual = nomLocalActual,
                        FecIngreso = k.FecIngreso,
                        FecSalida = k.FecSalida,
                        UsuCreacion = k.UsuCreacion,
                        FecCreacion = k.FecCreacion
                    });
                }




                var pagedResult = new PagedResult<ListarMaeSerieProductoDto>
                {
                    PageNumber = pagedRegistros.PageNumber,
                    PageSize = pagedRegistros.PageSize,
                    TotalRecords = pagedRegistros.TotalRecords,
                    TotalPages = pagedRegistros.TotalPages,
                    Items = dtoItems
                };

                response.Data = pagedResult;
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
