using System;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    public class CCom_SolicitudCabDto
    {
        public int NroSolicitud { get; set; }
        public string RznSocial { get; set; }
        public string TipEstado { get; set; }
        public DateTime? FecSolicitud { get; set; }
        public string UsuSolicitud { get; set; }
        public DateTime? FecRecepcion { get; set; }
        public string UsuRecepcion { get; set; }
        public DateTime? FecRegistro { get; set; }
        public string UsuRegistro { get; set; }
        public DateTime? FecCreacion { get; set; }

        public List<CCom_SolicitudDetDto> Detalles { get; set; }
    }
}
