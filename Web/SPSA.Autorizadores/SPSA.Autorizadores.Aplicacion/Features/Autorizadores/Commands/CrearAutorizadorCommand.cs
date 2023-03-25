using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
    public class CrearAutorizadorCommand : IRequest<RespuestaComunDTO>
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public int CodLocal { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
    }

    public class CrearAutorizadorHandler : IRequestHandler<CrearAutorizadorCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public CrearAutorizadorHandler(IRepositorioAutorizadores repositorioAutorizadores)
        {
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<RespuestaComunDTO> Handle(CrearAutorizadorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();
            try
            {
                var autorizador = new Autorizador(request.Codigo, request.Nombres, request.ApellidoPaterno, request.ApellidoMaterno, request.NumeroDocumento, request.CodLocal, request.UsuarioCreacion);
                await _repositorioAutorizadores.Crear(autorizador);

                respuesta.Mensaje = $"Autorizador con DNI {request.NumeroDocumento} creado";
                respuesta.Ok = true;
            }
            catch (Exception)
            {
                respuesta.Mensaje = $"Error al crear el autorizador con DNI {request.Codigo}";
            }

            return respuesta;
        }
    }
}
