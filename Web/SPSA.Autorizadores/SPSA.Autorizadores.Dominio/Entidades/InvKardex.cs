using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class InvKardex
    {
        public int Id { get; set; }
        public string Kardex { get; set; }
        public DateTime Fecha { get; set; }
        public string Guia { get; set; }
        public string ActivoId { get; set; }
        public string Serie { get; set; }
        public string OrigenId { get; set; }
        public string DestinoId { get; set; }
        public string Tk { get; set; }
        public int Cantidad { get; set; }
        public string TipoStock { get; set; }
        public string Oc { get; set; }
        public string Sociedad { get; set; }

        [ForeignKey("ActivoId")]
        public virtual InvKardexActivo InvKardexActivo { get; set; }

        [ForeignKey("OrigenId")]
        public virtual InvKardexLocal Origen { get; set; }  // Relación para OrigenId

        [ForeignKey("DestinoId")]
        public virtual InvKardexLocal Destino { get; set; }  // Relación para DestinoId
    }
}
