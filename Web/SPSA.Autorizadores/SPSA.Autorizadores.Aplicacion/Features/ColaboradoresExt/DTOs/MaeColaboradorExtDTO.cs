using System;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs
{
    public class MaeColaboradorExtDTO
    {
        public string UsuAsociado { get; set; }
        public string CodEmpresa { get; set; }
        public string CodLocalAlterno { get; set; }
        public string CodigoOfisis { get; set; }
        public string ApelPaterno { get; set; }
        public string ApelMaterno { get; set; }
        public string NombreTrabajador { get; set; }
        public string TipoDocIdent { get; set; } = "DNI";
        public string NumDocIndent { get; set; }
        public DateTime FechaIngresoEmpresa { get; set; }
        public DateTime FechaCeseTrabajador { get; set; }
        public string TiSitu { get; set; }
        public string PuestoTrabajo { get; set; }
        public string MotiSepa { get; set; }
        public string IndPersonal { get; set; } = "S";
        public string TipoUsuario { get; set; } = "C";
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; } = DateTime.UtcNow;
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
    }
}
