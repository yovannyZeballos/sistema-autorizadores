using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mdr_BinesIzipay
    {
        public string NumBin8 { get; set; }
        public string NumBin6 { get; set; }
        public string Marca { get; set; }
        public string Tipo { get; set; }
        public string NomTarjeta { get; set; }
        public string BancoEmisor { get; set; }
        public decimal FactorMdr { get; set; }
        public string CodOperador { get; set; }
        public int CodPeriodo { get; set; }
        public string CodEmpresa { get; set; }
    }
}
