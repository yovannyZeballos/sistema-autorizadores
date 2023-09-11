using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ListarLocalXEmpresaResponseDTO : RespuestaComunDTO
	{
        public List<SovosLocalDTO> Locales { get; set; }
    }
}
