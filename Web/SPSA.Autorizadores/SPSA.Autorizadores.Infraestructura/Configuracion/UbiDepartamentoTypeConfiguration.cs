
using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class UbiDepartamentoTypeConfiguration : EntityTypeConfiguration<UbiDepartamento>
    {

        public UbiDepartamentoTypeConfiguration()
        {
            ToTable("UBI_DEPARTAMENTO", "SGP");

            HasKey(x => new { x.CodDepartamento });
            Property(x => x.CodDepartamento).HasColumnName("COD_DEPARTAMENTO");
            Property(x => x.NomDepartamento).HasColumnName("NOM_DEPARTAMENTO");
        }
    }
}
