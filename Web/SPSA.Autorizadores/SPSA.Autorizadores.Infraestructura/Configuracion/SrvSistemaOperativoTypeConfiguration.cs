using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvSistemaOperativoTypeConfiguration : EntityTypeConfiguration<SrvSistemaOperativo>
    {
        public SrvSistemaOperativoTypeConfiguration()
        {
            ToTable("srv_sistema_operativo", "SGP");

            HasKey(x => new { x.Id });

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.NomSo).HasColumnName("nom_so");
            Property(x => x.Version).HasColumnName("version");
            Property(x => x.Edition).HasColumnName("edition");
        }
    }
}
