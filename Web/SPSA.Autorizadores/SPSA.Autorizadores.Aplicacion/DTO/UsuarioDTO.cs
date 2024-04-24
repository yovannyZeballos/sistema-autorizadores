using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class UsuarioDTO : RespuestaComunDTO
	{
		public UsuarioDTO()
		{
			Locales = new List<LocalDTO>();
			Ok = true;
		}
		public AplicacionDTO Aplicacion { get; set; }
		public string CodEmpleado { get; set; }
		public string NombreUsuario { get; set; }
		public List<LocalDTO> Locales { get; set; }
		public List<ListarMenuDTO> MenusAsociados { get; set; }
	}
}
