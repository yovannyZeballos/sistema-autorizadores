using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class ProductoSerieDetalle
    {
        public int CodProducto { get; set; }
        public string NumSerie { get; set; }
        public string CodAlmacen { get; set; }
        public DateTime? FecIngreso { get; set; }
        public int NumGarantiaMeses { get; set; }
        public string TipEstado { get; set; }
        public string NumAdenda { get; set; }
        public DateTime? FecIniAdenda { get; set; }
        public DateTime? FecFinAdenda { get; set; }
        public string CodEmpresa { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
    }
}
