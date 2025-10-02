using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvTipoServidorTypeConfiguration : EntityTypeConfiguration<SrvTipoServidor>
    {
        public SrvTipoServidorTypeConfiguration()
        {
            ToTable("srv_tipo_servidor", "SGP");

            HasKey(x => new { x.Id});

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.NomTipo).HasColumnName("nom_tipo");
        }
    }
}
