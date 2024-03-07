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
            //context.MapRoute(
            //    "Maestros_default",
            //    "Maestros/{controller}/{action}/{id}",
            //    new { controller = "Maestros", action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "MaeCadena_default",
                "Maestros/{controller}/{action}/{id}",
                new { controller = "MaeCadena", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "MaeCaja_default",
                "Maestros/{controller}/{action}/{id}",
                new { controller = "MaeCaja", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "MaeEmpresa_default",
                "Maestros/{controller}/{action}/{id}",
                new { controller = "MaeEmpresa", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "MaeLocal_default",
                "Maestros/{controller}/{action}/{id}",
                new { controller = "MaeLocal", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "MaeRegion_default",
                "Maestros/{controller}/{action}/{id}",
                new { controller = "MaeRegion", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
               "MaeTablas_default",
               "Maestros/{controller}/{action}/{id}",
               new { controller = "MaeTablas", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
               "Zona_default",
               "Maestros/{controller}/{action}/{id}",
               new { controller = "MaeZona", action = "Index", id = UrlParameter.Optional }
           );

        }
    }
}