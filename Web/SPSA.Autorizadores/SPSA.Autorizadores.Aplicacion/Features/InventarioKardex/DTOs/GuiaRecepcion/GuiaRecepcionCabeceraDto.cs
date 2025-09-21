using System;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion
{
    public class GuiaRecepcionCabeceraDto
    {
        public int Id { get; set; }
        public string NumGuia { get; set; }
        public DateTime Fecha { get; set; }

        public string ProveedorRuc { get; set; }
        public string Proveedor { get; set; }

        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string NomEmpresaOrigen { get; set; }
        public string NomLocalOrigen { get; set; }

        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public string NomEmpresaDestino { get; set; }
        public string NomLocalDestino { get; set; }

        public string AreaGestion { get; set; }
        public string ClaseStock { get; set; }
        public string EstadoStock { get; set; }

        public string Observaciones { get; set; }

        public string IndEstado { get; set; }// BORRADOR | REGISTRADA | ANULADA
        public string IndTransferencia { get; set; }// S | N

        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        public List<GuiaRecepcionDetalleDto> Detalles { get; set; }

        public int Items { get; set; }
    }
}
