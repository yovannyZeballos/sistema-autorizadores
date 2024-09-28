using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class InvKardexActivoTypeConfiguration : EntityTypeConfiguration<InvKardexActivo>
    {
        public InvKardexActivoTypeConfiguration()
        {
            ToTable("INV_KARDEX_ACTIVO", "SGP");
            HasKey(x => new { x.Id });
            Property(x => x.Id).HasColumnName("ID");
            Property(x => x.Modelo).HasColumnName("MODELO");
            Property(x => x.Descripcion).HasColumnName("DESCRIPCION");
            Property(x => x.Marca).HasColumnName("MARCA");
            Property(x => x.Area).HasColumnName("AREA");
            Property(x => x.Tipo).HasColumnName("TIPO");
        }
    }
}
