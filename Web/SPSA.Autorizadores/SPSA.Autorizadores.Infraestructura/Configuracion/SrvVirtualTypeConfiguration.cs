using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvVirtualTypeConfiguration : EntityTypeConfiguration<SrvVirtual>
    {
        public SrvVirtualTypeConfiguration()
        {
            ToTable("srv_vm", "SGP");

            HasKey(x => x.Id);

            Property(x => x.Id)
                .HasColumnName("id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.HostSerieId).HasColumnName("host_serie_id"); //  FK real
            Property(x => x.PlataformaId).HasColumnName("plataforma_id");
            Property(x => x.Hostname).HasColumnName("hostname");
            Property(x => x.Ip).HasColumnName("ip");
            Property(x => x.RamGb).HasColumnName("ram_gb");
            Property(x => x.VCores).HasColumnName("vcores");
            Property(x => x.HddTotal).HasColumnName("hdd_total");
            Property(x => x.SoId).HasColumnName("so_id");
            Property(x => x.IndActivo).HasColumnName("ind_activo");
            Property(x => x.Url).HasColumnName("url");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");

            Ignore(x => x.Host);
            Ignore(x => x.Plataforma);
            Ignore(x => x.SistemaOperativo);

        }
    }
}
