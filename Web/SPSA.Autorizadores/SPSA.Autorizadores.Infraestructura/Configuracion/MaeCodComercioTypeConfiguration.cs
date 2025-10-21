using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeCodComercioTypeConfiguration : EntityTypeConfiguration<Mae_CodComercio>
    {
        public MaeCodComercioTypeConfiguration()
        {
            ToTable("mae_codcomercio", "SGP");

            HasKey(loc => new { loc.CodEmpresa, loc.CodLocal, loc.CodComercio });

            Property(loc => loc.NroSolicitud).HasColumnName("nro_solicitud");
            Property(loc => loc.CodEmpresa).HasColumnName("cod_empresa");
            Property(loc => loc.CodLocal).HasColumnName("cod_local");
            Property(loc => loc.CodComercio).HasColumnName("cod_comercio");
            Property(loc => loc.NomCanalVta).HasColumnName("nom_canal_vta");
            Property(loc => loc.DesOperador).HasColumnName("des_operador");
            Property(loc => loc.NroCaso).HasColumnName("nro_caso");
            Property(loc => loc.FecComercio).HasColumnName("fec_comercio");
            Property(loc => loc.IndEstado).HasColumnName("ind_estado");
            Property(loc => loc.FecCreacion).HasColumnName("fec_creacion");
            Property(loc => loc.UsuCreacion).HasColumnName("usu_creacion");
            Property(loc => loc.FecModifica).HasColumnName("fec_modifica");
            Property(loc => loc.UsuModifica).HasColumnName("usu_modifica");
        }
    }
}
