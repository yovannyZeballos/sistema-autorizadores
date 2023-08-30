using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarComunDTO<T> : RespuestaComunDTO
	{
		public List<T> Data { get; set; }
		public List<string> Columnas { get; set; }
	}
}
