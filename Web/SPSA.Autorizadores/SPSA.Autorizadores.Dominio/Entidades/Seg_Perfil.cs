using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Seg_Perfil
	{
		public string CodPerfil { get; set; }
		public string NomPerfil { get; set; }
		public string TipPerfil { get; set; }
		public string IndActivo { get; set; }
		public DateTime FecCreacion { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecModifica { get; set; }
		public string UsuModifica { get; set; }
    }




}
