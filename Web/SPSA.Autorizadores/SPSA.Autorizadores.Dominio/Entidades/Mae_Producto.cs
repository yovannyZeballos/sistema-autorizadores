using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_Producto
    {
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }      
        public int MarcaId { get; set; }
        public string TipProducto { get; set; }
        public int AreaGestionId { get; set; }
        public string IndSerializable { get; set; }
        public string IndActivo { get; set; }
        public decimal StkMinimo { get; set; }
        public decimal StkMaximo { get; set; }
        public string NomModelo { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }

        public virtual Mae_Marca Marca { get; set; }
    }
}
