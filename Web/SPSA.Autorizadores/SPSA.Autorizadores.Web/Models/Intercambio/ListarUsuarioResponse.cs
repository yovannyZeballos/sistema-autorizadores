using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarUsuarioResponse : RespuestaComunDTO
    {
        public List<Dictionary<string, object>> Usuarios { get; set; }
        public List<string> Columnas { get; set; }
    }
}