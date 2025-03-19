using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    internal class MaeColaboradorIntTypeConfiguration : EntityTypeConfiguration<Mae_ColaboradorInt>
    {
        /// <summary>
		/// Constructor de la configuración de la entidad Mae_ColaboradorInt.
		/// </summary>
		public MaeColaboradorIntTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad Mae_ColaboradorInt.
            ToTable("MAE_COLABORADOR", "SGP");

            // Configura la clave primaria de la entidad Mae_ColaboradorInt.
            HasKey(mc => new { mc.CodEmpresa, mc.CodCadena, mc.CodRegion, mc.CodZona, mc.CodLocal, mc.CodigoOfisis });

            // Configura las propiedades de la entidad Mae_ColaboradorInt.
            Property(mc => mc.CodEmpresa).HasColumnName("COD_EMPRESA");
            Property(mc => mc.CodCadena).HasColumnName("COD_CADENA");
            Property(mc => mc.CodRegion).HasColumnName("COD_REGION");
            Property(mc => mc.CodZona).HasColumnName("COD_ZONA");
            Property(mc => mc.CodLocal).HasColumnName("COD_LOCAL");
            Property(mc => mc.CodigoOfisis).HasColumnName("CODIGO_OFISIS");
            Property(mc => mc.ApePaterno).HasColumnName("NO_APEL_PATE");
            Property(mc => mc.ApeMaterno).HasColumnName("NO_APEL_MATE");
            Property(mc => mc.NomTrabajador).HasColumnName("NO_TRAB");
            Property(mc => mc.TipDocIdent).HasColumnName("TI_DOCU_IDEN");
            Property(mc => mc.NumDocIdent).HasColumnName("NU_DOCU_IDEN");
            Property(mc => mc.FecIngrEmp).HasColumnName("FE_INGR_EMPR");
            Property(mc => mc.FecCeseTrab).HasColumnName("FE_CESE_TRAB");
            Property(mc => mc.CodPlan).HasColumnName("CO_PLAN");
            Property(mc => mc.DesPlan).HasColumnName("DE_PLAN");
            Property(mc => mc.TiSitu).HasColumnName("TI_SITU");
            Property(mc => mc.CodPuesTrab).HasColumnName("CO_PUES_TRAB");
            Property(mc => mc.CodSede).HasColumnName("CO_SEDE");
            Property(mc => mc.CodMotiSepa).HasColumnName("CO_MOTI_SEPA");
            Property(mc => mc.TipUsuario).HasColumnName("TIP_USUARIO");
            Property(mc => mc.Codjerarquia).HasColumnName("COD_JERARQUIA");
            Property(mc => mc.UsuCreacion).HasColumnName("USU_CREACION");
            Property(mc => mc.FecCreacion).HasColumnName("FEC_CREACION");
            Property(mc => mc.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(mc => mc.FecModifica).HasColumnName("FEC_MODIFICA");
        }
    }
}
