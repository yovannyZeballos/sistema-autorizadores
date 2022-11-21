using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarAutorizadorResponse : RespuestaComunDTO
    {
        public List<AutorizadorDTO> Autorizadores { get; set; }
    }
}