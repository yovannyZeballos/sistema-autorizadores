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
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajaes.Commands;

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
            builder.RegisterType<Aplicacion.Features.Empresas.Queries.ListarEmpresasHandler>().As<IRequestHandler<Aplicacion.Features.Empresas.Queries.ListarEmpresasQuery, List<EmpresaDTO>>>();
            builder.RegisterType<ListarLocalesHandler>().As<IRequestHandler<ListarLocalesQuery, List<LocalDTO>>>();
            builder.RegisterType<ListarColaboradoresHandler>().As<IRequestHandler<ListarColaboradoresQuery, DataTable>>();
            builder.RegisterType<CrearAutorizadorHandler>().As<IRequestHandler<CrearAutorizadorCommand, RespuestaComunDTO>>();
            builder.RegisterType<GenerarArchivoHandler>().As<IRequestHandler<GenerarArchivoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ListarAutorizadoresHandler>().As<IRequestHandler<ListarAutorizadoresQuery, DataTable>>();
            builder.RegisterType<ActualizarEstadoArchivoHandler>().As<IRequestHandler<ActualizarEstadoArchivoCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarAutorizadorHandler>().As<IRequestHandler<EliminarAutorizadorCommand, RespuestaComunDTO>>();
            builder.RegisterType<Aplicacion.Features.Locales.Queries.ObtenerLocalHandler>().As<IRequestHandler<Aplicacion.Features.Locales.Queries.ObtenerLocalQuery, LocalDTO>>();
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
            builder.RegisterType<Aplicacion.Features.MantenimientoLocales.Queries.ListarEmpresasHandler>().As<IRequestHandler<Aplicacion.Features.MantenimientoLocales.Queries.ListarEmpresasQuery, List<EmpresaDTO>>>();
            builder.RegisterType<ListarFormatossHandler>().As<IRequestHandler<ListarFormatosQuery, List<FormatoDTO>>>();
            builder.RegisterType<CrearSovosLocalHandler>().As<IRequestHandler<CrearSovosLocalCommand, RespuestaComunDTO>>();
            builder.RegisterType<ListarCajasPorLocalHandler>().As<IRequestHandler<ListarCajasPorLocalQuery, ListarCajasPorLocalDTO>>();
            builder.RegisterType<CrearSovosCajaHandler>().As<IRequestHandler<CrearSovosCajaCommand, RespuestaComunDTO>>();
            builder.RegisterType<ListarLocalesPorEmpresaHandler>().As<IRequestHandler<ListarLocalesPorEmpresaQuery, ListarLocalesPorEmpresaDTO>>();
            builder.RegisterType<Aplicacion.Features.MantenimientoLocales.Queries.ObtenerLocalHandler>().As<IRequestHandler<Aplicacion.Features.MantenimientoLocales.Queries.ObtenerLocalQuery, SovosLocalDTO>>();
            builder.RegisterType<EliminarSovosCajaHandler>().As<IRequestHandler<EliminarSovosCajasCommand, RespuestaComunDTO>>();
            builder.RegisterType<ImportarCajasHandler>().As<IRequestHandler<ImportarCajasCommand, RespuestaComunExcelDTO>>();
            builder.RegisterType<DescargarMaestroHandler>().As<IRequestHandler<DescargarMaestroCommand, DescargarMaestroDTO>>();
            builder.RegisterType<ImportarInventarioCajaHandler>().As<IRequestHandler<ImportarInventarioCajaCommand, RespuestaComunExcelDTO>>();
            builder.RegisterType<DescargarPlantillasHandler>().As<IRequestHandler<DescargarPlantillasCommand, DescargarPlantillasDTO>>();
            builder.RegisterType<ObtenerLocalOfiplanHandler>().As<IRequestHandler<ObtenerLocalOfiplanQuery, LocalOfiplanDTO>>();
            builder.RegisterType<AsociarLocalPMMHandler>().As<IRequestHandler<AsociarLocalPMMCommand, RespuestaComunDTO>>();

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