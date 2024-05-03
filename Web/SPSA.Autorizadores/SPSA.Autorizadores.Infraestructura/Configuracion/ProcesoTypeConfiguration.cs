using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class ProcesoTypeConfiguration : EntityTypeConfiguration<Proceso>
	{
        public ProcesoTypeConfiguration()
        {
			ToTable("PROCESO", "SGP");
			HasKey(x => new { x.CodProceso });
			Property(p => p.CodProceso).HasColumnName("COD_PROCESO");
			Property(p => p.DesProceso).HasColumnName("DES_PROCESO");
			Property(p => p.IndActivo).HasColumnName("IND_ACTIVO");
			Property(p => p.IndReproceso).HasColumnName("IND_REPROCESO");
			Property(p => p.NumOrden).HasColumnName("NUM_ORDEN");
			Property(p => p.NomProceso).HasColumnName("NOM_PROCESO");
			Property(p => p.Email).HasColumnName("EMAIL");
			Property(p => p.FecCreacion).HasColumnName("FEC_CREACION");
			Property(p => p.UsuCreacion).HasColumnName("USU_CREACION");
			Property(p => p.FecElimina).HasColumnName("FEC_ELIMINA");
			Property(p => p.UsuElimina).HasColumnName("USU_ELIMINA");
		}
    }
}
