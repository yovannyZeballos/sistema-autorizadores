using System.Collections.Generic;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;

namespace SPSA.Autorizadores.Aplicacion.ViewModel
{
    public class MaeColaboradorExtViewModel
    {
        public MaeColaboradorExtDTO ColaboradorExt { get; set; }
        public List<ListarEmpresaDTO> Empresas { get; set; }
        public List<ListarLocalDTO> Locales { get; set; }
    }
}
