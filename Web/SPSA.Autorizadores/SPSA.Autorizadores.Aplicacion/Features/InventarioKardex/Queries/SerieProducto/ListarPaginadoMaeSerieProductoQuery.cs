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


                ParameterExpression param = Expression.Parameter(typeof(Mae_SerieProducto), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrWhiteSpace(request.CodProducto))
                {
                    var prop = Expression.Property(param, nameof(Mae_SerieProducto.CodProducto));
                    var cte = Expression.Constant(request.CodProducto);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.FiltroVarios))
                {
                    var filtroConst = Expression.Constant(request.FiltroVarios.ToUpper());
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var numSerieContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_SerieProducto.NumSerie)), contains, filtroConst);

                    var indEstadoContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_SerieProducto.IndEstado)), contains, filtroConst);

                    var orExpr = Expression.OrElse(numSerieContains, indEstadoContains);
                    combined = Expression.AndAlso(combined, orExpr);
                }

                Expression<Func<Mae_SerieProducto, bool>> predicate = Expression.Lambda<Func<Mae_SerieProducto, bool>>(combined, param);

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
