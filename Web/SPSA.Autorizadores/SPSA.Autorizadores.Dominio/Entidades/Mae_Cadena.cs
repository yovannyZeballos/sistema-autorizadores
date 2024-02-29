namespace SPSA.Autorizadores.Dominio.Entidades
{
	/// <summary>
	/// Representa la entidad Mae_Cadena en el dominio.
	/// </summary>
	public class Mae_Cadena
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
		/// Obtiene o establece el número de la cadena.
		/// </summary>
		public int CadNumero { get; set; }
	}
}