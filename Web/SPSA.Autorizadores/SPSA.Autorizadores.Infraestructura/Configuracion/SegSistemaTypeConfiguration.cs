using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegSistemaTypeConfiguration : EntityTypeConfiguration<Seg_Sistema>
	{
        public SegSistemaTypeConfiguration()
        {
			ToTable("SEG_SISTEMA", "SEGURIDAD");
			HasKey(s => s.CodSistema);
			Property(s => s.CodSistema).HasColumnName("COD_SISTEMA");
			Property(s => s.NomSistema).HasColumnName("NOM_SISTEMA");
			Property(s => s.Sigla).HasColumnName("SIGLA");
			Property(s => s.IndActivo).HasColumnName("IND_ACTIVO");
			Property(s => s.FecCreacion).HasColumnName("FEC_CREACION");
			Property(s => s.UsuCreacion).HasColumnName("USU_CREACION");
			Property(s => s.FecModifica).HasColumnName("FEC_MODIFICA");
			Property(s => s.UsuModifica).HasColumnName("USU_MODIFICA");
		}
    }
}
