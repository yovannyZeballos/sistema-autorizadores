using System;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class CCom_SolicitudCab
    {
        public int NroSolicitud { get; set; }
        public string TipEstado { get; set; }
        public string RznSocial { get; set; }
        public DateTime? FecSolicitud { get; set; }
        public string UsuSolicitud { get; set; }
        public DateTime? FecRecepcion { get; set; }
        public string UsuRecepcion { get; set; }
        public DateTime? FecRegistro { get; set; }
        public string UsuRegistro { get; set; }
        public DateTime? FecCreacion { get; set; }

        public virtual ICollection<CCom_SolicitudDet> Detalles { get; set; }
    }
}
