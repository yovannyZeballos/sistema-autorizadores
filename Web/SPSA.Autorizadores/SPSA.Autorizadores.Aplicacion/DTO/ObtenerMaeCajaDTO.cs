namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ObtenerMaeCajaDTO
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumCaja { get; set; }
        public string IpAddress { get; set; }
        public string TipOs { get; set; }
        public string TipEstado { get; set; }
    }
}
