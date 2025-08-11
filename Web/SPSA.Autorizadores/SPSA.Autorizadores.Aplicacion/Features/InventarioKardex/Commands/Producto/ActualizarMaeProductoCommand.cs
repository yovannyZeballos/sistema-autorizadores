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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Producto
{
    public class ActualizarMaeProductoCommand : IRequest<RespuestaComunDTO>
    {
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }
        public int MarcaId { get; set; }
        public string TipProducto { get; set; }
        public int AreaGestionId { get; set; }
        public string IndActivo { get; set; }
        public decimal StkMinimo { get; set; }
        public decimal StkMaximo { get; set; }
        public string NomModelo { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMaeProductoHandler : IRequestHandler<ActualizarMaeProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeProductoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var producto = await _contexto.RepositorioMaeProducto
                    .Obtener(x =>
                        x.CodProducto == request.CodProducto)
                    .FirstOrDefaultAsync(); ;

                if (producto is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra producto con este código.";
                    return respuesta;
                }

                producto.DesProducto = request.DesProducto.ToUpper();
                producto.TipProducto = request.TipProducto;
                producto.MarcaId = request.MarcaId;
                producto.IndActivo = request.IndActivo;
                producto.StkMinimo = request.StkMinimo;
                producto.StkMaximo = request.StkMaximo;
                producto.NomModelo = request.NomModelo;
                producto.UsuModifica = request.UsuModifica;
                producto.FecModifica = DateTime.Now;

                _contexto.RepositorioMaeProducto.Actualizar(producto);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Producto actualizado exitosamente.";
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
