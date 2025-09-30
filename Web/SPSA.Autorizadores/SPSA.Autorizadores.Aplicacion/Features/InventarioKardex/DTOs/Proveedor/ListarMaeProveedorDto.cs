using System;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Proveedor
{
    public class ListarMaeProveedorDto
    {
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string NomComercial { get; set; }
        public string IndActivo { get; set; }
        public string DirFiscal { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
    }
}
