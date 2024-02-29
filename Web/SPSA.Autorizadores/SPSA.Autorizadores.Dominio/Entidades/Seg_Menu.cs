using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Seg_Menu
	{
        public string CodSistema { get; set; }
        public string CodMenu { get; set; }
        public string NomMenu { get; set; }
        public string CodMenuPadre { get; set; }
        public string IndActivo { get; set; }
        public DateTime FecCreacion { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }
	}
}
