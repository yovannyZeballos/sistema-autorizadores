using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegPerfilMenuTypeConfiguration : EntityTypeConfiguration<Seg_PerfilMenu>
	{
		public SegPerfilMenuTypeConfiguration()
		{
			ToTable("SEG_PERFIL_MENU", "SEGURIDAD");
			HasKey(s => new { s.CodPerfil, s.CodSistema, s.CodMenu });
			Property(s => s.CodPerfil).HasColumnName("COD_PERFIL");
			Property(s => s.CodSistema).HasColumnName("COD_SISTEMA");
			Property(s => s.CodMenu).HasColumnName("COD_MENU");
			Property(s => s.FecCreacion).HasColumnName("FEC_CREACION");
			Property(s => s.UsuCreacion).HasColumnName("USU_CREACION");
			Property(s => s.FecModifica).HasColumnName("FEC_MODIFICA");
			Property(s => s.UsuModifica).HasColumnName("USU_MODIFICA");
			HasRequired(s => s.Menu).WithMany().HasForeignKey(s => new { s.CodSistema, s.CodMenu });

		}
	}
}
