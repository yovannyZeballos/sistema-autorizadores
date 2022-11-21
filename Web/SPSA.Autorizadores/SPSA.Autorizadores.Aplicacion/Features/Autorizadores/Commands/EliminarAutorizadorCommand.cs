using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
    public class EliminarAutorizadorCommand : IRequest<RespuestaComunDTO>
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoAutorizador { get; set; } = string.Empty;
        public string UsuarioCreacion { get; set; } = string.Empty;
    }

    public class EliminarAutorizadorHandler : IRequestHandler<EliminarAutorizadorCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public EliminarAutorizadorHandler(IRepositorioAutorizadores repositorioAutorizadores)
        {
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarAutorizadorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();
            try
            {
                var autorizador = new Autorizador(request.Codigo, request.UsuarioCreacion, request.CodigoAutorizador);
                await _repositorioAutorizadores.Eliminar(autorizador);

                respuesta.Mensaje = $"Se cambió de estado al Autorizador {request.CodigoAutorizador}";
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al cambiar el estado al autorizador {request.Codigo} | {ex.Message}";
            }

            return respuesta;
        }
    }
}
