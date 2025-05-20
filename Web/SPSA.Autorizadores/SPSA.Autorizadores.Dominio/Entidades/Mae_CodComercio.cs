using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_CodComercio
    {
        public decimal NroSolicitud { get; set; }
        public int CodLocalAlterno { get; set; }
        public string CodComercio { get; set; }
        public string NomCanalVta { get; set; }
        public string DesOperador { get; set; }
        public string NroCaso { get; set; }
        public DateTime? FecComercio { get; set; }
        public string IndActiva { get; set; }
        public DateTime? FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }
        public long? NomProcesador { get; set; }

        public virtual CCom_SolicitudDet SolicitudDet { get; set; }
    }
}
