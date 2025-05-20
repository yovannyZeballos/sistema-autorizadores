namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    public class MaeCodComercioDTO
    {
        public decimal NroSolicitud { get; set; }
        public int CodLocalAlterno { get; set; }
        public string CodComercio { get; set; }
        public string NomCanalVta { get; set; }
        public string DesOperador { get; set; }
        public string IndActiva { get; set; }
    }
}
