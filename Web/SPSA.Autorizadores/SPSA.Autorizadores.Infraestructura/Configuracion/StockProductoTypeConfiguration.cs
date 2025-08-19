using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class StockProductoTypeConfiguration : EntityTypeConfiguration<StockProducto>
    {
        public StockProductoTypeConfiguration()
        {
            ToTable("stock_producto", "SGP");

            HasKey(x => new { x.CodProducto, x.CodEmpresa, x.CodLocal });

            Property(x => x.CodProducto).HasColumnName("cod_producto");
            Property(x => x.CodEmpresa).HasColumnName("cod_empresa");
            Property(x => x.CodLocal).HasColumnName("cod_local");
            Property(x => x.StkDisponible).HasColumnName("stk_disponible");
            Property(x => x.StkReservado).HasColumnName("stk_reservado");
            Property(x => x.StkTransito).HasColumnName("stk_transito");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
        }
    }
}
