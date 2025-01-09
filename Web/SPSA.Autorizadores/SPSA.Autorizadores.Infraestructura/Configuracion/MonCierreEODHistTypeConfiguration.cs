using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class MonCierreEODHistTypeConfiguration : EntityTypeConfiguration<MonCierreEODHist>
	{
		public MonCierreEODHistTypeConfiguration()
		{
			ToTable("MON_CIERRE_EOD_HIST", "SGP");
			HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona, x.CodLocal, x.FechaCierre, x.Tipo });
			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(x => x.CodCadena).HasColumnName("COD_CADENA");
			Property(x => x.CodRegion).HasColumnName("COD_REGION");
			Property(x => x.CodZona).HasColumnName("COD_ZONA");
			Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
			Property(x => x.FechaCierre).HasColumnName("FEC_CIERRE");
			Property(x => x.Tipo).HasColumnName("TIPO");
			Property(x => x.FechaProceso).HasColumnName("FEC_PROCESO");
			Property(x => x.HoraInicio).HasColumnName("HORA_INICIO");
			Property(x => x.HoraFin).HasColumnName("HORA_FIN");
			Property(x => x.Estado).HasColumnName("TIP_ESTADO");
			Property(x => x.Observacion).HasColumnName("DES_OBS");
		}
	}
}
