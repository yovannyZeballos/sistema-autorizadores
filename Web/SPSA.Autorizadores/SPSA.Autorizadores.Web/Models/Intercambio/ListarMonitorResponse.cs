using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarMonitorResponse : RespuestaComunDTO
    {
        public List<Dictionary<string, object>> Locales { get; set; }
        public List<string> Columnas { get; set; }
    }
}