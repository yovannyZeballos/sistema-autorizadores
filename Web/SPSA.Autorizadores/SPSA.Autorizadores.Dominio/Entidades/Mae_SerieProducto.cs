using System;
namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_SerieProducto
    {
        public long Id { get; set; }

        public string CodProducto { get; set; } = null;
        public string NumSerie { get; set; } = null;
        public string IndEstado { get; set; }
        public string StkEstado { get; set; }

        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public int StkActual { get; set; }

        public DateTime? FecIngreso { get; set; }
        public DateTime? FecSalida { get; set; }

        public DateTime FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }

        // Relaciones
        public virtual Mae_Producto Producto { get; set; } = null;
    }
}
