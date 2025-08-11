using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.SerieProducto
{
    public class ActualizarMaeSerieProductoCommand : IRequest<RespuestaComunDTO>
    {
        public long Id { get; set; }

        public string CodProducto { get; set; }
        public string NumSerie { get; set; }
        public string IndEstado { get; set; }

        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public int StkActual { get; set; }

        public DateTime? FecIngreso { get; set; }
        public DateTime? FecSalida { get; set; }

        public DateTime FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMaeSerieProductoHandler : IRequestHandler<ActualizarMaeSerieProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeSerieProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeSerieProductoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var serieproducto = await _contexto.RepositorioMaeSerieProducto
                    .Obtener(x =>
                        x.Id == request.Id)
                    .FirstOrDefaultAsync(); ;

                if (serieproducto is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra serie.";
                    return respuesta;
                }

                serieproducto.CodProducto = request.CodProducto;
                serieproducto.NumSerie = request.NumSerie.ToUpper();
                serieproducto.IndEstado = request.IndEstado;
                serieproducto.CodEmpresa = request.CodEmpresa;
                serieproducto.CodLocal = request.CodLocal;
                serieproducto.StkActual = request.StkActual;
                serieproducto.FecIngreso = request.FecIngreso;
                serieproducto.FecSalida = request.FecSalida;
                serieproducto.FecModifica = DateTime.Now;

                _contexto.RepositorioMaeSerieProducto.Actualizar(serieproducto);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Serie actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
