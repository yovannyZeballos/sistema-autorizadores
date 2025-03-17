using AutoMapper;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.Queries
{
    public class ListarMaeColaboradorIntQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaeColaboradorIntDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        //public string CodigoOfisis { get; set; }
        //public string NroDocIdent { get; set; }
        //public string TipoUsuario { get; set; }
        public string FiltroVarios { get; set; }
    }

    public class ListarMaeColaboradorIntHandler : IRequestHandler<ListarMaeColaboradorIntQuery, GenericResponseDTO<PagedResult<ListarMaeColaboradorIntDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeColaboradorIntHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaeColaboradorIntDTO>>> Handle(ListarMaeColaboradorIntQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaeColaboradorIntDTO>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaeColaboradorIntDTO>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_ColaboradorInt), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(request.CodEmpresa))
                {
                    Expression codEmpresaProperty = Expression.Property(param, nameof(Mae_ColaboradorInt.CodEmpresa));
                    Expression codEmpresaValue = Expression.Constant(request.CodEmpresa);
                    Expression codEmpresaEqual = Expression.Equal(codEmpresaProperty, codEmpresaValue);

                    combined = Expression.AndAlso(combined, codEmpresaEqual);
                }

                if (!string.IsNullOrEmpty(request.CodLocal))
                {
                    Expression codLocalProperty = Expression.Property(param, nameof(Mae_ColaboradorInt.CodLocal));
                    Expression codLocalValue = Expression.Constant(request.CodLocal);
                    Expression codLocalEqual = Expression.Equal(codLocalProperty, codLocalValue);

                    combined = Expression.AndAlso(combined, codLocalEqual);
                }

                //if (!string.IsNullOrEmpty(request.TipoUsuario))
                //{
                //    Expression tipoUsuarioProperty = Expression.Property(param, nameof(Mae_ColaboradorInt.TipoUsuario));
                //    Expression tipoUsuarioValue = Expression.Constant(request.TipoUsuario);
                //    Expression tipoUsuarioEqual = Expression.Equal(tipoUsuarioProperty, tipoUsuarioValue);

                //    combined = Expression.AndAlso(combined, tipoUsuarioEqual);
                //}

                //if (!string.IsNullOrEmpty(request.CodigoOfisis))
                //{
                //    Expression codigoOfisisProperty = Expression.Property(param, nameof(Mae_ColaboradorInt.CodigoOfisis));
                //    Expression codigoOfisisValue = Expression.Constant(request.CodigoOfisis);
                //    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                //    Expression codigoOfisisContains = Expression.Call(codigoOfisisProperty, containsMethod, codigoOfisisValue);

                //    combined = Expression.AndAlso(combined, codigoOfisisContains);
                //}

                //if (!string.IsNullOrEmpty(request.NroDocIdent))
                //{
                //    Expression nroDocProperty = Expression.Property(param, nameof(Mae_ColaboradorInt.NumDocIdent));
                //    Expression nroDocValue = Expression.Constant(request.NroDocIdent);
                //    var containsMethod2 = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                //    Expression nroDocContains = Expression.Call(nroDocProperty, containsMethod2, nroDocValue);

                //    combined = Expression.AndAlso(combined, nroDocContains);
                //}

                if (!string.IsNullOrEmpty(request.FiltroVarios))
                {
                    Expression filtroConst = Expression.Constant(request.FiltroVarios);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression numDocContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorInt.NumDocIdent)),
                        containsMethod,
                        filtroConst
                    );

                    Expression nomTrabajadorContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorInt.NomTrabajador)),
                        containsMethod,
                        filtroConst
                    );

                    Expression apelPaternoContains = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorInt.ApePaterno)),
                        containsMethod,
                        filtroConst
                    );

                    Expression codigoOfisisContainsFiltro = Expression.Call(
                        Expression.Property(param, nameof(Mae_ColaboradorInt.CodigoOfisis)),
                        containsMethod,
                        filtroConst
                    );

                    Expression filtroOr = Expression.OrElse(
                        Expression.OrElse(numDocContains, nomTrabajadorContains),
                        Expression.OrElse(apelPaternoContains, codigoOfisisContainsFiltro)
                    );

                    combined = Expression.AndAlso(combined, filtroOr);
                }

                Expression<Func<Mae_ColaboradorInt, bool>> predicate = Expression.Lambda<Func<Mae_ColaboradorInt, bool>>(combined, param);

                var pagedColaboradores = await _contexto.RepositorioMaeColaboradorInt.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.ApePaterno, x.ApeMaterno },
                    ascending: true
                );

                var mappedItems = _mapper.Map<List<ListarMaeColaboradorIntDTO>>(pagedColaboradores.Items);

                foreach (var item in mappedItems)
                {
                    Mae_Local maeLocal = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == item.CodEmpresa && x.CodLocal == item.CodLocal).FirstOrDefaultAsync();
                    
                    item.NomLocal = maeLocal.NomLocal;
                    item.CodLocalAlterno = maeLocal.CodLocalAlterno;

                    Mae_Puesto maePuesto = await _contexto.RepositorioMaePuesto.Obtener(x => x.CodPuesto == item.CodPuesTrab).FirstOrDefaultAsync();

                    if (maePuesto != null)
                    {
                        item.NomPuesto = item.CodPuesTrab + " " + maePuesto.DesPuesto;
                    }
                    else
                    {
                        item.NomPuesto = item.CodPuesTrab + "";
                    }
                }

                var pagedResult = new PagedResult<ListarMaeColaboradorIntDTO>
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
