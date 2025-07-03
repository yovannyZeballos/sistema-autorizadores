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
        public string DesPuesto { get; set; }
        public string IndAutAut { get; set; }
        public string IndAutOpe { get; set; }
        public string IndManAut { get; set; }
        public string IndManOpe { get; set; }
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

                //if (!string.IsNullOrEmpty(request.IndAutAut))
                if (request.IndAutAut == "S")
                {
                    Expression indAutAutProperty = Expression.Property(param, nameof(Mae_Puesto.IndAutAut));
                    Expression indAutAutValue = Expression.Constant(request.IndAutAut);
                    Expression indAutAutEqual = Expression.Equal(indAutAutProperty, indAutAutValue);

                    combined = Expression.AndAlso(combined, indAutAutEqual);
                }

                //if (!string.IsNullOrEmpty(request.IndAutOpe))
                if (request.IndAutOpe == "S")
                {
                    Expression indAutOpeProperty = Expression.Property(param, nameof(Mae_Puesto.IndAutOpe));
                    Expression indAutOpeValue = Expression.Constant(request.IndAutOpe);
                    Expression indAutOpeEqual = Expression.Equal(indAutOpeProperty, indAutOpeValue);

                    combined = Expression.AndAlso(combined, indAutOpeEqual);
                }

                //if (!string.IsNullOrEmpty(request.IndManAut))
                if (request.IndManAut == "S")
                {
                    Expression indManAutProperty = Expression.Property(param, nameof(Mae_Puesto.IndManAut));
                    Expression indManAutValue = Expression.Constant(request.IndManAut);
                    Expression indManAutEqual = Expression.Equal(indManAutProperty, indManAutValue);

                    combined = Expression.AndAlso(combined, indManAutEqual);
                }

                //if (!string.IsNullOrEmpty(request.IndManOpe))
                if (request.IndManOpe == "S")
                {
                    Expression indAutOpeProperty = Expression.Property(param, nameof(Mae_Puesto.IndManOpe));
                    Expression indAutOpeValue = Expression.Constant(request.IndManOpe);
                    Expression indAutOpeEqual = Expression.Equal(indAutOpeProperty, indAutOpeValue);

                    combined = Expression.AndAlso(combined, indAutOpeEqual);
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
                    //Mae_Empresa maeEmpresa = await _contexto.RepositorioMaeEmpresa.Obtener(s => s.CodEmpresa == item.CodEmpresa || s.CodEmpresaOfi == item.CodEmpresa).FirstOrDefaultAsync();
                    Mae_Empresa maeEmpresa = await _contexto.RepositorioMaeEmpresa.Obtener(s => s.CodEmpresa == item.CodEmpresa).FirstOrDefaultAsync();

                    if (maeEmpresa != null)
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
