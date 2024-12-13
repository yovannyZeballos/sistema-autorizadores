using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioReportes : CadenasConexion, IRepositorioReportes
    {
        private readonly int _commandTimeout;

        public RepositorioReportes()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<DataTable> ListarLocalesCambioPrecio(string codLocal, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                var command = new OracleCommand("PKG_CT2_SCTRX.SP_CONSULTA_CAMBPRECIO_CRB", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add("PINVC_FECINI", OracleDbType.Varchar2, fechaInicio.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINVC_FECFIN", OracleDbType.Varchar2, fechaFin.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, Convert.ToInt32(codLocal), ParameterDirection.Input);
                command.Parameters.Add("IO_CURSOR", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);

                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async Task<DataTable> ListarLocalesNotaCredito(string codLocal, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                var command = new OracleCommand("PKG_CT2_SCTRX.SP_CONSULTA_NOTACREDITO", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add("PINVC_FECINI", OracleDbType.Varchar2, fechaInicio.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINVC_FECFIN", OracleDbType.Varchar2, fechaFin.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, Convert.ToInt32(codLocal), ParameterDirection.Input);
                command.Parameters.Add("IO_CURSOR", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);

                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async Task<DataTable> ListarValesRedimidosAsync(string codLocal, DateTime fechaInicio, DateTime fechaFin, int startRow, int endRow)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                try
                {
                    string query = $@"
            WITH filtered_header AS (
                SELECT HED_FCONTABLE, HED_PAIS, HED_ORIGENTRX, HED_LOCAL, HED_POS, HED_FECHATRX, HED_HORATRX, HED_NUMTRX
                FROM ect2sp.ctx_header_trx
                WHERE HED_FCONTABLE BETWEEN TO_DATE(:fechaInicio, 'DD-MM-YYYY') AND TO_DATE(:fechaFin, 'DD-MM-YYYY')
                    AND HED_TIPOTRX = 'PVT'
                    AND HED_ANULADO = 'N'
                    AND HED_LOCAL = :codLocal
            ),
            filtered_pagos AS (
                SELECT PAG_TIPOPAGO, PAG_MONTO, HED_PAIS, HED_ORIGENTRX, HED_LOCAL, HED_POS, HED_FECHATRX, HED_HORATRX, HED_NUMTRX
                FROM ect2sp.ctx_pagos_trx
                WHERE PAG_FCONTABLE BETWEEN TO_DATE(:fechaInicio, 'DD-MM-YYYY') AND TO_DATE(:fechaFin, 'DD-MM-YYYY')
                    AND PAG_TIPOPAGO IN ('16', '06')
                    AND HED_LOCAL = :codLocal
            )
            SELECT *
            FROM (
                SELECT
                    FP.PAG_TIPOPAGO AS TIPO_PAGO,
                    tp.TPA_DESPAGO AS DES_PAGO,
                    IL.CAD_NUMERO,
                    IC.CAD_DESCRIPCION,
                    FH.HED_LOCAL AS LOC_NUMERO,
                    IL.LOC_DESCRIPCION,
                    FH.HED_POS AS NUM_CAJA,
                    FH.HED_NUMTRX AS NUM_TRX,
                    TO_CHAR(FH.HED_FCONTABLE, 'yyyy-mm-dd') AS FCONTABLE,
                    FP.PAG_MONTO,
                    ROW_NUMBER() OVER (ORDER BY FH.HED_FCONTABLE, FH.HED_NUMTRX) AS ROW_NUM,
                    COUNT(IL.CAD_NUMERO) OVER () AS TOTAL_RECORDS
                FROM
                    filtered_header FH
                JOIN
                    filtered_pagos FP
                    ON FP.HED_PAIS = FH.HED_PAIS
                        AND FP.HED_ORIGENTRX = FH.HED_ORIGENTRX
                        AND FP.HED_LOCAL = FH.HED_LOCAL
                        AND FP.HED_POS = FH.HED_POS
                        AND FP.HED_FECHATRX = FH.HED_FECHATRX
                        AND FP.HED_HORATRX = FH.HED_HORATRX
                        AND FP.HED_NUMTRX = FH.HED_NUMTRX
                JOIN
                    ECT2SP.CUA_TIPOS_PAGO tp
                    ON tp.TPA_CODIGO = FP.PAG_TIPOPAGO
                JOIN
                    IRS_LOCALES IL
                    ON FH.HED_LOCAL = IL.LOC_NUMERO
                JOIN
                    IRS_CADENAS IC
                    ON IL.CAD_NUMERO = IC.CAD_NUMERO
            )
            WHERE ROW_NUM BETWEEN {startRow} AND {endRow}";

                    var command = new OracleCommand(query, connection)
                    {
                        CommandType = CommandType.Text,
                        CommandTimeout = 600
                    };

                    command.Parameters.Add(":fechaInicio", OracleDbType.Varchar2).Value = fechaInicio.ToString("dd-MM-yyyy");
                    command.Parameters.Add(":fechaFin", OracleDbType.Varchar2).Value = fechaFin.ToString("dd-MM-yyyy");
                    command.Parameters.Add(":codLocal", OracleDbType.Int32).Value = Convert.ToInt32(codLocal);
                    //command.Parameters.Add(":startRow", OracleDbType.Int32).Value = startRow;
                    //command.Parameters.Add(":endRow", OracleDbType.Int32).Value = endRow;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        //connection.Close();
                        return dataTable;
                    }
                }
                catch (OracleException ex)
                {
                    // Log error específico de Oracle
                    throw new Exception($"Error en Oracle: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    // Log general
                    throw new Exception($"Error general: {ex.Message}", ex);
                }
            }
        }

    }
}
