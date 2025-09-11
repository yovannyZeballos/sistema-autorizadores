using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SGP.Api.Services.BctService.DTOs;
using System.Data;

namespace SGP.Api.Services.BctService
{
    public class BctTpsaService
    {
        private readonly string _conexionBCT;
        public BctTpsaService(IConfiguration configuration)
        {
            _conexionBCT = configuration.GetConnectionString("BCT_TP") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'BCT_TP' not found.");
        }

        public async Task<MonitorBctDTO> ObtenerMonitorBct()
        {
            using (var connection = new OracleConnection(_conexionBCT))
            {
                using (var command = new OracleCommand("ADM_TPSA.SF_MONITOR_BCT_TRXS_XMIGR", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    OracleParameter retVal = new("return_value", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(retVal);

                    OracleParameter fechaParam = new("DO_FECHA_TRX", OracleDbType.Date)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(fechaParam);

                    OracleParameter cantParam = new("NO_CANT_TRX", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cantParam);

                    OracleParameter sqlCodeParam = new("NO_SQL_CODE", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(sqlCodeParam);

                    OracleParameter errorParam = new("VO_ERROR", OracleDbType.Varchar2, 4000)
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

                    int noCantTrx = (cantObj == null || Convert.IsDBNull(cantObj) || cantObj.ToString() == "null")
                        ? 0
                        : (cantObj is OracleDecimal oracleDec2 ? oracleDec2.ToInt32() : Convert.ToInt32(cantObj));

                    object sqlCodeObj = sqlCodeParam.Value;
                    int noSqlCode = (sqlCodeObj == null || Convert.IsDBNull(sqlCodeObj))
                        ? 0
                        : (sqlCodeObj is OracleDecimal oracleDec3 ? oracleDec3.ToInt32() : Convert.ToInt32(sqlCodeObj));

                    string? errorMessage = (errorParam.Value == null || Convert.IsDBNull(errorParam.Value))
                                          ? string.Empty
                                          : errorParam.Value.ToString();

                    DateTime fechaTrx = (fechaParam.Value == null || Convert.IsDBNull(fechaParam.Value) || cantObj.ToString() == "null")
                    ? DateTime.Today
                    : ((OracleDate)fechaParam.Value).Value;

                    MonitorBctDTO monitorBct = new()
                    {
                        FechaHora = fechaTrx,
                        //Fecha = fechaTrx,
                        //Hora = fechaTrx.ToString("HH"),
                        //Minuto = fechaTrx.ToString("mm"),
                        Cantidad = noCantTrx
                    };


                    return monitorBct;
                }
            }
        }

    }
}
