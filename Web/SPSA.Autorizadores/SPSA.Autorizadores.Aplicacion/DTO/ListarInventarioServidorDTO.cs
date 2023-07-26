using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarInventarioServidorDTO : RespuestaComunDTO
	{
		public List<Dictionary<string, object>> Servidores { get; set; }
		public List<string> Columnas { get; set; }
	}
}
