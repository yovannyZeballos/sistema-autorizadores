using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrBinesInretailTypeConfiguration : EntityTypeConfiguration<Mdr_BinesInretail>
    {
        public MdrBinesInretailTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("mdr_bines_inretail", "SGP");

            // Configura la clave primaria de la entidad.
            //HasKey(x => new { x.CodPeriodo, x.CodEmpresa, x.NumBin6 });

            // Configura las propiedades de la entidad.
            Property(x => x.Longitud).HasColumnName("longitud");
            Property(x => x.NumBin89).HasColumnName("bin_8_9");
            Property(x => x.NumBin6).HasColumnName("bin_6");
            Property(x => x.Marca).HasColumnName("marca");
            Property(x => x.TipoTarjeta).HasColumnName("tipo_tarjeta");
            Property(x => x.SubTipoTarjeta).HasColumnName("subtipo_tarjeta");
            Property(x => x.SubProducto).HasColumnName("subproducto");
            Property(x => x.CategoriaTarjeta).HasColumnName("categoria_tarjeta");
            Property(x => x.BancoEmisor).HasColumnName("banco_emisor");
            Property(x => x.TraduccionTarifario).HasColumnName("traduccion_tarifario");
            Property(x => x.CodOperador).HasColumnName("cod_operador");
            Property(x => x.CodClasificacion).HasColumnName("cod_clasificacion");

        }
    }
}
