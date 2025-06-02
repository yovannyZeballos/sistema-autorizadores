using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.MdrBinesIzipay
{
    public class MdrBinesIzipayAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MdrBinesIzipay";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MdrBinesIzipay_default",
                "MdrBinesIzipay/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}