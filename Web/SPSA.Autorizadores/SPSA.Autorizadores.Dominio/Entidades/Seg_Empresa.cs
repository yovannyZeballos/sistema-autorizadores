namespace SPSA.Autorizadores.Dominio.Entidades
{
	/// <summary>
	/// Representa una empresa en el dominio de seguridad.
	/// </summary>
	public class Seg_Empresa
	{
		/// <summary>
		/// Obtiene o establece el código de usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Obtiene o establece el código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

        public virtual Mae_Empresa Mae_Empresa { get; set; }
    }
}