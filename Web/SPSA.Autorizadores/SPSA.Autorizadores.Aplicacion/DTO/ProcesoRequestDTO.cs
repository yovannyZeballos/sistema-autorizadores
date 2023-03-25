using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ProcesoRequestDTO
    {
        public string CodLocal { get; private set; }
        public string Ip { get; private set; }
        public string Estado { get; private set; }
    }

}
