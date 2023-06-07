using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Locales
{
	public class LocalesAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Locales";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Locales_default",
				"Locales/{controller}/{action}/{id}",
				 new { controller = "AdministrarLocal", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
			   "InventarioCaja_default",
			   "Locales/{controller}/{action}/{id}",
				new { controller = "InventarioCaja", action = "Index", id = UrlParameter.Optional }
		   );
		}
	}
}