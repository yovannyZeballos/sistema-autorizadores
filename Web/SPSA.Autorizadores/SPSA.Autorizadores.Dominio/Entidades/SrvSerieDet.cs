using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SrvSerieDet
    {
        public long SerieProductoId { get; set; }

        // Clasificación
        public short TipoId { get; set; }

        // Identidad
        public string Hostname { get; set; } = null;
        public string Ip { get; set; }

        // HW
        public decimal? RamGb { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public string HddTotal { get; set; }

        // SO
        public long? SoId { get; set; }

        // Endpoints por sede
        public string HostnameBranch { get; set; }
        public string IpBranch { get; set; }
        public string HostnameFileserver { get; set; }
        public string IpFileserver { get; set; }
        public string HostnameUnicola { get; set; }
        public string IpUnicola { get; set; }
        public string EnlaceUrl { get; set; }
        public string IpIdracIlo { get; set; }

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
