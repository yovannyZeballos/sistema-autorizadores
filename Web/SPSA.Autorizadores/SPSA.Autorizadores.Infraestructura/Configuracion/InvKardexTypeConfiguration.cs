using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class InvKardexTypeConfiguration : EntityTypeConfiguration<InvKardex>
    {
        public InvKardexTypeConfiguration()
        {
            ToTable("INV_KARDEX", "SGP");
            HasKey(x => new { x.Id});
            Property(x => x.Id).HasColumnName("ID");
            Property(x => x.Kardex).HasColumnName("KARDEX");
            Property(x => x.Fecha).HasColumnName("FECHA");
            Property(x => x.Guia).HasColumnName("GUIA");
            Property(x => x.ActivoId).HasColumnName("ACTIVO_ID");
            Property(x => x.Serie).HasColumnName("SERIE");
            Property(x => x.OrigenId).HasColumnName("ORIGEN_ID");
            Property(x => x.DestinoId).HasColumnName("DESTINO_ID");
            Property(x => x.Tk).HasColumnName("TK");
            Property(x => x.Cantidad).HasColumnName("CANTIDAD");
            Property(x => x.TipoStock).HasColumnName("TIPO_STOCK");
            Property(x => x.Oc).HasColumnName("OC");
            Property(x => x.Sociedad).HasColumnName("SOCIEDAD");
        }
    }
}
