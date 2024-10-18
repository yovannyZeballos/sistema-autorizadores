using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class CrearInvKardexActivoCommand : IRequest<RespuestaComunDTO>
    {
        public string Id { get; set; }
        public string Modelo { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Area { get; set; }
        public string Tipo { get; set; }
    }

    public class CrearInvKardexActivoHandler : IRequestHandler<CrearInvKardexActivoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearInvKardexActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearInvKardexActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invKardex = _mapper.Map<InvKardexActivo>(request);
                _contexto.RepositorioInvKardexActivo.Agregar(invKardex);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Registro ingresado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear";
                _logger.Error(ex, "Ocurrió un error al crear KADEX ACTIVO");
            }
            return respuesta;
        }
    }
}
