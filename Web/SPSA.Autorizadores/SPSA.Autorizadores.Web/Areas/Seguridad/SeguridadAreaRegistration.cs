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
        }
    }
}