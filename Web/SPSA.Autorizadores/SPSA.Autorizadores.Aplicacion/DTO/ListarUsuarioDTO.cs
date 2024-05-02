using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	/// <summary>
	/// DTO para listar los usuarios.
	/// </summary>
	public class ListarUsuarioDTO
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

		/// <summary>
		/// Fecha de creación del usuario.
		/// </summary>
		public DateTime FecCreacion { get; set; }

		/// <summary>
		/// Usuario que creó el registro.
		/// </summary>
		public string UsuCreacion { get; set; }

		/// <summary>
		/// Fecha de eliminación del usuario.
		/// </summary>
		public DateTime? FecElimina { get; set; }

		/// <summary>
		/// Usuario que eliminó el registro.
		/// </summary>
		public string UsuElimina { get; set; }

        /// <summary>
        /// Fecha ingreso del usuario.
        /// </summary>
        public DateTime? FecIngreso { get; set; }

        /// <summary>
        /// Hora ingreso del usuario.
        /// </summary>
        public TimeSpan? HoraIngreso { get; set; }
    }
}