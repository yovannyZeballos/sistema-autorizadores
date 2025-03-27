using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.DTOs
{
    public class ObtenerListasInvCajaDTO
    {
        public List<string> TiposModelo { get; set; }
        public List<string> TiposProcesador { get; set; }
        public List<string> TiposMemoria { get; set; }
        public List<string> TiposSo { get; set; }
        public List<string> TiposVerSo { get; set; }
        public List<string> TiposCapDisco { get; set; }
        public List<string> TiposPuertoBalanza { get; set; }
    }
}
