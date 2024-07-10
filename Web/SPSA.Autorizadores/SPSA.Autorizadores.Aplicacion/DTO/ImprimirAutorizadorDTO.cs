
namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class ImprimirAutorizadorDTO
	{
		public string CodLocal { get; set; }
		public string CodColaborador { get; set; }
		public string CodAutorizador { get; set; }
		public string NomAutorizador { get; set; }
		public string Cargo { get; set; }
		internal bool Imprimir { get; set; } = true;
	}
}
