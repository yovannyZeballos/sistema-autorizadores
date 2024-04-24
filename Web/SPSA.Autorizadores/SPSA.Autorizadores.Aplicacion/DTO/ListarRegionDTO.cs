namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar Region.
	/// </summary>
	public class ListarRegionDTO
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
		/// Obtiene o establece el código de la región.
		/// </summary>
		public string CodRegion { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre de la región.
		/// </summary>
		public string NomRegion { get; set; }

		/// <summary>
		/// Indicador de si la region está asociada.
		/// </summary>
		public bool IndAsociado { get; set; }
	}
}
