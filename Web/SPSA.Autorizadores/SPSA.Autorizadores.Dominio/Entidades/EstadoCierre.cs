using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class EstadoCierre
	{
        public int CodLocal { get; set; }
        public string DesLocal { get; set; }
        public DateTime? FechaCierreContable { get; set; }
        public string DiaCierre { get; set; }
		public DateTime? FechaCierre { get; set; }
		public string Estado { get; set; }


	}
}
