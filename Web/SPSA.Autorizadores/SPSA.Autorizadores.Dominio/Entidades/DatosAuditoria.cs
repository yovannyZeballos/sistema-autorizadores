using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public abstract class DatosAuditoria
	{
		public string UsuarioCreacion { get; private set; }
		public string UsuarioModificacion { get; private set; }
		public DateTime? FechaCreacion { get; private set; }
		public DateTime? FechaModificacion { get; private set; }

		protected DatosAuditoria(string usuarioCreacion, string usuarioModificacion, DateTime? fechaCreacion, DateTime? fechaModificacion)
		{
			UsuarioCreacion = usuarioCreacion;
			UsuarioModificacion = usuarioModificacion;
			FechaCreacion = fechaCreacion;
			FechaModificacion = fechaModificacion;
		}
	}
}
