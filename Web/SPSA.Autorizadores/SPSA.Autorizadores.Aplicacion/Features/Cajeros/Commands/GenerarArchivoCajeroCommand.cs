using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands
{
	public class GenerarArchivoCajeroCommand : IRequest<RespuestaComunDTO>
	{
		public string TipoSO { get; set; }
		public string CodLocal { get; set; }
		public List<string> CodCajeros { get; set; }
	}

	public class GenerarArchivoCajeroHandler : IRequestHandler<GenerarArchivoCajeroCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public GenerarArchivoCajeroHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<RespuestaComunDTO> Handle(GenerarArchivoCajeroCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();
			try
			{
				foreach (var cajero in request.CodCajeros)
					await _repositorioCajero.ActualizarEstado(request.CodLocal, cajero);

				var msg = await _repositorioCajero.GenerarArchivo(request.CodLocal, request.TipoSO);
				respuesta.Mensaje = string.IsNullOrEmpty(msg) ? "No se generó ningun archivo" : $"Archivo Generado \n {(msg.Split('|').Count() == 0 ? "" : $"{msg.Split('|')[0]}/{msg.Split('|')[1]}")}";
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
