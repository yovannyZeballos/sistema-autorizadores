using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeProductoTypeConfiguration : EntityTypeConfiguration<Mae_Producto>
    {
        public MaeProductoTypeConfiguration()
        {
            ToTable("mae_producto", "SGP");

            HasKey(x => new { x.CodProducto });

            Property(x => x.CodProducto).HasColumnName("cod_producto");
            Property(x => x.DesProducto).HasColumnName("des_producto");
            Property(x => x.MarcaId).HasColumnName("marca_id");
            Property(x => x.NomModelo).HasColumnName("nom_modelo");
            Property(x => x.TipProducto).HasColumnName("tip_producto");
            Property(x => x.AreaGestionId).HasColumnName("area_gestion_id");
            Property(x => x.IndSerializable).HasColumnName("ind_serializable");
            Property(x => x.IndActivo).HasColumnName("ind_activo");
            Property(x => x.StkMinimo).HasColumnName("stk_min");
            Property(x => x.StkMaximo).HasColumnName("stk_max");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuElimina).HasColumnName("usu_elimina");
            Property(x => x.FecElimina).HasColumnName("fec_elimina");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
