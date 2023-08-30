using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands
{
	public class GenerarArchivoCajeroCommand : IRequest<RespuestaComunDTO>
	{
		public string TipoSO { get; set; }
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
				respuesta.Mensaje = await _repositorioCajero.GenerarArchivo(request.TipoSO);
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
