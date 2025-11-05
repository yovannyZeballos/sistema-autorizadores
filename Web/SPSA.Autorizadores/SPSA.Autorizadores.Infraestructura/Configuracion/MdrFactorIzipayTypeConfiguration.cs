using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MdrFactorIzipayTypeConfiguration : EntityTypeConfiguration<Mdr_FactorIzipay>
    {
        public MdrFactorIzipayTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad.
            ToTable("mdr_factor_izipay", "SGP");

            // Configura la clave primaria de la entidad.
            HasKey(x => new { x.CodEmpresa, x.CodPeriodo, x.CodOperador, x.CodClasificacion });

            // Configura las propiedades de la entidad.
            Property(x => x.CodEmpresa).HasColumnName("cod_empresa");
            Property(x => x.CodPeriodo).HasColumnName("cod_periodo");
            Property(x => x.CodOperador).HasColumnName("cod_operador");
            Property(x => x.CodClasificacion).HasColumnName("cod_clasificacion");
            Property(x => x.Factor).HasColumnName("factor");
            Property(x => x.IndActivo).HasColumnName("ind_activo");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");

        }
    }
}
