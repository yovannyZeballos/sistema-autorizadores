using Newtonsoft.Json;

namespace SPSA.Autorizadores.Infraestructura.Agente.AgenteCen.Dto
{
    public class InsertarClienteRecurso
    {
		[JsonProperty("tipoDocumento")]
		public string TipoDocumento { get; set; }
		[JsonProperty("numeroDocumento")]
		public string NumeroDocumento { get; set; }
		[JsonProperty("nombres")]
		public string Nombres { get; set; }
		[JsonProperty("apellidos")]
		public string Apellidos { get; set; }
		[JsonProperty("razonSocial")]
		public string RazonSocial { get; set; }
		[JsonProperty("usuarioCreacion")]
		public string UsuarioCreacion { get; set; }
		[JsonProperty("sistema")]
		public string Sistema { get; set; }
	}
}
