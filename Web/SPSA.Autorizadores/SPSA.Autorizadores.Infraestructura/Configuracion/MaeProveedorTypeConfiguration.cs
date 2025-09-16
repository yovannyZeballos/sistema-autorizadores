using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeProveedorTypeConfiguration : EntityTypeConfiguration<Mae_Proveedor>
    {
        public MaeProveedorTypeConfiguration()
        {
            ToTable("mae_proveedor", "SGP");

            HasKey(x => new { x.Ruc});

            Property(x => x.Ruc).HasColumnName("ruc");
            Property(x => x.RazonSocial).HasColumnName("razon_social");
            Property(x => x.NomComercial).HasColumnName("nom_comercial");
            Property(x => x.IndActivo).HasColumnName("ind_activo");
            Property(x => x.DirFiscal).HasColumnName("dir_fiscal");
            Property(x => x.Contacto).HasColumnName("contacto");
            Property(x => x.Telefono).HasColumnName("telefono");
            Property(x => x.Email).HasColumnName("email");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuElimina).HasColumnName("usu_elimina");
            Property(x => x.FecElimina).HasColumnName("fec_elimina");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
