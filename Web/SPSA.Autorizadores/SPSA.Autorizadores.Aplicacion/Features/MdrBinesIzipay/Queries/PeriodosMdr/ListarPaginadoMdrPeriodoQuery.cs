using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.PeriodosMdr
{
    public class ListarPaginadoMdrPeriodoQuery : IRequest<GenericResponseDTO<PagedResult<ListarMdrPeriodoDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ListarPaginadoMdrPeriodoHandler : IRequestHandler<ListarPaginadoMdrPeriodoQuery, GenericResponseDTO<PagedResult<ListarMdrPeriodoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMdrPeriodoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMdrPeriodoDto>>> Handle(ListarPaginadoMdrPeriodoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMdrPeriodoDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMdrPeriodoDto>()
            };

            try
            {
                Expression<Func<Mdr_Periodo, bool>> predicate = null;

                var pagedPeriodos = await _contexto.RepositorioMdrPeriodo.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => x.DesPeriodo,
                    ascending: true
                    );

                var pagedResult = new PagedResult<ListarMdrPeriodoDto>
                {
                    PageNumber = pagedPeriodos.PageNumber,
                    PageSize = pagedPeriodos.PageSize,
                    TotalRecords = pagedPeriodos.TotalRecords,
                    TotalPages = pagedPeriodos.TotalPages,
                    Items = _mapper.Map<List<ListarMdrPeriodoDto>>(pagedPeriodos.Items)
                };

                response.Data = pagedResult;

            }
            catch (System.Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }

            return response;
        }
    }
}
