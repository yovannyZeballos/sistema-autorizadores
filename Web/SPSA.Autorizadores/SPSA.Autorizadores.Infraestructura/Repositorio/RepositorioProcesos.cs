using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProcesos : CadenasConexion, IRepositorioProcesos
	{
		private readonly int _commandTimeout;

		public RepositorioProcesos()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task<DataTable> ListarGrilla(int opcion, string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin)
		{
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_LISTA_GRILLA", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("OPCION", OracleDbType.Decimal, opcion, ParameterDirection.Input);
				command.Parameters.Add("cCOD_EMPR", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
				command.Parameters.Add("cCOD_LOCAL", OracleDbType.Varchar2, codLocal, ParameterDirection.Input);
				command.Parameters.Add("cFEC_INI", OracleDbType.Date, fechaInicio, ParameterDirection.Input);
				command.Parameters.Add("cFEC_FIN", OracleDbType.Date, fechaFin, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
				var datatable = new DataTable();
				datatable.Load(dr);

				connection.Close();
				connection.Dispose();

				return datatable;

			}
		}

		public async Task<List<ListBox>> ListarListBox(string usuario)
		{
			var listado = new List<ListBox>();
			using (var connection = new OracleConnection(CadenaConexionAutorizadores))
			{
				var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_LISTA_LISTBOX", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add("vCOD_USUARIO", OracleDbType.Varchar2, usuario, ParameterDirection.Input);
				command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						listado.Add(new ListBox
						(
							dr["NOMBRE"].ToString(),
							Convert.ToInt32(dr["OPCION"])
						));
					}
				}
				connection.Close();
				connection.Dispose();
				return listado;
			}
		}
	}
}
