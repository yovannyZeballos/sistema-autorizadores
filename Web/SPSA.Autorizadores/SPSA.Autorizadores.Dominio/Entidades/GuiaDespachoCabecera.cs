using System;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class GuiaDespachoCabecera
    {
        public long Id { get; set; }
        public string NumGuia { get; set; }
        public DateTime Fecha { get; set; }

        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }

        public string AreaGestion { get; set; }
        public string ClaseStock { get; set; }
        public string EstadoStock { get; set; }
        public string Observaciones { get; set; }

        // 'BORRADOR','REGISTRADA','ANULADA'
        public string IndEstado { get; set; } = "REGISTRADA";
        public string TipoMovimiento { get; set; } = "TRANSFERENCIA";
        public bool UsarTransitoDestino { get; set; }

        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        public virtual ICollection<GuiaDespachoDetalle> Detalles { get; set; } = new List<GuiaDespachoDetalle>();
    }
}
