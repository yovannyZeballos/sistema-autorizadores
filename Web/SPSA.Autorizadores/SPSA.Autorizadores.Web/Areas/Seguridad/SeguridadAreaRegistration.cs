using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Seguridad
{
    public class SeguridadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Seguridad";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Seguridad_default",
                "Seguridad/{controller}/{action}/{id}",
                new {controller = "CambioLocal", action = "Index", id = UrlParameter.Optional }
            );

			context.MapRoute(
			   "Sistema_default",
			   "Seguridad/{controller}/{action}/{id}",
				new { controller = "Sistema", action = "Index", id = UrlParameter.Optional }
		    );

			context.MapRoute(
			   "Perfil_default",
			   "Seguridad/{controller}/{action}/{id}",
				new { controller = "Perfil", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
			   "Menu_default",
			   "Seguridad/{controller}/{action}/{id}",
				new { controller = "Menu", action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}