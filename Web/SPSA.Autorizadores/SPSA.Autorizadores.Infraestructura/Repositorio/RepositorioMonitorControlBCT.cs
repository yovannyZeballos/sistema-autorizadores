﻿using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Text;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMonitorControlBCT : CadenasConexion, IRepositorioMonitorControlBCT
	{
		private readonly int _commandTimeout;

		public RepositorioMonitorControlBCT()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}
	
		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2Spsa(string fecha, int local)
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

        public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTSpsa(string fecha, int sucursal)
        {
            var listado = new List<MonitorControlBCT>();

            using (var connection = new OracleConnection(CadenaConexionBCT))
            {
                using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_CONTROL_BCT_CT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                })
                {
                    var returnParameter = new OracleParameter("return_value", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnParameter);

                    command.Parameters.Add("p_fecha", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
                    command.Parameters.Add("p_suc", OracleDbType.Int32, sucursal, ParameterDirection.Input);

                    await connection.OpenAsync();

                    using (var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (dr != null && await dr.ReadAsync())
                        {
                            listado.Add(new MonitorControlBCT
                            {
                                CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                UltimaTransf = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                Diferencia = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
                            });
                        }
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

		public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTTpsa(string fecha, int sucursal)
		{
            var listado = new List<MonitorControlBCT>();

            using (var connection = new OracleConnection(CadenaConexionBCT_TPSA))
            {
                using (var command = new OracleCommand("ADM_TPSA.SF_MONITOR_CONTROL_BCT_CT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                })
                {
                    var returnParameter = new OracleParameter("return_value", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnParameter);

                    command.Parameters.Add("p_fecha", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
                    command.Parameters.Add("p_suc", OracleDbType.Int32, sucursal, ParameterDirection.Input);

                    await connection.OpenAsync();

                    using (var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (dr != null && await dr.ReadAsync())
                        {
                            listado.Add(new MonitorControlBCT
                            {
                                CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                UltimaTransf = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                Diferencia = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return listado;
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

        public async Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTHpsa(string fecha, int sucursal)
        {
            var listado = new List<MonitorControlBCT>();

            using (var connection = new OracleConnection(CadenaConexionBCT_HPSA))
            {
                using (var command = new OracleCommand("ADM_HPSA.SF_MONITOR_CONTROL_BCT_CT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                })
                {
                    var returnParameter = new OracleParameter("return_value", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnParameter);

                    command.Parameters.Add("p_fecha", OracleDbType.Varchar2, fecha, ParameterDirection.Input);
                    command.Parameters.Add("p_suc", OracleDbType.Int32, sucursal, ParameterDirection.Input);

                    await connection.OpenAsync();

                    using (var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (dr != null && await dr.ReadAsync())
                        {
                            listado.Add(new MonitorControlBCT
                            {
                                CodSucursal = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                UltimaTransf = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                Diferencia = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return listado;
        }

 	}
}
