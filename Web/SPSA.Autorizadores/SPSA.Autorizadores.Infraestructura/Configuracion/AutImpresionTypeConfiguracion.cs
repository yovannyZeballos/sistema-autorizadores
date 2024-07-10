using SPSA.Autorizadores.Aplicacion.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	/// <summary>
	/// Configuración de tipo para la entidad AutImpresion.
	/// </summary>
	public class AutImpresionTypeConfiguracion : EntityTypeConfiguration<AutImpresion>
	{
		/// <summary>
		/// Inicializa una nueva instancia de la clase AutImpresionTypeConfiguracion.
		/// </summary>
		public AutImpresionTypeConfiguracion()
		{
			// Configura la tabla a la que se mapea la entidad AutImpresion.
			ToTable("AUT_IMPRESION", "SGP");

			// Configura la clave primaria de la entidad AutImpresion.
			HasKey(x => new { x.Id });

			// Configura las propiedades de la entidad AutImpresion.
			Property(x => x.Id).HasColumnName("ID")
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(x => x.CodColaborador).HasColumnName("COD_COLABORADOR");
			Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
			Property(x => x.CodAutorizador).HasColumnName("COD_AUTORIZADOR");
			Property(x => x.CodBarra).HasColumnName("COD_BARRA");
			Property(x => x.UsuImpresion).HasColumnName("USU_IMPRESION");
			Property(x => x.FecImpresion).HasColumnName("FEC_IMPRESION");
			Property(x => x.Correlativo).HasColumnName("CORRELATIVO");
			Property(x => x.MotivoReimpresion).HasColumnName("MOTIVO_REIMPRESION");
			Property(x => x.UsuReimpresion).HasColumnName("USU_REIMPRESION");
			Property(x => x.FecReimpresion).HasColumnName("FEC_REIMPRESION");
		}
	}
}
