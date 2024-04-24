namespace SPSA.Autorizadores.Dominio.Entidades
{
	/// <summary>
	/// Representa la entidad Seg_Cadena en el dominio.
	/// </summary>
	public class Seg_Cadena
	{
		/// <summary>
		/// Obtiene o establece el código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la cadena.
		/// </summary>
		public string CodCadena { get; set; }

		/// <summary>
		/// Obtiene o establece la entidad Mae_Cadena asociada.
		/// </summary>
		public virtual Mae_Cadena Mae_Cadena { get; set; }
	}
}