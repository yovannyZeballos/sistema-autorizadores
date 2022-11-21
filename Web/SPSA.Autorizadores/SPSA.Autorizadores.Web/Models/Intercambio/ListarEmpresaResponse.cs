using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Web.Models.Intercambio
{
    public class ListarEmpresaResponse : RespuestaComunDTO
    {
        public List<EmpresaDTO> Empresas { get; set; }
    }
}