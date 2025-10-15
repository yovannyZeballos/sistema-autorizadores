using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaRecepcion
{
    public class ObtenerGuiaRecepcionQuery : IRequest<GenericResponseDTO<GuiaRecepcionCabeceraDto>>
    {
        public long Id { get; set; }
    }

    public class ObtenerGuiaRecepcionHandler : IRequestHandler<ObtenerGuiaRecepcionQuery, GenericResponseDTO<GuiaRecepcionCabeceraDto>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerGuiaRecepcionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<GuiaRecepcionCabeceraDto>> Handle(ObtenerGuiaRecepcionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<GuiaRecepcionCabeceraDto> { Ok = true };
            try
            {
                var gr = await _contexto.RepositorioGuiaRecepcionCabecera
                    .Obtener(g => g.Id == request.Id)
                    .Include(g => g.Detalles.Select(d => d.SerieProducto))
                    .FirstOrDefaultAsync(cancellationToken);

                if (gr == null)
                {
                    response.Ok = false;
                    response.Mensaje = "La guía de recepción no existe.";
                    return response;
                }

                // Distintos códigos de producto del detalle
                var cods = gr.Detalles
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
                var detalleDtos = gr.Detalles.Select(item =>
                {
                    dicProd.TryGetValue(item.CodProducto ?? string.Empty, out var prod);

                    return new GuiaRecepcionDetalleDto
                    {
                        Id = item.Id,
                        CodProducto = item.CodProducto,
                        DesProducto = prod?.DesProducto + " " + prod?.NomMarca + " " + prod?.NomModelo,
                        Cantidad = item.Cantidad,
                        CodActivo = item.CodActivo,
                        Observaciones = item.Observaciones,
                        //StkEstado = item.SerieProducto?.StkEstado,
                        StkEstado = item.StkEstado,
                        NumSerie = item.SerieProducto?.NumSerie,
                        EsSerializable = item.SerieProductoId.HasValue
                    };
                }).ToList();

                var razonSocial = await _contexto.RepositorioMaeProveedor.Obtener(p => p.Ruc == gr.ProveedorRuc)
                                                                            .Select(p => p.RazonSocial)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;

                var localOrigen = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == gr.CodEmpresaOrigen && p.CodLocal == gr.CodLocalOrigen)
                                                                            .Select(p => p.NomLocal)
                                                                            .FirstOrDefaultAsync() ?? string.Empty;
                var localDestino = await _contexto.RepositorioMaeLocal.Obtener(p => p.CodEmpresa == gr.CodEmpresaDestino && p.CodLocal == gr.CodLocalDestino)
                                                                        .Select(p => p.NomLocal)
                                                                        .FirstOrDefaultAsync() ?? string.Empty;

                var nomLocalOrigen = (!string.IsNullOrWhiteSpace(gr.ProveedorRuc) && !string.IsNullOrWhiteSpace(razonSocial))
                        ? razonSocial
                        : localOrigen;


                var dto = new GuiaRecepcionCabeceraDto
                {
                    Id = gr.Id,
                    Fecha = gr.Fecha,
                    NumGuia = gr.NumGuia,
                    Proveedor = razonSocial ?? string.Empty,
                    CodEmpresaOrigen = gr.CodEmpresaOrigen,
                    CodLocalOrigen = gr.CodLocalOrigen,
                    NomLocalOrigen = nomLocalOrigen,
                    CodEmpresaDestino = gr.CodEmpresaDestino,
                    CodLocalDestino = gr.CodLocalDestino,
                    NomLocalDestino = localDestino,
                    IndTransferencia = gr.IndTransferencia,
                    IndEstado = gr.IndEstado,
                    AreaGestion = gr.AreaGestion,
                    ClaseStock = gr.ClaseStock,
                    Observaciones = gr.Observaciones,
                    UsuCreacion = gr.UsuCreacion,
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
