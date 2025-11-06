using System;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Producto
{
    public class ListarMaeProductoDto
    {
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }
        public int MarcaId { get; set; }
        public string NomMarca { get; set; }
        public string TipProducto { get; set; }
        public string NomTipProducto { get; set; }
        public long AreaGestionId { get; set; }
        public string NomAreaGestion { get; set; }
        public string IndSerializable { get; set; }
        public string IndActivo { get; set; }
        public decimal StkMinimo { get; set; }
        public decimal StkMaximo { get; set; }
        public decimal StkDisponible { get; set; }
        public string NomModelo { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }

    }
}
