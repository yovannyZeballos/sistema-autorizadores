using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class RespuestaComunExcelDTO : RespuestaComunDTO
    {
        public string Archivo { get; set; } // Base64 del Excel
        public string NombreArchivo { get; set; }
        public List<ErroresExcelDTO> Errores { get; set; }
    }
}
