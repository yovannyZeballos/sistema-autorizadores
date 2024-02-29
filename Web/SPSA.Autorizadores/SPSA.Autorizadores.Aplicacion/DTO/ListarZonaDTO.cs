namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar Zona.
	/// </summary>
	public class ListarZonaDTO
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
		/// Obtiene o establece el código de la zona.
		/// </summary>
		public string CodZona { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre de la zona.
		/// </summary>
		public string NomZona { get; set; }

		/// <summary>
		/// Indicador de si la region está asociada.
		/// </summary>
		public bool IndAsociado { get; set; }
	}
}
