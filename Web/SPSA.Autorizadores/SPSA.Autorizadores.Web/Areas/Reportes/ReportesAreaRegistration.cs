using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Reportes
{
    public class ReportesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Reportes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Reportes_default",
            //    "Reportes/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "Reportes_default",
                "Reportes/{controller}/{action}/{id}",
                new { controller = "AdministrarReportes", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}