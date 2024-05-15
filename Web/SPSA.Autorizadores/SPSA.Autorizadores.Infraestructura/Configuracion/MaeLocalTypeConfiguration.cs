using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	/// <summary>
	/// Configuración de la entidad Mae_Local para el mapeo con la base de datos.
	/// </summary>
	public class MaeLocalTypeConfiguration : EntityTypeConfiguration<Mae_Local>
	{
		/// <summary>
		/// Constructor de la configuración de la entidad Mae_Local.
		/// </summary>
		public MaeLocalTypeConfiguration()
		{
			// Configura la tabla a la que se mapea la entidad Mae_Local.
			ToTable("MAE_LOCAL", "SGP");

			// Configura la clave primaria de la entidad Mae_Local.
			HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona, x.CodLocal });
			//HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodLocal });

			// Configura las propiedades de la entidad Mae_Local.
			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(x => x.CodCadena).HasColumnName("COD_CADENA");
			Property(x => x.CodRegion).HasColumnName("COD_REGION");
			Property(x => x.CodZona).HasColumnName("COD_ZONA");
			Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
			Property(x => x.NomLocal).HasColumnName("NOM_LOCAL");
			Property(x => x.TipEstado).HasColumnName("TIP_ESTADO");
			Property(x => x.CodLocalPMM).HasColumnName("COD_LOCAL_PMM");
			Property(x => x.CodLocalOfiplan).HasColumnName("COD_LOCAL_OFIPLAN");
			Property(x => x.NomLocalOfiplan).HasColumnName("NOM_LOCAL_OFIPLAN");
			Property(x => x.CodLocalSunat).HasColumnName("COD_LOCAL_SUNAT");
		}
	}
}