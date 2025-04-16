using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class ActualizarMotivoRechazoCommand : IRequest<RespuestaComunDTO>
	{
		public List<int> NumSolicitudes { get; set; }
		public string Motivo { get; set; }
		public string Estado { get; set; }
	}

	public class ActualizarMotivoRechazoHandler : IRequestHandler<ActualizarMotivoRechazoCommand, RespuestaComunDTO>
	{
		public async Task<RespuestaComunDTO> Handle(ActualizarMotivoRechazoCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true, Mensaje = "Solicitud rechazada" };
			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					foreach (var numSolicitud in request.NumSolicitudes)
					{
						await contexto.RepositorioSolicitudUsuarioASR.ActualizarMotivoRechazo(numSolicitud, request.Motivo, request.Estado);
					}
					await contexto.GuardarCambiosAsync();
				}
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}
			return respuesta;
		}
	}
}
