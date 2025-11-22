using Newtonsoft.Json;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid.Dto;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid
{
	public class AgenteAxteroid : IAgenteAxteroid
	{
		private static readonly string urlBase = ConfigurationManager.AppSettings["UrlServicioAxteroid"];
		private static readonly HttpClient _httpClient = new HttpClient();

		public async Task<ConsultaDocumentoElectronicoRespuesta> ConsultarDocumento(ConsultaDocumentoElectronicoRecurso consultaDocumentoElectronicoRecurso)
		{
			var serie = consultaDocumentoElectronicoRecurso.Folio?.Split('-')[0] ?? "";
			var folio = consultaDocumentoElectronicoRecurso.Folio?.Split('-')[1] ?? "";

			var queryParams = new StringBuilder();
			queryParams.Append($"?type={Uri.EscapeDataString(consultaDocumentoElectronicoRecurso.TipoDoc ?? "")}");
			queryParams.Append($"&series={Uri.EscapeDataString(serie)}");
			queryParams.Append($"&serial={Uri.EscapeDataString(folio)}");
			queryParams.Append("&test=false");
			queryParams.Append("&best=true");

			var url = urlBase + queryParams.ToString();

			using (var request = new HttpRequestMessage(HttpMethod.Get, url))
			{
				request.Headers.Add("x-ax-workspace", consultaDocumentoElectronicoRecurso.Workspace);
				request.Headers.Add("x-ax-tax-id", consultaDocumentoElectronicoRecurso.Ruc);
				request.Headers.Add("Authorization", $"Token {consultaDocumentoElectronicoRecurso.Token}");

				using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
				{
					var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					var consultaDocumentoElectronicoRespuesta = JsonConvert.DeserializeObject<ConsultaDocumentoElectronicoRespuesta>(jsonResponse);
					consultaDocumentoElectronicoRespuesta.Exito = response.IsSuccessStatusCode;
					return consultaDocumentoElectronicoRespuesta;
				}
			}
		}

		public async Task<byte[]> DescargarDocumento(string url)
		{
			using (var response = await _httpClient.GetAsync(url).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode || response.Content.Headers.ContentType?.MediaType != "application/pdf")
				{
					throw new Exception("Error al descargar el documento, el documento noexiste o el contenido no es un PDF.");
				}

				return  await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			}
		}
	}
}
