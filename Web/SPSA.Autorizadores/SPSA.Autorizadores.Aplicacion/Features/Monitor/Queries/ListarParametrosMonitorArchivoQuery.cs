using MediatR;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ListarParametrosMonitorArchivoQuery : IRequest<GenericResponseDTO<MonitorArchivoParametrosDTO>>
	{
	}

	public class ListarParametrosMonitorArchivoHandler : IRequestHandler<ListarParametrosMonitorArchivoQuery, GenericResponseDTO<MonitorArchivoParametrosDTO>>
	{
		private readonly ILogger _logger;

		public ListarParametrosMonitorArchivoHandler()
		{
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<MonitorArchivoParametrosDTO>> Handle(ListarParametrosMonitorArchivoQuery request, CancellationToken cancellationToken)
		{
			var parametrosMonitor = new GenericResponseDTO<MonitorArchivoParametrosDTO> { Ok = true };
			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					var parametros = await contexto.RepositorioProcesoParametro
						.Obtener(x => x.CodProceso == Constantes.CodigoProcesoMonitorArchivos)
						.ToDictionaryAsync(x => x.CodParametro, x => x.ValParametro);

					parametros.TryGetValue(Constantes.CodigoUsuarioCaja_ProcesoMonitorArchivo, out var usuarioCaja);
					parametros.TryGetValue(Constantes.CodigoClaveCaja_ProcesoMonitorArchivo, out var claveCaja);
					parametros.TryGetValue(Constantes.CodigoRutaArchivos1_ProcesoMonitorArchivo, out var rutaArchivos1);
					parametros.TryGetValue(Constantes.CodigoRutaArchivos2_ProcesoMonitorArchivo, out var rutaArchivos2);

					parametrosMonitor.Data = new MonitorArchivoParametrosDTO
					{
						Usuario = usuarioCaja,
						Clave = claveCaja,
						Ruta = $"{rutaArchivos1}{rutaArchivos2}"
					};


				}
			}
			catch (Exception ex)
			{
				parametrosMonitor.Ok = false;
				parametrosMonitor.Mensaje = ex.Message;
				_logger.Error(ex, ex.Message);
			}

			return parametrosMonitor;

		}
	}
}
