using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class MaeLocalAlternoTypeConfiguration : EntityTypeConfiguration<MaeLocalAlterno>
	{
		public MaeLocalAlternoTypeConfiguration()
		{
			ToTable("MAE_LOCAL_ALTERNO", "SGP");

			HasKey(x => new { x.CodLocalAlterno });
			Property(x => x.CodLocalAlterno).HasColumnName("COD_LOCAL_ALTERNO");
			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(x => x.CodCadena).HasColumnName("COD_CADENA");
			Property(x => x.CodRegion).HasColumnName("COD_REGION");
			Property(x => x.CodZona).HasColumnName("COD_ZONA");
			Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
		}
	}
}
