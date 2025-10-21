using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class CcomSolicitudCabTypeConfiguration : EntityTypeConfiguration<CCom_SolicitudCab>
    {
        public CcomSolicitudCabTypeConfiguration()
        {
            ToTable("ccom_solicitud_cabecera", "SGP");

            HasKey(cab => cab.NroSolicitud);

            Property(cab => cab.NroSolicitud)
                .HasColumnName("nro_solicitud")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(cab => cab.RznSocial).HasColumnName("rzn_social");
            Property(cab => cab.TipEstado).HasColumnName("tip_estado");
            
            Property(cab => cab.FecSolicitud).HasColumnName("fec_solicitud");
            Property(cab => cab.UsuSolicitud).HasColumnName("usu_solicitud");
            Property(cab => cab.FecRecepcion).HasColumnName("fec_recepcion");
            Property(cab => cab.UsuRecepcion).HasColumnName("usu_recepcion");
            Property(cab => cab.FecRegistro).HasColumnName("fec_registro");
            Property(cab => cab.UsuRegistro).HasColumnName("usu_registro");
            Property(cab => cab.FecCreacion).HasColumnName("fec_creacion");
        }
    }
}
