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
            // Configura la tabla a la que se mapea la entidad Mae_Puesto.
            ToTable("MAE_PUESTO", "SGP");

            // Configura la clave primaria de la entidad Mae_Puesto.
            HasKey(mp => new { mp.CodEmpresa, mp.CodPuesto });

            // Configura las propiedades de la entidad Mae_Puesto.
            Property(mp => mp.CodEmpresa).HasColumnName("CO_EMPR");
            Property(mp => mp.CodPuesto).HasColumnName("CO_PUES_TRAB");
            Property(mp => mp.DesPuesto).HasColumnName("DE_PUES_TRAB");
            Property(mp => mp.IndAutAut).HasColumnName("IND_AUT_AUT");
            Property(mp => mp.IndAutOpe).HasColumnName("IND_AUT_OPE");
            Property(mp => mp.IndManAut).HasColumnName("IND_MAN_AUT");
            Property(mp => mp.IndManOpe).HasColumnName("IND_MAN_OPE");
            Property(mp => mp.FecAsigna).HasColumnName("FEC_ASIGNA");
            Property(mp => mp.UsuAsigna).HasColumnName("USU_ASIGNA");
            Property(mp => mp.FecCreacion).HasColumnName("FEC_CREACION");
            Property(mp => mp.UsuCreacion).HasColumnName("USU_CREACION");
            Property(mp => mp.FecElimina).HasColumnName("FEC_ELIMINA");
            Property(mp => mp.UsuElimina).HasColumnName("USU_ELIMINA");
        }
    }
}
