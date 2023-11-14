using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Commands
{
	public class EliminarCajeroVolanteCommand : IRequest<RespuestaComunDTO>
	{
		public string CodOfisis { get; set; }
		public decimal CodSede { get; set; }
		public string CodEmpresa { get; set; }
		public string Usuario { get; set; }
	}

	public class EliminarCajeroVolanteHandler : IRequestHandler<EliminarCajeroVolanteCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public EliminarCajeroVolanteHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<RespuestaComunDTO> Handle(EliminarCajeroVolanteCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();
			try
			{
				await _repositorioCajero.EliminarCajeroVolante(new CajeroVolante(request.CodOfisis, request.CodEmpresa, request.CodSede, request.Usuario));
				respuesta.Ok = true;
			}
			catch (Exception ex)
			{
				respuesta.Mensaje = $"Error al eliminar el cajero: {request.CodOfisis} para el local {request.CodSede} | {ex.Message}";
				respuesta.Ok = false;
			}

			return respuesta;
		}
	}
}
