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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.AreaGestion
{
    public class CrearMaeAreaGestionCommand : IRequest<RespuestaComunDTO>
    {
        public string Id { get; set; }
        public string NomAreaGestion { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearMaeAreaGestionHandler : IRequestHandler<CrearMaeAreaGestionCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeAreaGestionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeAreaGestionCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var entidad = _mapper.Map<Mae_AreaGestion>(request);
                entidad.FecCreacion = DateTime.Now;

                _contexto.RepositorioMaeAreaGestion.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Área gestión creado exitosamente.";
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
