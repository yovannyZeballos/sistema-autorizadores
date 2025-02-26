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
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Puestos.DTOs
{
    public class ListarMaePuestoQuery : IRequest<GenericResponseDTO<PagedResult<ListarMaePuestoDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string CodEmpresa { get; set; }
        public string CodPuesto { get; set; }
        public string DesPuesto { get; set; }
    }

    public class ListarMaePuestoHandler : IRequestHandler<ListarMaePuestoQuery, GenericResponseDTO<PagedResult<ListarMaePuestoDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaePuestoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMaePuestoDTO>>> Handle(ListarMaePuestoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMaePuestoDTO>>
            {
                Ok = true,
                Data = new PagedResult<ListarMaePuestoDTO>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mae_Puesto), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(request.CodEmpresa))
                {
                    Expression codEmpresaProperty = Expression.Property(param, nameof(Mae_Puesto.CodEmpresa));
                    Expression codEmpresaValue = Expression.Constant(request.CodEmpresa);
                    Expression codEmpresaEqual = Expression.Equal(codEmpresaProperty, codEmpresaValue);

                    combined = Expression.AndAlso(combined, codEmpresaEqual);
                }

                if (!string.IsNullOrEmpty(request.CodPuesto))
                {
                    Expression codPuestoProperty = Expression.Property(param, nameof(Mae_Puesto.CodPuesto));
                    Expression codPuestoValue = Expression.Constant(request.CodPuesto);
                    Expression codPuestoEqual = Expression.Equal(codPuestoProperty, codPuestoValue);

                    combined = Expression.AndAlso(combined, codPuestoEqual);
                }

                if (!string.IsNullOrEmpty(request.DesPuesto))
                {
                    Expression desPuestoProperty = Expression.Property(param, nameof(Mae_Puesto.DesPuesto));
                    Expression desPuestoValue = Expression.Constant(request.DesPuesto);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression desPuestoContains = Expression.Call(desPuestoProperty, containsMethod, desPuestoValue);

                    combined = Expression.AndAlso(combined, desPuestoContains);
                }

                Expression<Func<Mae_Puesto, bool>> predicate = Expression.Lambda<Func<Mae_Puesto, bool>>(combined, param);

                var pagedPuestos = await _contexto.RepositorioMaePuesto.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.CodEmpresa, x.CodPuesto },
                    ascending: true
                );

                var mappedItems = _mapper.Map<List<ListarMaePuestoDTO>>(pagedPuestos.Items);

                foreach (var item in mappedItems)
                {
                    Mae_Empresa maeEmpresa = await _contexto.RepositorioMaeEmpresa.Obtener(s => s.CodEmpresa == item.CodEmpresa).FirstOrDefaultAsync();

                    item.NomEmpresa = maeEmpresa.NomEmpresa;
                }

                var pagedResult = new PagedResult<ListarMaePuestoDTO>
                {
                    PageNumber = pagedPuestos.PageNumber,
                    PageSize = pagedPuestos.PageSize,
                    TotalRecords = pagedPuestos.TotalRecords,
                    TotalPages = pagedPuestos.TotalPages,
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
