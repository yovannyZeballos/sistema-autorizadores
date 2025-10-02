using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvSerieDetTypeConfiguration : EntityTypeConfiguration<SrvSerieDet>
    {
        public SrvSerieDetTypeConfiguration()
        {
            ToTable("srv_serie_det", "SGP");

            HasKey(x => new { x.SerieProductoId });

            //Property(x => x.SerieProductoId).HasColumnName("serie_producto_id");
            Property(x => x.SerieProductoId)
    .HasColumnName("serie_producto_id")
    .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            Property(x => x.TipoId).HasColumnName("tipo_id");
            Property(x => x.Hostname).HasColumnName("hostname");
            Property(x => x.Ip).HasColumnName("ip");
            Property(x => x.RamGb).HasColumnName("ram_gb");
            Property(x => x.CpuSockets).HasColumnName("cpu_sockets");
            Property(x => x.CpuCores).HasColumnName("cpu_cores");
            Property(x => x.HddTotal).HasColumnName("hdd_total");
            Property(x => x.SoId).HasColumnName("so_id");
            Property(x => x.HostnameBranch).HasColumnName("hostname_branch");
            Property(x => x.IpBranch).HasColumnName("ip_branch");
            Property(x => x.HostnameFileserver).HasColumnName("hostname_fileserver");
            Property(x => x.IpFileserver).HasColumnName("ip_fileserver");
            Property(x => x.HostnameUnicola).HasColumnName("hostname_unicola");
            Property(x => x.IpUnicola).HasColumnName("ip_unicola");
            Property(x => x.EnlaceUrl).HasColumnName("enlace_url");
            Property(x => x.IpIdracIlo).HasColumnName("ip_idrac_ilo");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
