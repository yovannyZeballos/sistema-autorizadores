using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class RespuestaComunExcelDTO : RespuestaComunDTO
    {
        public List<ErroresExcelDTO> Errores { get; set; }
    }
}
