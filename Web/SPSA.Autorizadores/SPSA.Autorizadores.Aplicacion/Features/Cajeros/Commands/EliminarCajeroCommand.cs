using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands
{
	public class EliminarCajeroCommand : IRequest<RespuestaComunDTO>
	{
		public List<string> NroDocumentos { get; set; }
		public string Usuario { get; set; }
		public string TipoSO { get; set; }
		public string CodLocal { get; set; }
	}

	public class EliminarCajeroHandler : IRequestHandler<EliminarCajeroCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public EliminarCajeroHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<RespuestaComunDTO> Handle(EliminarCajeroCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			var mensajes = new StringBuilder();

			foreach (var item in request.NroDocumentos)
			{

				try
				{
					await _repositorioCajero.Eliminar(item, request.Usuario);
					mensajes.AppendLine($"Cajero {item} eliminado");
				}
				catch (Exception ex)
				{
					mensajes.AppendLine($"Ocurrio un error al eliminar el Cajero {item} | {ex.Message}");
				}
			}

			var msgGenrarArchivo = await _repositorioCajero.GenerarArchivo(request.CodLocal, request.TipoSO);
			mensajes.AppendLine(string.IsNullOrEmpty(msgGenrarArchivo) ? "No se generó ningun archivo" : $"Archivo Generado \n {(msgGenrarArchivo.Split('|').Count() == 0 ? "" : $"{msgGenrarArchivo.Split('|')[0]}/{msgGenrarArchivo.Split('|')[1]}")}");

			respuesta.Mensaje = mensajes.ToString();
			return respuesta;
		}
	}
}
