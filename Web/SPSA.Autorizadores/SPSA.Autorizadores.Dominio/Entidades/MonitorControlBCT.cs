namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class MonitorControlBCT
	{
		public string Fecha { get; set; }
		public int CodSucursal { get; set; }
		public string DesSucursal { get; set; }
		public string Horario { get; set; }
		public int TiempoLim { get; set; }
		public string UltimaTransf { get; set; }
		public int Diferencia { get; set; }
		public string Semaforo { get; set; }
	}
}
