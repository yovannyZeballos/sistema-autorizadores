namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar locales.
	/// </summary>
	public class ListarLocalDTO
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
		/// Obtiene o establece el código del local.
		/// </summary>
		public string CodLocal { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del local.
		/// </summary>
		public string NomLocal { get; set; }

        /// <summary>
        /// Obtiene o establece el codigo del local alterno.
        /// </summary>
        public string CodLocalAlterno { get; set; }

        /// <summary>
        /// Indicador de si el local está asociada.
        /// </summary>
        public bool IndAsociado { get; set; }
	}
}
