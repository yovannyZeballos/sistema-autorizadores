using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrPeriodoTypeConfiguration : EntityTypeConfiguration<Mdr_Periodo>
    {
        public MdrPeriodoTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_PERIODO", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodPeriodo});

            // Configura las propiedades de la entidad.
            Property(x => x.CodPeriodo).HasColumnName("COD_PERIODO");
            Property(x => x.DesPeriodo).HasColumnName("DES_PERIODO");
            Property(x => x.IndActivo).HasColumnName("IND_ACTIVO");
            Property(x => x.UsuCreacion).HasColumnName("USU_CREACION");
            Property(x => x.FecCreacion).HasColumnName("FEC_CREACION");
            Property(x => x.UsuElimina).HasColumnName("USU_ELIMINA");
            Property(x => x.FecElimina).HasColumnName("FEC_ELIMINA");
            Property(x => x.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(x => x.FecModifica).HasColumnName("FEC_MODIFICA");

        }
    }
}
