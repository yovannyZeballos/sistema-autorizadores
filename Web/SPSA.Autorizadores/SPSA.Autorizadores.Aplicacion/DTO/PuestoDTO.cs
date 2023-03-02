using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class PuestoDTO : BaseDTO
    {
        public List<Dictionary<string, object>> Puestos { get; set; }
    }
}
