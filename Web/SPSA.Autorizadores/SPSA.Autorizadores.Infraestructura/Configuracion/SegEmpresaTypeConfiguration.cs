using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class SegEmpresaTypeConfiguration : EntityTypeConfiguration<Seg_Empresa>
	{
        public SegEmpresaTypeConfiguration()
        {
			ToTable("SEG_EMPRESA", "SEGURIDAD");
			HasKey(s => new {  s.CodEmpresa, s.CodUsuario });
			Property(s => s.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(s => s.CodUsuario).HasColumnName("COD_USUARIO");
			HasRequired(s => s.Mae_Empresa).WithMany().HasForeignKey(s => s.CodEmpresa);
		}
    }
}
