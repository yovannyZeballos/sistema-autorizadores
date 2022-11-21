using SPSA.Autorizadores.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SPSA.Autorizadores.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutofacConfig.ConfigureContainer();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var services = new ServiceCollection();

            //// Register all your controllers and other services here:
            //services.AddTransient<HomeController>();

            //var provider = services.BuildServiceProvider(new ServiceProviderOptions
            //{
            //    // Prefer to keep validation on at all times
            //    ValidateOnBuild = true,
            //    ValidateScopes = true
            //});

            //ControllerBuilder.Current.SetControllerFactory(
            //    new MsDiControllerFactory(provider));
        }
    }
}
