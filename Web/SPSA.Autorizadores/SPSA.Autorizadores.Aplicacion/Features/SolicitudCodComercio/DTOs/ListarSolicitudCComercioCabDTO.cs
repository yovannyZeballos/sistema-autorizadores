using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs
{
    internal class ListarSolicitudCComercioCabDTO
    {
        public decimal NroSolicitud { get; set; }
        public long? TipEstado { get; set; }
        public DateTime? FecSolicitud { get; set; }
    }
}
