
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class LocalesAsignadosDTO : BaseDTO
    {
        public List<Dictionary<string, object>> Locales { get; set; }
    }
}
