namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ProcesoMonitorDTO : RespuestaComunDTO
    {
        public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
		public string CodRegion { get; set; }
		public string CodZona { get; set; }
		public string CodLocal { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
    }
}
