using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Seg_PerfilMenu
	{
        public string CodPerfil { get; set; }
        public string CodMenu { get; set; }
        public string CodSistema { get; set; }
		public DateTime FecCreacion { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecModifica { get; set; }
		public string UsuModifica { get; set; }
        public Seg_Menu Menu { get; set; }
    }
}
