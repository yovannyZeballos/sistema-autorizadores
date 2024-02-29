using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	/// <summary>
	/// Configuración de la entidad Mae_Cadena para el mapeo con la base de datos.
	/// </summary>
	public class MaeCadenaTypeConfiguration : EntityTypeConfiguration<Mae_Cadena>
	{
		/// <summary>
		/// Constructor de la configuración de la entidad Mae_Cadena.
		/// </summary>
		public MaeCadenaTypeConfiguration()
		{
			// Configura la tabla a la que se mapea la entidad Mae_Cadena.
			ToTable("MAE_CADENA", "SGP");

			// Configura la clave primaria de la entidad Mae_Cadena.
			HasKey(u => new { u.CodEmpresa, u.CodCadena });

			// Configura las propiedades de la entidad Mae_Cadena.
			Property(u => u.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(u => u.CodCadena).HasColumnName("COD_CADENA");
			Property(u => u.NomCadena).HasColumnName("NOM_CADENA");
			Property(u => u.CadNumero).HasColumnName("CAD_NUMERO");
		}
	}
}