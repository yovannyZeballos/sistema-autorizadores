using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class ProcesoEmpresaTypeConfiguration : EntityTypeConfiguration<ProcesoEmpresa>
	{
        public ProcesoEmpresaTypeConfiguration()
        {
			ToTable("PROCESO_XEMP", "SGP");
			HasKey(x => new { x.CodProceso, x.CodEmpresa });
			Property(p => p.CodProceso).HasColumnName("COD_PROCESO");
			Property(p => p.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(p => p.IndActivo).HasColumnName("IND_ACTIVO");
			Property(p => p.Mail).HasColumnName("MAIL");
			Property(p => p.FecCreacion).HasColumnName("FEC_CREACION");
			Property(p => p.UsuCreacion).HasColumnName("USU_CREACION");
			Property(p => p.FecElimina).HasColumnName("FEC_ELIMINA");
			Property(p => p.UsuElimina).HasColumnName("USU_ELIMINA");
		}
    }
}
