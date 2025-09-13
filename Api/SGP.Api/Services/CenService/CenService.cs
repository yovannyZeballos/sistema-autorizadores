using SGP.Api.Controllers.Request;
using SGP.Api.Controllers.Response;

namespace SGP.Api.Services.CenService
{
	public class CenService
	{
		public Task<ConsultaClienteCenResponse?> ObtenreCliente(ConsultaClienteCenRequest request)
		{
			var listaClientes = ObtenerListaClientesMockeada();

			// Buscar cliente por tipo y número de documento
			var cliente = listaClientes.FirstOrDefault(c =>
				c.TipoDocumento == request.TipoDocumento &&
				c.NumeroDocumento == request.NumeroDocumento);

			return Task.FromResult(cliente);
		}

		public async Task InsertarCliente(InsertarClienteCenRequest recurso)
		{
			await Task.CompletedTask;
		}

		private List<ConsultaClienteCenResponse> ObtenerListaClientesMockeada()
		{
			return new List<ConsultaClienteCenResponse>
							{
								new ConsultaClienteCenResponse
								{
									TipoDocumento = "DNI",
									NumeroDocumento = "12345678",
									Nombres = "Juan Carlos",
									Apellidos = "Pérez García",
									RazonSocial = null
								},
								new ConsultaClienteCenResponse
								{
									TipoDocumento = "DNI",
									NumeroDocumento = "87654321",
									Nombres = "María Elena",
									Apellidos = "Rodríguez López",
									RazonSocial = null
								},
								new ConsultaClienteCenResponse
								{
									TipoDocumento = "RUC",
									NumeroDocumento = "20123456789",
									Nombres = null,
									Apellidos = null,
									RazonSocial = "Empresa Comercial SAC"
								},
								new ConsultaClienteCenResponse
								{
									TipoDocumento = "RUC",
									NumeroDocumento = "20987654321",
									Nombres = null,
									Apellidos = null,
									RazonSocial = "Distribuidora del Norte EIRL"
								},
								new ConsultaClienteCenResponse
								{
									TipoDocumento = "CE",
									NumeroDocumento = "001234567",
									Nombres = "Roberto",
									Apellidos = "Silva Montenegro",
									RazonSocial = null
								}
							};
		}
	}
}
