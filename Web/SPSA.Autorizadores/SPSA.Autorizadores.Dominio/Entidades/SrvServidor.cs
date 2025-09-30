using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SrvServidor
    {
        public long Id { get; set; }

        // Ubicación
        public string CodEmpresa { get; set; } = null;
        public string CodLocal { get; set; } = null;

        // Clasificación
        public short TipoId { get; set; }
        public string IndActivo { get; set; } = "S";

        // Identidad
        public string Hostname { get; set; } = null;
        public string Ip { get; set; }
        public long? MarcaId { get; set; }
        public string Modelo { get; set; }
        public string NumSerie { get; set; }

        // HW
        public decimal? RamGb { get; set; }
        public string CpuModelo { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public decimal? HddTotalGb { get; set; }
        public string StorageDesc { get; set; }

        // SO
        public long? SoId { get; set; }
        public string SoTexto { get; set; }

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
        public SrvTipoServidor Tipo { get; set; }
        public SrvSistemaOperativo SistemaOperativo { get; set; }
        public Mae_Marca Marca { get; set; }
        // public MaeLocal Local { get; set; }         // si la tienes en el modelo
    }
}
