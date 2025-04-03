using Oracle.ManagedDataAccess.Client;
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

                // Parámetro de retorno (RETURN NUMBER)
                var returnParam = new OracleParameter("RETURN_VALUE", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnParam);

                // Parámetros de entrada
                command.Parameters.Add("V_FECHA", OracleDbType.Varchar2).Value = fecha;
                command.Parameters.Add("N_SUCURSAL", OracleDbType.Int32).Value = local;

                // Parámetros de salida
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

                // Leer los resultados
                int cantidad = Convert.ToInt32(cantTrxParam.Value);
                decimal monto = Convert.ToDecimal(impVtaParam.Value);
                int returnCode = Convert.ToInt32(returnParam.Value);
                int errorCode = Convert.ToInt32(sqlCodeParam.Value);
                string errorMsg = errorParam.Value?.ToString();

                return (cantidad, monto);
            }

            //{
            //	var command = new SqlCommand(
            //		"select count(1) from trxheader WITH (NOLOCK) where budate = @fecha and branchid = @local and idtransactiontype in (2,10,56) and void = 0 and idpos not in (994,999,995,996); " +
            //		"select sum(T.amount - T.changeAmount + T.amttnfee) as montofinal from trxheader H WITH (NOLOCK) inner join trxtender T WITH (NOLOCK) ON H.trxid = T.trxid where H.budate = @fecha and H.branchid = @local and H.idtransactiontype in (10,56) and void = 0 and H.idpos not in (994,999,995,996);",
            //		connection)
            //	{
            //		CommandType = CommandType.Text,
            //		CommandTimeout = _commandTimeout
            //	};

            //	command.Parameters.AddWithValue("@fecha", fecha);
            //	command.Parameters.AddWithValue("@local", local);

            //	await connection.OpenAsync();

            //	using (var reader = await command.ExecuteReaderAsync())
            //	{
            //		int cantidadTransacciones = 0;
            //		decimal montoFinal = 0;

            //		// Leer el primer resultado
            //		if (await reader.ReadAsync())
            //		{
            //			cantidadTransacciones = reader.GetInt32(0);
            //		}

            //		// Moverse al siguiente conjunto de resultados
            //		if (await reader.NextResultAsync() && await reader.ReadAsync())
            //		{
            //			montoFinal = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
            //		}

            //		return (cantidadTransacciones, montoFinal);
            //	}
            //}
        }
	}
}
