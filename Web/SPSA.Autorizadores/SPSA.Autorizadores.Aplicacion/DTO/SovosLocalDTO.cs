namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class SovosLocalDTO : RespuestaComunDTO
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodFormato { get; set; }
        public string NomLocal { get; set; }
        public string Ip { get; set; }
        public string IpMascara { get; set; }
        public string SO { get; set; }
        public decimal? Grupo { get; set; }
        public string Estado { get; set; }
        public string TipoLocal { get; set; }
        public string IndFactura { get; set; }
        public string CodigoSunat { get; set; }

    }
}
