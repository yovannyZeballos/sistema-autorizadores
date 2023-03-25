using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Commands;
using SPSA.Autorizadores.Infraestructura.IoC;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using static SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands.ActualizarEstadoArchivoCommand;
using SPSA.Autorizadores.Aplicacion.Mappings;
using System.Data;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;

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


            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            builder.RegisterType<LoginHandler>().As<IRequestHandler<LoginCommand, UsuarioDTO>>();
            builder.RegisterType<ListarEmpresasHandler>().As<IRequestHandler<ListarEmpresasQuery, List<EmpresaDTO>>>();
            builder.RegisterType<ListarLocalesHandler>().As<IRequestHandler<ListarLocalesQuery, List<LocalDTO>>>();
            builder.RegisterType<ListarColaboradoresHandler>().As<IRequestHandler<ListarColaboradoresQuery,DataTable>>();
            builder.RegisterType<CrearAutorizadorHandler>().As<IRequestHandler<CrearAutorizadorCommand, RespuestaComunDTO>>();
            builder.RegisterType<GenerarArchivoHandler>().As<IRequestHandler<GenerarArchivoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ListarAutorizadoresHandler>().As<IRequestHandler<ListarAutorizadoresQuery, DataTable>>();
            builder.RegisterType<ActualizarEstadoArchivoHandler>().As<IRequestHandler<ActualizarEstadoArchivoCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarAutorizadorHandler>().As<IRequestHandler<EliminarAutorizadorCommand, RespuestaComunDTO>>();
            builder.RegisterType<ObtenerLocalHandler>().As<IRequestHandler<ObtenerLocalQuery, LocalDTO>>();
            builder.RegisterType<ListarColaboradoresCesadosHandler>().As<IRequestHandler<ListarColaboradoresCesadosQuery, ColaboradoresCesadosDTO>>();
            builder.RegisterType<ListarLocalesAsignarHandler>().As<IRequestHandler<ListarLocalesAsignarQuery, LocalesAsignadosDTO>>();
            builder.RegisterType<AsignarLocalHandler>().As<IRequestHandler<AsignarLocalCommand, RespuestaComunDTO>>();
            builder.RegisterType<ListarPuestosHandler>().As<IRequestHandler<ListarPuestosQuery, PuestoDTO>>();
            builder.RegisterType<ListarEmpresasOfiplanHandler>().As<IRequestHandler<ListarEmpresasOfiplanQuery, List<EmpresaDTO>>>();
            builder.RegisterType<ActualizarPuestoHandler>().As<IRequestHandler<ActualizarPuestoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ListarColaboradoresMassHandler>().As<IRequestHandler<ListarColaboradoresMassQuery, DataTable>>();
            builder.RegisterType<ListarLocalMonitorHandler>().As<IRequestHandler<ListarLocalMonitorQuery, DataTable>>();
            builder.RegisterType<ListarEmpresasMonitorHandler>().As<IRequestHandler<ListarEmpresasMonitorQuery, List<EmpresaDTO>>>();
            builder.RegisterType<CrearLocalMonitorHandler>().As<IRequestHandler<CrearLocalMonitorCommand, RespuestaComunDTO>>();
            builder.RegisterType<ProcesarMonitorHandler>().As<IRequestHandler<ProcesarMonitorCommand, RespuestaComunDTO>>();

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