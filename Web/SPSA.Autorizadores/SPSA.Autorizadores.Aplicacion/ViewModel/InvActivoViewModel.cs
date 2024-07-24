using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.ViewModel
{
    public class InvActivoViewModel
    {
        public InvActivoDTO InvActivo { get; set; }
        public List<InvTipoActivoDTO> TiposActivo { get; set; }
    }
}
