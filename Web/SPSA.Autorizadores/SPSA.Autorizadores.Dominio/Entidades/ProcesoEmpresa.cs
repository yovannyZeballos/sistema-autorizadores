using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class ProcesoEmpresa
	{
		public decimal CodProceso { get; set; }
		public string CodEmpresa { get; set; }
		public string IndActivo { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecCreacion { get; set; }
		public string UsuElimina { get; set; }
		public DateTime? FecElimina { get; set; }
		public string Mail { get; set; }
	}
}
