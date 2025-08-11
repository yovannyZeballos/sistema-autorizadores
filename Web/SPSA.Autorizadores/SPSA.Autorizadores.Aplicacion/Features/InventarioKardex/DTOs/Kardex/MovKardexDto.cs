using System;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Kardex
{
    public class MovKardexDto
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; }

        public long SerieProductoId { get; set; }
        public long AreaGestionId { get; set; }

        public string NumGuia { get; set; }
        public string CodActivo { get; set; }

        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }

        public string Observaciones { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
    }
}
