using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mdr_Clasificacion
    {
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
        public string NomClasificacion { get; set; }
        public string IndActivo { get; set; }

        public virtual ICollection<Mdr_FactorIzipay> Factores { get; set; }
    }
}
