using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegCadenaTypeConfiguration : EntityTypeConfiguration<Seg_Cadena>
	{
		public SegCadenaTypeConfiguration()
		{
			ToTable("SEG_CADENA", "SEGURIDAD");
			HasKey(s => new { s.CodEmpresa, s.CodUsuario, s.CodCadena });
			Property(s => s.CodCadena).HasColumnName("COD_CADENA");
			Property(s => s.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(s => s.CodUsuario).HasColumnName("COD_USUARIO");
			HasRequired(s => s.Mae_Cadena).WithMany().HasForeignKey(s => new { s.CodEmpresa, s.CodCadena });
		}
	}
}
