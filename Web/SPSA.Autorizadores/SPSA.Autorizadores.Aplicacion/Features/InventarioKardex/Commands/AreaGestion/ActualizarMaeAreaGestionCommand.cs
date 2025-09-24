using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.AreaGestion
{
    public class ActualizarMaeAreaGestionCommand : IRequest<RespuestaComunDTO>
    {
        public long Id { get; set; }
        public string NomAreaGestion { get; set; }
        public string IndActivo { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMaeAreaGestionHandler : IRequestHandler<ActualizarMaeAreaGestionCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeAreaGestionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeAreaGestionCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var area = await _contexto.RepositorioMaeAreaGestion
                    .Obtener(x =>
                        x.Id == request.Id)
                    .FirstOrDefaultAsync(); ;

                if (area is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra area de gestión con este código.";
                    return respuesta;
                }

                area.NomAreaGestion = request.NomAreaGestion.ToUpper();
                area.IndActivo = request.IndActivo;
                area.UsuModifica = request.UsuModifica;
                area.FecModifica = DateTime.Now;


                _contexto.RepositorioMaeAreaGestion.Actualizar(area);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Area Gestión actualizado exitosamente.";
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
