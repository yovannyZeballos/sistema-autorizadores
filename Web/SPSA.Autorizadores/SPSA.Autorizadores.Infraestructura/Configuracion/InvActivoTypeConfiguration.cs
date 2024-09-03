using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    internal class InvActivoTypeConfiguration : EntityTypeConfiguration<Inv_Activo>
    {
        public InvActivoTypeConfiguration()
        {
            // Configura la tabla a la que se mapea la entidad Mae_Local.
            ToTable("INV_ACTIVO", "SGP");

            // Configura la clave primaria de la entidad Mae_Local.
            HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona, x.CodLocal , x.CodActivo, x.CodModelo, x.NomMarca, x.CodSerie});

            // Configura las propiedades de la entidad Mae_Local.
            Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
            Property(x => x.CodCadena).HasColumnName("COD_CADENA");
            Property(x => x.CodRegion).HasColumnName("COD_REGION");
            Property(x => x.CodZona).HasColumnName("COD_ZONA");
            Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
            Property(x => x.CodActivo).HasColumnName("COD_ACTIVO");
            Property(x => x.CodModelo).HasColumnName("COD_MODELO");
            Property(x => x.CodSerie).HasColumnName("COD_SERIE");
            Property(x => x.NomMarca).HasColumnName("NOM_MARCA");
            Property(x => x.Ip).HasColumnName("IP");
            Property(x => x.NomArea).HasColumnName("NOM_AREA");
            Property(x => x.NumOc).HasColumnName("NUM_OC");
            Property(x => x.NumGuia).HasColumnName("NUM_GUIA");
            Property(x => x.FecSalida).HasColumnName("FEC_SALIDA");
            Property(x => x.Antiguedad).HasColumnName("ANTIGUEDAD");
            Property(x => x.IndOperativo).HasColumnName("IND_OPERATIVO");
            Property(x => x.Observacion).HasColumnName("OBSERVACION");
            Property(x => x.Garantia).HasColumnName("GARANTIA");
            Property(x => x.FecActualiza).HasColumnName("FEC_ACTUALIZA");
            Property(x => x.Cantidad).HasColumnName("CANTIDAD");
        }
    }
}
