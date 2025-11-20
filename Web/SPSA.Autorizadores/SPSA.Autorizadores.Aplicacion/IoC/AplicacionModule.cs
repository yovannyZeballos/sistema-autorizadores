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
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Queries;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Correo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.DataTableSGP.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.AreaGestion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaRecepcion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Kardex;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Marca;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Producto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Proveedor;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Servidor;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.AreaGestion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Marca;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Producto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Proveedor;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Servidor;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.AreaGestion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaRecepcion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Marca;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Producto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Proveedor;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Servidor;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.Bines;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.ClasificacionMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.OperadorMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.ClasificacionMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.OperadorMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Features.Monitor;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Operaciones.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries;
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
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Queries;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Queries;
using SPSA.Autorizadores.Aplicacion.Features.TablasMae.Commands;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Command;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Ubigeos.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Queries;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen.Dto;
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
			builder.RegisterType<ListarAutorizadoresHandler>().As<IRequestHandler<ListarAutorizadoresQuery, ListarAutorizadorDTO>>();
			builder.RegisterType<ActualizarEstadoArchivoHandler>().As<IRequestHandler<ActualizarEstadoArchivoCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarAutorizadorHandler>().As<IRequestHandler<EliminarAutorizadorCommand, RespuestaComunDTO>>();
			builder.RegisterType<Features.Locales.Queries.ObtenerLocalHandler>().As<IRequestHandler<Features.Locales.Queries.ObtenerLocalQuery, LocalDTO>>();
			//builder.RegisterType<ListarColaboradoresCesadosHandler>().As<IRequestHandler<ListarColaboradoresCesadosQuery, ColaboradoresCesadosDTO>>();
			builder.RegisterType<ListarLocalesAsignarHandler>().As<IRequestHandler<ListarLocalesAsignarQuery, LocalesAsignadosDTO>>();
			builder.RegisterType<AsignarLocalHandler>().As<IRequestHandler<AsignarLocalCommand, RespuestaComunDTO>>();
			builder.RegisterType<ListarPuestosHandler>().As<IRequestHandler<ListarPuestosQuery, PuestoDTO>>();
			builder.RegisterType<ListarEmpresasOfiplanHandler>().As<IRequestHandler<ListarEmpresasOfiplanQuery, ListarEmpresaResponseDTO>>();
			builder.RegisterType<ActualizarPuestoHandler>().As<IRequestHandler<ActualizarPuestoCommand, RespuestaComunDTO>>();
			//builder.RegisterType<ListarColaboradoresMassHandler>().As<IRequestHandler<ListarColaboradoresMassQuery, DataTable>>();
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
			//builder.RegisterType<GenerarArchivoPorLocalHandler>().As<IRequestHandler<GenerarArchivoPorLocalCommand, DescargarPlantillasDTO>>();
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
			builder.RegisterType<ObtenerFechaNegocioHandler>().As<IRequestHandler<ObtenerFechaNegocioQuery, GenericResponseDTO<List<ParametrosMonitorBctDTO>>>>();
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
			builder.RegisterType<ListarLocalesAsociadasPorEmpresaHandler>().As<IRequestHandler<ListarLocalesAsociadasPorEmpresaQuery, GenericResponseDTO<List<ListarLocalDTO>>>>();
			builder.RegisterType<ObtenerJerarquiaOrganizacionalHandler>().As<IRequestHandler<ObtenerJerarquiaOrganizacionalQuery, JerarquiaOrganizacionalDTO>>();
			builder.RegisterType<ObtenerMenuHandler>().As<IRequestHandler<ObtenerMenuQuery, GenericResponseDTO<ListarMenuDTO>>>();
			builder.RegisterType<ObtenerMenusUsuarioHandler>().As<IRequestHandler<ObtenerMenusUsuarioQuery, GenericResponseDTO<List<ListarMenuDTO>>>>();
			builder.RegisterType<ImportarExcelInventarioCajaHandler>().As<IRequestHandler<ImportarExcelInventarioCajaCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<ImportarExcelInvCajaHandler>().As<IRequestHandler<ImportarExcelInvCajaCommand, RespuestaComunExcelDTO>>();//POR EL MOMENT

            builder.RegisterType<DescargarInventarioCajaHandler>().As<IRequestHandler<DescargarInventarioCajaCommand, RespuestaComunExcelDTO>>().InstancePerDependency();

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
			builder.RegisterType<ListarMaeLocalPorEmpresaHandler>().As<IRequestHandler<ListarMaeLocalPorEmpresaQuery, GenericResponseDTO<List<ListarMaeLocalDTO>>>>();
			builder.RegisterType<ObtenerMaeLocalHandler>().As<IRequestHandler<ObtenerMaeLocalQuery, GenericResponseDTO<ObtenerMaeLocalDTO>>>();
			builder.RegisterType<CrearMaeLocalHandler>().As<IRequestHandler<CrearMaeLocalCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaeLocalHandler>().As<IRequestHandler<ActualizarMaeLocalCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaeLocalHandler>().As<IRequestHandler<ImportarMaeLocalCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<DescargarMaeLocalHandler>().As<IRequestHandler<DescargarMaeLocalCommand, DescargarMaestroDTO>>();
			builder.RegisterType<DescargarMaeLocalPorEmpresaHandler>().As<IRequestHandler<DescargarMaeLocalPorEmpresaCommand, DescargarMaestroDTO>>();

			builder.RegisterType<ListarMaeCajaHandler>().As<IRequestHandler<ListarMaeCajaQuery, GenericResponseDTO<List<ListarMaeCajaDTO>>>>();
			builder.RegisterType<ObtenerMaeCajaHandler>().As<IRequestHandler<ObtenerMaeCajaQuery, GenericResponseDTO<ObtenerMaeCajaDTO>>>();
			builder.RegisterType<CrearMaeCajaHandler>().As<IRequestHandler<CrearMaeCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarMaeCajaHandler>().As<IRequestHandler<ActualizarMaeCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarMaeCajaHandler>().As<IRequestHandler<ImportarMaeCajaCommand, RespuestaComunExcelDTO>>();
			builder.RegisterType<EliminarMaeCajasHandler>().As<IRequestHandler<EliminarMaeCajasCommand, RespuestaComunDTO>>();
			builder.RegisterType<DescargarMaestroCajaHandler>().As<IRequestHandler<DescargarMaeCajaCommand, DescargarMaestroDTO>>();
			builder.RegisterType<DescargarMaestroCajaPorEmpresaHandler>().As<IRequestHandler<DescargarMaeCajaPorEmpresaCommand, DescargarMaestroDTO>>();

            builder.RegisterType<ListarMaeHorarioHandler>().As<IRequestHandler<ListarMaeHorarioQuery, GenericResponseDTO<List<ListarMaeHorarioDTO>>>>();
            builder.RegisterType<ObtenerMaeHorarioHandler>().As<IRequestHandler<ObtenerMaeHorarioQuery, GenericResponseDTO<ObtenerMaeHorarioDTO>>>();
            builder.RegisterType<CrearMaeHorarioHandler>().As<IRequestHandler<CrearMaeHorarioCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaeHorarioHandler>().As<IRequestHandler<ActualizarMaeHorarioCommand, RespuestaComunDTO>>();
            builder.RegisterType<ImportarMaeHorarioHandler>().As<IRequestHandler<ImportarMaeHorarioCommand, RespuestaComunExcelDTO>>();
            builder.RegisterType<EliminarMaeHorarioHandler>().As<IRequestHandler<EliminarMaeHorarioCommand, RespuestaComunDTO>>();
            builder.RegisterType<DescargarMaestroHorarioHandler>().As<IRequestHandler<DescargarMaeHorarioCommand, DescargarMaestroDTO>>();
            //builder.RegisterType<DescargarMaestroHorarioPorEmpresaHandler>().As<IRequestHandler<DescargarMaeHorarioPorEmpresaCommand, DescargarMaestroDTO>>();

            builder.RegisterType<ListarInvTipoActivoHandler>().As<IRequestHandler<ListarInvTipoActivoQuery, GenericResponseDTO<List<InvTipoActivoDTO>>>>();
            builder.RegisterType<DescargarInvTiposActivoHandler>().As<IRequestHandler<DescargarInvTiposActivoCommand, DescargarMaestroDTO>>();

            builder.RegisterType<ListarInvActivoHandler>().As<IRequestHandler<ListarInvActivoQuery, GenericResponseDTO<List<ListarInvActivoDTO>>>>();
			builder.RegisterType<ObtenerInvActivoHandler>().As<IRequestHandler<ObtenerInvActivoQuery, GenericResponseDTO<ObtenerInvActivoDTO>>>();
			builder.RegisterType<CrearInvActivoHandler>().As<IRequestHandler<CrearInvActivoCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarInvActivoHandler>().As<IRequestHandler<ActualizarInvActivoCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImportarInventarioActivoHandler>().As<IRequestHandler<ImportarInventarioActivoCommand, RespuestaComunExcelDTO>>();
            builder.RegisterType<DescargarInvActivoHandler>().As<IRequestHandler<DescargarInvActivoCommand, DescargarMaestroDTO>>();
            builder.RegisterType<EliminarInvActivoHandler>().As<IRequestHandler<EliminarInvActivoCommand, RespuestaComunDTO>>();

            builder.RegisterType<ListarInvCajaHandler>().As<IRequestHandler<ListarInvCajaQuery, GenericResponseDTO<List<ListarInvCajaDTO>>>>();
			builder.RegisterType<ObtenerInvCajaHandler>().As<IRequestHandler<ObtenerInvCajaQuery, GenericResponseDTO<ObtenerInvCajaDTO>>>();
			builder.RegisterType<ObtenerListasInvCajaHandler>().As<IRequestHandler<ObtenerListasInvCajaQuery, GenericResponseDTO<ObtenerListasInvCajaDTO>>>();
			builder.RegisterType<CrearInvCajaHandler>().As<IRequestHandler<CrearInvCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<ActualizarInvCajaHandler>().As<IRequestHandler<ActualizarInvCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarInvCajaHandler>().As<IRequestHandler<EliminarInvCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarInvCajaPorCajaHandler>().As<IRequestHandler<EliminarInvCajaPorCajaCommand, RespuestaComunDTO>>();
			builder.RegisterType<EliminarInvCajaPorLocalHandler>().As<IRequestHandler<EliminarInvCajaPorLocalCommand, RespuestaComunDTO>>();

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

			#region <--REPORTES-->

			builder.RegisterType<ListarLocalesCambioPrecioHandler>().As<IRequestHandler<ListarLocalesCambioPrecioQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ListarLocalesNotaCreditoHandler>().As<IRequestHandler<ListarLocalesNotaCreditoQuery, ListarComunDTO<Dictionary<string, object>>>>();
			builder.RegisterType<ListarValesRedimidosHandler>().As<IRequestHandler<ListarValesRedimidosQuery, ListarComunDTO<Dictionary<string, object>>>>();
            builder.RegisterType<DescargarValesRedimidosHandler>().As<IRequestHandler<DescargarValesRedimidosCommand, DescargarMaestroDTO>>();
            builder.RegisterType<ListarAutorizadoresPaginadoHandler>().As<IRequestHandler<ListarAutorizadoresPaginadoQuery, ListarComunDTO<Dictionary<string, object>>>>();
            builder.RegisterType<DescargarMaeAutorizadoresHandler>().As<IRequestHandler<DescargarMaeAutorizadoresCommand, DescargarMaestroDTO>>();
            builder.RegisterType<ListarCajerosPaginadoHandler>().As<IRequestHandler<ListarCajerosPaginadoQuery, ListarComunDTO<Dictionary<string, object>>>>();
            builder.RegisterType<DescargarMaeCajerosHandler>().As<IRequestHandler<DescargarMaeCajerosCommand, DescargarMaestroDTO>>();

            #endregion <--REPORTES-->
         
			builder.RegisterType<ProcesarActualizacionEstadoCierreHandler>().As<IRequestHandler<ProcesarActualizacionEstadoCierreCommand, RespuestaComunDTO>>();
			builder.RegisterType<ImprimirBarrasAutorizadorHandler>().As<IRequestHandler<ImprimirBarrasAutorizadorCommand, ImprimirAutorizadorResponseDTO>>();
			builder.RegisterType<ReimprimirBarrasAutorizadorHandler>().As<IRequestHandler<ReimprimirBarrasAutorizadorCommand, ImprimirAutorizadorResponseDTO>>();
			builder.RegisterType<ProcesarMonitorLocalBCTHandler>().As<IRequestHandler<ProcesarMonitorLocalBCTCommand, GenericResponseDTO<List<ProcesarMonitorLocalBCTDTO>>>>();
			builder.RegisterType<ListarEmpresasPorProcesoHandler>().As<IRequestHandler<ListarEmpresasPorProcesoQuery, GenericResponseDTO<List<ListarEmpresaDTO>>>>();
			builder.RegisterType<ListarMotivosReimpresionHandler>().As<IRequestHandler<ListarMotivosReimpresionQuery, GenericResponseDTO<Dictionary<string,string>>>>();
			builder.RegisterType<ListarProcesosHandler>().As<IRequestHandler<ListarProcesosQuery, ListarComunDTO<ListarProcesoDTO>>>();
			builder.RegisterType<ListarParametrosMonitorArchivoHandler>().As<IRequestHandler<ListarParametrosMonitorArchivoQuery, GenericResponseDTO<MonitorArchivoParametrosDTO>>>();
			builder.RegisterType<ProcesarMonitorArchivosHandler>().As<IRequestHandler<ProcesarMonitorArchivoscommand, GenericResponseDTO<List<ProcesarMonitorArchivoDTO>>>>();
            builder.RegisterType<ProcesarControlBCTSpsaHandler>().As<IRequestHandler<ProcesarControlBCTSpsaCommand, GenericResponseDTO<List<MonitorControlBCTDTO>>>>();
            builder.RegisterType<ProcesarControlBCTTpsaHandler>().As<IRequestHandler<ProcesarControlBCTTpsaCommand, GenericResponseDTO<List<MonitorControlBCTDTO>>>>();
			builder.RegisterType<ProcesarControlBCTHpsaHandler>().As<IRequestHandler<ProcesarControlBCTHpsaCommand, GenericResponseDTO<List<MonitorControlBCTDTO>>>>();

            #region MAE_COLABORADOR_EXT
            builder.RegisterType<ListarMaeColaboradorExtHandler>().As<IRequestHandler<ListarMaeColaboradorExtQuery, GenericResponseDTO<PagedResult<ListarMaeColaboradorExtDto>>>>();
            builder.RegisterType<ObtenerMaeColaboradorExtHandler>().As<IRequestHandler<ObtenerMaeColaboradorExtQuery, GenericResponseDTO<ObtenerMaeColaboradorExtDTO>>>();
            builder.RegisterType<CrearMaeColaboradorExtHandler>().As<IRequestHandler<CrearMaeColaboradorExtCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaeColaboradorExtHandler>().As<IRequestHandler<ActualizarMaeColaboradorExtCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarMaeColaboradorExtHandler>().As<IRequestHandler<EliminarMaeColaboradorExtCommand, RespuestaComunDTO>>();
            //builder.RegisterType<DescargarMaeColaboradorExtoHandler>().As<IRequestHandler<DescargarMaeColaboradorExtCommand, DescargarMaestroDTO>>();
            builder.RegisterType<ImportarMaeColaboradorExtHandler>().As<IRequestHandler<ImportarMaeColaboradorExtCommand, RespuestaComunExcelDTO>>();
            #endregion 

            #region MAE_COLABORADOR_INT
            builder.RegisterType<ListarMaeColaboradorIntHandler>().As<IRequestHandler<ListarMaeColaboradorIntQuery, GenericResponseDTO<PagedResult<ListarMaeColaboradorIntDTO>>>>();
            builder.RegisterType<ObtenerMaeColaboradorIntHandler>().As<IRequestHandler<ObtenerMaeColaboradorIntQuery, GenericResponseDTO<ObtenerMaeColaboradorIntDTO>>>();
            //builder.RegisterType<CrearMaeColaboradorExtHandler>().As<IRequestHandler<CrearMaeColaboradorExtCommand, RespuestaComunDTO>>();
            //builder.RegisterType<ActualizarMaeColaboradorExtHandler>().As<IRequestHandler<ActualizarMaeColaboradorExtCommand, RespuestaComunDTO>>();
            //builder.RegisterType<EliminarMaeColaboradorExtHandler>().As<IRequestHandler<EliminarMaeColaboradorExtCommand, RespuestaComunDTO>>();
            //builder.RegisterType<DescargarMaeColaboradorExtoHandler>().As<IRequestHandler<DescargarMaeColaboradorExtCommand, DescargarMaestroDTO>>();
            //builder.RegisterType<ImportarMaeColaboradorExtHandler>().As<IRequestHandler<ImportarMaeColaboradorExtCommand, RespuestaComunExcelDTO>>();
            #endregion 

            #region MAE_PUESTO
            builder.RegisterType<ListarMaePuestoHandler>().As<IRequestHandler<ListarMaePuestoQuery, GenericResponseDTO<PagedResult<ListarMaePuestoDTO>>>>();
            //builder.RegisterType<ObtenerMaeColaboradorExtHandler>().As<IRequestHandler<ObtenerMaeColaboradorExtQuery, GenericResponseDTO<ObtenerMaeColaboradorExtDTO>>>();
            //builder.RegisterType<CrearMaeColaboradorExtHandler>().As<IRequestHandler<CrearMaeColaboradorExtCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaePuestoHandler>().As<IRequestHandler<ActualizarMaePuestoCommand, RespuestaComunDTO>>();
            //builder.RegisterType<EliminarMaeColaboradorExtHandler>().As<IRequestHandler<EliminarMaeColaboradorExtCommand, RespuestaComunDTO>>();
            //builder.RegisterType<ImportarMaeColaboradorExtHandler>().As<IRequestHandler<ImportarMaeColaboradorExtCommand, RespuestaComunExcelDTO>>();
            #endregion

            builder.RegisterType<EnviarCorreoHandler>().As<IRequestHandler<EnviarCorreoCommand, RespuestaComunDTO>>();

            #region ASR_SOLICITUD_USUARIO
            builder.RegisterType<ListarSolicitudUsuarioHandler>().As<IRequestHandler<ListarSolicitudUsuarioQuery, GenericResponseDTO<PagedResult<ListarSolictudUsuarioDto>>>>();
            builder.RegisterType<CrearSolicitudUsuarioHandler>().As<IRequestHandler<CrearSolicitudUsuarioCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarSolicitudUsuarioHandler>().As<IRequestHandler<EliminarSolicitudUsuarioCommand, RespuestaComunDTO>>();
            builder.RegisterType<DescargarSolicitudesUsuarioHandler>().As<IRequestHandler<DescargarSolicitudesUsuarioCommand, DescargarMaestroDTO>>();
			builder.RegisterType<ListarUsuariosHandler>().As<IRequestHandler<ListarUsuariosQuery, GenericResponseDTO<PagedResult<ASR_UsuarioListado>>>>();
			builder.RegisterType<ListarSolicitudesSolicitadasHandler>().As<IRequestHandler<ListarSolicitudesSolicitadasQuery, GenericResponseDTO<PagedResult<ASR_UsuarioListado>>>>();
			builder.RegisterType<ActualizarMotivoRechazoHandler>().As<IRequestHandler<ActualizarMotivoRechazoCommand, RespuestaComunDTO>>();
			builder.RegisterType<AprobarSolicitudCrearHandler>().As<IRequestHandler<AprobarSolicitudCrearCommand, RespuestaComunDTO>>();
			builder.RegisterType<AprobarSolicitudEliminarHandler>().As<IRequestHandler<AprobarSolicitudEliminarCommand, RespuestaComunDTO>>();

            #endregion

            #region SOLICITUD_COD_COMERCIO
            builder.RegisterType<ListarSolicitudCComercioCabHandler>().As<IRequestHandler<ListarSolicitudCComercioCabQuery, GenericResponseDTO<PagedResult<CCom_SolicitudCabDto>>>>();
            builder.RegisterType<ImportarSolicitudCodComercioHandler>().As<IRequestHandler<ImportarSolicitudCodComercioCommand, RespuestaComunExcelDTO>>();
            builder.RegisterType<ImportarMaeLocalComercioHandler>().As<IRequestHandler<ImportarMaeLocalComercioCommand, RespuestaComunExcelDTO>>();
            builder.RegisterType<CrearEditarMaeCodComercioHandler>().As<IRequestHandler<CrearEditarMaeCodComercioCommand, RespuestaComunDTO>>();
            #endregion

            #region MDR_BINES_IZIPAY
            builder.RegisterType<ListarMdrFactorIzipayHandler>().As<IRequestHandler<ListarMdrFactorIzipayQuery, GenericResponseDTO<PagedResult<ListarMdrFactorDto>>>>();
            builder.RegisterType<ListarMdrOperadorHandler>().As<IRequestHandler<ListarMdrOperadorQuery, GenericResponseDTO<List<ListarMdrOperadorDto>>>>();
            builder.RegisterType<ListarMdrClasificacionHandler>().As<IRequestHandler<ListarMdrClasificacionQuery, GenericResponseDTO<List<ListarMdrClasificacionDto>>>>();
            builder.RegisterType<ListarMdrPeriodoHandler>().As<IRequestHandler<ListarMdrPeriodoQuery, GenericResponseDTO<List<ListarMdrPeriodoDto>>>>();
            builder.RegisterType<ListarPaginadoMdrPeriodoHandler>().As<IRequestHandler<ListarPaginadoMdrPeriodoQuery, GenericResponseDTO<PagedResult<ListarMdrPeriodoDto>>>>();

            builder.RegisterType<CrearMdrFactorIzipayHandler>().As<IRequestHandler<CrearMdrFactorIzipayCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMdrFactorIzipayHandler>().As<IRequestHandler<ActualizarMdrFactorIzipayCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarMdrFactorIzipayHandler>().As<IRequestHandler<EliminarMdrFactorIzipayCommand, RespuestaComunDTO>>();
            builder.RegisterType<ImportarMdrBinesInretailHandler>().As<IRequestHandler<ImportarMdrBinesInretailCommand, RespuestaComunExcelDTO>>();

            builder.RegisterType<CrearMdrPeriodoHandler>().As<IRequestHandler<CrearMdrPeriodoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMdrPeriodoHandler>().As<IRequestHandler<ActualizarMdrPeriodoCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarMdrPeriodoHandler>().As<IRequestHandler<EliminarMdrPeriodoCommand, RespuestaComunDTO>>();
            #endregion

            #region INV_KARDEX

            builder.RegisterType<CrearMaeMarcaHandler>().As<IRequestHandler<CrearMaeMarcaCommand, RespuestaComunDTO>>();
            builder.RegisterType<CrearMaeAreaGestionHandler>().As<IRequestHandler<CrearMaeAreaGestionCommand, RespuestaComunDTO>>();
            builder.RegisterType<CrearMaeProductoHandler>().As<IRequestHandler<CrearMaeProductoCommand, RespuestaComunDTO>>();
            builder.RegisterType<CrearMaeProveedorHandler>().As<IRequestHandler<CrearMaeProveedorCommand, RespuestaComunDTO>>();
            builder.RegisterType<CrearMaeSerieProductoHandler>().As<IRequestHandler<CrearMaeSerieProductoCommand, RespuestaComunDTO>>();
            builder.RegisterType<CrearSrvFisicoHandler>().As<IRequestHandler<CrearSrvFisicoCommand, RespuestaComunDTO>>();
            builder.RegisterType<CrearSrvVirtualHandler>().As<IRequestHandler<CrearSrvVirtualCommand, RespuestaComunDTO>>();

            builder.RegisterType<DarBajaSerieProductoHandler>().As<IRequestHandler<DarBajaSerieProductoCommand, RespuestaComunDTO>>();

            builder.RegisterType<RegistrarGuiaRecepcionHandler>().As<IRequestHandler<RegistrarGuiaRecepcionCommand, RespuestaComunDTO>>();
            builder.RegisterType<RegistrarGuiaDespachoHandler>().As<IRequestHandler<RegistrarGuiaDespachoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ConfirmarDespachoEnDestinoHandler>().As<IRequestHandler<ConfirmarDespachoEnDestinoCommand, RespuestaComunDTO>>();

            builder.RegisterType<ActualizarMaeMarcaHandler>().As<IRequestHandler<ActualizarMaeMarcaCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaeAreaGestionHandler>().As<IRequestHandler<ActualizarMaeAreaGestionCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaeProductoHandler>().As<IRequestHandler<ActualizarMaeProductoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarMaeProveedorHandler>().As<IRequestHandler<ActualizarMaeProveedorCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarSrvFisicoHandler>().As<IRequestHandler<ActualizarSrvFisicoCommand, RespuestaComunDTO>>();
            builder.RegisterType<ActualizarSrvVirtualHandler>().As<IRequestHandler<ActualizarSrvVirtualCommand, RespuestaComunDTO>>();

            builder.RegisterType<EliminarMaeMarcaHandler>().As<IRequestHandler<EliminarMaeMarcaCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarMaeAreaGestionHandler>().As<IRequestHandler<EliminarMaeAreaGestionCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarMaeProductoHandler>().As<IRequestHandler<EliminarMaeProductoCommand, RespuestaComunDTO>>();
            builder.RegisterType<EliminarMaeProveedorHandler>().As<IRequestHandler<EliminarMaeProveedorCommand, RespuestaComunDTO>>();

            builder.RegisterType<ObtenerGuiaDespachoHandler>().As<IRequestHandler<ObtenerGuiaDespachoQuery, GenericResponseDTO<GuiaDespachoCabeceraDto>>>();
            builder.RegisterType<ObtenerGuiaRecepcionHandler>().As<IRequestHandler<ObtenerGuiaRecepcionQuery, GenericResponseDTO<GuiaRecepcionCabeceraDto>>>();
            builder.RegisterType<ObtenerMaeSerieProductoHandler>().As<IRequestHandler<ObtenerMaeSerieProductoQuery, GenericResponseDTO<MaeSerieProductoDto>>>();

            builder.RegisterType<ListarMaeMarcaHandler>().As<IRequestHandler<ListarMaeMarcaQuery, GenericResponseDTO<List<ListarMaeMarcaDto>>>>();
            builder.RegisterType<ListarMaeAreaGestionHandler>().As<IRequestHandler<ListarMaeAreaGestionQuery, GenericResponseDTO<List<ListarMaeAreaGestionDto>>>>();
            builder.RegisterType<ListarMaeProductoHandler>().As<IRequestHandler<ListarMaeProductoQuery, GenericResponseDTO<List<ListarMaeProductoDto>>>>();
            builder.RegisterType<ListarMaeProductoPorLocalHandler>().As<IRequestHandler<ListarMaeProductoPorLocalQuery, GenericResponseDTO<List<ListarMaeProductoDto>>>>();
            builder.RegisterType<ListarMaeSerieProductoHandler>().As<IRequestHandler<ListarMaeSerieProductoQuery, GenericResponseDTO<List<ListarMaeSerieProductoDto>>>>();
            builder.RegisterType<ListarMaeSeriesPorProductoHandler>().As<IRequestHandler<ListarMaeSeriesPorProductoQuery, GenericResponseDTO<List<ListarMaeSerieProductoDto>>>>();
            builder.RegisterType<ListarMaeSeriesPorProductoDisponiblesHandler>().As<IRequestHandler<ListarMaeSeriesPorProductoDisponiblesQuery, GenericResponseDTO<List<ListarMaeSerieProductoDto>>>>();
            builder.RegisterType<ListarMaeProveedorHandler>().As<IRequestHandler<ListarMaeProveedorQuery, GenericResponseDTO<List<ListarMaeProveedorDto>>>>();
            builder.RegisterType<ListarTipoServidorHandler>().As<IRequestHandler<ListarTipoServidorQuery, GenericResponseDTO<List<SrvTipoServidor>>>>();
            builder.RegisterType<ListarSistemaOperativoHandler>().As<IRequestHandler<ListarSistemaOperativoQuery, GenericResponseDTO<List<SrvSistemaOperativo>>>>();
            builder.RegisterType<ListarPlataformasVmHandler>().As<IRequestHandler<ListarPlataformasVmQuery, GenericResponseDTO<List<SrvPlataformaVm>>>>();

            builder.RegisterType<ListarPaginadoMaeMarcaHandler>().As<IRequestHandler<ListarPaginadoMaeMarcaQuery, GenericResponseDTO<PagedResult<ListarMaeMarcaDto>>>>();
            builder.RegisterType<ListarPaginadoMaeAreaGestionHandler>().As<IRequestHandler<ListarPaginadoMaeAreaGestionQuery, GenericResponseDTO<PagedResult<ListarMaeAreaGestionDto>>>>();
            builder.RegisterType<ListarPaginadoMaeProveedorHandler>().As<IRequestHandler<ListarPaginadoMaeProveedorQuery, GenericResponseDTO<PagedResult<ListarMaeProveedorDto>>>>();
            builder.RegisterType<ListarPaginadoMaeProductoHandler>().As<IRequestHandler<ListarPaginadoMaeProductoQuery, GenericResponseDTO<PagedResult<ListarMaeProductoDto>>>>();
            builder.RegisterType<ListarPaginadoMaeSerieProductoHandler>().As<IRequestHandler<ListarPaginadoMaeSerieProductoQuery, GenericResponseDTO<PagedResult<ListarMaeSerieProductoDto>>>>();
            builder.RegisterType<ListarPaginadoGuiaRecepcionHandler>().As<IRequestHandler<ListarPaginadoGuiaRecepcionQuery, GenericResponseDTO<PagedResult<GuiaRecepcionCabeceraDto>>>>();
            builder.RegisterType<ListarPaginadoGuiaDespachoHandler>().As<IRequestHandler<ListarPaginadoGuiaDespachoQuery, GenericResponseDTO<PagedResult<GuiaDespachoCabeceraDto>>>>();
            builder.RegisterType<ListarPaginadoServidorFisicoHandler>().As<IRequestHandler<ListarPaginadoServidorFisicoQuery, GenericResponseDTO<PagedResult<ServidorFisicoDto>>>>();
            builder.RegisterType<ListarVmPorHostHandler>().As<IRequestHandler<ListarVmPorHostQuery, GenericResponseDTO<PagedResult<ServidorVirtualDto>>>>();

            builder.RegisterType<DescargarMovKardexPorFechasHandler>().As<IRequestHandler<DescargarMovKardexPorFechasCommand, RespuestaComunExcelDTO>>();

            #endregion

            #region OPERACIONES
            builder.RegisterType<ListarDocumentosElectronicosHandler>().As<IRequestHandler<ListarDocumentosElectronicosQuery, GenericResponseDTO<PagedResult<DocumentoElectronico>>>>();
			builder.RegisterType<ConsultarClienteCenQueryHandler>().As<IRequestHandler<ConsultarClienteCenQuery, GenericResponseDTO<ConsultaClienteRespuesta>>>();
			builder.RegisterType<InsertarClienteCenHandler>().As<IRequestHandler<InsertarClienteCenCommand, RespuestaComunDTO>>();
			builder.RegisterType<DescargarDocumentoElectronicoHandler>().As<IRequestHandler<DescargarDocumentoElectronicoCommand, GenericResponseDTO<byte[]>>>();
			#endregion


			base.Load(builder);
		}
	}
}
