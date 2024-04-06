using Autofac;
using Autofac.Features.Variance;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Aperturas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Caja.Command;
using SPSA.Autorizadores.Aplicacion.Features.Caja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Aplicacion.Features.DataTableSGP.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Login.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries;
using SPSA.Autorizadores.Aplicacion.Features.TablasMae.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Ubigeos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Queries;
using SPSA.Autorizadores.Dominio.Entidades;
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
			builder.RegisterType<Features.Locales.Queries.ListarLocalesHandler>().As<IRequestHandler<Features.Locales.Queries.ListarLocalesQuery, List<LocalDTO>>>();
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
			builder.RegisterType<ListarLocalMonitorHandler>().As<IRequestHandler<ListarLocalMonitorQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ListarEmpresasMonitorHandler>().As<IRequestHandler<ListarEmpresasMonitorQuery, List<EmpresaDTO>>>();
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
			builder.RegisterType<ListarListBoxHandler>().As<IRequestHandler<ListarListBoxQuery, GenericResponseDTO<List<ListBoxDTO>>>>();
			builder.RegisterType<ReporteCierrePivotHandler>().As<IRequestHandler<ReporteCierrePivotQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ReporteCierreHandler>().As<IRequestHandler<ReporteCierreQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ReporteDiferenciaCajaExcelHandler>().As<IRequestHandler<ReporteDiferenciaCajaExcelCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ReporteCierreResumenHandler>().As<IRequestHandler<ReporteCierreResumenQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ListarCajeroVolanteHandler>().As<IRequestHandler<ListarCajeroVolanteQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ListarCajeroVolanteOfiplanHandler>().As<IRequestHandler<ListarCajeroVolanteOfiplanQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<CrearCajeroVolanteHandler>().As<IRequestHandler<CrearCajeroVolanteCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarCajeroVolanteHandler>().As<IRequestHandler<EliminarCajeroVolanteCommand, RespuestaComunDTO>>();
			builder.RegisterType<ProcesarCajaDefectuosaHandler>().As<IRequestHandler<ProcesarCajaDefectuosaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ProcesarMonitorBCTHandler>().As<IRequestHandler<ProcesarMonitorBCTCommand, ObtenerComunDTO<(bool, bool, bool, int)>>>();
			builder.RegisterType<ObtenerRegistrosMonitorBCTHandler>().As<IRequestHandler<ObtenerRegistrosMonitorBCTQuery, ObtenerComunDTO<TransactionXmlCT2>>>();
			builder.RegisterType<ObtenerParametrosHandler>().As<IRequestHandler<ObtenerParametrosQuery, GenericResponseDTO<List<ParametrosMonitorBctDTO>>>>();
			builder.RegisterType<ListarSistemasHandler>().As<IRequestHandler<ListarSistemasQuery, GenericResponseDTO<List<ListarSistemaDTO>>>>();
			builder.RegisterType<CrearSistemaHandler>().As<IRequestHandler<CrearSistemaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarSistemaHandler>().As<IRequestHandler<ActualizarSistemaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ObtenerSistemaHandler>().As<IRequestHandler<ObtenerSistemaQuery, GenericResponseDTO<ObtenerSistemaDTO>>>();
			builder.RegisterType<ListarUsuarioHandler>().As<IRequestHandler<ListarUsuarioQuery, GenericResponseDTO<List<ListarUsuarioDTO>>>>();
			builder.RegisterType<CrearUsuarioHandler>().As<IRequestHandler<CrearUsuarioCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarUsuarioHandler>().As<IRequestHandler<ActualizarUsuarioCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.Seguridad.Usuario.Queries.ListarEmpresasHandler>().As<IRequestHandler<Features.Seguridad.Usuario.Queries.ListarEmpresasQuery, GenericResponseDTO<List<ListarEmpresaDTO>>>>();
			builder.RegisterType<AsociarUsuarioEmpresaHandler>().As<IRequestHandler<AsociarUsuarioEmpresaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarEmpresasAsociadasHandler>().As<IRequestHandler<ListarEmpresasAsociadasQuery, GenericResponseDTO<List<ListarEmpresaDTO>>>>();
			builder.RegisterType<ListarCadenasHandler>().As<IRequestHandler<ListarCadenasQuery, GenericResponseDTO<List<ListarCadenaDTO>>>>();
			builder.RegisterType<AsociarUsuarioCadenaHandler>().As<IRequestHandler<AsociarUsuarioCadenaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarRegionesHandler>().As<IRequestHandler<ListarRegionesQuery, GenericResponseDTO<List<ListarRegionDTO>>>>();
			builder.RegisterType<ListarCadenasAsociadasHandler>().As<IRequestHandler<ListarCadenasAsociadasQuery, GenericResponseDTO<List<ListarCadenaDTO>>>>();
			builder.RegisterType<AsociarUsuarioRegionHandler>().As<IRequestHandler<AsociarUsuarioRegionCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarRegionesAsociadasHandler>().As<IRequestHandler<ListarRegionesAsociadasQuery, GenericResponseDTO<List<ListarRegionDTO>>>>();
			builder.RegisterType<ListarZonasHandler>().As<IRequestHandler<ListarZonasQuery, GenericResponseDTO<List<ListarZonaDTO>>>>();
			builder.RegisterType<AsociarUsuarioZonaHandler>().As<IRequestHandler<AsociarUsuarioZonaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarZonasAsociadasHandler>().As<IRequestHandler<ListarZonasAsociadasQuery, GenericResponseDTO<List<ListarZonaDTO>>>>();
			builder.RegisterType<Features.Seguridad.Usuario.Queries.ListarLocalesHandler>().As<IRequestHandler<Features.Seguridad.Usuario.Queries.ListarLocalesQuery, GenericResponseDTO<List<ListarLocalDTO>>>>();
			builder.RegisterType<AsociarUsuarioLocalHandler>().As<IRequestHandler<AsociarUsuarioLocalCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.Seguridad.Usuario.Queries.ListarPerfilesHandler>().As<IRequestHandler<Features.Seguridad.Usuario.Queries.ListarPerfilesQuery, GenericResponseDTO<List<ListarPerfilDTO>>>>();
			builder.RegisterType<AsociarUsuarioPerfilHandler>().As<IRequestHandler<AsociarUsuarioPerfilCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.Seguridad.Perfil.Queries.ListarPerfilesHandler>().As<IRequestHandler<Features.Seguridad.Perfil.Queries.ListarPerfilesQuery, GenericResponseDTO<List<ListarPerfilDTO>>>>();
			builder.RegisterType<CrearPerfilHandler>().As<IRequestHandler<CrearPerfilCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarPerfilHandler>().As<IRequestHandler<ActualizarPerfilCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarSistemasActivosHandler>().As<IRequestHandler<ListarSistemasActivosQuery, GenericResponseDTO<List<ListarSistemaDTO>>>>();
			builder.RegisterType<ListarMenuHandler>().As<IRequestHandler<ListarMenuQuery, GenericResponseDTO<List<ListarMenuDTO>>>>();
			builder.RegisterType<CrearMenuHandler>().As<IRequestHandler<CrearMenuCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMenuHandler>().As<IRequestHandler<ActualizarMenuCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarMenuHandler>().As<IRequestHandler<EliminarMenuCommand, RespuestaComunDTO>>();
			builder.RegisterType<AsociarMenusHandler>().As<IRequestHandler<AsociarMenusCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarMenusHandler>().As<IRequestHandler<ListarMenusQuery, GenericResponseDTO<List<ListarMenuDTO>>>>();
			builder.RegisterType<ObtenerUsuarioHandler>().As<IRequestHandler<ObtenerUsuarioQuery, GenericResponseDTO<ObtenerUsuarioDTO>>>();
			builder.RegisterType<ListarLocalesAsociadasHandler>().As<IRequestHandler<ListarLocalesAsociadasQuery, GenericResponseDTO<List<ListarLocalDTO>>>>();
			builder.RegisterType<ObtenerJerarquiaOrganizacionalHandler>().As<IRequestHandler<ObtenerJerarquiaOrganizacionalQuery, JerarquiaOrganizacionalDTO>>();
			builder.RegisterType<ObtenerMenuHandler>().As<IRequestHandler<ObtenerMenuQuery, GenericResponseDTO<ListarMenuDTO>>>();
			builder.RegisterType<ObtenerMenusUsuarioHandler>().As<IRequestHandler<ObtenerMenusUsuarioQuery, GenericResponseDTO<List<ListarMenuDTO>>>>();
			builder.RegisterType<ImportarExcelInventarioCajaHandler>().As<IRequestHandler<ImportarExcelInventarioCajaCommand, RespuestaComunExcelDTO>>();

            builder.RegisterType<DtUsuariosHandler>().As<IRequestHandler<DtUsuariosQuery, DataTable>>();

            #region <--TABLA MAESTROS SGP-->
            builder.RegisterType<DescargarTablaPlantillasHandler>().As<IRequestHandler<DescargarTablaPlantillasCommand, DescargarPlantillasDTO>>();

			builder.RegisterType<ListarMaeEmpresaHandler>().As<IRequestHandler<ListarMaeEmpresaQuery, GenericResponseDTO<List<ListarMaeEmpresaDTO>>>>();
			builder.RegisterType<ObtenerMaeEmpresaHandler>().As<IRequestHandler<ObtenerMaeEmpresaQuery, GenericResponseDTO<ObtenerMaeEmpresaDTO>>>();
			builder.RegisterType<CrearMaestroEmpresaHandler>().As<IRequestHandler<CrearMaeEmpresaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaestroEmpresaHandler>().As<IRequestHandler<ActualizarMaeEmpresaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaestroEmpresaHandler>().As<IRequestHandler<ImportarMaeEmpresaCommand, RespuestaComunExcelDTO>>();

			builder.RegisterType<ListarMaeCadenaHandler>().As<IRequestHandler<ListarMaeCadenaQuery, GenericResponseDTO<List<ListarMaeCadenaDTO>>>>();
			builder.RegisterType<ObtenerMaeCadenaHandler>().As<IRequestHandler<ObtenerMaeCadenaQuery, GenericResponseDTO<ObtenerMaeCadenaDTO>>>();
			builder.RegisterType<CrearMaeCadenaHandler>().As<IRequestHandler<CrearMaeCadenaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaeCadenaHandler>().As<IRequestHandler<ActualizarMaeCadenaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaeCadenaHandler>().As<IRequestHandler<ImportarMaeCadenaCommand, RespuestaComunExcelDTO>>();

			builder.RegisterType<ListarMaeRegionHandler>().As<IRequestHandler<ListarMaeRegionQuery, GenericResponseDTO<List<ListarMaeRegionDTO>>>>();
			builder.RegisterType<ObtenerMaeRegionHandler>().As<IRequestHandler<ObtenerMaeRegionQuery, GenericResponseDTO<ObtenerMaeRegionDTO>>>();
			builder.RegisterType<CrearMaeRegionHandler>().As<IRequestHandler<CrearMaeRegionCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaeRegionHandler>().As<IRequestHandler<ActualizarMaeRegionCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaeRegionHandler>().As<IRequestHandler<ImportarMaeRegionCommand, RespuestaComunExcelDTO>>();

			builder.RegisterType<ListarMaeZonaHandler>().As<IRequestHandler<ListarMaeZonaQuery, GenericResponseDTO<List<ListarMaeZonaDTO>>>>();
			builder.RegisterType<ObtenerMaeZonaHandler>().As<IRequestHandler<ObtenerMaeZonaQuery, GenericResponseDTO<ObtenerMaeZonaDTO>>>();
			builder.RegisterType<CrearMaeZonaHandler>().As<IRequestHandler<CrearMaeZonaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaeZonaHandler>().As<IRequestHandler<ActualizarMaeZonaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaeZonaHandler>().As<IRequestHandler<ImportarMaeZonaCommand, RespuestaComunExcelDTO>>();

            builder.RegisterType<ListarMaeLocalHandler>().As<IRequestHandler<ListarMaeLocalQuery, GenericResponseDTO<List<ListarMaeLocalDTO>>>>();
            builder.RegisterType<ObtenerMaeLocalHandler>().As<IRequestHandler<ObtenerMaeLocalQuery, GenericResponseDTO<ObtenerMaeLocalDTO>>>();
            builder.RegisterType<CrearMaeLocalHandler>().As<IRequestHandler<CrearMaeLocalCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaeLocalHandler>().As<IRequestHandler<ActualizarMaeLocalCommand, RespuestaComunDTO>>();
            builder.RegisterType<ImportarMaeLocalHandler>().As<IRequestHandler<ImportarMaeLocalCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<DescargarMaeLocalHandler>().As<IRequestHandler<DescargarMaeLocalCommand, DescargarMaestroDTO>>();
			builder.RegisterType<DescargarMaeLocalXEmpresaHandler>().As<IRequestHandler<DescargarMaeLocalXEmpresaCommand, DescargarMaestroDTO>>();


			builder.RegisterType<ListarMaeCajaHandler>().As<IRequestHandler<ListarMaeCajaQuery, GenericResponseDTO<List<ListarMaeCajaDTO>>>>();
			builder.RegisterType<ObtenerMaeCajaHandler>().As<IRequestHandler<ObtenerMaeCajaQuery, GenericResponseDTO<ObtenerMaeCajaDTO>>>();
			builder.RegisterType<CrearMaeCajaHandler>().As<IRequestHandler<CrearMaeCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaeCajaHandler>().As<IRequestHandler<ActualizarMaeCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaeCajaHandler>().As<IRequestHandler<ImportarMaeCajaCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<EliminarMaeCajasHandler>().As<IRequestHandler<EliminarMaeCajasCommand, RespuestaComunDTO>>();
			builder.RegisterType<DescargarMaestroCajalHandler>().As<IRequestHandler<DescargarMaeCajaCommand, DescargarMaestroDTO>>();

			builder.RegisterType<ImportarInventarioActivoHandler>().As<IRequestHandler<ImportarInventarioActivoCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<ListarAperturaHandler>().As<IRequestHandler<ListarAperturaQuery, GenericResponseDTO<List<ListarAperturaDTO>>>>();
			builder.RegisterType<ObtenerAperturaHandler>().As<IRequestHandler<ObtenerAperturaQuery, GenericResponseDTO<ObtenerAperturaDTO>>>();
			builder.RegisterType<CrearAperturaHandler>().As<IRequestHandler<CrearAperturaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarAperturaHandler>().As<IRequestHandler<ActualizarAperturaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarAperturaHandler>().As<IRequestHandler<ImportarAperturaCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<DescargarAperturaHandler>().As<IRequestHandler<DescargarAperturaCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ListarUbiDepartamentoHandler>().As<IRequestHandler<ListarUbiDepartamentoQuery, GenericResponseDTO<List<ListarUbiDepartamentoDTO>>>>();
			builder.RegisterType<ListarUbiProvinciaHandler>().As<IRequestHandler<ListarUbiProvinciaQuery, GenericResponseDTO<List<ListarUbiProvinciaDTO>>>>();
			builder.RegisterType<ListarUbiDistritoHandler>().As<IRequestHandler<ListarUbiDistritoQuery, GenericResponseDTO<List<ListarUbiDistritoDTO>>>>();
			builder.RegisterType<ObtenerUbigeoHandler>().As<IRequestHandler<ObtenerUbigeoQuery, GenericResponseDTO<ObtenerUbiDistritoDTO>>>();

			#endregion <--TABLA MAESTROS SGP-->


			base.Load(builder);
		}
	}
}
