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

        public string CodLocalAlterno { get; set; }
        //public string TipUsuario { get; set; }
        //public string TipColaborador { get; set; }
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

                //if (!string.IsNullOrEmpty(request.CodLocalAlterno))
                //{
                //    Expression codLocalProperty = Expression.Property(param, nameof(ASR_SolicitudUsuario.CodLocalAlterno));
                //    Expression codLocalValue = Expression.Constant(request.CodLocalAlterno);
                //    Expression codLocalEqual = Expression.Equal(codLocalProperty, codLocalValue);

                //    combined = Expression.AndAlso(combined, codLocalEqual);
                //}

                //if (!string.IsNullOrEmpty(request.CodigoOfisis))
                //{
                //    Expression codigoOfisisProperty = Expression.Property(param, nameof(Mae_ColaboradorExt.CodigoOfisis));
                //    Expression codigoOfisisValue = Expression.Constant(request.CodigoOfisis);
                //    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                //    Expression codigoOfisisContains = Expression.Call(codigoOfisisProperty, containsMethod, codigoOfisisValue);

                //    combined = Expression.AndAlso(combined, codigoOfisisContains);
                //}

                //if (!string.IsNullOrEmpty(request.NroDocIdent))
                //{
                //    Expression nroDocProperty = Expression.Property(param, nameof(Mae_ColaboradorExt.NumDocIndent));
                //    Expression nroDocValue = Expression.Constant(request.NroDocIdent);
                //    var containsMethod2 = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                //    Expression nroDocContains = Expression.Call(nroDocProperty, containsMethod2, nroDocValue);

                //    combined = Expression.AndAlso(combined, nroDocContains);
                //}

                Expression<Func<ASR_SolicitudUsuario, bool>> predicate = Expression.Lambda<Func<ASR_SolicitudUsuario, bool>>(combined, param);

                var pagedColaboradores = await _contexto.RepositorioSolicitudUsuarioASR.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.FecSolicita },
                    ascending: false
                );

                var mappedItems = _mapper.Map<List<ListarSolictudUsuarioDTO>>(pagedColaboradores.Items);

                //foreach (var item in mappedItems)
                //{
                //    Mae_Local maeLocal = await _contexto.RepositorioMaeLocal.Obtener(s => s.CodLocalAlterno == item.CodLocalAlterno).FirstOrDefaultAsync();

                //    item.NomLocal = maeLocal.NomLocal;
                //}

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
