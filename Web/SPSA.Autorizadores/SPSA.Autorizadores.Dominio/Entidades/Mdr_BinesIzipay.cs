namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mdr_BinesIzipay
    {
        public string CodEmpresa { get; set; }
        public string NumAno { get; set; }
        public string NumBin6 { get; set; }
        public string NumBin8 { get; set; }
        public string NomTarjeta { get; set; }
        public string BancoEmisor { get; set; }
        public string Tipo { get; set; }
        public decimal FactorMdr { get; set; }
        public string CodOperador { get; set; }
    }
}
