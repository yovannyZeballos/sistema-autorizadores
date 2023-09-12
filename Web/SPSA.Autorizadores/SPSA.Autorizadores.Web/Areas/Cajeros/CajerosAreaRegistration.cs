using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Cajeros
{
    public class CajerosAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Cajeros";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Cajeros_default",
				"Cajeros/{controller}/{action}/{id}",
                new { controller = "AdministrarCajero", action = "Index", id = UrlParameter.Optional }
            );

			context.MapRoute(
				"Cajeros_Reporte_Sobres",
				"Cajeros/{controller}/{action}/{id}",
				new { controller = "Reportes", action = "Sobres", id = UrlParameter.Optional }
			);
		}
    }
}