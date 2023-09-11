using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarEmpresaResponseDTO : RespuestaComunDTO
	{
		public List<EmpresaDTO> Empresas { get; set; }
	}
}
