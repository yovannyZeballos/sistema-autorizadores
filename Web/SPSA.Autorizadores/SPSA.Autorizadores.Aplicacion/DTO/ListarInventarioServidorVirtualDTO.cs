using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarInventarioServidorVirtualDTO : RespuestaComunDTO
	{
		public List<Dictionary<string, object>> Virtuales { get; set; }
		public List<string> Columnas { get; set; }
	}
}
