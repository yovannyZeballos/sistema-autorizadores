using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_Marca
    {
        public int Id { get; set; }
        public string NomMarca { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
    }
}
