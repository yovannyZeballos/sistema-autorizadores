using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaDespacho
{
    public class ObtenerGuiaDespachoQuery : IRequest<GenericResponseDTO<GuiaDespachoCabeceraDto>>
    {
        public long Id { get; set; }
    }

    public class ObtenerGuiaDespachoHandler : IRequestHandler<ObtenerGuiaDespachoQuery, GenericResponseDTO<GuiaDespachoCabeceraDto>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerGuiaDespachoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<GuiaDespachoCabeceraDto>> Handle(ObtenerGuiaDespachoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<GuiaDespachoCabeceraDto> { Ok = true };
            try
            {
                var gd = await _contexto.RepositorioGuiaDespachoCabecera
                    .Obtener(g => g.Id == request.Id)
                    .Include(g => g.Detalles.Select(d => d.SerieProducto))
                    .FirstOrDefaultAsync(cancellationToken);

                if (gd == null)
                {
                    response.Ok = false;
                    response.Mensaje = "La guía de despacho no existe.";
                    return response;
                }

                // Distintos códigos de producto del detalle
                var cods = gd.Detalles
                    .Select(d => d.CodProducto)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var productos = await _contexto.RepositorioMaeProducto
                    .Obtener(p => cods.Contains(p.CodProducto))
                    .Include(p => p.Marca)
                    .Select(p => new
                    {
                        p.CodProducto,
                        p.DesProducto,
                        p.Marca.NomMarca,
                        p.NomModelo
                    })
                    .ToListAsync(cancellationToken);

                var dicProd = productos.ToDictionary(x => x.CodProducto, StringComparer.OrdinalIgnoreCase);

                // === Mapear detalle con datos de producto ===
                var detalleDtos = gd.Detalles.Select(item =>
                {
                    dicProd.TryGetValue(item.CodProducto ?? string.Empty, out var prod);

                    return new GuiaDespachoDetalleDto
                    {
                        Id = item.Id,
                        CodProducto = item.CodProducto,
                        DesProducto = prod?.DesProducto + " " + prod?.NomMarca + " " + prod?.NomModelo,
                        Cantidad = item.Cantidad,
                        CantidadConfirmada = item.CantidadConfirmada,
                        CodActivo = item.CodActivo,
                        Observaciones = item.Observaciones,
                        NumSerie = item.SerieProducto?.NumSerie,
                        EsSerializable = item.SerieProductoId.HasValue
                    };
                }).ToList();


                var localOrigen = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == gd.CodEmpresaOrigen && p.CodLocal == gd.CodLocalOrigen)
                                                                            .Select(p => p.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                var localDestino = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == gd.CodEmpresaDestino && p.CodLocal == gd.CodLocalDestino)
                                                                        .Select(p => p.NomLocal)
                                                                        .FirstOrDefaultAsync() ?? string.Empty;


                var dto = new GuiaDespachoCabeceraDto
                {
                    Id = gd.Id,
                    Fecha = gd.Fecha,
                    NumGuia = gd.NumGuia,
                    CodEmpresaOrigen = gd.CodEmpresaOrigen,
                    CodLocalOrigen = gd.CodLocalOrigen,
                    NomLocalOrigen = localOrigen,
                    CodEmpresaDestino = gd.CodEmpresaDestino,
                    CodLocalDestino = gd.CodLocalDestino,
                    NomLocalDestino = localDestino,
                    TipoMovimiento = gd.TipoMovimiento,
                    IndEstado = gd.IndEstado,
                    AreaGestion = gd.AreaGestion,
                    ClaseStock = gd.ClaseStock,
                    EstadoStock = gd.EstadoStock,
                    Observaciones = gd.Observaciones,
                    IndConfirmacion = gd.IndConfirmacion,
                    FecConfirmacion = gd.FecConfirmacion,
                    UsuCreacion = gd.UsuCreacion,
                    Detalles = detalleDtos
                };


                response.Data = dto;
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
