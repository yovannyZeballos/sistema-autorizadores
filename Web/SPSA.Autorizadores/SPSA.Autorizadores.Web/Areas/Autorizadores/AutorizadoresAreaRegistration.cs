using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Autorizadores
{
    public class AutorizadoresAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Autorizadores";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
			context.MapRoute(
			"AutorizadorMasivo_default",
			"Autorizadores/AutorizadorMasivoLocal/{action}/{id}",
			new { controller = "AutorizadorMasivo", action = "Index", id = UrlParameter.Optional }
		   );

			context.MapRoute(
            "Administrar_default",
            "Autorizadores/Administrar/{action}/{id}",
            new { controller = "Autorizador", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
                "AsignarLocal_default",
                "Autorizadores/{controller}/{action}/{id}",
                new { controller = "AsignarLocal", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
               "AutorizadoresMass_default",
               "Autorizadores/{controller}/{action}/{id}",
               new { controller = "AutorizadoresMass", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
              "EliminarAutorizador_default",
              "Autorizadores/{controller}/{action}/{id}",
              new { controller = "EliminarAutorizador", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
             "Puesto_default",
             "Autorizadores/{controller}/{action}/{id}",
             new { controller = "Puesto", action = "Index", id = UrlParameter.Optional }
            );

			

		}
    }
}