using AutoMapper;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs;
using System.Linq;
using System.Data.Entity;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Queries
{
    public class ListarSolicitudCComercioCabQuery : IRequest<GenericResponseDTO<PagedResult<CCom_SolicitudCabDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string NroSolicitud { get; set; }
        public string TipEstado { get; set; }
    }

    public class ListarSolicitudCComercioCabHandler : IRequestHandler<ListarSolicitudCComercioCabQuery, GenericResponseDTO<PagedResult<CCom_SolicitudCabDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarSolicitudCComercioCabHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<CCom_SolicitudCabDto>>> Handle(ListarSolicitudCComercioCabQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<CCom_SolicitudCabDto>>
            {
                Ok = true,
                Data = new PagedResult<CCom_SolicitudCabDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(CCom_SolicitudCab), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(request.NroSolicitud))
                {
                    Expression nroSolicitudProperty = Expression.Property(param, nameof(CCom_SolicitudCab.NroSolicitud));
                    Expression nroSolicitudValue = Expression.Constant(request.NroSolicitud);
                    Expression nroSolicitudEqual = Expression.Equal(nroSolicitudProperty, nroSolicitudValue);

                    combined = Expression.AndAlso(combined, nroSolicitudEqual);
                }

                if (!string.IsNullOrEmpty(request.TipEstado))
                {
                    Expression tipoEstadoProperty = Expression.Property(param, nameof(CCom_SolicitudCab.TipEstado));
                    Expression tipoEstadoValue = Expression.Constant(request.TipEstado);
                    Expression tipoEstadoEqual = Expression.Equal(tipoEstadoProperty, tipoEstadoValue);

                    combined = Expression.AndAlso(combined, tipoEstadoEqual);
                }


                Expression<Func<CCom_SolicitudCab, bool>> predicate = Expression.Lambda<Func<CCom_SolicitudCab, bool>>(combined, param);


                var pagedRegistros = await _contexto.RepositorioCComSolicitudCab.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.NroSolicitud },
                    ascending: false,
                    includes: cab => cab.Detalles.Select(det => det.Comercios)
                );

                foreach (var cab in pagedRegistros.Items)
                {
                    foreach (var det in cab.Detalles)
                    {
                        //var nomLocal = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == det.CodEmpresa && p.CodLocal == det.CodLocal)
                        //                                                    .Select(p => p.NomLocal)
                        //                                                    .FirstOrDefaultAsync() ?? string.Empty;

                        var nomLocal = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == det.CodEmpresa && p.CodLocal == det.CodLocal)
                                                                            .Select(p => p.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                        det.NomLocal = nomLocal;
                        det.NomEmpresa = nomLocal;
                    }
                }

                var mappedItems = _mapper.Map<List<CCom_SolicitudCabDto>>(pagedRegistros.Items);

                var pagedResult = new PagedResult<CCom_SolicitudCabDto>
                {
                    PageNumber = pagedRegistros.PageNumber,
                    PageSize = pagedRegistros.PageSize,
                    TotalRecords = pagedRegistros.TotalPages,
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
