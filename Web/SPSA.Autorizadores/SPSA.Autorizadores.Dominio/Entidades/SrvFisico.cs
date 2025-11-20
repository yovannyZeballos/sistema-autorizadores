using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SrvFisico
    {
        public long SerieProductoId { get; set; }

        // Clasificación
        public short TipoId { get; set; }

        // Identidad
        public string Hostname { get; set; } = null;
        public string IpSo { get; set; }

        // HW
        public decimal? RamGb { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public string HddTotal { get; set; }

        // SO
        public long? SoId { get; set; }

        // Endpoints por sede
        public string MacAddress { get; set; }
        public DateTime? FecIngreso { get; set; }
        public string ConexionRemota { get; set; }
        public string IpRemota { get; set; }

        // Auditoría
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        // Navs (opcionales, según tu modelo)
        //public Mae_SerieProducto SerieProducto { get; set; }
        public SrvTipoServidor Tipo { get; set; }
        //public SrvSistemaOperativo SistemaOperativo { get; set; }
    }
}
