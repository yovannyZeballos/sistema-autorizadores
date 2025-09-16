using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class StockProducto
    {
        public string CodProducto { get; set; } = null;
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public decimal StkDisponible { get; set; }
        public decimal StkReservado { get; set; }
        public decimal StkTransito { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }

    }
}
