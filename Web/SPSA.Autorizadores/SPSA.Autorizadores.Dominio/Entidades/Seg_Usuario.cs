using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Seg_Usuario
	{
        public string CodUsuario { get; set; }
        public string CodColaborador { get; set; }
        public string IndActivo { get; set; }
        public string TipUsuario { get; set; }
        public string DirEmail { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
		public DateTime? FecElimina { get; set; }
		public string UsuElimina { get; set; }
        public DateTime? FecIngreso { get; set; }
        public TimeSpan? HoraIngreso { get; set; }
    }
}
