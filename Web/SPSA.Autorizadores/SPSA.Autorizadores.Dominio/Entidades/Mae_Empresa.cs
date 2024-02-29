namespace SPSA.Autorizadores.Dominio.Entidades
{
	/// <summary>
	/// Representa una empresa en el dominio de autorizadores.
	/// </summary>
	public class Mae_Empresa
	{
		/// <summary>
		/// Obtiene o establece el código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre de la empresa.
		/// </summary>
		public string NomEmpresa { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la sociedad.
		/// </summary>
		public string CodSociedad { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la empresa ofisis.
		/// </summary>
		public string CodEmpresaOfi { get; set; }

		/// <summary>
		/// Obtiene o establece el RUC de la empresa.
		/// </summary>
		public string Ruc { get; set; }
	}
}