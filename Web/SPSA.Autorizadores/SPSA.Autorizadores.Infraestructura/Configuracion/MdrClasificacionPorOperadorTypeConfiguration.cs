using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrClasificacionPorOperadorTypeConfiguration : EntityTypeConfiguration<Mdr_ClasificacionPorOperador>
    {
        public MdrClasificacionPorOperadorTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_CLASIFICACION_XOPE", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodOperador, x.CodClasificacion });

            // Configura las propiedades de la entidad.
            Property(x => x.CodOperador).HasColumnName("COD_OPERADOR");
            Property(x => x.CodClasificacion).HasColumnName("COD_CLASIFICACION");
            Property(x => x.NomClasificacion).HasColumnName("NOM_CLASIFICACION");

        }
    }
}
