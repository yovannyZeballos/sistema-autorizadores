using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario
{
    public class InventarioAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Inventario";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Inventario_default",
                "Inventario/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

			context.MapRoute(
			   "InventarioCaja",
			   "Inventario/{controller}/{action}/{id}",
				new { controller = "InventarioCaja", action = "Index", id = UrlParameter.Optional }
		   );
        }
    }
}