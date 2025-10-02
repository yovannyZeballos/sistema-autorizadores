using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Operaciones
{
    public class OperacionesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Operaciones";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
			context.MapRoute(
				"Operaciones_reporte",
				"Operaciones/{controller}/{action}/{id}",
				new { controller = "Reportes", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Operaciones_gestion",
				"Operaciones/{controller}/{action}/{id}",
				new { controller = "Gestion", action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}