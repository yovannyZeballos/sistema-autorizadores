using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen.Dto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries
{
    public class ConsultarClienteCenQuery : IRequest<GenericResponseDTO<ConsultaClienteRespuesta>>
	{
		public string TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }
	}

	public class ConsultarClienteCenQueryHandler : IRequestHandler<ConsultarClienteCenQuery, GenericResponseDTO<ConsultaClienteRespuesta>>
	{
		private readonly IAgenteCen _agenteCen;
		private readonly ILogger _logger;

		public ConsultarClienteCenQueryHandler(IAgenteCen agenteCen)
		{
			_agenteCen = agenteCen;
			_logger = SerilogClass._log;
		}
		public async Task<GenericResponseDTO<ConsultaClienteRespuesta>> Handle(ConsultarClienteCenQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new GenericResponseDTO<ConsultaClienteRespuesta> { Ok = true };

			var recurso = new ConsultaClienteRecurso
			{
				TipoDocumento = request.TipoDocumento,
				NumeroDocumento = request.NumeroDocumento
			};

			try
			{
				respuesta.Data = await _agenteCen.ConsultarCliente(recurso);

				if (respuesta.Data == null)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "Cliente no encontrado en CEN. Complete los datos para registrar como nuevo.";
				}
			}
			catch (Exception ex)
			{

				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrio un error al consultar el cliente en CEN";
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}
	}
}
