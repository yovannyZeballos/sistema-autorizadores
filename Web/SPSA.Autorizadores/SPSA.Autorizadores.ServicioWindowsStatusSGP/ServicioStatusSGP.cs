using Serilog;
using SPSA.Autorizadores.Aplicacion.Logger;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace SPSA.Autorizadores.ServicioWindowsStatusSGP
{
	partial class ServicioStatusSGP : ServiceBase
	{
		private readonly ILogger _logger;
		private int _numeroEjecuciones = 0;
		readonly System.Timers.Timer timer = new System.Timers.Timer();
		public ServicioStatusSGP()
		{
			InitializeComponent();

			_logger = SerilogClass._log;
		}

		protected override void OnStart(string[] args)
		{
			var interval = ConfigurationManager.AppSettings.Get("TiempoEjecucion");
			_logger.Information("Iniciando el Servicio de monitoreo de Status SGP");
			timer.Elapsed += new ElapsedEventHandler(Procesar);
			timer.Interval = Convert.ToInt32(interval);
			timer.Enabled = true;
		}

		protected override void OnStop()
		{
			_logger.Information("Deteniendo el Servicio de monitoreo de Status SGP");
		}

		private void Procesar(object source, ElapsedEventArgs e)
		{
			int count = Interlocked.Increment(ref _numeroEjecuciones);
			_logger.Information($"Ejecutando el Servicio de monitoreo de Status SGP, iteracion Nro: {count}");

			ServicePointManager.ServerCertificateValidationCallback +=
					(sender, cert, chain, sslPolicyErrors) => { return true; };

			using (var client = new HttpClient())
			{
				try
				{
					HttpResponseMessage response = client.GetAsync(ConfigurationManager.AppSettings.Get("UrlServicio")).Result;
					string json = response.Content.ReadAsStringAsync().Result;
					_logger.Information($"Respuesta del servicio: " + json);
				}
				catch (HttpRequestException ex)
				{
					_logger.Error(ex, $"Error al intentar conectarse al servicio: {ex.Message}");
				}
				catch (Exception ex)
				{
					_logger.Error(ex, $"Error inesperado: {ex.Message}");
				}
			}


			_logger.Information($"Fin de ejecución del Servicio de monitoreo de Status SGP, iteracion Nro: {count}");
		}
	}
}
