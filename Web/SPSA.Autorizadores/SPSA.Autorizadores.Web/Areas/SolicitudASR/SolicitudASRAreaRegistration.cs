using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.SolicitudASR
{
    public class SolicitudASRAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SolicitudASR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
			context.MapRoute(
				"SolicitudASR_default",
				"SolicitudASR/{controller}/{action}/{id}",
				new { controller = "SolicitudUsuario", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AprobacionASR_default",
				"SolicitudASR/{controller}/{action}/{id}",
				new { controller = "Aprobacion", action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}