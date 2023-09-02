using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands
{
	public class ActualizarPuestoCommand : IRequest<RespuestaComunDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodPuesto { get; set; }
		public string IndAutorizador { get; set; }
		public string IndCajero { get; set; }
		public string IndAutoCajero { get; set; }
		public string IndAutoAutorizador { get; set; }
	}

	public class ActualizarPuestoHandler : IRequestHandler<ActualizarPuestoCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioPuesto _repositorioPuesto;

		public ActualizarPuestoHandler(IRepositorioPuesto repositorioPuesto)
		{
			_repositorioPuesto = repositorioPuesto;
		}

		public async Task<RespuestaComunDTO> Handle(ActualizarPuestoCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();
			try
			{
				var puesto = new Puesto(request.CodEmpresa, request.CodPuesto, request.IndAutorizador, 
					request.IndCajero, request.IndAutoCajero, request.IndAutoAutorizador);

				await _repositorioPuesto.Actualizar(puesto);
				respuesta.Ok = true;
			}
			catch (Exception)
			{
				respuesta.Mensaje = $"Error al actualozar el puesto {request.CodPuesto}";
			}

			return respuesta;
		}
	}
}
