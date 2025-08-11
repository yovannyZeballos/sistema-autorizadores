using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mov_Kardex
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; }  // INGRESO | SALIDA

        public long SerieProductoId { get; set; } 
        public string DesAreaGestion { get; set; }
        public string DesClaseStock { get; set; }
        public string DesEstadoStock { get; set; }

        public string NumGuia { get; set; }
        public string OrdenCompra { get; set; }
        public string NumTicket { get; set; }
        public string CodActivo { get; set; }
        public string Cantidad { get; set; }

        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }

        public string Observaciones { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }

        // --- Navegaciones ---
        public virtual Mae_SerieProducto SerieProducto { get; set; } = null;
        //public virtual Mae_AreaGestion AreaGestion { get; set; } = null;

        //public virtual Mae_Local? LocalOrigen { get; set; }
        //public virtual Mae_Local LocalDestino { get; set; } = null;
    }
}
