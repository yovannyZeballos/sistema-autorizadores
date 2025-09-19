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
			var soapEnvelope = $@"
				<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ws=""http://ws.online.asp.core.paperless.cl"">
				   <soapenv:Header/>
				   <soapenv:Body>
					  <ws:OnlineRecovery>
						 <ws:ruc>{consultaDocumentoElectronicoRecurso.Ruc}</ws:ruc>
						 <ws:login>{consultaDocumentoElectronicoRecurso.Login}</ws:login>
						 <ws:clave>{consultaDocumentoElectronicoRecurso.Clave}</ws:clave>
						 <ws:tipoDoc>{consultaDocumentoElectronicoRecurso.TipoDoc}</ws:tipoDoc>
						 <ws:folio>{consultaDocumentoElectronicoRecurso.Folio}</ws:folio>
						 <ws:tipoRetorno>{consultaDocumentoElectronicoRecurso.TipoRetorno}</ws:tipoRetorno>
					  </ws:OnlineRecovery>
				   </soapenv:Body>
				</soapenv:Envelope>";

			var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
			content.Headers.Add("SOAPAction", ""); // Si el servicio requiere un SOAPAction, colócalo aquí


			using (var response = await _httpClient.PostAsync(urlBase, content).ConfigureAwait(false))
			{
				var responseXml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				// Parsear el CDATA con el XML de respuesta
				var xdoc = XDocument.Parse(responseXml);
				var cdata = xdoc
					.Descendants()
					.FirstOrDefault(e => e.Name.LocalName == "return")?.Value;

				if (string.IsNullOrEmpty(cdata))
					throw new Exception("No se encontró el bloque CDATA en la respuesta SOAP.");

				var respuestaDoc = XDocument.Parse(cdata);
				var codigo = respuestaDoc.Root.Element("Codigo")?.Value;
				var mensaje = respuestaDoc.Root.Element("Mensaje")?.Value;
				var docId = respuestaDoc.Root.Element("DocId")?.Value;

				return new ConsultaDocumentoElectronicoRespuesta
				{
					Codigo = codigo,
					Mensaje = mensaje,
					DocId = docId
				};
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
