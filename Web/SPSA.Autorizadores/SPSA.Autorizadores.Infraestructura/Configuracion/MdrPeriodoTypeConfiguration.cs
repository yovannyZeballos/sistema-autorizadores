using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrPeriodoTypeConfiguration : EntityTypeConfiguration<Mdr_Periodo>
    {
        public MdrPeriodoTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("mdr_periodo", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodPeriodo});

            // Configura las propiedades de la entidad.
            Property(x => x.CodPeriodo).HasColumnName("cod_periodo");
            Property(x => x.DesPeriodo).HasColumnName("des_periodo");
            Property(x => x.IndActivo).HasColumnName("ind_activo");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuElimina).HasColumnName("usu_elimina");
            Property(x => x.FecElimina).HasColumnName("fec_elimina");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");

        }
    }
}
