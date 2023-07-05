using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class SovosCajaInventario
	{
		public string CodEmpresa { get; private set; }
		public string CodFormato { get; private set; }
		public string CodLocal { get; private set; }
		public decimal NumPos { get; private set; }
		public string Ranking { get; private set; }
		public string Estado { get; private set; }
		public string Sede { get; private set; }
		public string Ubicacion { get; private set; }
		public string Caja { get; private set; }
		public string ModeloCpu { get; private set; }
		public string Serie { get; private set; }
		public string ModeloPrint { get; private set; }
		public string SeriePrint { get; private set; }
		public string ModeloDinakey { get; private set; }
		public string SerieDinakey { get; private set; }
		public string ModeloScanner { get; private set; }
		public string SerieScanner { get; private set; }
		public string ModeloGaveta { get; private set; }
		public string SerieGaveta { get; private set; }
		public string ModeloMonitor { get; private set; }
		public string SerieMonitor { get; private set; }
		public DateTime? FechaApertura { get; private set; }
		public string Caract1 { get; private set; }
		public string Caract2 { get; private set; }
		public string Caract3 { get; private set; }
		public DateTime? FechaLising { get; private set; }
		public string So { get; private set; }
		public string VesionSo { get; private set; }
		public DateTime? FechaAsignacion { get; private set; }
		public string Usuario { get; private set; }
		public string UsuarioCreacion { get; private set; }
		public string UsuarioModificacion { get; private set; }
		public DateTime? FechaCreacion { get; private set; }
		public DateTime? FechaModificacion { get; private set; }


		public SovosCajaInventario(string codEmpresa, string codFormato, string codLocal, decimal numPos, string ranking,
			string estado, string sede, string ubicacion, string caja, string modeloCpu, string serie, string modeloPrint,
			string seriePrint, string modeloDinakey, string serieDinakey, string modeloScanner, string serieScanner, string
			modeloGaveta, string serieGaveta, string modeloMonitor, string serieMonitor, DateTime? fechaApertura, string caract1,
			string caract2, string caract3, DateTime? fechaLising, string so, string vesionSo, DateTime? fechaAsignacion, string usuario)
		{
			CodEmpresa = codEmpresa ?? "";
			CodFormato = codFormato ?? "";
			CodLocal = codLocal ?? "";
			NumPos = numPos;
			Ranking = ranking ?? "";
			Estado = estado ?? "";
			Sede = sede ?? "";
			Ubicacion = ubicacion ?? "";
			Caja = caja ?? "";
			ModeloCpu = modeloCpu ?? "";
			Serie = serie ?? "";
			ModeloPrint = modeloPrint ?? "";
			SeriePrint = seriePrint ?? "";
			ModeloDinakey = modeloDinakey ?? "";
			SerieDinakey = serieDinakey ?? "";
			ModeloScanner = modeloScanner ?? "";
			SerieScanner = serieScanner ?? "";
			ModeloGaveta = modeloGaveta ?? "";
			SerieGaveta = serieGaveta ?? "";
			ModeloMonitor = modeloMonitor ?? "";
			SerieMonitor = serieMonitor ?? "";
			FechaApertura = fechaApertura;
			Caract1 = caract1 ?? "";
			Caract2 = caract2 ?? "";
			Caract3 = caract3 ?? "";
			FechaLising = fechaLising;
			So = so ?? "";
			VesionSo = vesionSo ?? "";
			FechaAsignacion = fechaAsignacion;
			Usuario = usuario ?? "";
		}

		public SovosCajaInventario(string codEmpresa, string codFormato, string codLocal, decimal numPos, string ranking,
			string estado, string sede, string ubicacion, string caja, string modeloCpu, string serie, string modeloPrint,
			string seriePrint, string modeloDinakey, string serieDinakey, string modeloScanner, string serieScanner, string
			modeloGaveta, string serieGaveta, string modeloMonitor, string serieMonitor, DateTime? fechaApertura, string caract1,
			string caract2, string caract3, DateTime? fechaLising, string so, string vesionSo, DateTime? fechaAsignacion, string usuarioCreacion,
			string usuarioModificacion, DateTime? fechaCreacion, DateTime? fechaModificacion)
		{
			CodEmpresa = codEmpresa ?? "";
			CodFormato = codFormato ?? "";
			CodLocal = codLocal ?? "";
			NumPos = numPos;
			Ranking = ranking ?? "";
			Estado = estado ?? "";
			Sede = sede ?? "";
			Ubicacion = ubicacion ?? "";
			Caja = caja ?? "";
			ModeloCpu = modeloCpu ?? "";
			Serie = serie ?? "";
			ModeloPrint = modeloPrint ?? "";
			SeriePrint = seriePrint ?? "";
			ModeloDinakey = modeloDinakey ?? "";
			SerieDinakey = serieDinakey ?? "";
			ModeloScanner = modeloScanner ?? "";
			SerieScanner = serieScanner ?? "";
			ModeloGaveta = modeloGaveta ?? "";
			SerieGaveta = serieGaveta ?? "";
			ModeloMonitor = modeloMonitor ?? "";
			SerieMonitor = serieMonitor ?? "";
			FechaApertura = fechaApertura;
			Caract1 = caract1 ?? "";
			Caract2 = caract2 ?? "";
			Caract3 = caract3 ?? "";
			FechaLising = fechaLising;
			So = so ?? "";
			VesionSo = vesionSo ?? "";
			FechaAsignacion = fechaAsignacion;
			UsuarioCreacion = usuarioCreacion ?? "";
			UsuarioModificacion = usuarioModificacion ?? "";
			FechaCreacion = fechaCreacion;
			FechaModificacion = fechaModificacion;
		}
	}
}
