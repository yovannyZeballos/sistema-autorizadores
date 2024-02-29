using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegRegionTypeConfiguration : EntityTypeConfiguration<Seg_Region>
	{
		public SegRegionTypeConfiguration()
		{
			ToTable("SEG_REGION", "SEGURIDAD");
			HasKey(s => new { s.CodEmpresa, s.CodUsuario, s.CodCadena, s.CodRegion });
			Property(s => s.CodCadena).HasColumnName("COD_CADENA");
			Property(s => s.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(s => s.CodUsuario).HasColumnName("COD_USUARIO");
			Property(s => s.CodRegion).HasColumnName("COD_REGION");
			HasRequired(s => s.Mae_Region).WithMany().HasForeignKey(s => new { s.CodEmpresa, s.CodCadena, s.CodRegion });
		}
	}
}
