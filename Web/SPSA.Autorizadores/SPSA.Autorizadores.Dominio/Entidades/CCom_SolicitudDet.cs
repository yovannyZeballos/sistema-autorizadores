using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class CCom_SolicitudDet
    {
        public int NroSolicitud { get; set; }
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string TipEstado { get; set; }
        public DateTime? FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }

        [NotMapped]
        public string NomLocal { get; set; }

        [NotMapped]
        public string NomEmpresa { get; set; }

        public virtual CCom_SolicitudCab Cabecera { get; set; }

        public virtual ICollection<Mae_CodComercio> Comercios { get; set; }
    }
}
