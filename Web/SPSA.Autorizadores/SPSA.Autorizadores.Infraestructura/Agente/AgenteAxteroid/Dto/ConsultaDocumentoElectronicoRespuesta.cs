using Newtonsoft.Json;

namespace SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid.Dto
{
    public class ConsultaDocumentoElectronicoRespuesta
    {
		[JsonProperty("pdf_link")]
		public string Pdf { get; set; }

		[JsonProperty("message")]
		public string Mensaje { get; set; }

		public bool Exito { get; set; }
	}
}
