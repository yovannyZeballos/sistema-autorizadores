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
                var despacho = await _contexto.RepositorioGuiaDespachoCabecera
                    .Obtener(g => g.Id == request.Id)
                    .Include(g => g.Detalles.Select(d => d.SerieProducto))
                    .FirstOrDefaultAsync(cancellationToken);

                if (despacho == null)
                {
                    response.Ok = false;
                    response.Mensaje = "La guía de despacho no existe.";
                    return response;
                }

                var despachoDto = _mapper.Map<GuiaDespachoCabeceraDto>(despacho);

                //// Si tu perfil de AutoMapper ya mapea Detalles y NumSerie desde SerieProducto.NumSerie,
                //// esto no sería necesario. Si no, completamos manualmente NumSerie en el DTO.
                //if (despachoDto.Detalles != null && despachoDto.Detalles.Count > 0)
                //{
                //    // Creamos un diccionario (IdDetalle -> NumSerie) para completar el DTO de manera segura.
                //    var numSeriePorDetId = despacho.Detalles
                //        .Where(d => d.SerieProducto != null)
                //        .ToDictionary(d => d.Id, d => d.SerieProducto.NumSerie);

                //    foreach (var detDto in despachoDto.Detalles)
                //    {
                //        // Si tu GuiaDespachoDetalleDto expone Id, lo usamos para casar.
                //        // Si no expone Id, puedes casar por (CodProducto, Cantidad, CodActivo, etc.)
                //        if (detDto.Id != 0 && numSeriePorDetId.TryGetValue(detDto.Id, out var numSerie))
                //            detDto.NumSerie = numSerie;
                //        // En caso de no tener Id en el DTO, ejemplo alternativo:
                //        // var ent = despacho.Detalles.FirstOrDefault(x => x.CodProducto == detDto.CodProducto && x.Cantidad == detDto.Cantidad && x.CodActivo == detDto.CodActivo);
                //        // detDto.NumSerie = ent?.SerieProducto?.NumSerie;
                //    }
                //}

                response.Data = despachoDto;
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
