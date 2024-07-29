namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ProcesarMonitorLocalBCTDTO
	{
        public string CodEmpresa { get; set; }
        public string NomEmpresa { get; set; }
		public string CodLocal { get; set; }
        public string NomLocal { get; set; }
        public string IP { get; set; }
        public int CantTransaccionesLocal { get; set; }
        public int CanTransaccionesBCT { get; set; }
        public decimal NontoTransaccionesLocal { get; set; }
        public decimal MontoTransaccionesBCT { get; set; }
        public string Estado { get; set; }
        public string ColorEstado { get; set; }
		public string Observacion { get; set; }
    }
}
