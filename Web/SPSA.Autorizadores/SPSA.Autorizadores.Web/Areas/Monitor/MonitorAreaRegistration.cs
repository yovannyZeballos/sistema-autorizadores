using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor
{
	public class MonitorAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Monitor";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"CierreEOD_default",
				"Monitor/{controller}/{action}/{id}",
				new { controller = "CierreEOD", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
			   "ReporteCierre_default",
			   "Monitor/{controller}/{action}/{id}",
				new { controller = "ReporteCierre", action = "Index", id = UrlParameter.Optional }
		   );

			context.MapRoute(
				   "CajaDefectuosa_default",
				   "Monitor/{controller}/{action}/{id}",
					new { controller = "CajaDefectuosa", action = "Index", id = UrlParameter.Optional }
			   );
			context.MapRoute(
				   "BCT_default",
				   "Monitor/{controller}/{action}/{id}",
					new { controller = "BCT", action = "Index", id = UrlParameter.Optional }
			   );
		}
	}
}