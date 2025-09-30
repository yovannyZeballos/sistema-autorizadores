using System.Data.Entity.ModelConfiguration;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvServidorTypeConfiguration : EntityTypeConfiguration<SrvServidor>
    {
        public SrvServidorTypeConfiguration()
        {
            ToTable("srv_servidor", "SGP");

            HasKey(x => new { x.Id });

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.CodEmpresa).HasColumnName("cod_empresa");
            Property(x => x.CodLocal).HasColumnName("cod_local");
            Property(x => x.TipoId).HasColumnName("tipo_id");
            Property(x => x.IndActivo).HasColumnName("ind_activo");
            Property(x => x.Hostname).HasColumnName("hostname");
            Property(x => x.Ip).HasColumnName("ip");
            Property(x => x.MarcaId).HasColumnName("marca_id");
            Property(x => x.Modelo).HasColumnName("modelo");
            Property(x => x.NumSerie).HasColumnName("num_serie");
            Property(x => x.RamGb).HasColumnName("ram_gb");
            Property(x => x.CpuModelo).HasColumnName("cpu_modelo");
            Property(x => x.CpuSockets).HasColumnName("cpu_sockets");
            Property(x => x.CpuCores).HasColumnName("cpu_cores");
            Property(x => x.HddTotalGb).HasColumnName("hdd_total_gb");
            Property(x => x.StorageDesc).HasColumnName("storage_desc");
            Property(x => x.SoId).HasColumnName("so_id");
            Property(x => x.HostnameBranch).HasColumnName("hostname_branch");
            Property(x => x.IpBranch).HasColumnName("ip_branch");
            Property(x => x.HostnameFileserver).HasColumnName("hostname_fileserver");
            Property(x => x.IpFileserver).HasColumnName("ip_fileserver");
            Property(x => x.HostnameUnicola).HasColumnName("hostname_unicola");
            Property(x => x.IpUnicola).HasColumnName("ip_unicola");
            Property(x => x.EnlaceUrl).HasColumnName("enlace_url");
            Property(x => x.IpIdracIlo).HasColumnName("ip_idrac_ilo");
            Property(x => x.UsuCreacion).HasColumnName("usu_creacion");
            Property(x => x.FecCreacion).HasColumnName("fec_creacion");
            Property(x => x.UsuModifica).HasColumnName("usu_modifica");
            Property(x => x.FecModifica).HasColumnName("fec_modifica");
        }
    }
}
