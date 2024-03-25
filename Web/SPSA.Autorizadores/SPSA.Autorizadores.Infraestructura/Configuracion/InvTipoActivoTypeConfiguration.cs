using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	internal class InvTipoActivoTypeConfiguration : EntityTypeConfiguration<InvTipoActivo>
	{
		public InvTipoActivoTypeConfiguration()
		{
			ToTable("INV_TIPO_ACTIVO", "SGP");

			HasKey(x => x.CodActivo);

			Property(x => x.CodActivo).HasColumnName("COD_ACTIVO").IsRequired().HasMaxLength(3);
			Property(x => x.NomActivo).HasColumnName("NOM_ACTIVO").HasMaxLength(50);
			Property(x => x.IndEstado).HasColumnName("IND_ESTADO").HasMaxLength(1);
			Property(x => x.TipInventario).HasColumnName("TIP_INVENTARIO").HasMaxLength(2);
		}
	}
}