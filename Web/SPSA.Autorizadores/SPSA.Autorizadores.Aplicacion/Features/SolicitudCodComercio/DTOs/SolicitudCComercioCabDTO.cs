using System;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    public class SolicitudCComercioCabDTO
    {
        public decimal NroSolicitud { get; set; }
        public long? TipEstado { get; set; }
        public DateTime? FecSolicitud { get; set; }
        public string UsuSolicitud { get; set; }
        public DateTime? FecRecepcion { get; set; }
        public string UsuRecepcion { get; set; }
        public DateTime? FecRegistro { get; set; }
        public string UsuRegistro { get; set; }

        public List<SolicitudCComercioDetDTO> Detalles { get; set; }
    }
}
