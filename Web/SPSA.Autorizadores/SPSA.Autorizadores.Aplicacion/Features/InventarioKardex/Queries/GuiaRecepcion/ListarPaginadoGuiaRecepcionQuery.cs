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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaRecepcion
{
    public class ListarPaginadoGuiaRecepcionQuery : IRequest<GenericResponseDTO<PagedResult<GuiaRecepcionCabeceraDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string ProveedorRuc { get; set; }   // opcional
        public string IndTransferencia { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public string FiltroVarios { get; set; }   // busca en NumGuia / Observaciones
    }

    public class ListarPaginadoGuiaRecepcionHandler : IRequestHandler<ListarPaginadoGuiaRecepcionQuery, GenericResponseDTO<PagedResult<GuiaRecepcionCabeceraDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoGuiaRecepcionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<GuiaRecepcionCabeceraDto>>> Handle(ListarPaginadoGuiaRecepcionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<GuiaRecepcionCabeceraDto>>
            {
                Ok = true,
                Data = new PagedResult<GuiaRecepcionCabeceraDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(GuiaRecepcionCabecera), "x");
                Expression combined = Expression.Constant(true);

                DateTime? desde = request.FechaDesde?.Date;
                DateTime? hasta = request.FechaHasta?.Date;

                if (desde.HasValue && hasta.HasValue && hasta < desde)
                {
                    var tmp = desde; desde = hasta; hasta = tmp;
                }

                if (desde.HasValue)
                {
                    var prop = Expression.Property(param, nameof(GuiaRecepcionCabecera.Fecha));
                    var cte = Expression.Constant(desde.Value, typeof(DateTime));
                    combined = Expression.AndAlso(combined, Expression.GreaterThanOrEqual(prop, cte));
                }

                if (hasta.HasValue)
                {
                    var prop = Expression.Property(param, nameof(GuiaRecepcionCabecera.Fecha));
                    var cte = Expression.Constant(hasta.Value, typeof(DateTime));
                    combined = Expression.AndAlso(combined, Expression.LessThanOrEqual(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.IndTransferencia))
                {
                    var prop = Expression.Property(param, nameof(GuiaRecepcionCabecera.IndTransferencia));
                    var cte = Expression.Constant(request.IndTransferencia);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodEmpresaDestino))
                {
                    var prop = Expression.Property(param, nameof(GuiaRecepcionCabecera.CodEmpresaDestino));
                    var cte = Expression.Constant(request.CodEmpresaDestino);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodLocalDestino))
                {
                    var prop = Expression.Property(param, nameof(GuiaRecepcionCabecera.CodLocalDestino));
                    var cte = Expression.Constant(request.CodLocalDestino);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.ProveedorRuc))
                {
                    var prop = Expression.Property(param, nameof(GuiaRecepcionCabecera.ProveedorRuc));
                    var cte = Expression.Constant(request.ProveedorRuc);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                // Filtro varios: NumGuia / Observaciones
                if (!string.IsNullOrWhiteSpace(request.FiltroVarios))
                {
                    var filtroConst = Expression.Constant(request.FiltroVarios);
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var numGuiaContains = Expression.Call(
                        Expression.Property(param, nameof(GuiaRecepcionCabecera.NumGuia)), contains, filtroConst);

                    var obsContains = Expression.Call(
                        Expression.Property(param, nameof(GuiaRecepcionCabecera.Observaciones)), contains, filtroConst);

                    var orExpr = Expression.OrElse(numGuiaContains, obsContains);
                    combined = Expression.AndAlso(combined, orExpr);
                }

                Expression<Func<GuiaRecepcionCabecera, bool>> predicate = Expression.Lambda<Func<GuiaRecepcionCabecera, bool>>(combined, param);

                // Paginación (orden: los más recientes primero por Id)
                var pagedRegistros = await _contexto.RepositorioGuiaRecepcionCabecera.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.Id },
                    ascending: false
                );

                // Mapear DTOs
                var dtoItems = new List<GuiaRecepcionCabeceraDto>(pagedRegistros.Items.Count);
                foreach (var g in pagedRegistros.Items)
                {
                    var razonSocial = await _contexto.RepositorioMaeProveedor.Obtener(p => p.Ruc == g.ProveedorRuc)
                                                                            .Select(p => p.RazonSocial)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;

                    dtoItems.Add(new GuiaRecepcionCabeceraDto
                    {
                        Id = g.Id,
                        Fecha = g.Fecha,
                        NumGuia = g.NumGuia,
                        Proveedor = razonSocial ?? string.Empty,
                        CodEmpresaDestino = g.CodEmpresaDestino,
                        CodLocalDestino = g.CodLocalDestino,
                        //Items = lineas,
                        IndEstado = g.IndEstado,
                        UsuCreacion = g.UsuCreacion
                    });
                }

                var pagedResult = new PagedResult<GuiaRecepcionCabeceraDto>
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
