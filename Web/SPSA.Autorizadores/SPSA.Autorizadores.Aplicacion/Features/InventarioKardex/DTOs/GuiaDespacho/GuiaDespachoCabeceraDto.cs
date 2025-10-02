using System;
using System.Collections.Generic;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho
{
    public class GuiaDespachoCabeceraDto
    {
        public long Id { get; set; }
        public string NumGuia { get; set; }
        public DateTime Fecha { get; set; }

        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }

        public string NomLocalOrigen { get; set; }
        public string NomLocalDestino { get; set; }

        public string AreaGestion { get; set; }
        public string ClaseStock { get; set; }
        public string EstadoStock { get; set; }
        public string Observaciones { get; set; }


        public string IndEstado { get; set; }
        public string TipoMovimiento { get; set; }
        public bool UsarTransitoDestino { get; set; }

        public string IndConfirmacion { get; set; } 
        public DateTime? FecConfirmacion { get; set; }

        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        public List<GuiaDespachoDetalleDto> Detalles { get; set; }
        public int Items { get; set; }
    }
}
