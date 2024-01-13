using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarMonitorBCTCommand : IRequest<ObtenerComunDTO<(bool, bool)>>
	{
		public TransactionXmlCT2 RegistroTotal { get; set; }
		public List<TransactionXmlCT2> RegistroPorSegundo { get; set; }
		public ParametrosMonitorBctDTO Parametros { get; set; }
	}

	public class ProcesarMonitorBCTHandler : IRequestHandler<ProcesarMonitorBCTCommand, ObtenerComunDTO<(bool, bool)>>
	{
		public async Task<ObtenerComunDTO<(bool, bool)>> Handle(ProcesarMonitorBCTCommand request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				var response = new ObtenerComunDTO<(bool, bool)> { Ok = true };
				try
				{
					bool cantidadTotalOk;
					bool diferenciaSegundosOk;

					cantidadTotalOk = request.RegistroTotal.Cantidad < request.Parametros.ToleranciaCantidad;


					request.RegistroPorSegundo
							.Select(x => { x.Fecha = string.IsNullOrEmpty(x.FechaStr) ? x.Fecha : DateTime.ParseExact(x.FechaStr, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture); return x; }).ToList();


					if (request.RegistroPorSegundo.Count == 0)
						diferenciaSegundosOk = true;
					else
						diferenciaSegundosOk
							= (request.RegistroPorSegundo[request.RegistroPorSegundo.Count - 1].Fecha - request.RegistroPorSegundo[0].Fecha).TotalSeconds < request.Parametros.ToleranciaSegundos;


					response.Data = (cantidadTotalOk, diferenciaSegundosOk);
				}
				catch (Exception ex)
				{
					response.Ok = false;
					response.Mensaje = ex.Message;
				}

				return response;
			});

		}
	}
}
