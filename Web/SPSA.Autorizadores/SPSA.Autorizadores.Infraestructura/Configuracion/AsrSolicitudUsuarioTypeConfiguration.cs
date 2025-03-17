using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class AsrSolicitudUsuarioTypeConfiguration : EntityTypeConfiguration<ASR_SolicitudUsuario>
    {
        /// <summary>
		/// Constructor de la configuración de la entidad ASR_SolicitudUsuario.
		/// </summary>
		public AsrSolicitudUsuarioTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad ASR_SolicitudUsuario.
            ToTable("ASR_SOLICITUD_USUARIO", "SGP");

            // Configura la clave primaria de la entidad ASR_SolicitudUsuario.
            HasKey(asu => new { asu.NumSolicitud});

            // Configura las propiedades de la entidad ASR_SolicitudUsuario.
            Property(asu => asu.NumSolicitud).HasColumnName("NUM_SOLICITUD").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(asu => asu.CodLocalAlterno).HasColumnName("COD_LOCAL_ALTERNO");
            Property(asu => asu.CodColaborador).HasColumnName("COD_COLABORADOR");
            Property(asu => asu.TipUsuario).HasColumnName("TIP_USUARIO");
            Property(asu => asu.TipColaborador).HasColumnName("TIP_COLABORADOR");
            Property(asu => asu.UsuSolicita).HasColumnName("USU_SOLICITA");
            Property(asu => asu.FecSolicita).HasColumnName("FEC_SOLICITA");
            Property(asu => asu.TipAccion).HasColumnName("TIP_ACCION");
            Property(asu => asu.UsuAprobacion).HasColumnName("USU_APROBACION");
            Property(asu => asu.FecAprobacion).HasColumnName("FEC_APROBACION");
            Property(asu => asu.IndAprobado).HasColumnName("IND_APROBADO");
            Property(asu => asu.Motivo).HasColumnName("MOTIVO");
            Property(asu => asu.UsuElimina).HasColumnName("USU_ELIMINA");
            Property(asu => asu.FecElimina).HasColumnName("FEC_ELIMINA");
        }
    }
}
