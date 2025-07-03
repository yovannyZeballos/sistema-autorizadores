using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Vml;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.FactoresMdr
{
    public class ListarMdrFactorIzipayQuery : IRequest<GenericResponseDTO<PagedResult<ListarMdrFactorDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string CodEmpresa { get; set; }
        public int CodPeriodo { get; set; }
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
    }

    public class ListarMdrFactorIzipayHandler : IRequestHandler<ListarMdrFactorIzipayQuery, GenericResponseDTO<PagedResult<ListarMdrFactorDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMdrFactorIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMdrFactorDto>>> Handle(ListarMdrFactorIzipayQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMdrFactorDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMdrFactorDto>()
            };

            try
            {
                ParameterExpression param = Expression.Parameter(typeof(Mdr_FactorIzipay), "x");
                Expression combined = Expression.Constant(true);

                if (!string.IsNullOrEmpty(request.CodEmpresa) && request.CodEmpresa != "0")
                {
                    Expression prop = Expression.Property(param, nameof(Mdr_FactorIzipay.CodEmpresa));
                    Expression val = Expression.Constant(request.CodEmpresa);

                    combined = Expression.AndAlso(combined, Expression.Equal(prop, val));
                }

                if (request.CodPeriodo != 0)
                {
                    Expression prop = Expression.Property(param, nameof(Mdr_FactorIzipay.CodPeriodo));
                    Expression val  = Expression.Constant(request.CodPeriodo);

                    combined = Expression.AndAlso(combined, Expression.Equal(prop, val));
                }

                if (!string.IsNullOrEmpty(request.CodOperador) && request.CodOperador != "0")
                {
                    Expression prop = Expression.Property(param, nameof(Mdr_FactorIzipay.CodOperador));
                    Expression val  = Expression.Constant(request.CodOperador);

                    combined = Expression.AndAlso(combined, Expression.Equal(prop, val));
                }

                if (!string.IsNullOrEmpty(request.CodClasificacion) && request.CodClasificacion != "0")
                {
                    Expression prop = Expression.Property(param, nameof(Mdr_FactorIzipay.CodClasificacion));
                    Expression val  = Expression.Constant(request.CodClasificacion);

                    combined = Expression.AndAlso(combined, Expression.Equal(prop, val));
                }

                Expression<Func<Mdr_FactorIzipay, bool>> predicate = Expression.Lambda<Func<Mdr_FactorIzipay, bool>>(combined, param);

                var pagedFactores = await _contexto.RepositorioMdrFactorIzipay.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => x.CodEmpresa,
                    ascending: true,
                    includes: new Expression<Func<Mdr_FactorIzipay, object>>[]
                    {
                        f => f.Empresa,
                        f => f.Operador,
                        f => f.Clasificacion
                    }
                    );

                var mappedItems = _mapper.Map<List<ListarMdrFactorDto>>(pagedFactores.Items);

                var pagedResult = new PagedResult<ListarMdrFactorDto>
                {
                    PageNumber = pagedFactores.PageNumber,
                    PageSize = pagedFactores.PageSize,
                    TotalRecords = pagedFactores.TotalRecords,
                    TotalPages = pagedFactores.TotalPages,
                    Items = mappedItems
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
