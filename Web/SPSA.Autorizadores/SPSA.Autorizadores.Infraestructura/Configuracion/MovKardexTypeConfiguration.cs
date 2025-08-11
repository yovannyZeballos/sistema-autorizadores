using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MovKardexTypeConfiguration : EntityTypeConfiguration<Mov_Kardex>
    {
        public MovKardexTypeConfiguration()
        {
            ToTable("mov_kardex", "SGP");

            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.Fecha).HasColumnName("fecha").IsRequired();
            Property(x => x.TipoMovimiento).HasColumnName("tipo_movimiento").IsRequired();

            Property(x => x.SerieProductoId).HasColumnName("serie_producto_id").IsRequired();
            Property(x => x.DesAreaGestion).HasColumnName("area_gestion").IsRequired();
            Property(x => x.DesClaseStock).HasColumnName("clase_stock").IsRequired();
            Property(x => x.DesEstadoStock).HasColumnName("estado_stock").IsRequired();
            Property(x => x.NumGuia).HasColumnName("num_guia");
            Property(x => x.OrdenCompra).HasColumnName("orden_compra");
            Property(x => x.NumTicket).HasColumnName("num_ticket");
            Property(x => x.CodActivo).HasColumnName("cod_activo");
            Property(x => x.Cantidad).HasColumnName("cantidad");

            Property(x => x.CodEmpresaOrigen).HasColumnName("cod_empresa_origen");
            Property(x => x.CodLocalOrigen).HasColumnName("cod_local_origen");
            Property(x => x.CodEmpresaDestino).HasColumnName("cod_empresa_destino").IsRequired();
            Property(x => x.CodLocalDestino).HasColumnName("cod_local_destino").IsRequired();

            Property(x => x.Observaciones).HasColumnName("observaciones");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");

            //  FK: Serie
            //HasRequired(x => x.SerieProducto)
            //    .WithMany()
            //    .HasForeignKey(x => x.SerieProductoId);

            ////  FK: Área de gestión
            //HasRequired(x => x.AreaGestion)
            //    .WithMany()
            //    .HasForeignKey(x => x.AreaGestionId);

            ////  FK: Local destino (obligatorio)
            //HasRequired(x => x.LocalDestino)
            //    .WithMany()
            //    .HasForeignKey(x => new { x.CodEmpresaDestino, x.CodLocalDestino });

            ////  FK: Local origen (opcional)
            //HasOptional(x => x.LocalOrigen)
            //    .WithMany()
            //    .HasForeignKey(x => new { x.CodEmpresaOrigen, x.CodLocalOrigen });
        }
    }
}
