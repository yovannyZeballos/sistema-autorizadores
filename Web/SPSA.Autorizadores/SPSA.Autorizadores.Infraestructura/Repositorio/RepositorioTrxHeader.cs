using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
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

		public async Task<(int cantidadTransacciones, decimal montoFinal)> ObtenerCantidadTransacciones(int local, string fecha)
		{
            using (var connection = new OracleConnection(CadenaConexionBCT))

            using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_BCT_TRXS_XLOCFCH", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                var returnParam = new OracleParameter("RETURN_VALUE", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnParam);

                command.Parameters.Add("V_FECHA", OracleDbType.Varchar2).Value = fecha;
                command.Parameters.Add("N_SUCURSAL", OracleDbType.Int32).Value = local;

                var cantTrxParam = new OracleParameter("NO_CANT_TRX", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cantTrxParam);

                var impVtaParam = new OracleParameter("NO_IMP_VTA", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(impVtaParam);

                var sqlCodeParam = new OracleParameter("NO_SQL_CODE", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(sqlCodeParam);

                var errorParam = new OracleParameter("VO_ERROR", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(errorParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                int cantidad = ((OracleDecimal)cantTrxParam.Value).ToInt32();
                decimal monto = ((OracleDecimal)impVtaParam.Value).Value;
                int returnCode = ((OracleDecimal)returnParam.Value).ToInt32();
                int errorCode = ((OracleDecimal)sqlCodeParam.Value).ToInt32();
                string errorMsg = errorParam.Value?.ToString();

                return (cantidad, monto);
            }

        }
	}
}
