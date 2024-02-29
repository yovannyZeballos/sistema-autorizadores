using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegMenuTypeConfiguration : EntityTypeConfiguration<Seg_Menu>
	{
        public SegMenuTypeConfiguration()
        {
			ToTable("SEG_MENU", "SEGURIDAD");
			HasKey(s => new { s.CodSistema, s.CodMenu });
			Property(s => s.CodSistema).HasColumnName("COD_SISTEMA");
			Property(s => s.CodMenu).HasColumnName("COD_MENU");
			Property(s => s.NomMenu).HasColumnName("NOM_MENU");
			Property(s => s.CodMenuPadre).HasColumnName("COD_MENU_PADRE");
			Property(s => s.IndActivo).HasColumnName("IND_ACTIVO");
			Property(s => s.FecCreacion).HasColumnName("FEC_CREACION");
			Property(s => s.UsuCreacion).HasColumnName("USU_CREACION");
			Property(s => s.FecModifica).HasColumnName("FEC_MODIFICA");
			Property(s => s.UsuModifica).HasColumnName("USU_MODIFICA");
		}
    }
}
