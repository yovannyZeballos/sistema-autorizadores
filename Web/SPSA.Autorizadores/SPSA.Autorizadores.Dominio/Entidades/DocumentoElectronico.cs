using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class DocumentoElectronico
    {
		public DateTime Fecha { get; set; }
		public string Caja { get; set; }
		public decimal Importe { get; set; }
		public string DocElectronico { get; set; }
		public string MedioPago { get; set; }
		public string Cajero { get; set; }
		public string TipoDocumento { get; set; }
		public string NroDocumento { get; set; }
		public string Local { get; set; }
	}
}
