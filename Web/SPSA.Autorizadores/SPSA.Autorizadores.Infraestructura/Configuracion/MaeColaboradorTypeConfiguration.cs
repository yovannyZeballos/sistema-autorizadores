using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeColaboradorTypeConfiguration : EntityTypeConfiguration<Mae_Colaborador>
    {
        public MaeColaboradorTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad Mae_Colaborador.
            ToTable("MAE_COLABORADOR", "SGP");

            // Configura la clave primaria de la entidad Mae_Colaborador.
            HasKey(x => new { x.CoEmpr, x.CodigoOfisis });

            // Configura las propiedades de la entidad Mae_Colaborador.
            Property(x => x.CoEmpr).HasColumnName("CO_EMPR");
            Property(x => x.CodigoOfisis).HasColumnName("CODIGO_OFISIS");
            Property(x => x.NoApelPate).HasColumnName("NO_APEL_PATE");
            Property(x => x.NoApelMate).HasColumnName("NO_APEL_MATE");
            Property(x => x.NoTrab).HasColumnName("NO_TRAB");
            Property(x => x.TiDocuIden).HasColumnName("TI_DOCU_IDEN");
            Property(x => x.NuDocuIden).HasColumnName("NU_DOCU_IDEN");
            Property(x => x.FeIngrEmpr).HasColumnName("FE_INGR_EMPR");
            Property(x => x.FeCeseTrab).HasColumnName("FE_CESE_TRAB");
            Property(x => x.CoPlan).HasColumnName("CO_PLAN");
            Property(x => x.DePlan).HasColumnName("DE_PLAN");
            Property(x => x.TiSitu).HasColumnName("TI_SITU");
            Property(x => x.CoPuesTrab).HasColumnName("CO_PUES_TRAB");
            Property(x => x.CoSede).HasColumnName("CO_SEDE");
            Property(x => x.CoDepa).HasColumnName("CO_DEPA");
            Property(x => x.DeDepa).HasColumnName("DE_DEPA");
            Property(x => x.CoArea).HasColumnName("CO_AREA");
            Property(x => x.DeArea).HasColumnName("DE_AREA");
            Property(x => x.CoSecc).HasColumnName("CO_SECC");
            Property(x => x.DeSecc).HasColumnName("DE_SECC");
            Property(x => x.CoMotiSepa).HasColumnName("CO_MOTI_SEPA");
            Property(x => x.IndInterno).HasColumnName("IND_INTERNO");
        }
    }
}
