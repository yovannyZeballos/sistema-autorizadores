using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_ColaboradorExt
    {     
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CodigoOfisis { get; set; }
        public string ApelPaterno { get; set; }
        public string ApelMaterno { get; set; }
        public string NombreTrabajador { get; set; }
        public string TipoDocIdent { get; set; }
        public string NumDocIndent { get; set; }
        public DateTime FechaIngresoEmpresa { get; set; }
        public DateTime? FechaCeseTrabajador { get; set; }
        public string IndActivo { get; set; }
        public string PuestoTrabajo { get; set; }
        public string MotiSepa { get; set; }
        public string IndPersonal { get; set; }
        public string TipoUsuario { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
    }
}
