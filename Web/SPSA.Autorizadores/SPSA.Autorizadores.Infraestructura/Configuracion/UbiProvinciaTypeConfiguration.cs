
using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class UbiProvinciaTypeConfiguration : EntityTypeConfiguration<UbiProvincia>
    {
        public UbiProvinciaTypeConfiguration()
        {
            ToTable("UBI_PROVINCIA", "SGP");

            HasKey(x => new { x.CodDepartamento, x.CodProvincia });
            Property(x => x.CodDepartamento).HasColumnName("COD_DEPARTAMENTO");
            Property(x => x.CodProvincia).HasColumnName("COD_PROVINCIA");
            Property(x => x.NomProvincia).HasColumnName("NOM_PROVINCIA");
        }
    }
}
