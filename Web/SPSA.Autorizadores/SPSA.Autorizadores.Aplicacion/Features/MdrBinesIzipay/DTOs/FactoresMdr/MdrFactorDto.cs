namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.FactoresMdr
{
    public class MdrFactorDto
    {
        public string CodEmpresa { get; set; }
        public int CodPeriodo { get; set; }
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
        public decimal Factor { get; set; }
        public string IndActivo { get; set; }
    }
}
