using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.SolicitudCodComercio
{
    public class SolicitudCodComercioAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SolicitudCodComercio";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SolicitudCodComercio_default",
                "SolicitudCodComercio/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}