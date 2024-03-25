
using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class UbiDistritoTypeConfiguration : EntityTypeConfiguration<UbiDistrito>
    {
        public UbiDistritoTypeConfiguration()
        {
            ToTable("UBI_DISTRITO", "SGP");

            HasKey(x => new { x.CodDepartamento, x.CodProvincia, x.CodDistrito });
            Property(x => x.CodDepartamento).HasColumnName("COD_DEPARTAMENTO");
            Property(x => x.CodProvincia).HasColumnName("COD_PROVINCIA");
            Property(x => x.CodDistrito).HasColumnName("COD_DISTRITO");
            Property(x => x.NomDistrito).HasColumnName("NOM_DISTRITO");
            Property(x => x.NomDistrito).HasColumnName("NOM_DISTRITO");
            Property(x => x.CodUbigeo).HasColumnName("COD_UBIGEO");
        }
    }
}
