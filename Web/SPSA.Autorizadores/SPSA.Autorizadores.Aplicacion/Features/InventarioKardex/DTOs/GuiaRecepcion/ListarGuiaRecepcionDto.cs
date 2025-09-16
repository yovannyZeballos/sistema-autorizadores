using System;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion
{
    public class ListarGuiaRecepcionDto
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string NumGuia { get; set; }
        public string Proveedor { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public int Items { get; set; }                  // Cantidad de líneas del detalle
        public string IndEstado { get; set; }
        public string UsuCreacion { get; set; }

        public virtual List<GuiaRecepcionDetalle> Detalles { get; set; } = new List<GuiaRecepcionDetalle>();
    }
}
