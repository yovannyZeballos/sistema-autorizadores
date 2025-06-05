namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.FactoresMdr
{
    public class ListarMdrFactorDto
    {
        public string CodEmpresa { get; set; }
        public string NumAno { get; set; }
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
        public decimal Factor { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
        public System.DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public System.DateTime? FecModifica { get; set; }

        public string NomEmpresa { get; set; }
        public string NomOperador { get; set; }
        public string NomClasificacion { get; set; }
    }
}
