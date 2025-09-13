using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace SPSA.Autorizadores.Infraestructura.Agente.AgenteCen
{
	public class AgenteCen : IAgenteCen
	{
		private static readonly string urlBase = ConfigurationManager.AppSettings["UrlServicioClienteCen"];
		private static readonly HttpClient httpClient = new HttpClient();
		public async Task<ConsultaClienteRespuesta> ConsultarCliente(ConsultaClienteRecurso recurso)
		{
			var url = $"{urlBase}obtener?TipoDocumento={recurso.TipoDocumento}&NumeroDocumento={recurso.NumeroDocumento}";

			var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Add("Accept", "application/json");

			try
			{
				using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
				{
					if (response.StatusCode == HttpStatusCode.NotFound)
						return null;

					response.EnsureSuccessStatusCode();

					var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					return JsonConvert.DeserializeObject<ConsultaClienteRespuesta>(jsonResponse);
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception($"Error al consultar cliente: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error interno al consultar cliente: {ex.Message}", ex);
			}
		}

		public async Task InsertarCliente(InsertarClienteRecurso recurso)
		{
			var url = $"{urlBase}crear";
			var jsonBody = JsonConvert.SerializeObject(recurso);
			var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

			try
			{
				using (var response = await httpClient.PostAsync(url, content).ConfigureAwait(false))
				{
					response.EnsureSuccessStatusCode();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception($"Error al insertar cliente: {ex.Message}", ex);
			}
		}
	}
}
