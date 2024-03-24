using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Ubigeos
{
    public class UbigeosAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Ubigeos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Ubigeos_default",
                "Ubigeos/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}