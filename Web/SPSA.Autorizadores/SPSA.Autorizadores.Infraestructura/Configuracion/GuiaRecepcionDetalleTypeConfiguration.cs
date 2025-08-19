using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class GuiaRecepcionDetalleTypeConfiguration : EntityTypeConfiguration<GuiaRecepcionDetalle>
    {
        public GuiaRecepcionDetalleTypeConfiguration()
        {
            ToTable("guia_recepcion_detalle", "SGP");

            HasKey(x => new { x.Id });

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.GuiaRecepcionId).HasColumnName("guia_recepcion_id");
            Property(x => x.CodProducto).HasColumnName("cod_producto");
            Property(x => x.SerieProductoId).HasColumnName("serie_producto_id");
            Property(x => x.Cantidad).HasColumnName("cantidad");
            Property(x => x.CodActivo).HasColumnName("cod_activo");
            Property(x => x.Observaciones).HasColumnName("observaciones");
        }
    }
}
