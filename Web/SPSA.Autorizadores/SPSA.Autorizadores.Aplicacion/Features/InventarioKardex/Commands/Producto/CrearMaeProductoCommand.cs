using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Producto
{
    public class CrearMaeProductoCommand : IRequest<RespuestaComunDTO>
    {
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }
        public int MarcaId { get; set; }
        public string TipProducto { get; set; }
        public int AreaGestionId { get; set; }
        public string IndActivo { get; set; }
        public string IndSerializable { get; set; }
        public decimal StkMinimo { get; set; }
        public decimal StkMaximo { get; set; }
        public string NomModelo { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearMaeProductoHandler : IRequestHandler<CrearMaeProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeProductoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var entidad = _mapper.Map<Mae_Producto>(request);
                entidad.FecCreacion = DateTime.Now;

                _contexto.RepositorioMaeProducto.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Producto creado exitosamente.";
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
