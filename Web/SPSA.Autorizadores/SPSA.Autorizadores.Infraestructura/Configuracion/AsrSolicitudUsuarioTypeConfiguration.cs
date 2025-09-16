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
            ToTable("asr_solicitud_usuario", "SGP");

            // Configura la clave primaria de la entidad ASR_SolicitudUsuario.
            HasKey(asu => new { asu.NumSolicitud});

            // Configura las propiedades de la entidad ASR_SolicitudUsuario.
            Property(asu => asu.NumSolicitud).HasColumnName("num_solicitud").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(asu => asu.CodEmpresa).HasColumnName("cod_empresa");
            Property(asu => asu.CodLocal).HasColumnName("cod_local");
            Property(asu => asu.CodColaborador).HasColumnName("cod_colaborador");
            Property(asu => asu.TipUsuario).HasColumnName("tip_usuario");
            Property(asu => asu.TipColaborador).HasColumnName("tip_colaborador");
            Property(asu => asu.UsuSolicita).HasColumnName("usu_solicita");
            Property(asu => asu.FecSolicita).HasColumnName("fec_solicita");
            Property(asu => asu.TipAccion).HasColumnName("tip_accion");
            Property(asu => asu.UsuAprobacion).HasColumnName("usu_aprobacion");
            Property(asu => asu.FecAprobacion).HasColumnName("fec_aprobacion");
            Property(asu => asu.IndAprobado).HasColumnName("ind_aprobado");
            Property(asu => asu.Motivo).HasColumnName("motivo");
            Property(asu => asu.UsuElimina).HasColumnName("usu_elimina");
            Property(asu => asu.FecElimina).HasColumnName("fec_elimina");
        }
    }
}
