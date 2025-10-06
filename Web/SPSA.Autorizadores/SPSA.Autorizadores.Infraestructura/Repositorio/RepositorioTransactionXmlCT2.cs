using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioTransactionXmlCT2 : CadenasConexion, IRepositorioTransactionXmlCT2
    {
        private readonly int _commandTimeout;

        public RepositorioTransactionXmlCT2()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<TransactionXmlCT2> ObtenerSpsa()
        {
            using (var connection = new OracleConnection(CadenaConexionBCT))
            {
                using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_BCT_TRXS_XMIGR", connection)
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

        public async Task<TransactionXmlCT2> ObtenerHpsa()
        {

            using (var connection = new OracleConnection(CadenaConexionBCT_HPSA))
            {
                using (var command = new OracleCommand("ADM_HPSA.SF_MONITOR_BCT_TRXS_XMIGR", connection)
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

        public async Task<TransactionXmlCT2> ObtenerSpsaCt3()
        {
            using (var connection = new OracleConnection(CadenaConexionBCT))
            {
                using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_BCT_TRXS_XMIGR_CT3", connection)
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

        //public async Task<TransactionXmlCT2> ObtenerSpsaCt3()
        //{
        //    using (var connection = new OracleConnection(CadenaConexionBCT))
        //    {
        //    //    string query = @"
        //    //SELECT TRUNC(insertdate) AS DO_FECHA_TRX,
        //    //       COUNT(*) AS NO_CANT_TRX
        //    //FROM ADM_SPSA.TransactionXmlCT3
        //    //WHERE TRUNC(insertdate) = TRUNC(SYSDATE)
        //    //GROUP BY TRUNC(insertdate)";

        //        string query = @"
        //                SELECT TRUNC(insertdate) AS DO_FECHA_TRX,
        //                       COUNT(1) AS NO_CANT_TRX
        //                FROM ADM_SPSA.TransactionXmlCT3
        //                WHERE TRUNC(insertdate) = TRUNC(SYSDATE)
        //                GROUP BY TRUNC(insertdate)";

        //        using (var command = new OracleCommand(query, connection))
        //        {
        //            command.CommandType = CommandType.Text;
        //            command.CommandTimeout = _commandTimeout;

        //            await connection.OpenAsync();

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    DateTime fechaTrx = reader.IsDBNull(0)
        //                        ? DateTime.Today
        //                        : reader.GetDateTime(0);

        //                    int noCantTrx = reader.IsDBNull(1)
        //                        ? 0
        //                        : reader.GetInt32(1);

        //                    return new TransactionXmlCT2
        //                    {
        //                        Fecha = fechaTrx,
        //                        FechaFormato = fechaTrx.ToString("dd MMM yyyy"),
        //                        FechaStr = fechaTrx.ToString(),
        //                        Cantidad = noCantTrx
        //                    };
        //                }
        //                else
        //                {
        //                    // No hay registros hoy
        //                    return new TransactionXmlCT2
        //                    {
        //                        Fecha = DateTime.Today,
        //                        FechaFormato = DateTime.Today.ToString("dd MMM yyyy"),
        //                        FechaStr = DateTime.Today.ToString(),
        //                        Cantidad = 0
        //                    };
        //                }
        //            }
        //        }
        //    }
        //}

    }
}
