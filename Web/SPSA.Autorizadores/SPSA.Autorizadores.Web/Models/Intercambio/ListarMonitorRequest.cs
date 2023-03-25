using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarMonitorRequest
    {
        public string CodEmpresa { get; set; }
        public string Fecha { get; set; }
        public string Estado { get; set; }
    }
}