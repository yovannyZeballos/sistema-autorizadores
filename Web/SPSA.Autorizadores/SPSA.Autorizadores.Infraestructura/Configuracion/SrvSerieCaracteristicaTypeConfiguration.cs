using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvSerieCaracteristicaTypeConfiguration : EntityTypeConfiguration<SrvSerieCaracteristica>
    {
        public SrvSerieCaracteristicaTypeConfiguration()
        {
            ToTable("srv_serie_caracteristica", "SGP");

            HasKey(x => x.SerieProductoId );

            Property(x => x.SerieProductoId)
    .HasColumnName("serie_producto_id")
    .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            Property(x => x.TipoId).HasColumnName("tipo_id");
            Property(x => x.Hostname).HasColumnName("hostname");
            Property(x => x.IpSo).HasColumnName("ip_so");
            Property(x => x.RamGb).HasColumnName("ram_gb");
            Property(x => x.CpuSockets).HasColumnName("cpu_sockets");
            Property(x => x.CpuCores).HasColumnName("cpu_cores");
            Property(x => x.HddTotal).HasColumnName("hdd_total");
            Property(x => x.SoId).HasColumnName("so_id");
            Property(x => x.MacAddress).HasColumnName("mac_address");
            Property(x => x.FecIngreso).HasColumnName("fec_ingreso");
            Property(x => x.Antiguedad).HasColumnName("antiguedad");
            Property(x => x.ConexionRemota).HasColumnName("conexion_remota");
            Property(x => x.IpRemota).HasColumnName("ip_remota");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
