namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar empresas.
	/// </summary>
	public class ListarEmpresaDTO
	{
		/// <summary>
		/// Código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Nombre de la empresa.
		/// </summary>
		public string NomEmpresa { get; set; }

		/// <summary>
		/// RUC de la empresa.
		/// </summary>
		public string Ruc { get; set; }

		/// <summary>
		/// Indicador de si la empresa está asociada.
		/// </summary>
		public bool IndAsociado { get; set; }
	}
}