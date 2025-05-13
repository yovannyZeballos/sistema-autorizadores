using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeCodComercioTypeConfiguration : EntityTypeConfiguration<Mae_CodComercio>
    {
        public MaeCodComercioTypeConfiguration()
        {
            ToTable("MAE_LOCAL_COMERCIO", "SGP");

            HasKey(loc => new { loc.NroSolicitud, loc.CodLocalAlterno, loc.CodComercio });

            Property(loc => loc.NroSolicitud).HasColumnName("NRO_SOLICITUD");
            Property(loc => loc.CodLocalAlterno).HasColumnName("COD_LOCAL_ALTERNO");
            Property(loc => loc.CodComercio).HasColumnName("COD_COMERCIO");
            Property(loc => loc.NomCanalVta).HasColumnName("NOM_CANAL_VTA");
            Property(loc => loc.DesOperador).HasColumnName("DES_OPERADOR");
            Property(loc => loc.IndActiva).HasColumnName("IND_ACTIVA");
            Property(loc => loc.FecCreacion).HasColumnName("FEC_CREACION");
            Property(loc => loc.UsuCreacion).HasColumnName("USU_CREACION");
            Property(loc => loc.FecModifica).HasColumnName("FEC_MODIFICA");
            Property(loc => loc.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(loc => loc.NomProcesador).HasColumnName("NOM_PROCESADOR");
        }
    }
}
