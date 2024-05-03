using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class MonCierreLocalTypeConfiguration : EntityTypeConfiguration<MonCierreLocal>
	{
		public MonCierreLocalTypeConfiguration()
		{
			ToTable("MON_CIERRE_LOCAL", "SGP");
			HasKey(x => new { x.CodLocalAlterno, x.FechaContable });
			Property(x => x.CodLocalAlterno).HasColumnName("COD_LOCAL_ALTERNO");
			Property(x => x.FechaContable).HasColumnName("FEC_CONTABLE");
			Property(x => x.FechaCierre).HasColumnName("FEC_CIERRE");
			Property(x => x.TipEstado).HasColumnName("TIP_ESTADO");
			Property(x => x.FechaCarga).HasColumnName("FEC_CARGA");
			Property(x => x.HoraCarga).HasColumnName("HOR_CARGA");
		}
	}
}
