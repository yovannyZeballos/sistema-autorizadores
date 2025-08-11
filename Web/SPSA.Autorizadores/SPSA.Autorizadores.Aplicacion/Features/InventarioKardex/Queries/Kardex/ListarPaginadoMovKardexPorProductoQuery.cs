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
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Kardex;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Kardex
{
    public class ListarPaginadoMovKardexPorProductoQuery : IRequest<GenericResponseDTO<PagedResult<ListarMovKardexDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string CodProducto { get; set; } // único filtro
    }

    public class ListarPaginadoMovKardexPorProductoHandler : IRequestHandler<ListarPaginadoMovKardexPorProductoQuery, GenericResponseDTO<PagedResult<ListarMovKardexDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarPaginadoMovKardexPorProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ListarMovKardexDto>>> Handle(ListarPaginadoMovKardexPorProductoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<PagedResult<ListarMovKardexDto>>
            {
                Ok = true,
                Data = new PagedResult<ListarMovKardexDto>()
            };

            try
            {
                // Rechaza scan si no envían producto
                if (string.IsNullOrWhiteSpace(request.CodProducto))
                {
                    response.Data = new PagedResult<ListarMovKardexDto>
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0,
                        Items = new List<ListarMovKardexDto>()
                    };
                    return response;
                }

                Expression<Func<Mov_Kardex, bool>> predicate = x => x.SerieProducto.CodProducto == request.CodProducto;

                var pagedRegistros = await _contexto.RepositorioMovKardex.ObtenerPaginado(
                    predicado: predicate,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    orderBy: x => new { x.Id },
                    ascending: false,
                    includes: k => k.SerieProducto
                );

                var dtoItems = new List<ListarMovKardexDto>(pagedRegistros.Items.Count);
                foreach (var k in pagedRegistros.Items)
                {
                    var prod = k.SerieProducto?.Producto;
                    var marca = prod?.Marca?.NomMarca;
                    var desProducto = string.Join(" - ",
                        new[] { prod?.DesProducto, marca, prod?.NomModelo }.Where(s => !string.IsNullOrWhiteSpace(s)));

                    var nomLocalOri = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == k.CodEmpresaOrigen && x.CodLocal == k.CodLocalOrigen)
                                                                            .Select(x => x.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                    var nomLocalDes = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == k.CodEmpresaDestino && x.CodLocal == k.CodLocalDestino)
                                                                            .Select(x => x.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;

                    dtoItems.Add(new ListarMovKardexDto
                    {
                        Id = k.Id,
                        Fecha = k.Fecha,
                        TipoMovimiento = k.TipoMovimiento,
                        SerieProductoId = k.SerieProductoId,
                        DesProducto = desProducto,
                        NumSerie = k.SerieProducto?.NumSerie,
                        DesAreaGestion = k.DesAreaGestion,
                        NumGuia = k.NumGuia,
                        CodActivo = k.CodActivo,
                        CodEmpresaOrigen = k.CodEmpresaOrigen,
                        CodLocalOrigen = k.CodLocalOrigen,
                        LocalOrigen = nomLocalOri,
                        CodEmpresaDestino = k.CodEmpresaDestino,
                        CodLocalDestino = k.CodLocalDestino,
                        LocalDestino = nomLocalDes,
                        Observaciones = k.Observaciones,
                        UsuCreacion = k.UsuCreacion,
                        FecCreacion = k.FecCreacion
                    });
                }

                var pagedResult = new PagedResult<ListarMovKardexDto>
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
