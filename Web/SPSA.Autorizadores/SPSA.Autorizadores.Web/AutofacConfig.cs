using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Commands;
using SPSA.Autorizadores.Infraestructura.IoC;
using AutoMapper;
using System.Collections.Generic;
using System.Web.Mvc;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Aplicacion.Mappings;
using System.Data;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands;
using SPSA.Autorizadores.Aplicacion.IoC;

namespace SPSA.Autorizadores.Web
{
	public class AutofacConfig
	{
		public static void ConfigureContainer()
		{
			var builder = new ContainerBuilder();

			// Register dependencies in controllers
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			// Register dependencies in filter attributes
			builder.RegisterFilterProvider();

			// Register dependencies in custom views
			builder.RegisterSource(new ViewRegistrationSource());

			builder.Register(context => new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<AplicacionProfile>();

				//etc...
			})).AsSelf().SingleInstance();

			builder.Register(c =>
			{
				//This resolves a new context that can be used later.
				var context = c.Resolve<IComponentContext>();
				var config = context.Resolve<MapperConfiguration>();
				return config.CreateMapper(context.Resolve);
			})
		.As<IMapper>()
		.InstancePerLifetimeScope();


			//
			builder.RegisterModule(new AplicacionModule());

			builder.Register<ServiceFactory>(ctx =>
			{
				var c = ctx.Resolve<IComponentContext>();
				return t => c.Resolve(t);
			});

			// Register our Data dependencies
			builder.RegisterModule(new InfraestructuraModule());

			var container = builder.Build();

			// Set MVC DI resolver to use our Autofac container
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}