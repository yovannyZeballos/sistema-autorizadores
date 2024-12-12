using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeHorarioTypeConfiguration : EntityTypeConfiguration<Mae_Horario>
    {
        public MaeHorarioTypeConfiguration()
        {
            ToTable("MAE_LOCAL_HORARIO", "SGP");
            HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona, x.CodLocal, x.NumDia });

            Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
            Property(x => x.CodCadena).HasColumnName("COD_CADENA");
            Property(x => x.CodRegion).HasColumnName("COD_REGION");
            Property(x => x.CodZona).HasColumnName("COD_ZONA");
            Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
            Property(x => x.NumDia).HasColumnName("NUM_DIA");
            Property(x => x.CodDia).HasColumnName("COD_DIA");
            Property(x => x.HorOpen).HasColumnName("HOR_OPEN");
            Property(x => x.HorClose).HasColumnName("HOR_CLOSE");
            Property(x => x.MinLmt).HasColumnName("MIN_LMT");
            Property(x => x.UsuCreacion).HasColumnName("USU_CREACION");
            Property(x => x.FecCreacion).HasColumnName("FEC_CREACION");
            Property(x => x.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(x => x.FecModifica).HasColumnName("FEC_MODIFICA");
        }
    }
}
