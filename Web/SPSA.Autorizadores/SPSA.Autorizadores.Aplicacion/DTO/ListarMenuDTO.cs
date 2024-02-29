
namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarMenuDTO
	{
		public string CodSistema { get; set; }
		public string CodMenu { get; set; }
		public string NomMenu { get; set; }
		public string CodMenuPadre { get; set; }
		public bool IndAsociado { get; set; }
	}
}
