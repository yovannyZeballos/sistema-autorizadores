using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrFactorIzipayTypeConfiguration : EntityTypeConfiguration<Mdr_FactorIzipay>
    {
        public MdrFactorIzipayTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("MDR_FACTOR_IZIPAY", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodEmpresa, x.NumAno, x.CodOperador, x.CodClasificacion });

            // Configura las propiedades de la entidad.
            Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
            Property(x => x.NumAno).HasColumnName("NUM_ANO");
            Property(x => x.CodOperador).HasColumnName("COD_OPERADOR");
            Property(x => x.CodClasificacion).HasColumnName("COD_CLASIFICACION");
            Property(x => x.Factor).HasColumnName("FACTOR");
            Property(x => x.IndActivo).HasColumnName("IND_ACTIVO");
            Property(x => x.UsuCreacion).HasColumnName("USU_CREACION");
            Property(x => x.FecCreacion).HasColumnName("FEC_CREACION");
            Property(x => x.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(x => x.FecModifica).HasColumnName("FEC_MODIFICA");

        }
    }
}
