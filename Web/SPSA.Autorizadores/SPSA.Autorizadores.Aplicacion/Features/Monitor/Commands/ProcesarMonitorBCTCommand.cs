using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarMonitorBCTCommand : IRequest<ObtenerComunDTO<(bool, bool, bool, int)>>
	{
		public TransactionXmlCT2 RegistroTotal { get; set; }
		public string FechaAlerta { get; set; }
		public string CodEmpresa { get; set; }
		public int CantidadAnterior { get; set; }
		public int Color { get; set; } //1:Verde, 2:Naranja, 3:Rojo
	}

	public class ProcesarMonitorBCTHandler : IRequestHandler<ProcesarMonitorBCTCommand, ObtenerComunDTO<(bool, bool, bool, int)>>
	{
		private readonly IBCTContexto _contexto;

		public ProcesarMonitorBCTHandler()
		{
			_contexto = new BCTContexto();
		}

		public async Task<ObtenerComunDTO<(bool, bool, bool, int)>> Handle(ProcesarMonitorBCTCommand request, CancellationToken cancellationToken)
		{
			var luzVerde = true;
			var luzNaranja = false;
			var luzRoja = false;
			var envioNotificacion = 0;

			var response = new ObtenerComunDTO<(bool, bool, bool, int)> { Ok = true };
			try
			{
				var semaforoColor = (ColorSemaforo)request.Color;
				var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct &&
																							 x.CodEmpresa == request.CodEmpresa).ToListAsync();

				var toleranciaAlerta = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaAlerta).Select(x => x.ValParametro).FirstOrDefault();
				var toleranciaCantidad = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaCantidad).Select(x => x.ValParametro).FirstOrDefault();

				if (request.RegistroTotal == null)
					return response;

				if (request.RegistroTotal.Cantidad >= Convert.ToInt32(toleranciaCantidad) && semaforoColor != ColorSemaforo.ROJO)
				{
					luzNaranja = true;
					luzVerde = false;
				}

				if (semaforoColor == ColorSemaforo.ROJO)
				{
					var diferencia = request.CantidadAnterior * 0.8;

					if (request.RegistroTotal.Cantidad <= diferencia)
					{
						luzNaranja = true;
						luzVerde = false;
						luzRoja = false;
						request.FechaAlerta = "";
					}
					else
					{
						luzNaranja = false;
						luzVerde = false;
						luzRoja = true;
					}
				}

				if (!string.IsNullOrEmpty(request.FechaAlerta) && !luzVerde)
				{
					var fechaAlerta = DateTime.ParseExact(request.FechaAlerta, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
					var fechaActual = DateTime.Now;

					if ((fechaActual - fechaAlerta).TotalMinutes > Convert.ToInt32(toleranciaAlerta))
					{
						luzNaranja = false;
						luzVerde = false;
						luzRoja = true;

						//TODO: Enviar Alerta
						envioNotificacion = 1;
					}
				}

				response.Data = (luzVerde, luzNaranja, luzRoja, envioNotificacion);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return response;
		}
	}
}
