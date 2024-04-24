using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar los usuarios.
	/// </summary>
	public class ObtenerUsuarioDTO
	{
		/// <summary>
		/// Código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código del colaborador.
		/// </summary>
		public string CodColaborador { get; set; }

		/// <summary>
		/// Indicador de si el usuario está activo.
		/// </summary>
		public string IndActivo { get; set; }

		/// <summary>
		/// Tipo de usuario.
		/// </summary>
		public string TipUsuario { get; set; }

		/// <summary>
		/// Dirección de correo electrónico del usuario.
		/// </summary>
		public string DirEmail { get; set; }

	}
}