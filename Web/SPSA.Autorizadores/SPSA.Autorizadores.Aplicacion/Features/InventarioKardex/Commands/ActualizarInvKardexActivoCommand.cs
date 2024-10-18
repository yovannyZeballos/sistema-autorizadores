using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class ActualizarInvKardexActivoCommand : IRequest<RespuestaComunDTO>
    {
        public string Id { get; set; }
        public string Modelo { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Area { get; set; }
        public string Tipo { get; set; }
    }

    public class ActualizarInvKardexActivoHandler : IRequestHandler<ActualizarInvKardexActivoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarInvKardexActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarInvKardexActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invKardex = await _contexto.RepositorioInvKardexActivo.Obtener(x => x.Id == request.Id).FirstOrDefaultAsync();

                if (invKardex is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Registro no existe";
                    return respuesta;
                }

                _mapper.Map(request, invKardex);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Registro actualizado exitosamente.";
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
