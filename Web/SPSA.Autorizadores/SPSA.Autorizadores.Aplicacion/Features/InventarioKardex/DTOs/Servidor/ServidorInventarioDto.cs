namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Servidor
{
    public class ServidorInventarioDto
    {
        public long SerieProductoId { get; set; }   // FK a mae_serie_producto

        public string CodProducto { get; set; }
        public long AreaGestionId { get; set; }
        public string NomAreaGestion { get; set; }
        public string DesProducto { get; set; }
        public long? MarcaId { get; set; }
        public string NomMarca { get; set; }
        public string Modelo { get; set; }

        public string NumSerie { get; set; }
        public string IndEstadoSerie { get; set; } // serie: DISPONIBLE/EN_TRANSITO/EN_USO/DE_BAJA
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public decimal StkActual { get; set; }

        public short? TipoId { get; set; }
        public string TipoServidor { get; set; }
        public string IndActivo { get; set; } // S/N

        public string Hostname { get; set; }
        public string Ip { get; set; }
        public decimal? RamGb { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public string HddTotal { get; set; }

        public string HostnameBranch { get; set; }
        public string IpBranch { get; set; }
        public string HostnameFileServer { get; set; }
        public string IpFileServer { get; set; }
        public string HostnameUnicola { get; set; }
        public string IpUnicola { get; set; }
        public string EnlaceUrl { get; set; }
        public string IpIdracIlo { get; set; }

        public string NomLocal { get; set; }
        public string NomEmpresa { get; set; }

        public long? SoId { get; set; }
        public string NomSo { get; set; }
    }
}
