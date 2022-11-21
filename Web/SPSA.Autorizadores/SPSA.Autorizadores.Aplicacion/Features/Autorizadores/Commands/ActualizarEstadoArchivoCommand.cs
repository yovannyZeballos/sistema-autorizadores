using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
    public class ActualizarEstadoArchivoCommand : IRequest<RespuestaComunDTO>
    {
        public string Codigo { get; set; }
        public int CodLocal { get; set; }
        public string CodigoAutorizador { get; set; }
        public string NumeroTarjeta { get; set; }


        public class ActualizarEstadoArchivoHandler : IRequestHandler<ActualizarEstadoArchivoCommand, RespuestaComunDTO>
        {
            private readonly IRepositorioAutorizadores _repositorioAutorizadores;

            public ActualizarEstadoArchivoHandler(IRepositorioAutorizadores repositorioAutorizadores)
            {
                _repositorioAutorizadores = repositorioAutorizadores;
            }

            public async Task<RespuestaComunDTO> Handle(ActualizarEstadoArchivoCommand request, CancellationToken cancellationToken)
            {
                var respuesta = new RespuestaComunDTO();
                try
                {
                    var autorizador = new Autorizador(request.Codigo, request.CodLocal, request.CodigoAutorizador, request.NumeroTarjeta);
                    await _repositorioAutorizadores.ActualizarEstadoArchivo(autorizador);
                    respuesta.Ok = true;
                }
                catch (Exception ex)
                {
                    respuesta.Mensaje = $"Error al actualizar estado archivo {ex.Message}";
                    respuesta.Ok = false;
                }

                return respuesta;
            }
        }
    }
}