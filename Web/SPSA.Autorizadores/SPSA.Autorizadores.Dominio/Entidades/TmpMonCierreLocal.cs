using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class TmpMonCierreLocal
	{
		public int CodLocalAlterno { get; set; }
		public DateTime? FechaContable { get; set; }
		public DateTime? FechaCierre { get; set; }
		public string TipEstado { get; set; }
		public DateTime FechaCarga { get; set; }
		public TimeSpan HoraCarga { get; set; }

	}
}
