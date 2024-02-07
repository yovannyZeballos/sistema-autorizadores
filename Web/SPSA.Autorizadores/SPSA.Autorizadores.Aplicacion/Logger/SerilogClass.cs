using Serilog;
using System.Web.Hosting;

namespace SPSA.Autorizadores.Aplicacion.Logger
{
	public static class SerilogClass
	{
		public static readonly ILogger _log;
		static SerilogClass()
		{
			var logPath = HostingEnvironment.MapPath("~/Logs/log.txt");
			_log = new LoggerConfiguration()
				.WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}
	}
}
