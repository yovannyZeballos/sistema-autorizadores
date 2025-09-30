using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SrvSistemaOperativo
    {
        public long Id { get; set; }
        public string NomSo { get; set; } = null;
        public string Version { get; set; }
        public string Edition { get; set; }
    }
}
