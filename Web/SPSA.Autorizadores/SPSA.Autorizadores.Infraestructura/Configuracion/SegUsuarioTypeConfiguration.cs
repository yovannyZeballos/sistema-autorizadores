using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegUsuarioTypeConfiguration : EntityTypeConfiguration<Seg_Usuario>
	{
		public SegUsuarioTypeConfiguration()
		{
			ToTable("SEG_USUARIO", "SEGURIDAD");
			HasKey(u => u.CodUsuario);
			Property(u => u.CodUsuario).HasColumnName("COD_USUARIO");
			Property(u => u.CodColaborador).HasColumnName("COD_COLABORADOR");
			Property(u => u.IndActivo).HasColumnName("IND_ACTIVO");
			Property(u => u.TipUsuario).HasColumnName("TIP_USUARIO");
			Property(u => u.DirEmail).HasColumnName("DIR_EMAIL");
			Property(u => u.FecCreacion).HasColumnName("FEC_CREACION");
			Property(u => u.UsuCreacion).HasColumnName("USU_CREACION");
			Property(u => u.FecElimina).HasColumnName("FEC_ELIMINA");
			Property(u => u.UsuElimina).HasColumnName("USU_ELIMINA");
			Property(u => u.FecIngreso).HasColumnName("FEC_INGRESO");
			Property(u => u.HoraIngreso).HasColumnName("HORA_INGRESO");
		}
	}
}
