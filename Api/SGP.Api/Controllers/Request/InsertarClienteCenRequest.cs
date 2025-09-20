namespace SGP.Api.Controllers.Request
{
	public class InsertarClienteCenRequest
	{
		public decimal? TipoDocumento { get; set; }
		public decimal? NumeroDocumento { get; set; }
		public string? Nombres { get; set; }
		public string? Apellidos { get; set; }
		public string? RazonSocial { get; set; }

		public string? UsuarioCreacion { get; set; }
		public string? Sistema { get; set; }
	}
}
