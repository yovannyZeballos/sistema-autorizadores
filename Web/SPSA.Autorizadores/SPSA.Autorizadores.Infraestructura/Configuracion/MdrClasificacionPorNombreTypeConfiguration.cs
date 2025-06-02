using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrClasificacionPorNombreTypeConfiguration : EntityTypeConfiguration<Mdr_ClasificacionPorNombre>
    {
        public MdrClasificacionPorNombreTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_CLASIFICACION_XNOMBRE", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodOperador, x.CodClasificacion });

            // Configura las propiedades de la entidad.
            Property(x => x.CodOperador).HasColumnName("COD_OPERADOR");
            Property(x => x.CodClasificacion).HasColumnName("COD_CLASIFICACION");
            Property(x => x.Tipo).HasColumnName("TIPO");
            Property(x => x.Nombre).HasColumnName("NOMBRE");
            Property(x => x.Clasificacion).HasColumnName("CLASIFICACION");

        }
    }
}
