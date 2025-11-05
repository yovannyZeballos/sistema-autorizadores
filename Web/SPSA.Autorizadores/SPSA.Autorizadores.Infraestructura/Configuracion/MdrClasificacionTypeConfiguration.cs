using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrClasificacionTypeConfiguration : EntityTypeConfiguration<Mdr_Clasificacion>
    {
        public MdrClasificacionTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad.
            ToTable("mdr_clasificacion", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodOperador, x.CodClasificacion });

            // Configura las propiedades de la entidad.
            Property(x => x.CodOperador).HasColumnName("cod_operador");
            Property(x => x.CodClasificacion).HasColumnName("cod_clasificacion");
            Property(x => x.NomClasificacion).HasColumnName("nom_clasificacion");
            Property(x => x.IndActivo).HasColumnName("ind_activo");

        }
    }
}
