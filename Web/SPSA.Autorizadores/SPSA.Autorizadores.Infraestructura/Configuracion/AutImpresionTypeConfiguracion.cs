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
			ToTable("aut_impresion", "SGP");

			// Configura la clave primaria de la entidad AutImpresion.
			HasKey(x => new { x.Id });

			// Configura las propiedades de la entidad AutImpresion.
			Property(x => x.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(x => x.CodColaborador).HasColumnName("cod_colaborador");
			Property(x => x.CodLocal).HasColumnName("cod_local");
			Property(x => x.CodAutorizador).HasColumnName("cod_autorizador");
			Property(x => x.CodBarra).HasColumnName("cod_barra");
			Property(x => x.UsuImpresion).HasColumnName("usu_impresion");
			Property(x => x.FecImpresion).HasColumnName("fec_impresion");
			Property(x => x.Correlativo).HasColumnName("correlativo");
			Property(x => x.MotivoReimpresion).HasColumnName("motivo_reimpresion");
			Property(x => x.UsuReimpresion).HasColumnName("usu_reimpresion");
			Property(x => x.FecReimpresion).HasColumnName("fec_reimpresion");
		}
	}
}
