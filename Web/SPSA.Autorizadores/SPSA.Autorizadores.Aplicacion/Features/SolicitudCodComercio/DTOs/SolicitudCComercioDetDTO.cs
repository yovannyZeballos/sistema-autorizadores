using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    public class SolicitudCComercioDetDTO
    {
        public decimal NroSolicitud { get; set; }
        public int CodLocalAlterno { get; set; }

        public List<MaeCodComercioDTO> Comercios { get; set; }
    }
}
