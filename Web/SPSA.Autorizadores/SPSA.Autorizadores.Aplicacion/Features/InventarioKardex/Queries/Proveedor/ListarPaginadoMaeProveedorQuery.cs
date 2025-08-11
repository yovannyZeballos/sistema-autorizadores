using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Proveedor;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Proveedor
{
    public class ListarPaginadoMaeProveedorQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeProveedorDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;      
        public string IndActivo { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarPaginadoMaeProveedorHandler : IRequestHandler<ListarPaginadoMaeProveedorQuery, GenericResponseDTO<PagedResult<ListarMaeProveedorDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMaeProveedorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeProveedorDto>>> Handle(ListarPaginadoMaeProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeProveedorDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeProveedorDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_Proveedor), "x");
                Expression combined = Expression.Constant(true);

                if (request.IndActivo != "0")
                {
                    Expression property = Expression.Property(param, nameof(Mae_Proveedor.IndActivo));
                    Expression value = Expression.Constant(request.IndActivo);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }

                if (!string.IsNullOrEmpty(request.FiltroVarios))
                {
                    Expression filtroConst = Expression.Constant(request.FiltroVarios);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression rucContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_Proveedor.Ruc)),
                        containsMethod,
                        filtroConst
                    );

                    Expression razonSocialContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_Proveedor.RazonSocial)),
                        containsMethod,
                        filtroConst
                    );

                    Expression filtroOr = Expression.OrElse(rucContains, razonSocialContains);

                    combined = Expression.AndAlso(combined, filtroOr);
                }

                Expression<Func<Mae_Proveedor, bool>> predicate = Expression.Lambda<Func<Mae_Proveedor, bool>>(combined, param);

                var pagedColaboradores = await _contexto.RepositorioMaeProveedor.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.RazonSocial},
                    ascending: true
                );

                var pagedResult = new PagedResult<ListarMaeProveedorDto>
                {
                    PageNumber = pagedColaboradores.PageNumber,
                    PageSize = pagedColaboradores.PageSize,
                    TotalRecords = pagedColaboradores.TotalRecords,
                    TotalPages = pagedColaboradores.TotalPages,
                    Items = _mapper.Map<List<ListarMaeProveedorDto>>(pagedColaboradores.Items)
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
