using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Producto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Producto
{
    public class ListarPaginadoMaeProductoQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeProductoDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string TipProducto { get; set; }
        public int MarcaId { get; set; }
        public string IndActivo { get; set; }
        public int AreaGestionId { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarPaginadoMaeProductoHandler : IRequestHandler<ListarPaginadoMaeProductoQuery, GenericResponseDTO<PagedResult<ListarMaeProductoDto>>>
    {
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMaeProductoHandler()
        {
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeProductoDto>>> Handle(ListarPaginadoMaeProductoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeProductoDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeProductoDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_Producto), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(request.IndActivo))
                {
                    Expression property = Expression.Property(param, nameof(Mae_Producto.IndActivo));
                    Expression value = Expression.Constant(request.IndActivo);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }

                if (request.MarcaId != 0)
                {
                    Expression property = Expression.Property(param, nameof(Mae_Producto.MarcaId));
                    Expression value = Expression.Constant(request.MarcaId);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }

                if (!string.IsNullOrEmpty(request.TipProducto))
                {
                    Expression property = Expression.Property(param, nameof(Mae_Producto.TipProducto));
                    Expression value = Expression.Constant(request.TipProducto);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }

                if (request.AreaGestionId != 0)
                {
                    Expression property = Expression.Property(param, nameof(Mae_Producto.AreaGestionId));
                    Expression value = Expression.Constant(request.AreaGestionId);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }

                if (!string.IsNullOrEmpty(request.FiltroVarios))
                {
                    Expression filtroConst = Expression.Constant(request.FiltroVarios.ToUpper());
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression desProductoContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_Producto.DesProducto)),
                        containsMethod,
                        filtroConst
                    );

                    Expression codProductoContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_Producto.CodProducto)),
                        containsMethod,
                        filtroConst
                    );

                    Expression filtroOr = Expression.OrElse(desProductoContains, codProductoContains);

                    combined = Expression.AndAlso(combined, filtroOr);
                }

                Expression<Func<Mae_Producto, bool>> predicate = Expression.Lambda<Func<Mae_Producto, bool>>(combined, param);

                var pagedRegistros = await _contexto.RepositorioMaeProducto.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.CodProducto, x.DesProducto },
                    ascending: true,
                    includes: p => p.Marca
                );

                // Buscar las áreas de gestión asociadas a la página actual
                var areaGestionIds = pagedRegistros.Items
                    .Select(p => p.AreaGestionId)
                    .Distinct()
                    .ToList();

                var areas = _contexto.RepositorioMaeAreaGestion
                    .Obtener(a => areaGestionIds.Contains(a.Id))
                    .ToDictionary(a => a.Id, a => a.NomAreaGestion);


                var dtoItems = pagedRegistros.Items.Select(p => new ListarMaeProductoDto
                {
                    CodProducto = p.CodProducto,
                    DesProducto = p.DesProducto,
                    MarcaId = p.MarcaId,
                    NomMarca = p.Marca?.NomMarca,
                    TipProducto = p.TipProducto,
                    NomTipProducto = p.TipProducto == "A" ? "ACTIVO" : p.TipProducto == "R" ? "REPUESTO" : "",
                    AreaGestionId = p.AreaGestionId,
                    NomAreaGestion = areas.ContainsKey(p.AreaGestionId)? areas[p.AreaGestionId] : string.Empty,
                    IndSerializable = p.IndSerializable,
                    IndActivo = p.IndActivo,
                    StkMinimo = p.StkMinimo,
                    StkMaximo = p.StkMaximo,
                    NomModelo = p.NomModelo,
                    UsuCreacion = p.UsuCreacion,
                    FecCreacion = p.FecCreacion,
                    UsuModifica = p.UsuModifica,
                    FecModifica = p.FecModifica,
                    UsuElimina = p.UsuElimina,
                    FecElimina = p.FecElimina
                }).ToList();

                var pagedResult = new PagedResult<ListarMaeProductoDto>
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
