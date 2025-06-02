using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrOperadorTypeConfiguration : EntityTypeConfiguration<Mdr_Operador>
    {
        public MdrOperadorTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_OPERADOR", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodOperador });

            // Configura las propiedades de la entidad.
            Property(x => x.CodOperador).HasColumnName("COD_OPERADOR");
            Property(x => x.NomOperador).HasColumnName("NOM_OPERADOR");

        }
    }
}
