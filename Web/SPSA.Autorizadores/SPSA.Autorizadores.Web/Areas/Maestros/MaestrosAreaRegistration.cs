using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros
{
    public class MaestrosAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Maestros";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Maestros_default",
                "Maestros/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}