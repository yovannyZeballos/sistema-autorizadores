using Autofac;
using Autofac.Integration.Mvc;
using MediatR;
using SPSA.Autorizadores.Infraestructura.IoC;
using AutoMapper;
using System.Web.Mvc;
using SPSA.Autorizadores.Aplicacion.Mappings;
using SPSA.Autorizadores.Aplicacion.IoC;
using Quartz.Impl;
using Quartz;
using SPSA.Autorizadores.Aplicacion.Jobs;
using SPSA.Autorizadores.Aplicacion.Schedulers;

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

			builder.RegisterModule(new AplicacionModule());

			builder.Register<ServiceFactory>(ctx =>
			{
				var c = ctx.Resolve<IComponentContext>();
				return t => c.Resolve(t);
			});

			// Register our Data dependencies
			builder.RegisterModule(new InfraestructuraModule());

			// Schedule
			builder.Register(x => new StdSchedulerFactory().GetScheduler().Result).As<IScheduler>();
			builder.RegisterType<JobActualizacionEstadoCierre>();
			// Schedule jobs
			//builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IJob).IsAssignableFrom(x));


			var container = builder.Build();

			// Set MVC DI resolver to use our Autofac container
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


			//Schedules
			SchedulerActualizacionEstadoCierre.Start(container);
		}
	}
}