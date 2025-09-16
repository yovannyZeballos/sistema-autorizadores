using System;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class GuiaRecepcionCabecera
    {
        public int Id { get; set; }
        public string NumGuia { get; set; }
        public string OrdenCompra { get; set; }
        public DateTime Fecha { get; set; }
        public string ProveedorRuc { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public string AreaGestion { get; set; }
        public string ClaseStock { get; set; }
        public string EstadoStock { get; set; }
        public string Observaciones { get; set; }
        public string IndTransferencia { get; set; }
        public string IndEstado { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        public virtual ICollection<GuiaRecepcionDetalle> Detalles { get; set; } = new List<GuiaRecepcionDetalle>();
    }
}
