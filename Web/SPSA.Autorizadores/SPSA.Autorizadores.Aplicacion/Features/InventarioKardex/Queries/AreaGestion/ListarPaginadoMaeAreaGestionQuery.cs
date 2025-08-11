using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.AreaGestion;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.AreaGestion
{
    public class ListarPaginadoMaeAreaGestionQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeAreaGestionDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string NomAreaGestion { get; set; }
        public string IndActivo { get; set; }
    }

    public class ListarPaginadoMaeAreaGestionHandler : IRequestHandler<ListarPaginadoMaeAreaGestionQuery, GenericResponseDTO<PagedResult<ListarMaeAreaGestionDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMaeAreaGestionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeAreaGestionDto>>> Handle(ListarPaginadoMaeAreaGestionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeAreaGestionDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeAreaGestionDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_AreaGestion), "x");
                Expression combined = Expression.Constant(true);

                if (request.IndActivo != "0")
                {
                    Expression property = Expression.Property(param, nameof(Mae_AreaGestion.IndActivo));
                    Expression value = Expression.Constant(request.IndActivo);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }


                if (!string.IsNullOrEmpty(request.NomAreaGestion))
                {
                    Expression property = Expression.Property(param, nameof(Mae_AreaGestion.NomAreaGestion));
                    Expression value = Expression.Constant(request.NomAreaGestion);
                    var method2 = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression contains = Expression.Call(property, method2, value);

                    combined = Expression.AndAlso(combined, contains);
                }

                Expression<Func<Mae_AreaGestion, bool>> predicate = Expression.Lambda<Func<Mae_AreaGestion, bool>>(combined, param);

                var pagedRegistros = await _contexto.RepositorioMaeAreaGestion.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.NomAreaGestion },
                    ascending: true
                );

                var pagedResult = new PagedResult<ListarMaeAreaGestionDto>
                {
                    PageNumber = pagedRegistros.PageNumber,
                    PageSize = pagedRegistros.PageSize,
                    TotalRecords = pagedRegistros.TotalRecords,
                    TotalPages = pagedRegistros.TotalPages,
                    Items = _mapper.Map<List<ListarMaeAreaGestionDto>>(pagedRegistros.Items)
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
