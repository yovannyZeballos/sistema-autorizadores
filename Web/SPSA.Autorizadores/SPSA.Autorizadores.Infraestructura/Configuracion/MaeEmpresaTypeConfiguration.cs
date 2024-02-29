using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	/// <summary>
	/// Configuración de la entidad Mae_Empresa para el mapeo con la base de datos.
	/// </summary>
	public class MaeEmpresaTypeConfiguration : EntityTypeConfiguration<Mae_Empresa>
	{
		/// <summary>
		/// Constructor de la configuración de la entidad Mae_Empresa.
		/// </summary>
		public MaeEmpresaTypeConfiguration()
		{
			// Configura la tabla a la que se mapea la entidad Mae_Empresa.
			ToTable("MAE_EMPRESA", "SGP");

			// Configura la clave primaria de la entidad Mae_Empresa.
			HasKey(u => u.CodEmpresa);

			// Configura las propiedades de la entidad Mae_Empresa.
			Property(u => u.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(u => u.NomEmpresa).HasColumnName("NOM_EMPRESA");
			Property(u => u.CodSociedad).HasColumnName("COD_SOCIEDAD");
			Property(u => u.CodEmpresaOfi).HasColumnName("COD_EMPRESA_OFI");
			Property(u => u.Ruc).HasColumnName("RUC");
		}
	}
}