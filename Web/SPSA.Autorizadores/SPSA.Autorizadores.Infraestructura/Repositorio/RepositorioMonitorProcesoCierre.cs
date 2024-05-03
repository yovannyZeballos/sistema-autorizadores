using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMonitorProcesoCierre : IRepositorioMonitorProcesoCierre
	{
		private readonly int _commandTimeout;

		public RepositorioMonitorProcesoCierre()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task<List<EstadoCierre>> ObtenerDatos(DateTime fechaInicio, DateTime fechaFin, string cadenaConexion)
		{
			var resultado = new List<EstadoCierre>();
			var query = $@"SELECT " +
						  "C.LOC_NUMERO " +
						 ",L.LOC_DESCRIPCION " +
						 ",C.CIE_FCONTABLE " +
						 ",TO_CHAR(C.CIE_FCONTABLE, 'dd') CIE_FCONTABLE_FORMAT " +
						 ",C.CIE_FCIERRE " +
						 ",C.CIE_ESTADO " +
						"FROM CUA_CIERRE C " +
						"LEFT JOIN IRS_LOCALES L " +
						  "ON (C.LOC_NUMERO = L.LOC_NUMERO) " +
						$"WHERE C.CIE_FCONTABLE BETWEEN '{fechaInicio:dd/MM/yyyy}' AND '{fechaFin:dd/MM/yyyy}' " +
						"AND L.loc_numero < 5000 " +
						"ORDER BY C.CIE_FCIERRE";

			using (var connection = new OracleConnection(cadenaConexion))
			{
				var command = new OracleCommand(query, connection)
				{
					CommandType = CommandType.Text,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				//command.Parameters.Add("FECHA_INICIO", OracleDbType.Date, fechaInicio, ParameterDirection.Input);
				//command.Parameters.Add("FECHA_FIN", OracleDbType.Date, fechaCierre, ParameterDirection.Input);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						resultado.Add(new EstadoCierre
						{
							CodLocal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
							DesLocal = dr.IsDBNull(1) ? "" : dr.GetString(1),
							FechaCierreContable = dr.IsDBNull(2) ? (DateTime?)null : dr.GetDateTime(2),
							DiaCierre = dr.IsDBNull(3) ? "" : dr.GetString(3),
							FechaCierre = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4),
							Estado = dr.IsDBNull(5) ? "" : dr.GetString(5)
						});
					}
				}

			}

			return resultado;
		}

	}
}
