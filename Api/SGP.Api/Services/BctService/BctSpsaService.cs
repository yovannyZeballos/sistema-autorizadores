using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SGP.Api.Services.BctService.DTOs;

namespace SGP.Api.Services.BctService
{
    public class BctSpsaService
    {
        private readonly string _conexionBCT;
        public BctSpsaService(IConfiguration configuration)
        {
            _conexionBCT = configuration.GetConnectionString("BCT_SP") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'BCT_SP' not found.");
        }

        public LocalBctDTO ObtenerLocal(string rucEmpresa, int codLocal)
        {
            LocalBctDTO local = null;

            string query = @"SELECT ID, CODIGO, DESCRIPCION, IP, USUARIOBD, PASSWORDBD, LINUX, RUC_EMISOR
                                 FROM ADM_SPSA.SUCURSALES
                                 WHERE RUC_EMISOR = :ruc_emisor AND CODIGO = :codigo";

            using (OracleConnection connection = new OracleConnection(_conexionBCT))
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    try
                    {
                        command.Parameters.Add(new OracleParameter("ruc_emisor", rucEmpresa));
                        command.Parameters.Add(new OracleParameter("codigo", codLocal));

                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Solo lee si hay registros
                            {
                                local = new LocalBctDTO
                                {
                                    Codigo = reader["CODIGO"] != DBNull.Value ? Convert.ToInt32(reader["CODIGO"]) : 0,
                                    Descripcion = reader["DESCRIPCION"] != DBNull.Value ? reader["DESCRIPCION"].ToString() : null,
                                    Ip = reader["IP"] != DBNull.Value ? reader["IP"].ToString() : null,
                                    RucEmisor = reader["RUC_EMISOR"] != DBNull.Value ? reader["RUC_EMISOR"].ToString() : null
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al obtener local::" + ex.Message);
                    }
                }
            }

            return local;
        }

        public string CrearLocal(LocalBctDTO local)
        {
            using (OracleConnection connection = new OracleConnection(_conexionBCT))
            {
                try
                {
                    connection.Open();

                    string query = @"INSERT INTO ADM_SPSA.SUCURSALES 
                                        (CODIGO, DESCRIPCION, IP, USUARIOBD, PASSWORDBD, LINUX, RUC_EMISOR) 
                                        VALUES 
                                        (:codigo, :descripcion, :ip, 'mtxadmin', 'adminmtx', 0, :ruc_emisor)";


                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("codigo", local.Codigo));
                        command.Parameters.Add(new OracleParameter("descripcion", local.Descripcion));
                        command.Parameters.Add(new OracleParameter("ip", local.Ip));
                        command.Parameters.Add(new OracleParameter("ruc_emisor", local.RucEmisor));

                        command.ExecuteNonQuery();

                        return "Inserción exitosa.";
                    }
                }
                catch (Exception ex)
                {
                    return "Error al crear local::" + ex.Message;
                }
            }
        }

        public string ActualizarLocal(LocalBctDTO local)
        {
            using (OracleConnection connection = new OracleConnection(_conexionBCT))
            {
                try
                {
                    connection.Open();

                    string query = @"UPDATE ADM_SPSA.SUCURSALES
                                         SET DESCRIPCION = :descripcion, 
                                             IP = :ip
                                         WHERE CODIGO = :codigo 
                                           AND RUC_EMISOR = :ruc_emisor";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        // Agregar parámetros con OracleParameter para evitar problemas de tipo
                        command.Parameters.Add(new OracleParameter("descripcion", local.Descripcion));
                        command.Parameters.Add(new OracleParameter("ip", local.Ip));
                        command.Parameters.Add(new OracleParameter("codigo", local.Codigo));
                        command.Parameters.Add(new OracleParameter("ruc_emisor", local.RucEmisor));

                        // Ejecutar la actualización
                        int filasAfectadas = command.ExecuteNonQuery();

                        return filasAfectadas > 0 ? "Actualización exitosa." : "No se encontró el registro para actualizar.";

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar local::" + ex.Message);
                }
            }
        }

        public async Task<MonitorBctDTO> ObtenerMonitorBct()
        {
            using (var connection = new OracleConnection(_conexionBCT))
            {
                using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_BCT_TRXS_XMIGR_CT3", connection)
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

        public async Task<MonitorBctDTO> ObtenerMonitorBctCt3()
        {
            using (var connection = new OracleConnection(_conexionBCT))
            {
                //    string query = @"
                //SELECT TRUNC(insertdate) AS DO_FECHA_TRX,
                //       COUNT(*) AS NO_CANT_TRX
                //FROM ADM_SPSA.TransactionXmlCT3
                //WHERE TRUNC(insertdate) = TRUNC(SYSDATE)
                //GROUP BY TRUNC(insertdate)";
                string query = @"
                    SELECT TRUNC(insertdate) AS DO_FECHA_TRX,
                           COUNT(1) AS NO_CANT_TRX
                    FROM ADM_SPSA.TransactionXmlCT3
                    WHERE TRUNC(insertdate) = TRUNC(SYSDATE)
                    GROUP BY TRUNC(insertdate)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DateTime fechaTrx = reader.IsDBNull(0)
                                ? DateTime.Today
                                : reader.GetDateTime(0);

                            int noCantTrx = reader.IsDBNull(1)
                                ? 0
                                : reader.GetInt32(1);

                            return new MonitorBctDTO
                            {
                                Fecha = fechaTrx,
                                //FechaFormato = fechaTrx.ToString("dd MMM yyyy"),
                                //FechaStr = fechaTrx.ToString(),
                                Cantidad = noCantTrx
                            };
                        }
                        else
                        {
                            // No hay registros hoy
                            return new MonitorBctDTO
                            {
                                FechaHora = DateTime.Today,
                                //FechaFormato = DateTime.Today.ToString("dd MMM yyyy"),
                                //FechaStr = DateTime.Today.ToString(),
                                Cantidad = 0
                            };
                        }
                    }
                }
            }
        }
    }
}
