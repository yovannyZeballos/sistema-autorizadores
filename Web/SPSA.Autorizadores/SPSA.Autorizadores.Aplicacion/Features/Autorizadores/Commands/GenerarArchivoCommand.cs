using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
    public class GenerarArchivoCommand : IRequest<RespuestaComunDTO>
    {
        public string TipoSO { get; set; }
    }

    public class GenerarArchivoHandler : IRequestHandler<GenerarArchivoCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public GenerarArchivoHandler(IRepositorioAutorizadores repositorioAutorizadores)
        {
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<RespuestaComunDTO> Handle(GenerarArchivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();
            try
            {
                respuesta.Mensaje = await _repositorioAutorizadores.GenerarArchivo(request.TipoSO);
                respuesta.Ok = true;
            }
            catch (System.Exception ex)
            {
                respuesta.Mensaje = ex.Message;
                respuesta.Ok = false;
            }

            return respuesta;
        }
    }
}
