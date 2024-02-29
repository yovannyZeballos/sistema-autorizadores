namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar Cadena.
	/// </summary>
	public class ListarCadenaDTO
	{
		/// <summary>
		/// Obtiene o establece el código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la cadena.
		/// </summary>
		public string CodCadena { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre de la cadena.
		/// </summary>
		public string NomCadena { get; set; }

		/// <summary>
		/// Indicador de si la cadena está asociada.
		/// </summary>
		public bool IndAsociado { get; set; }

	}
}