using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mdr_Periodo
    {
        public int CodPeriodo { get; set; }
        public string DesPeriodo { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
    }
}
