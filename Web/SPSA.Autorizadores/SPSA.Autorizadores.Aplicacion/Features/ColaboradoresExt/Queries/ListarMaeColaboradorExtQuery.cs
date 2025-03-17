using System;
using System.Collections.Generic;
using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity;
using System.Linq.Expressions;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Queries
{
    public class ListarMaeColaboradorExtQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string CodLocalAlterno { get; set; }
        public string CodigoOfisis { get; set; }
        public string NroDocIdent { get; set; }
        public string TipoUsuario { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarMaeColaboradorExtHandler : IRequestHandler<ListarMaeColaboradorExtQuery, GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDTO>>> Handle(ListarMaeColaboradorExtQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDTO>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeColaboradorExtDTO>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_ColaboradorExt), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(request.CodLocalAlterno))
                {
                    Expression codLocalProperty = Expression.Property(param, nameof(Mae_ColaboradorExt.CodLocalAlterno));
                    Expression codLocalValue = Expression.Constant(request.CodLocalAlterno);
                    Expression codLocalEqual = Expression.Equal(codLocalProperty, codLocalValue);

                    combined = Expression.AndAlso(combined, codLocalEqual);
                }

                if (!string.IsNullOrEmpty(request.TipoUsuario))
                {
                    Expression tipoUsuarioProperty = Expression.Property(param, nameof(Mae_ColaboradorExt.TipoUsuario));
                    Expression tipoUsuarioValue = Expression.Constant(request.TipoUsuario);
                    Expression tipoUsuarioEqual = Expression.Equal(tipoUsuarioProperty, tipoUsuarioValue);

                    combined = Expression.AndAlso(combined, tipoUsuarioEqual);
                }

                if (!string.IsNullOrEmpty(request.CodigoOfisis))
                {
                    Expression codigoOfisisProperty = Expression.Property(param, nameof(Mae_ColaboradorExt.CodigoOfisis));
                    Expression codigoOfisisValue = Expression.Constant(request.CodigoOfisis);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression codigoOfisisContains = Expression.Call(codigoOfisisProperty, containsMethod, codigoOfisisValue);

                    combined = Expression.AndAlso(combined, codigoOfisisContains);
                }

                if (!string.IsNullOrEmpty(request.NroDocIdent))
                {
                    Expression nroDocProperty = Expression.Property(param, nameof(Mae_ColaboradorExt.NumDocIndent));
                    Expression nroDocValue = Expression.Constant(request.NroDocIdent);
                    var containsMethod2 = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression nroDocContains = Expression.Call(nroDocProperty, containsMethod2, nroDocValue);

                    combined = Expression.AndAlso(combined, nroDocContains);
                }

                if (!string.IsNullOrEmpty(request.FiltroVarios))
                {
                    Expression filtroConst = Expression.Constant(request.FiltroVarios);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression numDocContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorExt.NumDocIndent)),
                        containsMethod,
                        filtroConst
                    );

                    Expression nomTrabajadorContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorExt.NombreTrabajador)),
                        containsMethod,
                        filtroConst
                    );

                    Expression apelPaternoContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorExt.ApelPaterno)),
                        containsMethod,
                        filtroConst
                    );

                    Expression codigoOfisisContainsFiltro = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorExt.CodigoOfisis)),
                        containsMethod,
                        filtroConst
                    );

                    Expression filtroOr = Expression.OrElse(
                        Expression.OrElse(numDocContains, nomTrabajadorContains),
                        Expression.OrElse(apelPaternoContains, codigoOfisisContainsFiltro)
                    );

                    combined = Expression.AndAlso(combined, filtroOr);
                }

                Expression<Func<Mae_ColaboradorExt, bool>> predicate = Expression.Lambda<Func<Mae_ColaboradorExt, bool>>(combined, param);

                var pagedColaboradores = await _contexto.RepositorioMaeColaboradorExt.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.ApelPaterno, x.ApelMaterno },
                    ascending: true
                );

                var mappedItems = _mapper.Map<List<ListarMaeColaboradorExtDTO>>(pagedColaboradores.Items);

                foreach (var item in mappedItems)
                {
                    Mae_Local maeLocal = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodLocalAlterno == item.CodLocalAlterno).FirstOrDefaultAsync();

                    item.NomLocal = maeLocal.NomLocal;
                }

                var pagedResult = new PagedResult<ListarMaeColaboradorExtDTO>
                {
                    PageNumber = pagedColaboradores.PageNumber,
                    PageSize = pagedColaboradores.PageSize,
                    TotalRecords = pagedColaboradores.TotalRecords,
                    TotalPages = pagedColaboradores.TotalPages,
                    Items = mappedItems
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
