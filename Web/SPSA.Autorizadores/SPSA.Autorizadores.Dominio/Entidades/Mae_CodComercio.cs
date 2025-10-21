using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_CodComercio
    {
       
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodComercio { get; set; }
        public string NomCanalVta { get; set; }
        public string DesOperador { get; set; }
        public string NroCaso { get; set; }
        public DateTime? FecComercio { get; set; }
        public int NroSolicitud { get; set; }
        public string IndEstado { get; set; }
        public DateTime? FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }

        public virtual CCom_SolicitudDet SolicitudDet { get; set; }
    }
}
