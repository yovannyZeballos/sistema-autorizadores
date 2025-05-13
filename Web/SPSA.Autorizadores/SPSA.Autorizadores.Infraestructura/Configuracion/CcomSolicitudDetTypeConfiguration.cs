using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class CcomSolicitudDetTypeConfiguration : EntityTypeConfiguration<CCom_SolicitudDet>
    {
        public CcomSolicitudDetTypeConfiguration()
        {
            ToTable("CCOM_SOLICITUD_DET", "SGP");

            HasKey(det => new { det.NroSolicitud, det.CodLocalAlterno });

            Property(det => det.NroSolicitud).HasColumnName("NRO_SOLICITUD");
            Property(det => det.CodLocalAlterno).HasColumnName("COD_LOCAL_ALTERNO");
            Property(det => det.FecCreacion).HasColumnName("FEC_CREACION");
            Property(det => det.UsuCreacion).HasColumnName("USU_CREACION");
            Property(det => det.FecModifica).HasColumnName("FEC_MODIFICA");
            Property(det => det.UsuModifica).HasColumnName("USU_MODIFICA");
        }
    }
}
