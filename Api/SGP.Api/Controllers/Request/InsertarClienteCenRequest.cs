namespace SGP.Api.Controllers.Request
{
	public class InsertarClienteCenRequest
	{
		public string? TipoDocumento { get; set; }
		public string? NumeroDocumento { get; set; }
		public string? Nombres { get; set; }
		public string? Apellidos { get; set; }
		public string? RazonSocial { get; set; }
	}
}
