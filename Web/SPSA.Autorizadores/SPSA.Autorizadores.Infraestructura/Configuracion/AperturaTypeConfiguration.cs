using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class AperturaTypeConfiguration : EntityTypeConfiguration<Apertura>
    {
        public AperturaTypeConfiguration()
        {

            // Configura la tabla a la que se mapea la entidad Apertura.
            ToTable("APERTURA", "SGP");

            // Configura la clave primaria de la entidad Apertura.
            HasKey(x => new { x.CodLocalPMM});

            // Configura las propiedades de la entidad Apertura.
            Property(x => x.CodLocalPMM).HasColumnName("COD_LOCAL_PMM");
            Property(x => x.NomLocalPMM).HasColumnName("NOM_LOCAL_PMM");
            Property(x => x.CodLocalSAP).HasColumnName("COD_LOCAL_SAP");
            Property(x => x.NomLocalSAP).HasColumnName("NOM_LOCAL_SAP");
            Property(x => x.CodLocalSAPNew).HasColumnName("COD_LOCAL_SAP_NEW");
            Property(x => x.CodLocalOfiplan).HasColumnName("COD_LOCAL_OFIPLAN");
            Property(x => x.NomLocalOfiplan).HasColumnName("NOM_LOCAL_OFIPLAN");
            Property(x => x.Administrador).HasColumnName("ADMINISTRADOR");
            Property(x => x.NumTelefono).HasColumnName("NUM_TELEFONO");
            Property(x => x.Email).HasColumnName("EMAIL");
            Property(x => x.Direccion).HasColumnName("DIRECCION");
            Property(x => x.Ubigeo).HasColumnName("UBIGEO");
            Property(x => x.CodComercio).HasColumnName("COD_COMERCIO");
            Property(x => x.CodCentroCosto).HasColumnName("COD_CECO");
            Property(x => x.FecApertura).HasColumnName("FEC_APERTURA");
            Property(x => x.TipEstado).HasColumnName("TIP_ESTADO");
            Property(x => x.UsuCreacion).HasColumnName("USU_CREACION");
            Property(x => x.FecCreacion).HasColumnName("FEC_CREACION");
            Property(x => x.UsuModifica).HasColumnName("USU_MODIFICA");
            Property(x => x.FecModifica).HasColumnName("FEC_MODIFICA");
        }
    }
}
