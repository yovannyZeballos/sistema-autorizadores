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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Proveedor
{
    public class ActualizarMaeProveedorCommand : IRequest<RespuestaComunDTO>
    {
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string IndActivo { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMaeProveedorHandler : IRequestHandler<ActualizarMaeProveedorCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeProveedorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeProveedorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var proveedor = await _contexto.RepositorioMaeProveedor
                    .Obtener(x => x.Ruc == request.Ruc)
                    .FirstOrDefaultAsync(); ;

                if (proveedor is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra proveedor con este ruc.";
                    return respuesta;
                }

                proveedor.RazonSocial = request.RazonSocial.ToUpper();
                proveedor.IndActivo = request.IndActivo;
                proveedor.UsuModifica = request.UsuModifica;
                proveedor.FecModifica = DateTime.Now;

                _contexto.RepositorioMaeProveedor.Actualizar(proveedor);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Proveedor actualizado exitosamente.";
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
