using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioTransactionXmlCT2 : CadenasConexion, IRepositorioTransactionXmlCT2
    {
        private readonly int _commandTimeout;
        private readonly DBHelper _dbHelper;

        public RepositorioTransactionXmlCT2(DBHelper dbHelper)
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
            _dbHelper = dbHelper;
        }

        public async Task<TransactionXmlCT2> Obtener(string cadenaConexion, string nombreTabla)
        {
            _dbHelper.CadenaConexion = cadenaConexion;
            SqlParameter[] dbParams = null;
            var dr = await _dbHelper.ExecuteReaderText("SELECT CONVERT(char(12), insertdate, 113) FechaFormato, COUNT(*) Registros " +
                $"FROM dbo.{nombreTabla} WHERE CONVERT(char, insertdate, 111) = CONVERT(char, GETDATE(), 111) " +
                "GROUP BY CONVERT(char(12), insertdate, 113)", dbParams);
            TransactionXmlCT2 transactionXmlCT2 = new TransactionXmlCT2();

            while (await dr.ReadAsync())
            {
                transactionXmlCT2 = new TransactionXmlCT2
                {
                    Cantidad = Convert.ToInt32(dr["Registros"]),
                    FechaFormato = dr["FechaFormato"].ToString(),
                };
            }

            dr.Close();
            return transactionXmlCT2;
        }

        public async Task<TransactionXmlCT2> ObtenerTpsa()
        {

            using (var connection = new OracleConnection(CadenaConexionBCT_TPSA))
            {
                using (var command = new OracleCommand("ADM_TPSA.SF_MONITOR_BCT_TRXS_XMIGR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                })
                {
                    OracleParameter retVal = new OracleParameter("return_value", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(retVal);

                    OracleParameter fechaParam = new OracleParameter("DO_FECHA_TRX", OracleDbType.Date)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(fechaParam);

                    OracleParameter cantParam = new OracleParameter("NO_CANT_TRX", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cantParam);

                    OracleParameter sqlCodeParam = new OracleParameter("NO_SQL_CODE", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(sqlCodeParam);

                    OracleParameter errorParam = new OracleParameter("VO_ERROR", OracleDbType.Varchar2, 4000)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(errorParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    object retValObj = retVal.Value;
                    int returnValue = (retValObj == null || Convert.IsDBNull(retValObj))
                        ? 0
                        : (retValObj is OracleDecimal oracleDec ? oracleDec.ToInt32() : Convert.ToInt32(retValObj));

                    object cantObj = cantParam.Value;

                    string fsdf = cantObj.ToString();

                    int noCantTrx = (cantObj == null || Convert.IsDBNull(cantObj) || cantObj.ToString() == "null")
                        ? 0
                        : (cantObj is OracleDecimal oracleDec2 ? oracleDec2.ToInt32() : Convert.ToInt32(cantObj));

                    object sqlCodeObj = sqlCodeParam.Value;
                    int noSqlCode = (sqlCodeObj == null || Convert.IsDBNull(sqlCodeObj))
                        ? 0
                        : (sqlCodeObj is OracleDecimal oracleDec3 ? oracleDec3.ToInt32() : Convert.ToInt32(sqlCodeObj));

                    string errorMessage = (errorParam.Value == null || Convert.IsDBNull(errorParam.Value))
                                          ? string.Empty
                                          : errorParam.Value.ToString();

                    DateTime fechaTrx = (fechaParam.Value == null || Convert.IsDBNull(fechaParam.Value) || cantObj.ToString() == "null")
                                        ? DateTime.Today
                                        : ((Oracle.ManagedDataAccess.Types.OracleDate)fechaParam.Value).Value;

                    TransactionXmlCT2 transactionXmlCT2 = new TransactionXmlCT2();

                    transactionXmlCT2.Fecha = fechaTrx;
                    transactionXmlCT2.FechaFormato = fechaTrx.ToString("dd MMM yyyy");
                    transactionXmlCT2.FechaStr = fechaTrx.ToString();
                    transactionXmlCT2.Cantidad = noCantTrx;


                    return transactionXmlCT2;
                }
            }

        }


    }
}
