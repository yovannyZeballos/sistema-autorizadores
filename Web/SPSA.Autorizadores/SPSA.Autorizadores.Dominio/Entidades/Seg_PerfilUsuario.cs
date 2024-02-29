using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Seg_PerfilUsuario
	{
		public string CodUsuario { get; set; }
		public string CodPerfil { get; set; }
		public string IndActivo { get; set; }
		public DateTime FecCreacion { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecModifica { get; set; }
		public string UsuModifica { get; set; }
        public Seg_Perfil Seg_Perfil { get; set; }

	}
}
