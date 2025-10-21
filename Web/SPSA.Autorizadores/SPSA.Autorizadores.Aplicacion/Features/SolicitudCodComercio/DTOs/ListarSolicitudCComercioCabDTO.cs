using System;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    internal class ListarSolicitudCComercioCabDTO
    {
        public int NroSolicitud { get; set; }
        public string TipEstado { get; set; }
        public DateTime? FecSolicitud { get; set; }
    }
}
