using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrTmpBinesIzipayTypeConfiguration : EntityTypeConfiguration<Mdr_TmpBinesIzipay>
    {
        public MdrTmpBinesIzipayTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_TMP_BINES_IZIPAY", "SGP");

            // Configura la clave primaria de la entidad.
            //HasKey(x => new { x.CodLocalPMM });

            // Configura las propiedades de la entidad.
            Property(x => x.NumBin8).HasColumnName("NUM_BIN_8");
            Property(x => x.NumBin6).HasColumnName("NUM_BIN_6");
            Property(x => x.Marca).HasColumnName("MARCA");
            Property(x => x.Tipo).HasColumnName("TIPO");
            Property(x => x.Categoria).HasColumnName("CATEGORIA");
            Property(x => x.CategoriaIzipay).HasColumnName("CATEGORIA_IZIPAY");
            Property(x => x.BancoEmisor).HasColumnName("BANCO_EMISOR");
            Property(x => x.Validación).HasColumnName("VALIDACION");

        }
    }
}
