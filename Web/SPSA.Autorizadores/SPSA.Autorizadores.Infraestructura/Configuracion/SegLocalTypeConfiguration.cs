using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegLocalTypeConfiguration : EntityTypeConfiguration<Seg_Local>
	{
		public SegLocalTypeConfiguration()
		{
			ToTable("SEG_LOCAL", "SEGURIDAD");
			HasKey(s => new { s.CodEmpresa, s.CodUsuario, s.CodCadena, s.CodRegion, s.CodZona, s.CodLocal });
			Property(s => s.CodLocal).HasColumnName("COD_LOCAL");
			Property(s => s.CodZona).HasColumnName("COD_ZONA");
			Property(s => s.CodCadena).HasColumnName("COD_CADENA");
			Property(s => s.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(s => s.CodUsuario).HasColumnName("COD_USUARIO");
			Property(s => s.CodRegion).HasColumnName("COD_REGION");
			HasRequired(s => s.Mae_Local).WithMany().HasForeignKey(s => new { s.CodEmpresa, s.CodCadena, s.CodRegion, s.CodZona, s.CodLocal });
		}
	}
}
