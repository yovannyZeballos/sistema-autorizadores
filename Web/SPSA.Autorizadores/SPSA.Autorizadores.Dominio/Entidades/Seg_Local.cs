namespace SPSA.Autorizadores.Dominio.Entidades
{
	/// <summary>
	/// Representa la entidad Seg_Local en el dominio.
	/// </summary>
	public class Seg_Local
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
		/// Obtiene o establece el código del local.
		/// </summary>
		public string CodLocal { get; set; }

		/// <summary>
		/// Obtiene o establece la entidad Mae_Local asociada.
		/// </summary>
		public virtual Mae_Local Mae_Local { get; set; }
	}
}