using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class ASR_CajeroPaso
    {
        public string CajCodigo { get; set; }
        public int LocNumero { get; set; }
        public string CajNombre { get; set; }
        public string CajNom { get; set; }
        public string CajApellidos { get; set; }
        public string CajRut { get; set; }
        public string CajTipo { get; set; }
        public string CajTipoContrato { get; set; }
        public string CajTipoDocId { get; set; }
        public string CajCodigoEmp { get; set; }
        public string CajEstado { get; set; }
        public string CajActivo { get; set; }
        public string CajLogin { get; set; }
        public DateTime? CajFcreacion { get; set; }
        public string CajUsuarioCrea { get; set; }
        public DateTime? CajFbaja { get; set; }
        public string CajUsuarioBaja { get; set; }
        public DateTime? Cajfactualiza { get; set; }
        public string CajUsuarioActualiza { get; set; }
        public string CajCorrExtranjero { get; set; }
        public string CajRendAuto { get; set; }
        public string CajCarga { get; set; }
        public int CodPais { get; set; }
        public int CodComercio { get; set; }
    }
}
