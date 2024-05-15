using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Proceso
	{
        public int CodProceso { get; set; }
        public string DesProceso { get; set; }
        public string IndActivo { get; set; }
        public int NumOrden { get; set; }
        public string IndReproceso { get; set; }
		public string NomProceso { get; set; }
		public string Email { get; set; }
		public string UsuCreacion { get; set; }
        public DateTime? FecCreacion { get; set; }
		public string UsuElimina { get; set; }
		public DateTime? FecElimina { get; set; }
	}
}
