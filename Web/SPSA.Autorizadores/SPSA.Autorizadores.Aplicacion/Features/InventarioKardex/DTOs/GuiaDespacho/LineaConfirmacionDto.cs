using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho
{
    public class LineaConfirmacionDto
    {
        public long DespachoDetalleId { get; set; }
        public string CodProducto { get; set; }
        public string NumSerie { get; set; }   // requerido si serializable
        public decimal Cantidad { get; set; }  // 1 si serializable; >0 si no serializable
    }
}
