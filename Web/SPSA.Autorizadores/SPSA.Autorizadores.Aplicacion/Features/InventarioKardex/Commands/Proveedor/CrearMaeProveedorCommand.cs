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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Proveedor
{
    public class CrearMaeProveedorCommand : IRequest<RespuestaComunDTO>
    {
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearMaeProveedorHandler : IRequestHandler<CrearMaeProveedorCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeProveedorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeProveedorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var entidad = _mapper.Map<Mae_Proveedor>(request);
                entidad.FecCreacion = DateTime.Now;

                _contexto.RepositorioMaeProveedor.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Proveedor creado exitosamente.";
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
