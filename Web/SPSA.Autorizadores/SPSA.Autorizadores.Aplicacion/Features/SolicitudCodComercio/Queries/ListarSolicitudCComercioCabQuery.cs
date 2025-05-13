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

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Queries
{
    public class ListarSolicitudCComercioCabQuery : IRequest<GenericResponseDTO<PagedResult<SolicitudCComercioCabDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string NroSolicitud { get; set; }
        public string TipEstado { get; set; }
        //public DateTime? FecSolicitud { get; set; }
    }

    public class ListarSolicitudCComercioCabHandler : IRequestHandler<ListarSolicitudCComercioCabQuery, GenericResponseDTO<PagedResult<SolicitudCComercioCabDTO>>>
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

        public async Task<GenericResponseDTO<PagedResult<SolicitudCComercioCabDTO>>> Handle(ListarSolicitudCComercioCabQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<SolicitudCComercioCabDTO>>
            {
                Ok = true,
                Data = new PagedResult<SolicitudCComercioCabDTO>()
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


                var pagedSolicitudes = await _contexto.RepositorioCComSolicitudCab.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.NroSolicitud},
                    ascending: true,
                    includes: cab => cab.Detalles.Select(det => det.Comercios)
                );

                var mappedItems = _mapper.Map<List<SolicitudCComercioCabDTO>>(pagedSolicitudes.Items);

                var pagedResult = new PagedResult<SolicitudCComercioCabDTO>
                {
                    PageNumber = pagedSolicitudes.PageNumber,
                    PageSize = pagedSolicitudes.PageSize,
                    TotalRecords = pagedSolicitudes.TotalRecords,
                    TotalPages = pagedSolicitudes.TotalPages,
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
