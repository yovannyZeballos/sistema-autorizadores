using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ParametrosMonitorBctDTO : RespuestaComunDTO
	{
		public int ToleranciaSegundos { get; set; }
		public int ToleranciaCantidad { get; set; }
		public string CodEmpresa { get; set; }
		public string FechaNegocio { get; set; }
		public string EstadoConexion { get; set; }
	}
}
