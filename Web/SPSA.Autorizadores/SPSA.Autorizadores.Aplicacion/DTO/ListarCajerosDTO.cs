using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarCajerosDTO : RespuestaComunDTO
	{
		public List<Dictionary<string, object>> Cajeros { get; set; }
		public List<string> Columnas { get; set; }
	}
}
