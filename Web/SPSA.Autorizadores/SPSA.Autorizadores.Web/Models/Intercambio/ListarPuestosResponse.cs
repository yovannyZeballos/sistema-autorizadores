using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarPuestosResponse : RespuestaComunDTO
    {
        public List<Dictionary<string, object>> Puestos { get; set; }
        public List<string> Columnas { get; set; }
    }
}