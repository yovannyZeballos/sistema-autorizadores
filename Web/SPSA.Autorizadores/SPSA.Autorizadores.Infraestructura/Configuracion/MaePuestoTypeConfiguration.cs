using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class MaePuestoTypeConfiguration : EntityTypeConfiguration<Mae_Puesto>
    {
        /// <summary>
		/// Constructor de la configuración de la entidad Mae_Puesto.
		/// </summary>
		public MaePuestoTypeConfiguration()
        {
            ToTable("mae_puesto", "SGP");

            HasKey(mp => new { mp.CodEmpresa, mp.CodPuesto });

            // Configura las propiedades de la entidad Mae_Puesto.
            Property(mp => mp.CodEmpresa).HasColumnName("co_empr");
            Property(mp => mp.CodPuesto).HasColumnName("co_pues_trab");
            Property(mp => mp.DesPuesto).HasColumnName("de_pues_trab");
            Property(mp => mp.IndAutAut).HasColumnName("ind_aut_autorizador");
            Property(mp => mp.IndAutOpe).HasColumnName("ind_aut_operador");
            Property(mp => mp.IndManAut).HasColumnName("ind_man_autizador");
            Property(mp => mp.IndManOpe).HasColumnName("ind_man_operador");
            Property(mp => mp.FecAsigna).HasColumnName("fec_asigna");
            Property(mp => mp.UsuAsigna).HasColumnName("usu_asigna");
            Property(mp => mp.FecCreacion).HasColumnName("fec_creacion");
            Property(mp => mp.UsuCreacion).HasColumnName("usu_creacion");
            Property(mp => mp.FecElimina).HasColumnName("fec_elimina");
            Property(mp => mp.UsuElimina).HasColumnName("usu_elimina");
        }
    }
}
