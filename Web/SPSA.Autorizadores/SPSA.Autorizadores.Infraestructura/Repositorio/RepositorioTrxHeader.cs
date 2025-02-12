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
	public class RepositorioTrxHeader : CadenasConexion, IRepositorioTrxHeader
	{
		private readonly int _commandTimeout;

		public RepositorioTrxHeader()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

        public async Task<MonitorTrxInfo> ObtenerCantidadTransacciones(int sucursal, string fecha)
        {
            var resultado = new MonitorTrxInfo();

            using (var connection = new OracleConnection(CadenaConexionBCT))
            using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_BCT_TRXS_XLOCFCH", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                var returnParam = new OracleParameter("return_value", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnParam);

                var dFechaParam = new OracleParameter("D_FECHA", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = fecha
                };
                command.Parameters.Add(dFechaParam);

                var nSucursalParam = new OracleParameter("N_SUCURSAL", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Input,
                    Value = sucursal
                };
                command.Parameters.Add(nSucursalParam);

                // Parámetros de salida:
                var noCantTrxParam = new OracleParameter("NO_CANT_TRX", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(noCantTrxParam);

                var noImpVtaParam = new OracleParameter("NO_IMP_VTA", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(noImpVtaParam);

                var noSqlCodeParam = new OracleParameter("NO_SQL_CODE", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(noSqlCodeParam);

                var voErrorParam = new OracleParameter("VO_ERROR", OracleDbType.Varchar2, 2000)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(voErrorParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                resultado.ReturnValue = Convert.ToInt32(returnParam.Value.ToString());
                resultado.NoCantTrx = noCantTrxParam.Value != DBNull.Value ? Convert.ToInt32(noCantTrxParam.Value.ToString()) : 0;
                resultado.NoImpVta = noImpVtaParam.Value != DBNull.Value ? Convert.ToDecimal(noImpVtaParam.Value.ToString()) : 0m;
                resultado.NoSqlCode = noSqlCodeParam.Value != DBNull.Value ? Convert.ToInt32(noSqlCodeParam.Value.ToString()) : 0;
                resultado.VoError = voErrorParam.Value != DBNull.Value ? voErrorParam.Value.ToString() : string.Empty;
            }

            return resultado;
        }

    }
}
