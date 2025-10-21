using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class GuiaRecepcionCabeceraTypeConfiguration : EntityTypeConfiguration<GuiaRecepcionCabecera>
    {
        public GuiaRecepcionCabeceraTypeConfiguration()
        {
            ToTable("guia_recepcion_cabecera", "SGP");

            HasKey(x => new { x.Id });

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.NumGuia).HasColumnName("num_guia");
            Property(x => x.OrdenCompra).HasColumnName("orden_compra");
            Property(x => x.Fecha).HasColumnName("fecha");
            Property(x => x.ProveedorRuc).HasColumnName("proveedor_ruc");
            Property(x => x.CodEmpresaOrigen).HasColumnName("cod_empresa_origen");
            Property(x => x.CodLocalOrigen).HasColumnName("cod_local_origen");
            Property(x => x.CodEmpresaDestino).HasColumnName("cod_empresa_destino");
            Property(x => x.CodLocalDestino).HasColumnName("cod_local_destino");
            Property(x => x.AreaGestion).HasColumnName("area_gestion");
            Property(x => x.ClaseStock).HasColumnName("clase_stock");
            Property(x => x.Observaciones).HasColumnName("observaciones");
            Property(x => x.IndTransferencia).HasColumnName("ind_transferencia");
            Property(x => x.IndEstado).HasColumnName("ind_estado");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
