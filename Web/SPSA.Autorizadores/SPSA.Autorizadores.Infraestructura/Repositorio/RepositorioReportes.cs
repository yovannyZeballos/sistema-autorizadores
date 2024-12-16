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

        public async Task<DataTable> ListarValesRedimidosAsync(string codLocal, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
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
                                SELECT
	                                p.PAG_TIPOPAGO,
	                                tp.TPA_DESPAGO,
	                                IL.CAD_NUMERO,
	                                IC.CAD_DESCRIPCION,
	                                t.HED_LOCAL,
	                                IL.LOC_DESCRIPCION,
	                                t.HED_POS,
	                                t.HED_NUMTRX,
	                                TO_CHAR(t.HED_FCONTABLE, 'yyyy-mm-dd') AS HED_FCONTABLE,
	                                p.PAG_MONTO
                                FROM
	                                filtered_header t
                                JOIN
                                    filtered_pagos p
                                    ON
	                                p.HED_PAIS = t.HED_PAIS
	                                AND p.HED_ORIGENTRX = t.HED_ORIGENTRX
	                                AND p.HED_LOCAL = t.HED_LOCAL
	                                AND p.HED_POS = t.HED_POS
	                                AND p.HED_FECHATRX = t.HED_FECHATRX
	                                AND p.HED_HORATRX = t.HED_HORATRX
	                                AND p.HED_NUMTRX = t.HED_NUMTRX
                                JOIN
                                    ECT2SP.CUA_TIPOS_PAGO tp
                                    ON 
                                    tp.TPA_CODIGO = p.PAG_TIPOPAGO
                                JOIN
                                    IRS_LOCALES IL
                                    ON t.HED_LOCAL = IL.LOC_NUMERO
                                JOIN
                                    IRS_CADENAS IC
                                    ON IL.CAD_NUMERO = IC.CAD_NUMERO
                                ";

                var command = new OracleCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 600
                };

                command.Parameters.Add(":fechaInicio", OracleDbType.Varchar2).Value = fechaInicio.ToString("dd-MM-yyyy");
                command.Parameters.Add(":fechaFin", OracleDbType.Varchar2).Value = fechaFin.ToString("dd-MM-yyyy");
                command.Parameters.Add(":codLocal", OracleDbType.Int32).Value = Convert.ToInt32(codLocal);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }

        public async Task<DataTable> ListarValesRedimidosPaginadoAsync(string codLocal, DateTime fechaInicio, DateTime fechaFin, int startRow, int endRow)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
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
                    return dataTable;
                }
            }
        }

        public async Task<DataTable> ListarAutorizadoresAsync()
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                string query = @"
                                SELECT
                                    E.EMP_CODIGO AS ""COD. COLABORADOR"",
                                    E.EMP_APEPAT || ' ' || E.EMP_APEMAT || ' , ' ||E.EMP_NOMBRE AS ""NOM. AUTORIZADOR"",
                                    E.EMP_NUMDOC AS ""NUM. DOCUMENTO"",
                                    A.AUT_CODAUTORIZA AS ""COD. AUTORIZADOR"",
                                    DECODE(A.AUT_ESTAUT, 'A', 'A', 'I') AS ""EST. AUTORIZADOR"",
                                    A.LOC_NUMERO AS ""COD LOCAL"",
                                    A.AUT_FECCREA AS ""FEC. CREACIÓN"",
                                    A.AUT_USRCREA AS ""USU. CREACIÓN"",
                                    DECODE(A.AUT_ESTAUT , 'A', NULL, A.AUT_FECMOD) AS ""FEC. BAJA"",
                                    DECODE(A.AUT_ESTAUT , 'A', NULL, A.AUT_USRMOD) AS ""USR. BAJA"",
                                    (SELECT LOC_DESCRIPCION FROM ect2sp.irs_locales WHERE LOC_NUMERO = A.LOC_NUMERO) AS ""NOM. LOCAL""
                                FROM exct2sp.aut_empleado E
                                INNER JOIN exct2sp.aut_autorizador A ON A.EMP_CODIGO = E.EMP_CODIGO
                                ORDER BY A.LOC_NUMERO, A.AUT_ESTAUT
                                ";

                var command = new OracleCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 600
                };

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }

        public async Task<DataTable> ListarAutorizadoresPaginadoAsync(int startRow, int endRow)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                string query = @"
                                WITH paginated_data AS (
                                    SELECT
                                        E.EMP_CODIGO AS ""COD. COLABORADOR"",
                                        E.EMP_APEPAT || ' ' || E.EMP_APEMAT || ' , ' || E.EMP_NOMBRE AS ""NOM. AUTORIZADOR"",
                                        E.EMP_NUMDOC AS ""NUM. DOCUMENTO"",
                                        A.AUT_CODAUTORIZA AS ""COD. AUTORIZADOR"",
                                        DECODE(A.AUT_ESTAUT, 'A', 'A', 'I') AS ""EST. AUTORIZADOR"",
                                        A.LOC_NUMERO AS ""COD LOCAL"",
                                        A.AUT_FECCREA AS ""FEC. CREACIÓN"",
                                        A.AUT_USRCREA AS ""USU. CREACIÓN"",
                                        DECODE(A.AUT_ESTAUT , 'A', NULL, A.AUT_FECMOD) AS ""FEC. BAJA"",
                                        DECODE(A.AUT_ESTAUT , 'A', NULL, A.AUT_USRMOD) AS ""USR. BAJA"",
                                        (SELECT LOC_DESCRIPCION FROM ect2sp.irs_locales WHERE LOC_NUMERO = A.LOC_NUMERO) AS ""NOM. LOCAL"",
                                        ROW_NUMBER() OVER (ORDER BY A.LOC_NUMERO, A.AUT_ESTAUT) AS ROW_NUM,
                                        COUNT(E.EMP_CODIGO) OVER () AS TOTAL_RECORDS
                                    FROM exct2sp.aut_empleado E
                                    INNER JOIN exct2sp.aut_autorizador A ON A.EMP_CODIGO = E.EMP_CODIGO
                                )
                                SELECT *
                                FROM paginated_data
                                WHERE ROW_NUM BETWEEN :startRow AND :endRow";

                var command = new OracleCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 600
                };

                command.Parameters.Add(":startRow", OracleDbType.Int32).Value = startRow;
                command.Parameters.Add(":endRow", OracleDbType.Int32).Value = endRow;

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }

        public async Task<DataTable> ListarCajerosAsync()
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                string query = @"
                                SELECT
                                    irc.CAJ_CODIGO AS ""COD. CAJERO"",
                                    IRC.CAJ_CODIGO_EMP AS ""COD. COLABORADOR"",
                                    irc.caj_activo AS ""EST. CAJERO"",
                                    irc.loc_numero AS ""COD. LOCAL"",
                                    IDC.DE_SEDE AS ""NOM. LOCAL"",
                                    irC.CAJ_NOMBRE AS ""NOM. CAJERO"",
                                    irc.caj_rut AS ""NUM. DOCUME"",
                                    IRC.CAJ_FCREACION AS ""FEC. CREACION"",
                                    IRC.CAJ_USUARIO_CREA AS ""USU. CREACION"",
                                    IRC.CAJ_FBAJA AS ""FEC. BAJA"",
                                    IRC.CAJ_USUARIO_BAJA AS ""USU. BAJA"",
                                    IDC.FE_INGR_EMPR AS ""FEC. INGRESO"",
                                    IDC.TI_SITU AS ""EST. COLABORADOR"",
                                    IDC.FE_CESE_TRAB AS ""FEC. CESE"",
                                    IDC.DE_PUES_TRAB AS ""PUESTO""
                                FROM ECT2SP.IRS_CAJEROS irc
                                LEFT JOIN EAUTORIZADOR.INT_LOCAL_OFIPLAN ilo ON ILO.COD_LOC_CT2 = irc.LOC_NUMERO
                                LEFT JOIN EAUTORIZADOR.INT_DATOS_COLABORADOR idc ON IDC.CO_EMPR = ILO.COD_EMPRESA AND IDC.CO_SEDE = ILO.COD_LOC_OFI AND IDC.NU_DOCU_IDEN = IRC.CAJ_RUT
                                WHERE irc.caj_tipo = '01'
                                ORDER BY IRC.CAJ_FCREACION DESC, IRC.CAJ_FBAJA
                                ";

                var command = new OracleCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 600
                };

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }

        public async Task<DataTable> ListarCajerosPaginadoAsync(int startRow, int endRow)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                string query = @"
                                WITH paginated_data AS (
                                    SELECT
                                        irc.CAJ_CODIGO AS ""COD. CAJERO"",
                                        irc.CAJ_CODIGO_EMP AS ""COD. COLABORADOR"",
                                        irc.caj_activo AS ""EST. CAJERO"",
                                        irc.loc_numero AS ""COD. LOCAL"",
                                        idc.DE_SEDE AS ""NOM. LOCAL"",
                                        irc.CAJ_NOMBRE AS ""NOM. CAJERO"",
                                        irc.caj_rut AS ""NUM. DOCUME"",
                                        irc.CAJ_FCREACION AS ""FEC. CREACION"",
                                        irc.CAJ_USUARIO_CREA AS ""USU. CREACION"",
                                        irc.CAJ_FBAJA AS ""FEC. BAJA"",
                                        irc.CAJ_USUARIO_BAJA AS ""USU. BAJA"",
                                        idc.FE_INGR_EMPR AS ""FEC. INGRESO"",
                                        idc.TI_SITU AS ""EST. COLABORADOR"",
                                        idc.FE_CESE_TRAB AS ""FEC. CESE"",
                                        idc.DE_PUES_TRAB AS ""PUESTO"",
                                        ROW_NUMBER() OVER (ORDER BY irc.CAJ_FCREACION DESC, irc.CAJ_FBAJA) AS ROW_NUM,
                                        COUNT(irc.CAJ_CODIGO) OVER () AS TOTAL_RECORDS
                                    FROM ECT2SP.IRS_CAJEROS irc
                                    LEFT JOIN EAUTORIZADOR.INT_LOCAL_OFIPLAN ilo 
                                        ON ilo.COD_LOC_CT2 = irc.LOC_NUMERO
                                    LEFT JOIN EAUTORIZADOR.INT_DATOS_COLABORADOR idc 
                                        ON idc.CO_EMPR = ilo.COD_EMPRESA 
                                        AND idc.CO_SEDE = ilo.COD_LOC_OFI 
                                        AND idc.NU_DOCU_IDEN = irc.CAJ_RUT
                                    WHERE irc.caj_tipo = '01'
                                )
                                SELECT *
                                FROM paginated_data
                                WHERE ROW_NUM BETWEEN :startRow AND :endRow
                                ";

                var command = new OracleCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 600
                };

                command.Parameters.Add(":startRow", OracleDbType.Int32).Value = startRow;
                command.Parameters.Add(":endRow", OracleDbType.Int32).Value = endRow;

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }


    }
}
