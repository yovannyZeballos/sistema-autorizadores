using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	/// <summary>
	/// Configuración de la entidad Mae_Region para el mapeo con la base de datos.
	/// </summary>
	public class MaeRegionTypeConfiguration : EntityTypeConfiguration<Mae_Region>
	{
		/// <summary>
		/// Constructor de la configuración de la entidad Mae_Region.
		/// </summary>
		public MaeRegionTypeConfiguration()
		{
			// Configura la tabla a la que se mapea la entidad Mae_Region.
			ToTable("MAE_REGION", "SGP");

			// Configura la clave primaria de la entidad Mae_Region.
			HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion });

			// Configura las propiedades de la entidad Mae_Region.
			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(x => x.CodCadena).HasColumnName("COD_CADENA");
			Property(x => x.CodRegion).HasColumnName("COD_REGION");
			Property(x => x.NomRegion).HasColumnName("NOM_REGION");
			Property(x => x.CodRegional).HasColumnName("COD_REGIONAL");
		}
	}
}