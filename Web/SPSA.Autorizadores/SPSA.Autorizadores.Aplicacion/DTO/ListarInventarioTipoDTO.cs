using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarInventarioTipoDTO : RespuestaComunDTO
	{
        public List<InventarioTipoDTO> Tipos { get; set; }
    }
}
