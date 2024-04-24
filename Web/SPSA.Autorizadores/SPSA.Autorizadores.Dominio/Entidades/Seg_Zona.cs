namespace SPSA.Autorizadores.Dominio.Entidades
{
	/// <summary>
	/// Representa la entidad Seg_Zona en el dominio.
	/// </summary>
	public class Seg_Zona
	{
		/// <summary>
		/// Obtiene o establece el código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la zona.
		/// </summary>
		public string CodZona { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la cadena.
		/// </summary>
		public string CodCadena { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la region.
		/// </summary>
		public string CodRegion { get; set; }

		/// <summary>
		/// Obtiene o establece la entidad Mae_Cadena asociada.
		/// </summary>
		public virtual Mae_Zona Mae_Zona { get; set; }
	}
}