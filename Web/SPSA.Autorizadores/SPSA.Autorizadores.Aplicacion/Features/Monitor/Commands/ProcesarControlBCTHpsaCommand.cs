using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarControlBCTHpsaCommand : IRequest<GenericResponseDTO<List<MonitorControlBCTDTO>>>
	{
		public int CodSucursal { get; set; }
		public string Fecha { get; set; }
	}

	public class ProcesarControlBCTHpsaHandler : IRequestHandler<ProcesarControlBCTHpsaCommand, GenericResponseDTO<List<MonitorControlBCTDTO>>>
	{
		private readonly IRepositorioMonitorControlBCT _repositorioMonitorControlBCT;
		private readonly ILogger _logger;

		public ProcesarControlBCTHpsaHandler(IRepositorioMonitorControlBCT repositorioMonitorControlBCT)
		{
			_repositorioMonitorControlBCT = repositorioMonitorControlBCT;
			_logger = SerilogClass._log;
		}
		public async Task<GenericResponseDTO<List<MonitorControlBCTDTO>>> Handle(ProcesarControlBCTHpsaCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new GenericResponseDTO<List<MonitorControlBCTDTO>> { Ok = true };
			var culture = CultureInfo.InvariantCulture;
			try
			{
				var fecha = DateTime.ParseExact(request.Fecha, "dd/MM/yyyy", culture);
				var fechaStr = fecha.ToString("yyyyMMdd", culture);

				var horariosCT2 = await _repositorioMonitorControlBCT.ObtenerHorarioSucursalCT2Hpsa(fechaStr, request.CodSucursal);
                var horariosBCT = await _repositorioMonitorControlBCT.ObtenerHorarioSucursalBCTHpsa(fecha.ToString("dd/MM/yyyy"), request.CodSucursal);

                respuesta.Data = (from a in horariosCT2
								  join b in horariosBCT on a.CodSucursal equals b.CodSucursal
								  orderby (b.Diferencia <= a.TiempoLim ? "SI" : "NO"), b.Diferencia descending
								  select new MonitorControlBCTDTO
								  {
									  DesSucursal = a.DesSucursal,
									  Fecha = a.Fecha,
									  Horario = a.Horario,
									  TiempoLim = a.TiempoLim,
									  UltimaTransf = b.UltimaTransf,
									  Diferencia = b.Diferencia,
									  Semaforo = b.Diferencia <= a.TiempoLim ? "SI" : "NO"
								  }).ToList();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}
	}
}
