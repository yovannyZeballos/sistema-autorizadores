using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class ActualizarInvKardexCommand : IRequest<RespuestaComunDTO>
    {
        public int Id { get; set; }
        public string Kardex { get; set; }
        public DateTime Fecha { get; set; }
        public string Guia { get; set; }
        public string ActivoId { get; set; }
        public string Serie { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Tk { get; set; }
        public int Cantidad { get; set; }
        public string TipoStock { get; set; }
        public string Oc { get; set; }
        public string Sociedad { get; set; }
    }

    public class ActualizarInvKardexHandler : IRequestHandler<ActualizarInvKardexCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarInvKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarInvKardexCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invKardex = await _contexto.RepositorioInvKardex.Obtener(x => x.Id == request.Id).FirstOrDefaultAsync();

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
