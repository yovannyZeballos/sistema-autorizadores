using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegPerfilUsuarioTypeConfiguration : EntityTypeConfiguration<Seg_PerfilUsuario>
	{
		public SegPerfilUsuarioTypeConfiguration()
		{
			ToTable("SEG_PERFIL_USUARIO", "SEGURIDAD"); 
			HasKey(s => new { s.CodUsuario, s.CodPerfil });
			Property(s => s.CodUsuario).HasColumnName("COD_USUARIO");
			Property(s => s.CodPerfil).HasColumnName("COD_PERFIL");
			Property(s => s.IndActivo).HasColumnName("IND_ACTIVO");
			Property(s => s.FecCreacion).HasColumnName("FEC_CREACION");
			Property(s => s.UsuCreacion).HasColumnName("USU_CREACION");
			Property(s => s.FecModifica).HasColumnName("FEC_MODIFICA");
			Property(s => s.UsuModifica).HasColumnName("USU_MODIFICA");
			HasRequired(s => s.Seg_Perfil).WithMany().HasForeignKey(s => s.CodPerfil);

		}
	}
}
