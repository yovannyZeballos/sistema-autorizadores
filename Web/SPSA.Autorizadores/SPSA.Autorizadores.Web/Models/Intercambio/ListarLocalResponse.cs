using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarLocalResponse : RespuestaComunDTO
    {
        public List<LocalDTO> Locales { get; set; }
    }
}