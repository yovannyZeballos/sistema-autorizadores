using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Seg_Sistema
	{
		public string CodSistema { get; set; }
		public string NomSistema { get; set; }
		public string Sigla { get; set; }
		public string IndActivo { get; set; }
		public DateTime FecCreacion { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecModifica { get; set; }
		public string UsuModifica { get; set; }
	}
}
