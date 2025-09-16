using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarComunDTO<T> : RespuestaComunDTO
	{
		public List<T> Data { get; set; }
		public List<string> Columnas { get; set; }
		public int TotalRegistros { get; set; }
		public int TotalFiltrados { get; set; }
	}
}
