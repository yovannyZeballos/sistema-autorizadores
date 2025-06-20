using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class CcomSolicitudCabTypeConfiguration : EntityTypeConfiguration<CCom_SolicitudCab>
    {
        public CcomSolicitudCabTypeConfiguration()
        {
            ToTable("CCOM_SOLICITUD_CAB", "SGP");

            HasKey(cab => cab.NroSolicitud);

            Property(cab => cab.NroSolicitud)
                .HasColumnName("NRO_SOLICITUD")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(cab => cab.TipEstado).HasColumnName("TIP_ESTADO");
            Property(cab => cab.RznSocial).HasColumnName("RZN_SOCIAL");
            Property(cab => cab.FecSolicitud).HasColumnName("FEC_SOLICITUD");
            Property(cab => cab.UsuSolicitud).HasColumnName("USU_SOLICITUD");
            Property(cab => cab.FecRecepcion).HasColumnName("FEC_RECEPCION");
            Property(cab => cab.UsuRecepcion).HasColumnName("USU_RECEPCION");
            Property(cab => cab.FecRegistro).HasColumnName("FEC_REGISTRO");
            Property(cab => cab.UsuRegistro).HasColumnName("USU_REGISTRO");
            Property(cab => cab.FecCreacion).HasColumnName("FEC_CREACION");
        }
    }
}
