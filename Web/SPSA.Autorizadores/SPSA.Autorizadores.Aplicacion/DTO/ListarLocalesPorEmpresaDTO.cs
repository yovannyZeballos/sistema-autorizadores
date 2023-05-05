using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ListarLocalesPorEmpresaDTO : RespuestaComunDTO
    {
        public List<Dictionary<string, object>> Locales { get; set; }
        public List<string> Columnas { get; set; }
    }
}
