using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioCajero : CadenasConexion, IRepositorioCajero
	{
		private readonly int _commandTimeout;

		public RepositorioCajero()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task Crear(Cajero cajero)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_CAJERO_NUEVO", connection);
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = _commandTimeout;

				await command.Connection.OpenAsync();
				command.Parameters.Add("ni_Loc_numero", OracleDbType.Decimal, cajero.CodLocal, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_nombre", OracleDbType.Varchar2, cajero.Nombres, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_apellidos", OracleDbType.Varchar2, cajero.Apeliidos, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_tipo", OracleDbType.Varchar2, cajero.Tipo, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_tipo_contrato", OracleDbType.Varchar2, cajero.TipoContrato, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_rut", OracleDbType.Varchar2, cajero.Rut, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_tipo_docid", OracleDbType.Varchar2, cajero.TipoDocIdentidad, ParameterDirection.Input);
				command.Parameters.Add("vi_Caj_codigo_emp", OracleDbType.Varchar2, cajero.CodigoEmpleado, ParameterDirection.Input);
				command.Parameters.Add("vi_cod_usuario", OracleDbType.Varchar2, cajero.Usuario, ParameterDirection.Input);
				command.Parameters.Add("no_Retorno", OracleDbType.Decimal, 1, ParameterDirection.Output);
				command.Parameters.Add("no_Sqlcode", OracleDbType.Decimal, 1, ParameterDirection.Output);
				command.Parameters.Add("vo_Sqlerrm", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

				await command.ExecuteNonQueryAsync();

				var error = Convert.ToDecimal(command.Parameters["no_Retorno"].Value.ToString());
				var mensjaeError = command.Parameters["vo_Sqlerrm"].Value.ToString();

				if (error < 0)
					throw new Exception(mensjaeError);

				connection.Close();
				connection.Dispose();
			}
		}

		public async Task Eliminar(string nroDocumento, string usuario)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_CAJEROS_ELIMINA", connection);
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = _commandTimeout;

				await command.Connection.OpenAsync();
				command.Parameters.Add("v_CAJ_RUT", OracleDbType.Varchar2, nroDocumento, ParameterDirection.Input);
				command.Parameters.Add("v_USU_ELI", OracleDbType.Varchar2, usuario, ParameterDirection.Input);
				await command.ExecuteNonQueryAsync();
				connection.Close();
				connection.Dispose();
			}
		}

		public async Task<DataTable> ListarCajero(string codigoLocal)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_CAJERO_LISTA", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codigoLocal, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
				var datatable = new DataTable();
				datatable.Load(dr);

				connection.Close();
				connection.Dispose();

				return datatable;
			}
		}

		public async Task<DataTable> ListarColaboradores(string codEmpresa, string codigoLocal)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_CAJERO_LISTA_OFIPLAN", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("vCOD_EMPR", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
				command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codigoLocal, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
				var datatable = new DataTable();
				datatable.Load(dr);

				connection.Close();
				connection.Dispose();

				return datatable;
			}
		}

		public async Task<string> GenerarArchivo(string codigoLocal, string tipoSO)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_CAJERO_GENERA_ARCHIVO", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();

				command.Parameters.Add("NLOC_NUMERO", OracleDbType.Decimal, codigoLocal, ParameterDirection.Input);
				command.Parameters.Add("vTIPO_SO", OracleDbType.Varchar2, tipoSO, ParameterDirection.Input);
				command.Parameters.Add("resultado", OracleDbType.Varchar2, 500, "", ParameterDirection.Output);

				await command.ExecuteNonQueryAsync();

				var resultado = command.Parameters["resultado"].Value.ToString().Replace("null", "");

				connection.Close();
				connection.Dispose();

				return resultado;

			}


		}

		public async Task<DataTable> ReporteDiferenciaCajas(string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_REP_DIF_CAJA", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("VCOD_EMPRESA", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
				command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codLocal, ParameterDirection.Input);
				command.Parameters.Add("VFEC_INI", OracleDbType.Date, fechaInicio, ParameterDirection.Input);
				command.Parameters.Add("VFEC_FIN", OracleDbType.Date, fechaFin, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
				var datatable = new DataTable();
				datatable.Load(dr);

				connection.Close();
				connection.Dispose();

				return datatable;
			}
		}

		public async Task<DataTable> ReporteSobres(string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_SGC_CAJERO.SP_REP_SOBRE", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("VCOD_EMPRESA", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
				command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codLocal, ParameterDirection.Input);
				command.Parameters.Add("VFEC_INI", OracleDbType.Date, fechaInicio, ParameterDirection.Input);
				command.Parameters.Add("VFEC_FIN", OracleDbType.Date, fechaFin, ParameterDirection.Input);
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