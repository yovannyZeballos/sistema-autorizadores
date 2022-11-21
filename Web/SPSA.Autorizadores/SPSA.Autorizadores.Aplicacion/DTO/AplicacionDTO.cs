using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class AplicacionDTO
    {
        public string Version { get; set; }
        public List<PermisoDTO> Permisos { get; set; }
    }
}
