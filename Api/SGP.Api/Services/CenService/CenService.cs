using SGP.Api.Controllers.Request;
using SGP.Api.Controllers.Response;
using Microsoft.Data.SqlClient;

namespace SGP.Api.Services.CenService
{
	public class CenService
	{
		private readonly string _conexion;
		public CenService(IConfiguration configuration)
		{
			_conexion = configuration.GetConnectionString("CEM") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'CEM' not found.");
		}

		public async Task<ConsultaClienteCenResponse?> ObtenreCliente(ConsultaClienteCenRequest request)
		{
			using var connection = new SqlConnection(_conexion);
			using var command = new SqlCommand("SP_Obtener_Datos_Cliente", connection)
			{
				CommandType = System.Data.CommandType.StoredProcedure
			};

			command.Parameters.AddWithValue("@TipoDoc", request.TipoDocumento ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@NumeroDoc", request.NumeroDocumento ?? (object)DBNull.Value);

			await connection.OpenAsync();

			using var reader = await command.ExecuteReaderAsync();
			if (await reader.ReadAsync())
			{
				return new ConsultaClienteCenResponse
				{
					TipoDocumento = Convert.ToString(reader["identificationType"]),
					NumeroDocumento = reader["identificationNumber"] as string,
					Nombres = reader["name"] as string,
					Apellidos = reader["surname"] as string,
					RazonSocial = reader["tradeName"] as string
				};
			}
			return null;
		}

		public async Task<int> InsertarCliente(InsertarClienteCenRequest recurso)
		{
			using var connection = new SqlConnection(_conexion);
			using var command = new SqlCommand("SP_REG_CLIENTE_SP", connection)
			{
				CommandType = System.Data.CommandType.StoredProcedure
			};

			// Parámetros de entrada
			command.Parameters.AddWithValue("@Tipo_documento", recurso.TipoDocumento ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@Nro_documento", recurso.NumeroDocumento ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@Razon_social", recurso.RazonSocial ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@Nombres", recurso.Nombres ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@Apellidos", recurso.Apellidos ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@Sistema", recurso.Sistema ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@Usuario", recurso.UsuarioCreacion ?? (object)DBNull.Value);

			// Parámetro de salida
			var codigoRespuestaParam = new SqlParameter("@Codigo_respuesta", System.Data.SqlDbType.Int)
			{
				Direction = System.Data.ParameterDirection.Output
			};
			command.Parameters.Add(codigoRespuestaParam);

			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();

			// Retornar el valor del parámetro de salida
			return (int)(codigoRespuestaParam.Value ?? 0);  //3 error
		}
	}
}
