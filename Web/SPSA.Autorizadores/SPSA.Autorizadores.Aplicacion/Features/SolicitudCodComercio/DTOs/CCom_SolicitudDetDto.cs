using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    public class CCom_SolicitudDetDto
    {
        public int NroSolicitud { get; set; }
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string NomLocal { get; set; }
        public string NomEmpresa { get; set; }
        public string TipEstado { get; set; }

        public List<MaeCodComercioDto> Comercios { get; set; }
    }
}
