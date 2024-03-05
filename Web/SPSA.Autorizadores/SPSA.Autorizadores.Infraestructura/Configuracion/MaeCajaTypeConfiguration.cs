using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeCajaTypeConfiguration : EntityTypeConfiguration<Mae_Caja>
    {
        public MaeCajaTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad Mae_Zona.
            ToTable("MAE_LOCAL_CAJA", "SGP");

            // Configura la clave primaria de la entidad Mae_Zona.
            HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona, x.CodLocal, x.NumCaja });

            // Configura las propiedades de la entidad Mae_Zona.
            Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
            Property(x => x.CodCadena).HasColumnName("COD_CADENA");
            Property(x => x.CodRegion).HasColumnName("COD_REGION");
            Property(x => x.CodZona).HasColumnName("COD_ZONA");
            Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
            Property(x => x.NumCaja).HasColumnName("NUM_CAJA");
            //Property(x => x.CodCordina).HasColumnName("COD_CORDINA");
        }
    }
}
