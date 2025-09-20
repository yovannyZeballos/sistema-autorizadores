using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen.Dto;
using Stimulsoft.Base.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Operaciones.Commands
{
    public class InsertarClienteCenCommand : IRequest<RespuestaComunDTO>
	{
		public string TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }
		public string Nombres { get; set; }
		public string Apellidos { get; set; }
		public string RazonSocial { get; set; }
		public string Usuario { get; set; }
	}

	public class InsertarClienteCenHandler : IRequestHandler<InsertarClienteCenCommand, RespuestaComunDTO>
	{
		private readonly ILogger _logger;
		private readonly IAgenteCen _agenteCen;
		public InsertarClienteCenHandler(IAgenteCen agenteCen)
		{
			_agenteCen = agenteCen;
			_logger = SerilogClass._log;
		}

		public async Task<RespuestaComunDTO> Handle(InsertarClienteCenCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true, Mensaje = "Cliente registrado correctamente en CEN." };
			try
			{
				var recurso = new InsertarClienteRecurso
				{
					TipoDocumento = request.TipoDocumento,
					NumeroDocumento = request.NumeroDocumento,
					Nombres = request.Nombres,
					Apellidos = request.Apellidos,
					RazonSocial = request.RazonSocial,
					UsuarioCreacion = request.Usuario,
					Sistema = "SGP"
				};
				await _agenteCen.InsertarCliente(recurso);
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al registrar el cliente en CEN.";
				_logger.Error(ex, respuesta.Mensaje);
			}
			return respuesta;
		}
	}
}
