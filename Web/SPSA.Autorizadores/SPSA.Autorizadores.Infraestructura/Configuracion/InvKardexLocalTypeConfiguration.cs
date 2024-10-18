using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class InvKardexLocalTypeConfiguration : EntityTypeConfiguration<InvKardexLocal>
    {
        public InvKardexLocalTypeConfiguration()
        {
            ToTable("INV_KARDEX_LOCAL", "SGP");
            HasKey(x => new { x.Id });
            Property(x => x.Id).HasColumnName("ID");
            Property(x => x.Sociedad).HasColumnName("SOCIEDAD");
            Property(x => x.NomLocal).HasColumnName("NOM_LOCAL");
        }
    }
}
