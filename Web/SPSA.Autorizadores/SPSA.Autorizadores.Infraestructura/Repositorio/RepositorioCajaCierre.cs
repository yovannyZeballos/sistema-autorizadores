using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioCajaCierre : CadenasConexion, IRepositorioCajaCierre
	{
		private readonly int _commandTimeout;

		public RepositorioCajaCierre()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}


		public async Task<DataTable> Listar(string codEmpresa, DateTime fechaInicio, DateTime fechafin)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MONITOR_CIERRE", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("CCOD_EMPRE", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
				command.Parameters.Add("cFEC_INI", OracleDbType.Date, fechaInicio, ParameterDirection.Input);
				command.Parameters.Add("cFEC_FIN", OracleDbType.Date, fechafin, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
				var datatable = new DataTable();
				datatable.Load(dr);


				connection.Close();
				connection.Dispose();

				return datatable;
			}
		}

		public async Task<DataTable> ListarResumen(string codSociedad, int opcion, DateTime fechaInicio, DateTime fechafin)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MONITOR_CIERRE_RESU", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("cSociedad", OracleDbType.Varchar2, codSociedad, ParameterDirection.Input);
				command.Parameters.Add("cFEC_INI", OracleDbType.Date, fechaInicio, ParameterDirection.Input);
				command.Parameters.Add("cFEC_FIN", OracleDbType.Date, fechafin, ParameterDirection.Input);
				command.Parameters.Add("cOpc", OracleDbType.Decimal, opcion, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
				var datatable = new DataTable();
				datatable.Load(dr);


				connection.Close();
				connection.Dispose();

				return datatable;
			}
		}
	}
}
