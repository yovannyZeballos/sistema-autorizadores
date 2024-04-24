using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	/// <summary>
	/// Configuración de la entidad Mae_Zona para el mapeo con la base de datos.
	/// </summary>
	public class MaeZonaTypeConfiguration : EntityTypeConfiguration<Mae_Zona>
	{
		/// <summary>
		/// Constructor de la configuración de la entidad Mae_Zona.
		/// </summary>
		public MaeZonaTypeConfiguration()
		{
			// Configura la tabla a la que se mapea la entidad Mae_Zona.
			ToTable("MAE_ZONA", "SGP");

			// Configura la clave primaria de la entidad Mae_Zona.
			HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona });

			// Configura las propiedades de la entidad Mae_Zona.
			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(x => x.CodCadena).HasColumnName("COD_CADENA");
			Property(x => x.CodRegion).HasColumnName("COD_REGION");
			Property(x => x.CodZona).HasColumnName("COD_ZONA");
			Property(x => x.NomZona).HasColumnName("NOM_ZONA");
			Property(x => x.CodCordina).HasColumnName("COD_CORDINA");
		}
	}
}