using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarPerfilDTO
	{
		public string CodPerfil { get; set; }
		public string NomPerfil { get; set; }
		public string TipPerfil { get; set; }
		public string IndActivo { get; set; }
		public DateTime FecCreacion { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecModifica { get; set; }
		public string UsuModifica { get; set; }
		public bool IndAsociado { get; set; }
	}
}
