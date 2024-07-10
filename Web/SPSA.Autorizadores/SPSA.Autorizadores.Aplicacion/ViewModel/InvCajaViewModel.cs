using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.ViewModel
{
    public class InvCajaViewModel
    {
        public InvCajaDTO InvCaja { get; set; }
        public List<InvTipoActivoDTO> TiposActivo { get; set; }
    }
}
