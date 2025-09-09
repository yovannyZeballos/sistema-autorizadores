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
            ToTable("mae_colaborador_ext", "SGP");

            // Configura la clave primaria de la entidad Mae_ColaboradorExt.
            HasKey(mce => new { mce.CodEmpresa, mce.CodLocal, mce.CodigoOfisis });

            // Configura las propiedades de la entidad Mae_ColaboradorExt.
            Property(mce => mce.CodEmpresa).HasColumnName("cod_empresa");
            Property(mce => mce.CodLocal).HasColumnName("cod_local");
            Property(mce => mce.CodigoOfisis).HasColumnName("cod_ofisis").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(mce => mce.ApelPaterno).HasColumnName("nom_apel_pate");
            Property(mce => mce.ApelMaterno).HasColumnName("nom_apel_mate");
            Property(mce => mce.NombreTrabajador).HasColumnName("nom_trab");
            Property(mce => mce.TipoDocIdent).HasColumnName("tip_docu_iden");
            Property(mce => mce.NumDocIndent).HasColumnName("num_docu_iden");
            Property(mce => mce.FechaIngresoEmpresa).HasColumnName("fec_ingr_empr");
            Property(mce => mce.FechaCeseTrabajador).HasColumnName("fec_cese_trab");
            Property(mce => mce.IndActivo).HasColumnName("ind_activo");
            Property(mce => mce.PuestoTrabajo).HasColumnName("nom_pues_trab");
            Property(mce => mce.MotiSepa).HasColumnName("des_moti_sepa");
            Property(mce => mce.IndPersonal).HasColumnName("ind_personal");
            Property(mce => mce.TipoUsuario).HasColumnName("tip_usuario");
            Property(mce => mce.UsuCreacion).HasColumnName("usu_creacion");
            Property(mce => mce.FecCreacion).HasColumnName("fec_creacion");
            Property(mce => mce.UsuModifica).HasColumnName("usu_modifica");
            Property(mce => mce.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
