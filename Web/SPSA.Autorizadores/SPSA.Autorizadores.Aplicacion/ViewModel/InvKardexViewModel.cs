using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.ViewModel
{
    public class InvKardexViewModel
    {
        public InvKardexDTO InvKardex { get; set; }
        public List<InvKardexActivoDTO> Activos { get; set; }
        public List<InvKardexLocalDTO> Locales { get; set; }
    }
}
