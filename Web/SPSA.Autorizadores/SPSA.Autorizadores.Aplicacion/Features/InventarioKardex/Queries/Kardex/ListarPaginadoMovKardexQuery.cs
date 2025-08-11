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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Kardex;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Kardex
{
    public class ListarPaginadoMovKardexQuery : IRequest<GenericResponseDTO<PagedResult<ListarMovKardexDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string TipoMovimiento { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarPaginadoMovKardexHandler : IRequestHandler<ListarPaginadoMovKardexQuery, GenericResponseDTO<PagedResult<ListarMovKardexDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMovKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMovKardexDto>>> Handle(ListarPaginadoMovKardexQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMovKardexDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMovKardexDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mov_Kardex), "x");
                Expression combined = Expression.Constant(true);

                if (request.TipoMovimiento != "0")
                {
                    Expression property = Expression.Property(param, nameof(Mov_Kardex.TipoMovimiento));
                    Expression value = Expression.Constant(request.TipoMovimiento);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }

                DateTime? desde = request.FechaDesde?.Date;
                DateTime? hasta = request.FechaHasta?.Date;

                if (desde.HasValue && hasta.HasValue && hasta < desde)
                {
                    var tmp = desde; desde = hasta; hasta = tmp;
                }

                var propFecha = Expression.Property(param, nameof(Mov_Kardex.Fecha)); // tu columna es DATE

                if (desde.HasValue)
                {
                    var cteDesde = Expression.Constant(desde.Value, typeof(DateTime));
                    combined = Expression.AndAlso(combined, Expression.GreaterThanOrEqual(propFecha, cteDesde));
                }

                if (hasta.HasValue)
                {
                    var cteHasta = Expression.Constant(hasta.Value, typeof(DateTime));
                    combined = Expression.AndAlso(combined, Expression.LessThanOrEqual(propFecha, cteHasta));
                }

                if (!string.IsNullOrEmpty(request.FiltroVarios))
                {
                    Expression filtroConst = Expression.Constant(request.FiltroVarios);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression numGuiaContains = Expression.Call(Expression.Property(param, nameof(Mov_Kardex.NumGuia)), containsMethod, filtroConst);
                    Expression ordenCompraContains = Expression.Call(Expression.Property(param, nameof(Mov_Kardex.OrdenCompra)), containsMethod, filtroConst);

                    Expression filtroOr = Expression.OrElse(numGuiaContains, ordenCompraContains);

                    combined = Expression.AndAlso(combined, filtroOr);
                }

                Expression<Func<Mov_Kardex, bool>> predicate = Expression.Lambda<Func<Mov_Kardex, bool>>(combined, param);

                var pagedRegistros = await _contexto.RepositorioMovKardex.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.Id},
                    ascending: false,
                    includes: k => k.SerieProducto
                );

                var dtoItems = new List<ListarMovKardexDto>(pagedRegistros.Items.Count);
                foreach (var k in pagedRegistros.Items) 
                {
                    var prod = k.SerieProducto?.Producto;
                    var marca = prod?.Marca?.NomMarca;
                    var desProducto = string.Join(" - ",
                        new[] { prod?.DesProducto, marca, prod?.NomModelo }.Where(s => !string.IsNullOrWhiteSpace(s)));

                    var nomLocalOri= await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == k.CodEmpresaOrigen && x.CodLocal == k.CodLocalOrigen)
                                                                            .Select(x => x.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                    var nomLocalDes = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == k.CodEmpresaDestino && x.CodLocal == k.CodLocalDestino)
                                                                            .Select(x => x.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;

                    dtoItems.Add(new ListarMovKardexDto
                    {
                        Id = k.Id,
                        Fecha = k.Fecha,
                        TipoMovimiento = k.TipoMovimiento,
                        SerieProductoId = k.SerieProductoId,
                        DesProducto = desProducto,
                        NumSerie = k.SerieProducto?.NumSerie,
                        DesAreaGestion = k.DesAreaGestion,
                        NumGuia = k.NumGuia,
                        CodActivo = k.CodActivo,
                        CodEmpresaOrigen = k.CodEmpresaOrigen,
                        CodLocalOrigen = k.CodLocalOrigen,
                        LocalOrigen = nomLocalOri,
                        CodEmpresaDestino = k.CodEmpresaDestino,
                        CodLocalDestino = k.CodLocalDestino,
                        LocalDestino = nomLocalDes,
                        Observaciones = k.Observaciones,
                        UsuCreacion = k.UsuCreacion,
                        FecCreacion = k.FecCreacion
                    });
                }

                var pagedResult = new PagedResult<ListarMovKardexDto>
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
