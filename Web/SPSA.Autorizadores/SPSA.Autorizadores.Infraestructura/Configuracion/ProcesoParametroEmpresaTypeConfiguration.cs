using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class ProcesoParametroEmpresaTypeConfiguration : EntityTypeConfiguration<ProcesoParametroEmpresa>
	{
		public ProcesoParametroEmpresaTypeConfiguration()
		{
			ToTable("PROCESO_PARAM_XEMP", "SGP");
			HasKey(x => new { x.CodProceso, x.CodEmpresa, x.CodParametro });
			Property(p => p.CodProceso).HasColumnName("COD_PROCESO");
			Property(p => p.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(p => p.CodParametro).HasColumnName("COD_PARAMETRO");
			Property(p => p.TipParametro).HasColumnName("TIP_PARAMETRO");
			Property(p => p.NomParametro).HasColumnName("NOM_PARAMETRO");
			Property(p => p.ValParametro).HasColumnName("VAL_PARAMETRO");
			Property(p => p.IndActivo).HasColumnName("IND_ACTIVO");
		}
	}
}
