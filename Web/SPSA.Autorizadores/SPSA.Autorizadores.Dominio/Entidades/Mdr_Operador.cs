using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mdr_Operador
    {
        public string CodOperador { get; set; }
        public string NomOperador { get; set; }

        public virtual ICollection<Mdr_FactorIzipay> Factores { get; set; }
    }
}
