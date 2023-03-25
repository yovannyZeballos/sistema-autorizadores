namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ProcesoMonitorDTO : RespuestaComunDTO
    {
        public string CodLocal { get; set; }
        public string CodFormato { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
    }
}
