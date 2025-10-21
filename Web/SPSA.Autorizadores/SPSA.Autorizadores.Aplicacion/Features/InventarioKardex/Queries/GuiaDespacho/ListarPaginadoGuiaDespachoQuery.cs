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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaDespacho
{
    public class ListarPaginadoGuiaDespachoQuery : IRequest<GenericResponseDTO<PagedResult<GuiaDespachoCabeceraDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string IndEstado { get; set; }
        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarPaginadoGuiaDespachoHandler : IRequestHandler<ListarPaginadoGuiaDespachoQuery, GenericResponseDTO<PagedResult<GuiaDespachoCabeceraDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoGuiaDespachoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<GuiaDespachoCabeceraDto>>> Handle(ListarPaginadoGuiaDespachoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<GuiaDespachoCabeceraDto>>
            {
                Ok = true,
                Data = new PagedResult<GuiaDespachoCabeceraDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(GuiaDespachoCabecera), "x");
                Expression combined = Expression.Constant(true);

                DateTime? desde = request.FechaDesde?.Date;
                DateTime? hasta = request.FechaHasta?.Date;

                if (desde.HasValue && hasta.HasValue && hasta < desde)
                {
                    var tmp = desde; desde = hasta; hasta = tmp;
                }

                if (desde.HasValue)
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.Fecha));
                    var cte = Expression.Constant(desde.Value, typeof(DateTime));
                    combined = Expression.AndAlso(combined, Expression.GreaterThanOrEqual(prop, cte));
                }

                if (hasta.HasValue)
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.Fecha));
                    var cte = Expression.Constant(hasta.Value, typeof(DateTime));
                    combined = Expression.AndAlso(combined, Expression.LessThanOrEqual(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.IndEstado))
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.IndEstado));
                    var cte = Expression.Constant(request.IndEstado);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodEmpresaOrigen))
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.CodEmpresaOrigen));
                    var cte = Expression.Constant(request.CodEmpresaOrigen);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodLocalOrigen))
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.CodLocalOrigen));
                    var cte = Expression.Constant(request.CodLocalOrigen);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodEmpresaDestino))
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.CodEmpresaDestino));
                    var cte = Expression.Constant(request.CodEmpresaDestino);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodLocalDestino))
                {
                    var prop = Expression.Property(param, nameof(GuiaDespachoCabecera.CodLocalDestino));
                    var cte = Expression.Constant(request.CodLocalDestino);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                // Filtro varios: NumGuia / Observaciones
                if (!string.IsNullOrWhiteSpace(request.FiltroVarios))
                {
                    var filtroConst = Expression.Constant(request.FiltroVarios);
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var numGuiaContains = Expression.Call(
                        Expression.Property(param, nameof(GuiaDespachoCabeceraDto.NumGuia)), contains, filtroConst);

                    var obsContains = Expression.Call(
                        Expression.Property(param, nameof(GuiaDespachoCabeceraDto.Observaciones)), contains, filtroConst);

                    var orExpr = Expression.OrElse(numGuiaContains, obsContains);
                    combined = Expression.AndAlso(combined, orExpr);
                }

                Expression<Func<GuiaDespachoCabecera, bool>> predicate = Expression.Lambda<Func<GuiaDespachoCabecera, bool>>(combined, param);

                // Paginación (orden: los más recientes primero por Id)
                var pagedRegistros = await _contexto.RepositorioGuiaDespachoCabecera.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.Id },
                    ascending: false,
                    includes: c => c.Detalles
                );

                // Mapear DTOs
                var dtoItems = new List<GuiaDespachoCabeceraDto>(pagedRegistros.Items.Count);
                foreach (var g in pagedRegistros.Items)
                {
                    var detalleDtos = (g.Detalles != null ? g.Detalles : Enumerable.Empty<GuiaDespachoDetalle>())
                        .Select(d => new GuiaDespachoDetalleDto
                        {
                            Id = d.Id,
                            GuiaDespachoId = d.GuiaDespachoId,
                            CodProducto = d.CodProducto,
                            SerieProductoId = d.SerieProductoId,
                            Cantidad = d.Cantidad,
                            CodActivo = d.CodActivo,
                            Observaciones = d.Observaciones,
                        })
                        .ToList();

                    var localOrigen = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == g.CodEmpresaOrigen && p.CodLocal == g.CodLocalOrigen)
                                                                            .Select(p => p.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                    var localDestino = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == g.CodEmpresaDestino && p.CodLocal == g.CodLocalDestino)
                                                                            .Select(p => p.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;

                    dtoItems.Add(new GuiaDespachoCabeceraDto
                    {
                        Id = g.Id,
                        Fecha = g.Fecha,
                        NumGuia = g.NumGuia,
                        CodEmpresaOrigen= g.CodEmpresaOrigen,
                        CodLocalOrigen = g.CodLocalOrigen,
                        CodEmpresaDestino = g.CodLocalDestino,
                        CodLocalDestino = g.CodLocalDestino,
                        NomLocalOrigen = localOrigen,
                        NomLocalDestino = localDestino,
                        TipoMovimiento = g.TipoMovimiento,
                        Items = detalleDtos.Count,
                        Detalles = detalleDtos,
                        IndEstado = g.IndEstado,
                        UsuCreacion = g.UsuCreacion
                    });
                }

                var pagedResult = new PagedResult<GuiaDespachoCabeceraDto>
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
