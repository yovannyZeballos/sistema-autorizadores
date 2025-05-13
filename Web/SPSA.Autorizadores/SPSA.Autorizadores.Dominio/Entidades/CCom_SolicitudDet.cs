using System;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class CCom_SolicitudDet
    {
        public decimal NroSolicitud { get; set; }
        public int CodLocalAlterno { get; set; }
        public DateTime? FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }

        public virtual CCom_SolicitudCab Cabecera { get; set; }

        public virtual ICollection<Mae_CodComercio> Comercios { get; set; }
    }
}
