using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class SovosCajaInventarioDTO : RespuestaComunDTO
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public decimal NumPos { get; set; }
		public string Ranking { get; set; }
		public string Estado { get; set; }
		public string Sede { get; set; }
		public string Ubicacion { get; set; }
		public string Caja { get; set; }
		public string ModeloCpu { get; set; }
		public string Serie { get; set; }
		public string ModeloPrint { get; set; }
		public string SeriePrint { get; set; }
		public string ModeloDinakey { get; set; }
		public string SerieDinakey { get; set; }
		public string ModeloScanner { get; set; }
		public string SerieScanner { get; set; }
		public string ModeloGaveta { get; set; }
		public string SerieGaveta { get; set; }
		public string ModeloMonitor { get; set; }
		public string SerieMonitor { get; set; }
		public string FechaApertura { get; set; }
		public string Caract1 { get; set; }
		public string Caract2 { get; set; }
		public string Caract3 { get; set; }
		public string FechaLising { get; set; }
		public string So { get; set; }
		public string VesionSo { get; set; }
		public string FechaAsignacion { get; set; }
		public string Usuario { get; set; }
		public string UsuarioCreacion { get; set; }
		public string UsuarioModificacion { get; set; }
		public string FechaCreacion { get; set; }
		public string FechaModificacion { get; set; }
	}
}
