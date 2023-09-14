using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands
{
	public class AsignarCajeroCommand : IRequest<RespuestaComunDTO>
	{
		public List<CajeroDTO> Cajeros { get; set; }
		public string TipoSO { get; set; }
		public string CodLocal { get; set; }
	}

	public class AsignarCajeroHandler : IRequestHandler<AsignarCajeroCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public AsignarCajeroHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<RespuestaComunDTO> Handle(AsignarCajeroCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			var mensajes = new StringBuilder();

			foreach (var item in request.Cajeros)
			{
				var cajero = new Cajero(item.CodLocal, item.Nombres, item.Apellidos, item.Tipo, item.TipoContrato, item.Rut, item.TipoDocIdentidad, item.CodigoEmpleado, item.Usuario);

				try
				{
					await _repositorioCajero.Crear(cajero);
					mensajes.AppendLine($"Cajero {cajero.Rut} creado");
				}
				catch (Exception ex)
				{
					mensajes.AppendLine($"ERROR al crear el Cajero {cajero.Rut} | {ex.Message}");
					respuesta.Ok = false;
				}
			}

			var msgGenrarArchivo = await _repositorioCajero.GenerarArchivo(request.CodLocal, request.TipoSO);
			mensajes.AppendLine(string.IsNullOrEmpty(msgGenrarArchivo) ? "No se generó ningun archivo" : $"Archivo Generado \n {(msgGenrarArchivo.Split('|').Count() == 0 ? "" : $"{msgGenrarArchivo.Split('|')[0]}/{msgGenrarArchivo.Split('|')[1]}")}");
			
			respuesta.Mensaje = mensajes.ToString();
			return respuesta;
		}
	}
}
