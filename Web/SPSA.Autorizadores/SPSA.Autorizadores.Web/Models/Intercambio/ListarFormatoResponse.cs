using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarFormatoResponse : RespuestaComunDTO
    {
        public List<FormatoDTO> Formatos { get; set; }
    }
}