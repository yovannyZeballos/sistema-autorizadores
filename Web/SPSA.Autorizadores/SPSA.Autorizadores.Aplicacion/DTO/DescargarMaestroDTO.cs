using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class DescargarMaestroDTO : RespuestaComunDTO
    {
        public string Archivo { get; set; }
        public string NombreArchivo { get; set; }
    }
}
