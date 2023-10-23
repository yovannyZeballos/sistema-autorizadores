using Autofac;
using Autofac.Features.Variance;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Commands;
using System.Collections.Generic;
using System.Data;

namespace SPSA.Autorizadores.Aplicacion.IoC
{
	public class AplicacionModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterSource(new ContravariantRegistrationSource());
			builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
			builder.RegisterType<LoginHandler>().As<IRequestHandler<LoginCommand, UsuarioDTO>>();
			builder.RegisterType<Features.Empresas.Queries.ListarEmpresasHandler>().As<IRequestHandler<Features.Empresas.Queries.ListarEmpresasQuery, ListarEmpresaResponseDTO>>();
			builder.RegisterType<ListarLocalesHandler>().As<IRequestHandler<ListarLocalesQuery, List<LocalDTO>>>();
			builder.RegisterType<Features.Autorizadores.Queries.ListarColaboradoresHandler>().As<IRequestHandler<Features.Autorizadores.Queries.ListarColaboradoresQuery, DataTable>>();
			builder.RegisterType<CrearAutorizadorHandler>().As<IRequestHandler<CrearAutorizadorCommand, RespuestaComunDTO>>();
			builder.RegisterType<GenerarArchivoHandler>().As<IRequestHandler<GenerarArchivoCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarAutorizadoresHandler>().As<IRequestHandler<ListarAutorizadoresQuery, DataTable>>();
			builder.RegisterType<ActualizarEstadoArchivoHandler>().As<IRequestHandler<ActualizarEstadoArchivoCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarAutorizadorHandler>().As<IRequestHandler<EliminarAutorizadorCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.Locales.Queries.ObtenerLocalHandler>().As<IRequestHandler<Features.Locales.Queries.ObtenerLocalQuery, LocalDTO>>();
			builder.RegisterType<ListarColaboradoresCesadosHandler>().As<IRequestHandler<ListarColaboradoresCesadosQuery, ColaboradoresCesadosDTO>>();
			builder.RegisterType<ListarLocalesAsignarHandler>().As<IRequestHandler<ListarLocalesAsignarQuery, LocalesAsignadosDTO>>();
			builder.RegisterType<AsignarLocalHandler>().As<IRequestHandler<AsignarLocalCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarPuestosHandler>().As<IRequestHandler<ListarPuestosQuery, PuestoDTO>>();
			builder.RegisterType<ListarEmpresasOfiplanHandler>().As<IRequestHandler<ListarEmpresasOfiplanQuery, ListarEmpresaResponseDTO>>();
			builder.RegisterType<ActualizarPuestoHandler>().As<IRequestHandler<ActualizarPuestoCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarColaboradoresMassHandler>().As<IRequestHandler<ListarColaboradoresMassQuery, DataTable>>();
			builder.RegisterType<ListarLocalMonitorHandler>().As<IRequestHandler<ListarLocalMonitorQuery, DataTable>>();
			builder.RegisterType<ListarEmpresasMonitorHandler>().As<IRequestHandler<ListarEmpresasMonitorQuery, List<EmpresaDTO>>>();
			builder.RegisterType<CrearLocalMonitorHandler>().As<IRequestHandler<CrearLocalMonitorCommand, RespuestaComunDTO>>();
			builder.RegisterType<ProcesarMonitorHandler>().As<IRequestHandler<ProcesarMonitorCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.MantenimientoLocales.Queries.ListarEmpresasHandler>().As<IRequestHandler<Features.MantenimientoLocales.Queries.ListarEmpresasQuery, List<EmpresaDTO>>>();
			builder.RegisterType<ListarFormatossHandler>().As<IRequestHandler<ListarFormatosQuery, List<FormatoDTO>>>();
			builder.RegisterType<CrearSovosLocalHandler>().As<IRequestHandler<CrearSovosLocalCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarCajasPorLocalHandler>().As<IRequestHandler<ListarCajasPorLocalQuery, ListarCajasPorLocalDTO>>();
			builder.RegisterType<CrearSovosCajaHandler>().As<IRequestHandler<CrearSovosCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarLocalesPorEmpresaHandler>().As<IRequestHandler<ListarLocalesPorEmpresaQuery, ListarLocalesPorEmpresaDTO>>();
			builder.RegisterType<Features.MantenimientoLocales.Queries.ObtenerLocalHandler>().As<IRequestHandler<Features.MantenimientoLocales.Queries.ObtenerLocalQuery, SovosLocalDTO>>();
			builder.RegisterType<EliminarSovosCajaHandler>().As<IRequestHandler<EliminarSovosCajasCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarCajasHandler>().As<IRequestHandler<ImportarCajasCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<Features.MantenimientoLocales.Commands.DescargarMaestroHandler>().As<IRequestHandler<Features.MantenimientoLocales.Commands.DescargarMaestroCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ImportarInventarioCajaHandler>().As<IRequestHandler<ImportarInventarioCajaCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<DescargarPlantillasHandler>().As<IRequestHandler<DescargarPlantillasCommand, DescargarPlantillasDTO>>();
			builder.RegisterType<ObtenerLocalOfiplanHandler>().As<IRequestHandler<ObtenerLocalOfiplanQuery, LocalOfiplanDTO>>();
			builder.RegisterType<AsociarLocalPMMHandler>().As<IRequestHandler<AsociarLocalPMMCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarCaracteristicasCajaHandler>().As<IRequestHandler<ListarCaracteristicasCajaQuery, CaracetristicaCajaResponseDTO>>();
			builder.RegisterType<CrearSovosCajaInventarioHandler>().As<IRequestHandler<CrearSovosCajaInventarioCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarCajaInventarioHandler>().As<IRequestHandler<ListarCajaInventarioQuery, ListarCajaInventarioDTO>>();
			builder.RegisterType<ObtenerCajaInventarioHandler>().As<IRequestHandler<ObtenerCajaInventarioQuery, SovosCajaInventarioDTO>>();
			builder.RegisterType<Features.InventarioCaja.Commands.DescargarMaestroHandler>().As<IRequestHandler<Features.InventarioCaja.Commands.DescargarMaestroCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ListarInventarioTipoHandler>().As<IRequestHandler<ListarInventarioTipoQuery, ListarInventarioTipoDTO>>();
			builder.RegisterType<CrearInventarioServidorHandler>().As<IRequestHandler<CrearInventarioServidorCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarInventarioServidorVirtualHandler>().As<IRequestHandler<ListarInventarioServidorVirtualQuery, ListarInventarioServidorVirtualDTO>>();
			builder.RegisterType<CrearInventarioServidorVirtualHandler>().As<IRequestHandler<CrearInventarioServidorVirtualCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarInventarioServidorHandler>().As<IRequestHandler<ListarInventarioServidorQuery, ListarInventarioServidorDTO>>();
			builder.RegisterType<ObtenerInventarioServidorHandler>().As<IRequestHandler<ObtenerInventarioServidorQuery, InventarioServidorDTO>>();
			builder.RegisterType<EliminarInventarioServidorVirtualHandler>().As<IRequestHandler<EliminarInventarioServidorVirtualCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.InventarioServidor.Commands.DescargarMaestroHandler>().As<IRequestHandler<Features.InventarioServidor.Commands.DescargarMaestroCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ImportarInventarioServidorHandler>().As<IRequestHandler<ImportarInventarioServidorCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<GenerarArchivoPorLocalHandler>().As<IRequestHandler<GenerarArchivoPorLocalCommand, DescargarPlantillasDTO>>();
			builder.RegisterType<ListarCajerosHandler>().As<IRequestHandler<ListarCajerosQuery, ListarCajerosDTO>>();
			builder.RegisterType<Features.Cajeros.Queries.ListarColaboradoresHandler>().As<IRequestHandler<Features.Cajeros.Queries.ListarColaboradoresQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<AsignarCajeroHandler>().As<IRequestHandler<AsignarCajeroCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarCajeroHandler>().As<IRequestHandler<EliminarCajeroCommand, RespuestaComunDTO>>();
			builder.RegisterType<DescargarExcelCajerosHandler>().As<IRequestHandler<DescargarExcelCajerosCommand, DescargarMaestroDTO>>();
			builder.RegisterType<GenerarArchivoCajeroHandler>().As<IRequestHandler<GenerarArchivoCajeroCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarLocalesXEmpresaHandler>().As<IRequestHandler<ListarLocalesXEmpresaQuery, ListarLocalXEmpresaResponseDTO>>();
			builder.RegisterType<ReporteDiferenciaCajaHandler>().As<IRequestHandler<ReporteDiferenciaCajaQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ReporteSobresHandler>().As<IRequestHandler<ReporteSobresQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<DescargarExcelRolesHandler>().As<IRequestHandler<DescargarExcelRolesCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ListarGrillaHandler>().As<IRequestHandler<ListarGrillaQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ListarListBoxHandler>().As<IRequestHandler<ListarListBoxQuery, ListarResponseDTO<List<ListBoxDTO>>>>();
			builder.RegisterType<ReporteCierrePivotHandler>().As<IRequestHandler<ReporteCierrePivotQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ReporteCierreHandler>().As<IRequestHandler<ReporteCierreQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ReporteDiferenciaCajaExcelHandler>().As<IRequestHandler<ReporteDiferenciaCajaExcelCommand, DescargarMaestroDTO>>();

			base.Load(builder);
		}
	}
}
