using System;

namespace SPSA.Autorizadores.Dominio.Contrato.Dto
{
    public class ListarDocumentoElectronicoDto
    {
		public string CodEmpresa { get; set; }
		public string Codlocal { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
		public string TipoDocumento { get; set; }
		public string TipoDocCliente { get; set; }
		public string NroDocCliente { get; set; }
		public string Cajero { get; set; }
		public string Caja { get; set; }
		public string NroTransaccion { get; set; }
		public int NumeroPagina { get; set; }
		public int TamañoPagina { get; set; }
	}
}
