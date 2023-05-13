using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ListarCajasPorLocalDTO : RespuestaComunDTO
    {
        public List<Dictionary<string, object>> Cajas { get; set; }
        public List<string> Columnas { get; set; }
    }
}
