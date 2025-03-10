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
using System.Text;
using System.Runtime.Remoting;

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
						CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
						UltimaTransf = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
						Diferencia = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
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
							CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
							DesSucursal = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
							Fecha = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
							Horario = $"{(dr.IsDBNull(3) ? string.Empty : dr.GetString(3))} - {(dr.IsDBNull(4) ? string.Empty : dr.GetString(4))}",
							TiempoLim = dr.IsDBNull(5) ? 0 : dr.GetInt32(5)
						});
					}
				}
			}
			return listado;
		}

		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2Tpsa(string fecha, int local)
		{
			var listado = new List<MonitorControlBCT>();

			using (var connection = new OracleConnection(CadenaConexionCT2_TPSA))
			{
				var command = new OracleCommand(" EXCT2CTP.PKG_CT2_SCTRX.SP_HORARIO_SUCURSAL", connection)
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
							CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
							DesSucursal = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
							Fecha = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
							Horario = $"{(dr.IsDBNull(3) ? string.Empty : dr.GetString(3))} - {(dr.IsDBNull(4) ? string.Empty : dr.GetString(4))}",
							TiempoLim = dr.IsDBNull(5) ? 0 : dr.GetInt32(5)
						});
					}
				}
			}
			return listado;
		}

		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTTpsa(string fecha)
		{
			_dbHelper.CadenaConexion = CadenaConexionBCT_TPSA;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@buDate",fecha,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@realDate",fecha,SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var lista = new List<MonitorControlBCT>();

			var query = @"Select a.branchId,
							convert(varchar(8),max(insertDate),108),
							datediff(MINUTE,max(a.insertDate),getDate()) as dif 
							from trxheader a 
							where a.buDate = @buDate AND a.realDate = @realDate
							group by a.branchId";

			using (var dr = await _dbHelper.ExecuteReaderText(query, dbParams))
			{
				while (dr != null && await dr.ReadAsync())
				{
					lista.Add(new MonitorControlBCT
					{
						CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
						UltimaTransf = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
						Diferencia = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
					});
				}
			}
			return lista;
		}

		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2Hpsa(string fecha, int local)
		{
			var listado = new List<MonitorControlBCT>();

			var query = new StringBuilder();
			query.Append("SELECT a.cod_suc_pos AS codloc ");
			query.Append(",a.des_loc_ctx AS desloc ");
			query.Append(", :PINVC_FECHA");
			query.Append(",CASE ");
			query.Append("WHEN nvl(d.nid_feriado, 0) = 0 THEN ");
			query.Append("c.hour_open ");
			query.Append("ELSE ");
			query.Append("b.hour_open ");
			query.Append("END AS hour_open ");
			query.Append(",CASE ");
			query.Append("WHEN nvl(d.nid_feriado, 0) = 0 THEN ");
			query.Append("c.hour_close ");
			query.Append("ELSE ");
			query.Append("b.hour_close ");
			query.Append("END AS hour_close ");
			query.Append(",nvl(a.lim_time, 0) AS lim_time ");
			query.Append("FROM VTASUCCD a ");
			query.Append("LEFT JOIN VTASUDCD b ");
			query.Append("ON a.cod_suc_pos = b.org_lvl_number ");
			query.Append("AND ECTHP.PKG_CTX_MONITOR.FN_REPLACE_ACENTOS(b.cdia) = 'FE' ");
			query.Append("LEFT JOIN VTASUDCD c ");
			query.Append("ON a.cod_suc_pos = c.org_lvl_number ");
			query.Append("AND ECTHP.PKG_CTX_MONITOR.FN_REPLACE_ACENTOS(c.cdia) = ");
			query.Append("ECTHP.PKG_CTX_MONITOR.FN_REPLACE_ACENTOS(substr(to_char(to_date( :PINVC_FECHA, 'yyyyMMdd'), 'DAY', 'NLS_DATE_LANGUAGE=SPANISH'), 1, 2)) ");
			query.Append("LEFT JOIN VTAFERCD d ");
			query.Append("ON (d.canio || d.cmes || d.cdia) = :PINVC_FECHA");
			query.Append(" WHERE a.flg_activo = 1 ");
			query.Append("AND (a.cod_suc_pos = :PINNU_LOCAL");
			query.Append(" OR :PINNU_LOCAL = 99999) ");
			query.Append("ORDER BY a.des_loc_ctx ");

			using (var connection = new OracleConnection(CadenaConexionCT2_HPSA))
			{
				var command = new OracleCommand(query.ToString(), connection)
				{
					CommandType = CommandType.Text,
					CommandTimeout = _commandTimeout
				};

				command.Parameters.Add("PINVC_FECHA", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
				command.Parameters.Add("PINVC_FECHA", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
				command.Parameters.Add("PINVC_FECHA", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
				command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, local, ParameterDirection.Input);
				command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, local, ParameterDirection.Input);

				await connection.OpenAsync();

				using (var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
				{
					while (dr != null && await dr.ReadAsync())
					{
						listado.Add(new MonitorControlBCT
						{
							CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
							DesSucursal = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
							Fecha = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
							Horario = $"{(dr.IsDBNull(3) ? string.Empty : dr.GetString(3))} - {(dr.IsDBNull(4) ? string.Empty : dr.GetString(4))}",
							TiempoLim = dr.IsDBNull(5) ? 0 : dr.GetInt32(5)
						});
					}
				}
			}
			return listado;
		}

		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTHpsa(string fecha)
		{
			_dbHelper.CadenaConexion = CadenaConexionBCT_HPSA;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@buDate",fecha,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@realDate",fecha,SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var lista = new List<MonitorControlBCT>();

			var query = @"Select a.branchId,
							convert(varchar(8),max(insertDate),108),
							datediff(MINUTE,max(a.insertDate),getDate()) as dif 
							from trxheader a 
							where a.buDate = @buDate AND a.realDate = @realDate
							group by a.branchId";

			using (var dr = await _dbHelper.ExecuteReaderText(query, dbParams))
			{
				while (dr != null && await dr.ReadAsync())
				{
					lista.Add(new MonitorControlBCT
					{
						CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
						UltimaTransf = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
						Diferencia = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
					});
				}
			}
			return lista;
		}
	}
}
