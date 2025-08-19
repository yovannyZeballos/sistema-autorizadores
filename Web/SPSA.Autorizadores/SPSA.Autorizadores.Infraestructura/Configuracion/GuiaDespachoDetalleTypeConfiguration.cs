using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class GuiaDespachoDetalleTypeConfiguration : EntityTypeConfiguration<GuiaDespachoDetalle>
    {
        public GuiaDespachoDetalleTypeConfiguration()
        {
            ToTable("guia_despacho_detalle", "SGP");

            HasKey(x => new { x.Id });

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.GuiaDespachoId).HasColumnName("guia_despacho_id");
            Property(x => x.CodProducto).HasColumnName("cod_producto");
            Property(x => x.SerieProductoId).HasColumnName("serie_producto_id");
            Property(x => x.Cantidad).HasColumnName("cantidad");
            Property(x => x.CodActivo).HasColumnName("cod_activo");
            Property(x => x.Observaciones).HasColumnName("observaciones");
        }
    }
}
