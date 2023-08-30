using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarCajerosDTO : RespuestaComunDTO
	{
		public List<Dictionary<string, object>> Cajeros { get; set; }
		public List<string> Columnas { get; set; }
	}
}
