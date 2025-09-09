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
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Queries
{
    public class ListarMaeColaboradorExtQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string TipoUsuario { get; set; }
        public string IndActivo { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarMaeColaboradorExtHandler : IRequestHandler<ListarMaeColaboradorExtQuery, GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDto>>>
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

        public async Task<GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDto>>> Handle(ListarMaeColaboradorExtQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeColaboradorExtDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_ColaboradorExt), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrWhiteSpace(request.CodEmpresa))
                {
                    var prop = Expression.Property(param, nameof(Mae_ColaboradorExt.CodEmpresa));
                    var cte = Expression.Constant(request.CodEmpresa);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodLocal))
                {
                    var prop = Expression.Property(param, nameof(Mae_ColaboradorExt.CodLocal));
                    var cte = Expression.Constant(request.CodLocal);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrEmpty(request.TipoUsuario))
                {
                    var prop = Expression.Property(param, nameof(Mae_ColaboradorExt.TipoUsuario));
                    var cte = Expression.Constant(request.TipoUsuario);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrEmpty(request.IndActivo))
                {
                    var prop = Expression.Property(param, nameof(Mae_ColaboradorExt.IndActivo));
                    var cte = Expression.Constant(request.IndActivo);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
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

                var pagedRegistros = await _contexto.RepositorioMaeColaboradorExt.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.ApelPaterno, x.ApelMaterno },
                    ascending: true
                );

                var dtoItems = new List<ListarMaeColaboradorExtDto>(pagedRegistros.Items.Count);

                foreach (var item in pagedRegistros.Items)
                {
                    var nomLocal = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodEmpresa == item.CodEmpresa && s.CodLocal == item.CodLocal)
                                                                            .Select(p => p.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;

                    dtoItems.Add(new ListarMaeColaboradorExtDto
                    {
                        CodEmpresa = item.CodEmpresa,
                        CodLocal = item.CodLocal,
                        NomLocal = nomLocal,
                        CodigoOfisis = item.CodigoOfisis,
                        ApelMaterno = item.ApelMaterno,
                        ApelPaterno = item.ApelPaterno,
                        NombreTrabajador = item.NombreTrabajador,
                        TipoDocIdent = item.TipoDocIdent,
                        NumDocIndent = item.NumDocIndent,
                        FechaIngresoEmpresa = item.FechaIngresoEmpresa,
                        FechaCeseTrabajador = item.FechaCeseTrabajador,
                        IndActivo = item.IndActivo,
                        PuestoTrabajo = item.PuestoTrabajo,
                        MotiSepa = item.MotiSepa,
                        IndPersonal = item.IndPersonal,
                        TipoUsuario = item.TipoUsuario,
                        UsuCreacion = item.UsuCreacion,
                        FecCreacion = item.FecCreacion,
                    });
                }

                var pagedResult = new PagedResult<ListarMaeColaboradorExtDto>
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
