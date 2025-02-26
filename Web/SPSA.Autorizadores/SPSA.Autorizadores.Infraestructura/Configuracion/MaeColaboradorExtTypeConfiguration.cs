using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeColaboradorExtTypeConfiguration : EntityTypeConfiguration<Mae_ColaboradorExt>
    {
        /// <summary>
		/// Constructor de la configuración de la entidad Mae_ColaboradorExt.
		/// </summary>
		public MaeColaboradorExtTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad Mae_ColaboradorExt.
            ToTable("MAE_COLABORADOR_EXT", "SGP");

            // Configura la clave primaria de la entidad Mae_ColaboradorExt.
            HasKey(mce => new { mce.CodLocalAlterno, mce.CodigoOfisis });

            // Configura las propiedades de la entidad Mae_ColaboradorExt.
            Property(mce => mce.CodLocalAlterno).HasColumnName("COD_LOCAL_ALTERNO");
            Property(mce => mce.CodigoOfisis).HasColumnName("CODIGO_OFISIS").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(mce => mce.ApelPaterno).HasColumnName("NO_APEL_PATE");
            Property(mce => mce.ApelMaterno).HasColumnName("NO_APEL_MATE");
            Property(mce => mce.NombreTrabajador).HasColumnName("NO_TRAB");
            Property(mce => mce.TipoDocIdent).HasColumnName("TI_DOCU_IDEN");
            Property(mce => mce.NumDocIndent).HasColumnName("NU_DOCU_IDEN");
            Property(mce => mce.FechaIngresoEmpresa).HasColumnName("FE_INGR_EMPR");
            Property(mce => mce.FechaCeseTrabajador).HasColumnName("FE_CESE_TRAB");
            Property(mce => mce.TiSitu).HasColumnName("TI_SITU");
            Property(mce => mce.PuestoTrabajo).HasColumnName("NO_PUES_TRAB");
            Property(mce => mce.MotiSepa).HasColumnName("NO_MOTI_SEPA");
            Property(mce => mce.IndPersonal).HasColumnName("IND_PERSONAL");
            Property(mce => mce.TipoUsuario).HasColumnName("TIP_USUARIO");
            Property(mce => mce.UsuCreacion).HasColumnName("USU_CREACION");
            Property(mce => mce.FecCreacion).HasColumnName("FEC_CREACION");
            Property(mce => mce.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(mce => mce.FecModifica).HasColumnName("FEC_MODIFICA");
        }
    }
}
