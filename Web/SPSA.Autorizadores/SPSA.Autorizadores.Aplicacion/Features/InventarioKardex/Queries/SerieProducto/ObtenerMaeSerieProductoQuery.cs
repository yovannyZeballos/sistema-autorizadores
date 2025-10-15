using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Data.Entity;
using Serilog;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.SerieProducto
{
    public class ObtenerMaeSerieProductoQuery : IRequest<GenericResponseDTO<MaeSerieProductoDto>>
    {
        public string CodProducto { get; set; }
        public string NumSerie { get; set; }
    }

    public class ObtenerMaeSerieProductoHandler : IRequestHandler<ObtenerMaeSerieProductoQuery, GenericResponseDTO<MaeSerieProductoDto>>
    {
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ObtenerMaeSerieProductoHandler()
        {
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<MaeSerieProductoDto>> Handle(ObtenerMaeSerieProductoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<MaeSerieProductoDto> { Ok = true };
            try
            {
                var codProducto = (request?.CodProducto ?? string.Empty).Trim();
                var numSerie = (request?.NumSerie ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(codProducto) || string.IsNullOrWhiteSpace(numSerie))
                {
                    throw new InvalidOperationException("Producto y serie son obligatorios.");
                }

                var serie = await _contexto.RepositorioMaeSerieProducto
                    .Obtener(sp => sp.CodProducto == request.CodProducto && sp.NumSerie == request.NumSerie)
                    .FirstOrDefaultAsync(cancellationToken);

                if (serie == null)
                {
                    throw new InvalidOperationException("Serie del producto seleccionado no existe.");
                }

                var dto = new MaeSerieProductoDto
                {
                    Id = serie.Id,
                    CodProducto = serie.CodProducto,
                    NumSerie = serie.NumSerie,
                    IndEstado = serie.IndEstado,
                    StkEstado = serie.StkEstado,
                    CodEmpresa = serie.CodEmpresa,
                    CodLocal = serie.CodLocal,
                    StkActual = serie.StkActual,
                    FecIngreso = serie.FecIngreso,
                    FecSalida = serie.FecSalida
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
