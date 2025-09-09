using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands
{
    public class ActualizarMaeColaboradorExtCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa{ get; set; }
        public string CodLocal { get; set; }
        public string CodigoOfisis { get; set; }
        public DateTime FechaIngresoEmpresa { get; set; }
        public DateTime? FechaCeseTrabajador { get; set; }
        public string PuestoTrabajo { get; set; }
        public string MotiSepa { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMaeColaboradorExtHandler : IRequestHandler<ActualizarMaeColaboradorExtCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = Logger.SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {

                var colabExt = await _contexto.RepositorioMaeColaboradorExt
                    .Obtener( x => x.CodEmpresa == request.CodEmpresa && x.CodLocal == request.CodLocal && x.CodigoOfisis == request.CodigoOfisis)
                    .FirstOrDefaultAsync(); ;

                if (colabExt is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra colaborador externo seleccionado.";
                    return respuesta;
                }

                if (request.FechaCeseTrabajador != null)
                {
                    colabExt.FechaCeseTrabajador = request.FechaCeseTrabajador;
                    colabExt.IndActivo = "N";
                }
                
                colabExt.PuestoTrabajo = request.PuestoTrabajo;
                colabExt.MotiSepa = request.MotiSepa;
                colabExt.UsuModifica = request.UsuModifica;
                colabExt.FecModifica = DateTime.Now;

                _contexto.RepositorioMaeColaboradorExt.Actualizar(colabExt);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Registro actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, "Error al actualizar colaborador externo: " + ex.Message);
            }

            return respuesta;
        }
    }
}
