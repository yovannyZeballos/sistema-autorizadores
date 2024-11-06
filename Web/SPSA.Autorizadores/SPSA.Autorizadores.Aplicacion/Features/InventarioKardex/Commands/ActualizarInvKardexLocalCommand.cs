using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class ActualizarInvKardexLocalCommand : IRequest<RespuestaComunDTO>
    {
        public int Id { get; set; }
        public string Sociedad { get; set; }
        public string NomLocal { get; set; }
    }

    public class ActualizarInvKardexLocalHandler : IRequestHandler<ActualizarInvKardexLocalCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarInvKardexLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarInvKardexLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invKardexLocal = await _contexto.RepositorioInvKardexLocal.Obtener(x => x.Id == request.Id).FirstOrDefaultAsync();

                if (invKardexLocal is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Registro no existe";
                    return respuesta;
                }

                _mapper.Map(request, invKardexLocal);
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
