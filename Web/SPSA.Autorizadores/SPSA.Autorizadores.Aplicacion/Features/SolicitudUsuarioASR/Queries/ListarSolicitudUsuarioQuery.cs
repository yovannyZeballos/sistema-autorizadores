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
    public class ListarSolicitudUsuarioQuery : IRequest<GenericResponseDTO<PagedResult<ListarSolictudUsuarioDTO>>>
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

    public class ListarSolicitudUsuarioHandler : IRequestHandler<ListarSolicitudUsuarioQuery, GenericResponseDTO<PagedResult<ListarSolictudUsuarioDTO>>>
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

        public async Task<GenericResponseDTO<PagedResult<ListarSolictudUsuarioDTO>>> Handle(ListarSolicitudUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarSolictudUsuarioDTO>>
            {
                Ok = true,
                Data = new PagedResult<ListarSolictudUsuarioDTO>()
            };

            try
            {
                var objLocal = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodLocal == request.CodLocal).FirstOrDefaultAsync();


                ParameterExpression param = Expression.Parameter(typeof(ASR_SolicitudUsuario), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(objLocal.CodLocalAlterno))
                {
                    Expression codLocalProperty = Expression.Property(param, nameof(ASR_SolicitudUsuario.CodLocalAlterno));
                    Expression codLocalValue = Expression.Constant(objLocal.CodLocalAlterno);
                    Expression codLocalEqual = Expression.Equal(codLocalProperty, codLocalValue);

                    combined = Expression.AndAlso(combined, codLocalEqual);
                }

                if (!string.IsNullOrEmpty(request.TipUsuario))
                {
                    Expression tipUsuarioProperty = Expression.Property(param, nameof(ASR_SolicitudUsuario.TipUsuario));
                    Expression tipUsuarioValue = Expression.Constant(request.TipUsuario);
                    Expression tipUsuarioEqual = Expression.Equal(tipUsuarioProperty, tipUsuarioValue);

                    combined = Expression.AndAlso(combined, tipUsuarioEqual);
                }

                if (!string.IsNullOrEmpty(request.TipColaborador))
                {
                    Expression tipColaboradorProperty = Expression.Property(param, nameof(ASR_SolicitudUsuario.TipColaborador));
                    Expression tipColaboradorValue = Expression.Constant(request.TipColaborador);
                    Expression tipColaboradorEqual = Expression.Equal(tipColaboradorProperty, tipColaboradorValue);

                    combined = Expression.AndAlso(combined, tipColaboradorEqual);
                }

                if (!string.IsNullOrEmpty(request.IndAprobado))
                {
                    Expression indAprobadoProperty = Expression.Property(param, nameof(ASR_SolicitudUsuario.IndAprobado));
                    Expression indAprobadoValue = Expression.Constant(request.IndAprobado);
                    Expression indAprobadoEqual = Expression.Equal(indAprobadoProperty, indAprobadoValue);

                    combined = Expression.AndAlso(combined, indAprobadoEqual);
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

                var pagedColaboradores = await _contexto.RepositorioSolicitudUsuarioASR.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.FecSolicita },
                    ascending: false
                );

                var mappedItems = _mapper.Map<List<ListarSolictudUsuarioDTO>>(pagedColaboradores.Items);

                foreach (var item in mappedItems)
                {
                    if (item.TipColaborador == "I")
                    {
                        Mae_ColaboradorInt maeColab = await _contexto.RepositorioMaeColaboradorInt.Obtener(s => s.CodigoOfisis == item.CodColaborador).FirstOrDefaultAsync();
                        item.NomColaborador = $"{maeColab.NomTrabajador} {maeColab.ApePaterno} {maeColab.ApeMaterno}";
                    }
                    else
                    {
                        Mae_ColaboradorExt maeColab = await _contexto.RepositorioMaeColaboradorExt.Obtener(s => s.CodigoOfisis == item.CodColaborador).FirstOrDefaultAsync();
                        item.NomColaborador = $"{maeColab.NombreTrabajador} {maeColab.ApelPaterno} {maeColab.ApelMaterno}";
                    }
                }

                var pagedResult = new PagedResult<ListarSolictudUsuarioDTO>
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
