using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    public class SolicitudCComercioDetDTO
    {
        public decimal NroSolicitud { get; set; }
        public int CodLocalAlterno { get; set; }
        public string NomLocal { get; set; }
        public string NomEmpresa { get; set; }
        public string TipEstado { get; set; }

        public List<MaeCodComercioDTO> Comercios { get; set; }
    }
}
