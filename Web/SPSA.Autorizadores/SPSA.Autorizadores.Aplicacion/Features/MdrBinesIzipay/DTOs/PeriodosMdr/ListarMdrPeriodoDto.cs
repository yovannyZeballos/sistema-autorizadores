namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.PeriodosMdr
{
    public class ListarMdrPeriodoDto
    {
        public int CodPeriodo { get; set; }
        public string DesPeriodo { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
        public System.DateTime FecCreacion { get; set; }
        public string UsuElimina { get; set; }
        public System.DateTime? FecElimina { get; set; }
        public string UsuModifica { get; set; }
        public System.DateTime? FecModifica { get; set; }

    }
}
