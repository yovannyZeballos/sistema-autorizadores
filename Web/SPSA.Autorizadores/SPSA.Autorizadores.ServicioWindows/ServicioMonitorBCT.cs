using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace SPSA.Autorizadores.ServicioWindows
{
	public partial class ServicioMonitorBCT : ServiceBase
	{
		private readonly string _sourceEventLog = "ServicioMonitorBCT";
		private readonly string _logEventLog = "Application";
		private int _numeroEjecuciones = 0;
		readonly System.Timers.Timer timer = new System.Timers.Timer();

		public ServicioMonitorBCT()
		{
			InitializeComponent();

			eventLogMonitorBCT = new EventLog();
			if (!EventLog.SourceExists(_sourceEventLog))
			{
				EventLog.CreateEventSource(_sourceEventLog, _logEventLog);
			}

			eventLogMonitorBCT.Source = _sourceEventLog;
			eventLogMonitorBCT.Log = _logEventLog;
		}

		protected override void OnStart(string[] args)
		{
			var interval = ConfigurationManager.AppSettings.Get("TiempoEjecucionMonitorBCT");
			eventLogMonitorBCT.WriteEntry("Iniciando el Servicio de monitoreo Extractor BCT vs CT2");
			timer.Elapsed += new ElapsedEventHandler(Procesar);
			timer.Interval = Convert.ToInt32(interval);
			timer.Enabled = true;
		}

		protected override void OnStop()
		{
			eventLogMonitorBCT.WriteEntry("Deteniendo el Servicio de monitoreo Extractor BCT vs CT2");
		}

		private void Procesar(object source, ElapsedEventArgs e)
		{
			int count = Interlocked.Increment(ref _numeroEjecuciones);
			eventLogMonitorBCT.WriteEntry($"Ejecutando el Servicio de monitoreo Extractor BCT vs CT2, iteracion Nro: {count}");
		}
	}
}
