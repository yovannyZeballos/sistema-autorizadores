using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarAutorizadorDTO : RespuestaComunDTO
	{
		public List<Dictionary<string, object>> Autorizadores { get; set; }
		public List<string> Columnas { get; set; }
	}
}
