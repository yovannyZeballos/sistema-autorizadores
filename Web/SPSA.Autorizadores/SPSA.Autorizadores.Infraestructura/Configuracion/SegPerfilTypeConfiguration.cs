using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegPerfilTypeConfiguration : EntityTypeConfiguration<Seg_Perfil>
	{
		public SegPerfilTypeConfiguration()
		{
			ToTable("SEG_PERFIL", "SEGURIDAD");
			HasKey(s => s.CodPerfil);
			Property(s => s.CodPerfil).HasColumnName("COD_PERFIL");
			Property(s => s.NomPerfil).HasColumnName("NOM_PERFIL");
			Property(s => s.TipPerfil).HasColumnName("TIP_PERFIL");
			Property(s => s.IndActivo).HasColumnName("IND_ACTIVO");
			Property(s => s.FecCreacion).HasColumnName("FEC_CREACION");
			Property(s => s.UsuCreacion).HasColumnName("USU_CREAXION");
			Property(s => s.FecModifica).HasColumnName("FEC_MODIFICA");
			Property(s => s.UsuModifica).HasColumnName("USU_MODIFICA");
		}
	}
}
