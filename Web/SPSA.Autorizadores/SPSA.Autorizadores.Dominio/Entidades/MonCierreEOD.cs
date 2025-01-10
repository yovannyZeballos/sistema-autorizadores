using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class MonCierreEOD
	{
		public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
		public string CodRegion { get; set; }
		public string CodZona { get; set; }
		public string CodLocal { get; set; }
		public DateTime FechaProceso { get; set; }
		public DateTime? FechaCierre { get; set; }
		public string HoraInicio { get; set; }
		public string HoraFin { get; set; }
		public string Estado { get; set; }
		public string Observacion { get; set; }
		public int Tipo { get; set; }

	}
}
