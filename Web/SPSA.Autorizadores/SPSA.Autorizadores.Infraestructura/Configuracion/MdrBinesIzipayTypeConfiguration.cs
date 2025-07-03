using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrBinesIzipayTypeConfiguration : EntityTypeConfiguration<Mdr_BinesIzipay>
    {
        public MdrBinesIzipayTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_BINES_IZIPAY", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodPeriodo, x.CodEmpresa, x.NumBin6 });

            // Configura las propiedades de la entidad.
            Property(x => x.CodPeriodo).HasColumnName("COD_PERIODO");
            Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
            Property(x => x.NumBin6).HasColumnName("NUM_BIN_6");
            Property(x => x.NomTarjeta).HasColumnName("NOM_TARJETA");
            Property(x => x.NumBin8).HasColumnName("NUM_BIN_8");
            Property(x => x.BancoEmisor).HasColumnName("BANCO_EMISOR");
            Property(x => x.Tipo).HasColumnName("TIPO");
            Property(x => x.FactorMdr).HasColumnName("FACTOR_MDR");
            Property(x => x.CodOperador).HasColumnName("COD_OPERADOR");

        }
    }
}
