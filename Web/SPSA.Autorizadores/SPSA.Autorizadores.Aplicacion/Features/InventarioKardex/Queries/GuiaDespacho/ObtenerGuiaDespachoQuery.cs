using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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

                var detalleDtos = gd.Detalles.Select(item => new GuiaDespachoDetalleDto
                {
                    Id = item.Id,
                    CodProducto = item.CodProducto,
                    Cantidad = item.Cantidad,
                    CodActivo = item.CodActivo,
                    Observaciones = item.Observaciones,
                    NumSerie = item.SerieProducto?.NumSerie,
                    // Nuevo: lo exponemos explícito
                    EsSerializable = item.SerieProductoId.HasValue,
                    // Si aún no llevas parciales en BD, deja 0 y el front lo interpreta como todo pendiente
                    CantidadConfirmada = 0
                }).ToList();


                var dto = new GuiaDespachoCabeceraDto
                {
                    Id = gd.Id,
                    Fecha = gd.Fecha,
                    NumGuia = gd.NumGuia,
                    CodEmpresaOrigen = gd.CodEmpresaOrigen,
                    CodLocalOrigen = gd.CodLocalOrigen,
                    // FIX: aquí estaba el error
                    CodEmpresaDestino = gd.CodEmpresaDestino,
                    CodLocalDestino = gd.CodLocalDestino,
                    TipoMovimiento = gd.TipoMovimiento,
                    IndEstado = gd.IndEstado,
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
