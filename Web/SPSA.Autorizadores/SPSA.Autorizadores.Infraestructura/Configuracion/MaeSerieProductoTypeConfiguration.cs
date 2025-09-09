using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaeSerieProductoTypeConfiguration : EntityTypeConfiguration<Mae_SerieProducto>
    {
        public MaeSerieProductoTypeConfiguration()
        {
            ToTable("mae_serie_producto", "SGP");

            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.CodProducto).HasColumnName("cod_producto");
            Property(x => x.NumSerie).HasColumnName("num_serie");
            Property(x => x.IndEstado).HasColumnName("ind_estado");
            Property(x => x.CodEmpresa).HasColumnName("cod_empresa");
            Property(x => x.CodLocal).HasColumnName("cod_local");
            Property(x => x.FecIngreso).HasColumnName("fec_ingreso");
            Property(x => x.FecSalida).HasColumnName("fec_salida");
            Property(x => x.StkActual).HasColumnName("stk_actual");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
        }
    }
}
