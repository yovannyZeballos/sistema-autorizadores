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
        }
    }
}