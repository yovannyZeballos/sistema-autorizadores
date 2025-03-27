using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.ViewModel
{
    public class InvCajaViewModel
    {
        public InvCajaDTO InvCaja { get; set; }
        public List<InvTipoActivoDTO> TiposActivo { get; set; }

        public List<string> TiposModelo { get; set; }
        public List<string> TiposProcesador { get; set; }
        public List<string> TiposMemoria { get; set; }
        public List<string> TiposSo { get; set; }
        public List<string> TiposVerSo { get; set; }
        public List<string> TiposCapDisco { get; set; }
        public List<string> TiposPuertoBalanza { get; set; }
    }
}
