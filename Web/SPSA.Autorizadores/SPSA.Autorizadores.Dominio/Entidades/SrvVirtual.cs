using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SrvVirtual
    {
        public long Id { get; set; }
        public long HostSerieId { get; set; }        // 👈 Debe llamarse así
        public short? PlataformaId { get; set; }
        public string Hostname { get; set; }
        public string Ip { get; set; }
        public decimal? RamGb { get; set; }
        public int? VCores { get; set; }
        public string HddTotal { get; set; }
        public long? SoId { get; set; }
        public string IndActivo { get; set; }
        public string Url { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        public virtual SrvFisico Host { get; set; }     // 👈 navegación
        public virtual SrvPlataformaVm Plataforma { get; set; }
        public virtual SrvSistemaOperativo SistemaOperativo { get; set; }
    }
}
