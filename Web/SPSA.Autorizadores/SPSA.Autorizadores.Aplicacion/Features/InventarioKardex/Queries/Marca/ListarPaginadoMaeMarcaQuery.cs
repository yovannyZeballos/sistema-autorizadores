using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Marca;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Marca
{
    public class ListarPaginadoMaeMarcaQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeMarcaDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string NomMarca { get; set; }
        public string IndActivo { get; set; }
    }

    public class ListarPaginadoMaeMarcaHandler : IRequestHandler<ListarPaginadoMaeMarcaQuery, GenericResponseDTO<PagedResult<ListarMaeMarcaDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMaeMarcaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeMarcaDto>>> Handle(ListarPaginadoMaeMarcaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeMarcaDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeMarcaDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_Marca), "x");
                Expression combined = Expression.Constant(true);

                if (request.IndActivo != "0")
                {
                    Expression property = Expression.Property(param, nameof(Mae_Marca.IndActivo));
                    Expression value = Expression.Constant(request.IndActivo);
                    Expression equal = Expression.Equal(property, value);

                    combined = Expression.AndAlso(combined, equal);
                }


                if (!string.IsNullOrEmpty(request.NomMarca))
                {
                    Expression property = Expression.Property(param, nameof(Mae_Marca.NomMarca));
                    Expression value = Expression.Constant(request.NomMarca);
                    var method2 = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression contains = Expression.Call(property, method2, value);

                    combined = Expression.AndAlso(combined, contains);
                }

                Expression<Func<Mae_Marca, bool>> predicate = Expression.Lambda<Func<Mae_Marca, bool>>(combined, param);

                var pagedRegistros = await _contexto.RepositorioMaeMarca.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.Id, x.NomMarca },
                    ascending: true
                );

                var pagedResult = new PagedResult<ListarMaeMarcaDto>
                {
                    PageNumber = pagedRegistros.PageNumber,
                    PageSize = pagedRegistros.PageSize,
                    TotalRecords = pagedRegistros.TotalRecords,
                    TotalPages = pagedRegistros.TotalPages,
                    Items = _mapper.Map<List<ListarMaeMarcaDto>>(pagedRegistros.Items)
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
