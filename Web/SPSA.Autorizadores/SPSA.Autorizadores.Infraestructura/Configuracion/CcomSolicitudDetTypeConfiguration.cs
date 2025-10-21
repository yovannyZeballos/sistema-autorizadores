using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class CcomSolicitudDetTypeConfiguration : EntityTypeConfiguration<CCom_SolicitudDet>
    {
        public CcomSolicitudDetTypeConfiguration()
        {
            ToTable("ccom_solicitud_detalle", "SGP");

            HasKey(det => new { det.NroSolicitud, det.CodEmpresa, det.CodLocal });

            Property(det => det.NroSolicitud).HasColumnName("nro_solicitud");
            Property(det => det.CodEmpresa).HasColumnName("cod_empresa");
            Property(det => det.CodLocal).HasColumnName("cod_local");
            Property(det => det.TipEstado).HasColumnName("tip_estado");
            Property(det => det.FecCreacion).HasColumnName("fec_creacion");
            Property(det => det.UsuCreacion).HasColumnName("usu_creacion");
            Property(det => det.FecModifica).HasColumnName("fec_modifica");
            Property(det => det.UsuModifica).HasColumnName("usu_modifica");
        }
    }
}
