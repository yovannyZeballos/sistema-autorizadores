using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMonitorControlBCT : CadenasConexion, IRepositorioMonitorControlBCT
	{
		private readonly int _commandTimeout;
		private readonly DBHelper _dbHelper;

		public RepositorioMonitorControlBCT(DBHelper dbHelper)
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
			_dbHelper = dbHelper;
		}
		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCT(string fecha, int local)
		{
			_dbHelper.CadenaConexion = CadenaConexionBCT;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@p_fecha",fecha,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@p_suc",local,SqlDbType.Int,ParameterDirection.Input)
			};

			var lista = new List<MonitorControlBCT>();

			using (var dr = await _dbHelper.ExecuteReader("SCTRX_SP_CONTROL_BCT_CT2", dbParams))
			{
				while (dr != null && await dr.ReadAsync())
				{
					lista.Add(new MonitorControlBCT
					{
						CodSucursal = dr.GetInt32(0),
						UltimaTransf = dr.GetString(1),
						Diferencia = dr.GetInt32(2),
					});
				}
			}
			return lista;
		}

		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2(string fecha, int local)
		{
			var listado = new List<MonitorControlBCT>();

			using (var connection = new OracleConnection(CadenaConexionCT2))
			{
				var command = new OracleCommand("EXCT2SP.PKG_CT2_SCTRX.SP_HORARIO_SUCURSAL", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				command.Parameters.Add("PINVC_FECHA", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
				command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, local, ParameterDirection.Input);
				command.Parameters.Add("IO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

				await connection.OpenAsync();

				using (var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
				{
					while (dr != null && await dr.ReadAsync())
					{
						listado.Add(new MonitorControlBCT
						{
							CodSucursal = dr.GetInt32(0),
							DesSucursal = dr.GetString(1),
							Fecha = dr.GetString(2),
							Horario = dr.GetString(3),
							TiempoLim = dr.GetInt32(5)
						});
					}
				}
			}
			return listado;
		}
	}
}
