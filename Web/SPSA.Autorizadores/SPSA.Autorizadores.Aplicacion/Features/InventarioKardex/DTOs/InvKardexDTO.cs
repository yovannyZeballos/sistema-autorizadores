using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs
{
    public class InvKardexDTO
    {
        public int Id { get; set; }
        public string Kardex { get; set; }
        public DateTime Fecha { get; set; }
        public string Guia { get; set; }
        public string ActivoId { get; set; }
        public string Serie { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Tk { get; set; }
        public int Cantidad { get; set; }
        public string TipoStock { get; set; }
        public string Oc { get; set; }
        public string Sociedad { get; set; }

        public InvKardexActivo InvKardexActivo { get; set; }
    }
}
