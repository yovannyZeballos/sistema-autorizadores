using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ObtenerComunDTO<T> : RespuestaComunDTO
	{
		public T Data { get; set; }
		public List<string> Columnas { get; set; }
	}
}
