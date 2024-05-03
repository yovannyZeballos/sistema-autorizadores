using Serilog;
using System.Configuration;
using System.Web.Hosting;

namespace SPSA.Autorizadores.Aplicacion.Logger
{
	public static class SerilogClass
	{
		public static readonly ILogger _log;
		static SerilogClass()
		{
			var logPath = ConfigurationManager.AppSettings["RutaLog"];
			_log = new LoggerConfiguration()
				.WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 20)
				.CreateLogger();
		}
	}
}
