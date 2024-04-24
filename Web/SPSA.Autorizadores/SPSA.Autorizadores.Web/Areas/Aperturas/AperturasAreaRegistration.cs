using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Aperturas
{
    public class AperturasAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Aperturas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Aperturas_default",
            //    "Aperturas/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "AdministrarApertura_default",
                "Aperturas/{controller}/{action}/{id}",
                new { controller = "AdministrarApertura", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}