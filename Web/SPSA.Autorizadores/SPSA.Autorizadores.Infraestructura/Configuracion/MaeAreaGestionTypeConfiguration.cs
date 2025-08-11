using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeAreaGestionTypeConfiguration : EntityTypeConfiguration<Mae_AreaGestion>
    {
        public MaeAreaGestionTypeConfiguration()
        {
            ToTable("mae_area_gestion", "SGP");

            HasKey(x => new { x.Id });

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.NomAreaGestion).HasColumnName("nom_area_gestion");
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
