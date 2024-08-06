using System;

namespace SPSA.Autorizadores.Aplicacion.Entities
{

	/// <summary>
	/// Representa una autorizaci�n de impresi�n.
	/// </summary>
	public sealed class AutImpresion
	{
		/// <summary>
		/// Obtiene o establece el identificador de la autorizaci�n.
		/// </summary>
		public long Id { get; private set; }

		/// <summary>
		/// Obtiene o establece el c�digo del colaborador.
		/// </summary>
		public string CodColaborador { get; private set; }

		/// <summary>
		/// Obtiene o establece el c�digo del local.
		/// </summary>
		public string CodLocal { get; private set; }

		/// <summary>
		/// Obtiene o establece el c�digo del autorizador.
		/// </summary>
		public string CodAutorizador { get; private set; }

		/// <summary>
		/// Obtiene el c�digo de barras generado a partir del c�digo del local y del autorizador.
		/// </summary>
		/// 
		public string CodBarra { get; private set; }

		/// <summary>
		/// Obtiene o establece el usuario de impresi�n.
		/// </summary>
		public string UsuImpresion { get; private set; }

		/// <summary>
		/// Obtiene o establece la fecha de impresi�n.
		/// </summary>
		public DateTime FecImpresion { get; private set; }

		/// <summary>
		/// Obtiene o establece el correlativo de la autorizaci�n.
		/// </summary>
		public int Correlativo { get; private set; }

		/// <summary>
		/// Obtiene o establece el motivo de reimpresi�n.
		/// </summary>
		public string MotivoReimpresion { get; private set; }

		/// <summary>
		/// Obtiene o establece el usuario de reimpresi�n.
		/// </summary>
		public string UsuReimpresion { get; private set; }

		/// <summary>
		/// Obtiene o establece la fecha de reimpresi�n.
		/// </summary>
		public DateTime? FecReimpresion { get; private set; }

		/// <summary>
		/// Inicializa una nueva instancia de la clase AutImpresion.
		/// </summary>
		/// <param name="codColaborador">El c�digo del colaborador.</param>
		/// <param name="codLocal">El c�digo del local.</param>
		/// <param name="codAutorizador">El c�digo del autorizador.</param>
		/// <param name="usuImpresion">El usuario de impresi�n.</param>
		/// <exception cref="ArgumentException">Se produce cuando alguno de los par�metros requeridos es nulo o vac�o.</exception>
		public AutImpresion(string codColaborador, string codLocal, string codAutorizador, string usuImpresion)
		{
			if (string.IsNullOrWhiteSpace(codColaborador)) throw new ArgumentException("El c�digo del colaborador es obligatorio.", nameof(codColaborador));
			if (string.IsNullOrWhiteSpace(codLocal)) throw new ArgumentException("El c�digo del local es obligatorio.", nameof(codLocal));
			if (string.IsNullOrWhiteSpace(codAutorizador)) throw new ArgumentException("El c�digo del autorizador es obligatorio.", nameof(codAutorizador));
			if (string.IsNullOrWhiteSpace(usuImpresion)) throw new ArgumentException("El usuario de impresi�n es obligatorio.", nameof(usuImpresion));

			CodColaborador = codColaborador;
			CodLocal = codLocal;
			CodAutorizador = codAutorizador;
			UsuImpresion = usuImpresion;
			FecImpresion = DateTime.Now;
			Correlativo = 1;
			CodBarra = $"{CodLocal}{CodAutorizador}";
		}

		private AutImpresion()
		{
		}

		/// <summary>
		/// Crea una nueva instancia de la clase AutImpresion con los datos actualizados.
		/// </summary>
		/// <param name="id">El identificador de la autorizaci�n.</param>
		/// <param name="codColaborador">El c�digo del colaborador.</param>
		/// <param name="codLocal">El c�digo del local.</param>
		/// <param name="codAutorizador">El c�digo del autorizador.</param>
		/// <param name="usuImpresion">El usuario de impresi�n.</param>
		/// <param name="fecImpresion">La fecha de impresi�n.</param>
		/// <param name="correlativo">El correlativo de la autorizaci�n.</param>
		/// <param name="motivoReimpresion">El motivo de reimpresi�n.</param>
		/// <param name="usuReimpresion">El usuario de reimpresi�n.</param>
		/// <returns>Una nueva instancia de la clase AutImpresion con los datos actualizados.</returns>
		/// <exception cref="ArgumentException">Se produce cuando alguno de los par�metros requeridos es nulo o vac�o.</exception>
		public static AutImpresion Actualizar(long id, string codColaborador, string codLocal, string codAutorizador, string usuImpresion, DateTime fecImpresion, int correlativo, string motivoReimpresion, string usuReimpresion)
		{
			if (id == 0) throw new ArgumentException("El id es obligatorio.", nameof(id));
			if (string.IsNullOrWhiteSpace(codColaborador)) throw new ArgumentException("El c�digo del colaborador es obligatorio.", nameof(codColaborador));
			if (string.IsNullOrWhiteSpace(codLocal)) throw new ArgumentException("El c�digo del local es obligatorio.", nameof(codLocal));
			if (string.IsNullOrWhiteSpace(codAutorizador)) throw new ArgumentException("El c�digo del autorizador es obligatorio.", nameof(codAutorizador));
			if (string.IsNullOrWhiteSpace(usuImpresion)) throw new ArgumentException("El usuario de impresi�n es obligatorio.", nameof(usuImpresion));
			if (fecImpresion == DateTime.MinValue) throw new ArgumentException("La fecha de impresi�n es obligatoria.", nameof(fecImpresion));
			if (correlativo == 0) throw new ArgumentException("El correlativo es obligatorio.", nameof(correlativo));
			if (string.IsNullOrWhiteSpace(motivoReimpresion)) throw new ArgumentException("El motivo de reimpresi�n es obligatorio.", nameof(motivoReimpresion));
			if (string.IsNullOrWhiteSpace(usuReimpresion)) throw new ArgumentException("El usuario de reimpresi�n es obligatorio.", nameof(usuReimpresion));

			return new AutImpresion
			{
				Id = id,
				CodAutorizador = codAutorizador,
				CodColaborador = codColaborador,
				CodLocal = codLocal,
				UsuImpresion = usuImpresion,
				FecImpresion = fecImpresion,
				Correlativo = correlativo + 1,
				MotivoReimpresion = motivoReimpresion,
				UsuReimpresion = usuReimpresion,
				FecReimpresion = DateTime.Now,
				CodBarra = $"{codLocal}{codAutorizador}"
			};
		}
	}
}
