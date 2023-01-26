using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarColaboradoresCesadosResponse : RespuestaComunDTO
    {
        public List<Dictionary<string, object>> Colaboradores { get; set; }
        public List<string> Columnas { get; set; }
    }
}