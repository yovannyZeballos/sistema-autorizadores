﻿namespace SPSA.Autorizadores.Aplicacion
{
	public class Constantes
	{
		public const string MensajeArchivoNoEncontrado = "No se encontró el archivo de cierre.";
		public const string MensajeFechaInicioNoEncontrado = "No existe fecha inicial en el archivo de cierre, validar en unos minutos!";
		public const string MensajeFechaFinNoEncontrado = "No existe fecha final en el archivo de cierre, validar en unos minutos!";
		public const string CodigoComandoCantidadCajasDefectuosos = "001";
		public const string CodigoComandoCajasDefectuosos = "002";
		public const string CodigoComandoNombreArchivoCierreEOD = "001";
		public const string CodigoComandoObtenerArchivoCierreEOD = "002";
		public const string CodigoComandoHoraInicioArchivoCierreEOD = "003";
		public const string CodigoComandoHoraFinArchivoCierreEOD = "004";

		public const decimal CodigoProcesoBct = 29;
		public const decimal CodigoProcesoActualizacionEstadoCierre = 30;
		public const decimal CodigoProcesoDiferenciaTransacciones = 31;
		public const decimal CodigoProcesoImpresionCodigoBarras = 32;
		public const decimal CodigoProcesoMonitorArchivos = 33;

		public const string CodigoParametroToleranciaSegundos = "01";
		public const string CodigoParametroToleranciaCantidad = "02";
		public const string CodigoParametroServidorBD = "03";
		public const string CodigoParametroInstanciaBD = "04";
		public const string CodigoParametroNombreBD = "05";
		public const string CodigoParametroUsuarioBD = "06";
		public const string CodigoParametroClaveBD = "07";
		public const string CodigoParametroToleranciaAlerta = "08";
		public const string CodigoParametroFechaNegocioAlerta = "09";
		public const string CodigoParametroConexionAlerta = "10";
		public const string CodigoParametroNombreTabla = "11";

		public const string CodigoParametroUsuarioBD_ProcesoCierre = "01";
		public const string CodigoParametroClaveBD_ProcesoCierre = "02";
		public const string CodigoParametroServidorBD_ProcesoCierre = "03";
		public const string CodigoParametroServiceNameBD_ProcesoCierre = "04";
		public const string CodigoParametroPuertoBD_ProcesoCierre = "05";
		public const string CodigoParametroFormatoCodigoAlterno_ProcesoCierre = "06";

		public const string CodigoUsuarioCaja_ProcesoMonitorArchivo = "01";
		public const string CodigoClaveCaja_ProcesoMonitorArchivo = "02";
		public const string CodigoRutaArchivos1_ProcesoMonitorArchivo = "03";
		public const string CodigoRutaArchivos2_ProcesoMonitorArchivo = "04";
	}
}
