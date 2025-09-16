using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Queries
{
    public class ListarSolicitudUsuarioQuery : IRequest<GenericResponseDTO<PagedResult<ListarSolictudUsuarioDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string CodEmpresa{ get; set; }
        public string CodLocal{ get; set; }
        public string TipUsuario { get; set; }
        public string TipColaborador { get; set; }
        public string IndAprobado { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarSolicitudUsuarioHandler : IRequestHandler<ListarSolicitudUsuarioQuery, GenericResponseDTO<PagedResult<ListarSolictudUsuarioDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarSolicitudUsuarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarSolictudUsuarioDto>>> Handle(ListarSolicitudUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarSolictudUsuarioDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarSolictudUsuarioDto>()
            };

            try
            {
                var objLocal = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodLocal == request.CodLocal).FirstOrDefaultAsync();


                ParameterExpression param = Expression.Parameter(typeof(ASR_SolicitudUsuario), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrWhiteSpace(request.CodEmpresa))
                {
                    var prop = Expression.Property(param, nameof(ASR_SolicitudUsuario.CodEmpresa));
                    var cte = Expression.Constant(request.CodEmpresa);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrWhiteSpace(request.CodLocal))
                {
                    var prop = Expression.Property(param, nameof(ASR_SolicitudUsuario.CodLocal));
                    var cte = Expression.Constant(request.CodLocal);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrEmpty(request.TipUsuario))
                {
                    var prop = Expression.Property(param, nameof(ASR_SolicitudUsuario.TipUsuario));
                    var cte = Expression.Constant(request.TipUsuario);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrEmpty(request.TipColaborador))
                {
                    var prop = Expression.Property(param, nameof(ASR_SolicitudUsuario.TipColaborador));
                    var cte = Expression.Constant(request.TipColaborador);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrEmpty(request.IndAprobado))
                {
                    var prop = Expression.Property(param, nameof(ASR_SolicitudUsuario.IndAprobado));
                    var cte = Expression.Constant(request.IndAprobado);
                    combined = Expression.AndAlso(combined, Expression.Equal(prop, cte));
                }

                if (!string.IsNullOrEmpty(request.FiltroVarios))
                {
                    Expression filtroConst = Expression.Constant(request.FiltroVarios);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression codColabContains = Expression.Call(
                        Expression.Property(param, nameof(ASR_SolicitudUsuario.CodColaborador)),
                        containsMethod,
                        filtroConst
                    );

                    Expression motivoContains = Expression.Call(
                        Expression.Property(param, nameof(ASR_SolicitudUsuario.Motivo)),
                        containsMethod,
                        filtroConst
                    );

                    Expression filtroOr = Expression.OrElse(codColabContains, motivoContains);

                    combined = Expression.AndAlso(combined, filtroOr);
                }

                Expression<Func<ASR_SolicitudUsuario, bool>> predicate = Expression.Lambda<Func<ASR_SolicitudUsuario, bool>>(combined, param);

                var pagedRegistros = await _contexto.RepositorioSolicitudUsuarioASR.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.FecSolicita },
                    ascending: false
                );

                var dtoItems = new List<ListarSolictudUsuarioDto>(pagedRegistros.Items.Count);

                foreach (var item in pagedRegistros.Items)
                {
                    var nomColaborador = string.Empty;

                    if (item.TipColaborador == "I")
                    {
                        var colaboradorInt = await _contexto.RepositorioMaeColaboradorInt.Obtener(s => s.CodigoOfisis == item.CodColaborador).FirstOrDefaultAsync();

                        if (colaboradorInt != null)
                        {
                            nomColaborador = $"{colaboradorInt.NomTrabajador} {colaboradorInt.ApePaterno} {colaboradorInt.ApeMaterno}";
                        }
                        else
                        {
                            nomColaborador = string.Empty;
                        }
                    }
                    else
                    {
                        var colaboradorExt = await _contexto.RepositorioMaeColaboradorExt.Obtener(s => s.CodigoOfisis == item.CodColaborador).FirstOrDefaultAsync();

                        if (colaboradorExt != null)
                        {
                            nomColaborador = $"{colaboradorExt.NombreTrabajador} {colaboradorExt.ApelPaterno} {colaboradorExt.ApelMaterno}";
                        }
                        else
                        {
                            nomColaborador = string.Empty;
                        }
                    }

                    dtoItems.Add(new ListarSolictudUsuarioDto
                    {
                        NumSolicitud = item.NumSolicitud,
                        CodEmpresa = item.CodEmpresa,
                        CodLocal = item.CodLocal,
                        NomColaborador = nomColaborador,
                        CodColaborador = item.CodColaborador,
                        TipUsuario = item.TipUsuario,
                        TipColaborador = item.TipColaborador,
                        UsuSolicita = item.UsuSolicita,
                        FecSolicita = item.FecSolicita,
                        TipAccion = item.TipAccion,
                        UsuAprobacion = item.UsuAprobacion,
                        FecAprobacion = item.FecAprobacion,
                        IndAprobado = item.Motivo,
                        UsuElimina = item.UsuElimina,
                        FecElimina = item.FecElimina
                    });
                }

                var pagedResult = new PagedResult<ListarSolictudUsuarioDto>
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
