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
                                SELECT PAG_TIPOPAGO, PAG_MONTO, HED_PAIS, HED_ORIGENTRX, HED_LOCAL, HED_POS, HED_FECHATRX, HED_HORATRX, HED_NUMTRX,PAG_BMAGNETICA,PAG_NUMCTA
                                FROM ect2sp.ctx_pagos_trx
                                WHERE PAG_FCONTABLE BETWEEN TO_DATE(:fechaInicio, 'DD-MM-YYYY') AND TO_DATE(:fechaFin, 'DD-MM-YYYY')
                                    AND PAG_TIPOPAGO IN ('16', '06')
                                    AND HED_LOCAL = :codLocal
                                )
                                SELECT
                                    p.PAG_TIPOPAGO  AS TIPO,
                                    tp.TPA_DESPAGO  AS DESCRIPCION,
                                    DECODE(p.PAG_TIPOPAGO,'16',p.PAG_BMAGNETICA,'06',p.PAG_NUMCTA) NUMERO,
                                    IL.CAD_NUMERO AS ""COD EMPRESA"",
                                    IC.CAD_DESCRIPCION  AS ""NOM EMPRESA"",
                                    t.HED_LOCAL  AS ""COD LOCAL"",
                                    IL.LOC_DESCRIPCION AS ""NOM LOCAL"",
                                    t.HED_POS AS CAJA,
                                    t.HED_NUMTRX AS TICKET,
                                    TO_CHAR(t.HED_FCONTABLE, 'yyyy-mm-dd') AS ""F.CONTABLE"",
                                    p.PAG_MONTO AS IMPORTE
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
                                SELECT PAG_TIPOPAGO, PAG_MONTO, HED_PAIS, HED_ORIGENTRX, HED_LOCAL, HED_POS, HED_FECHATRX, HED_HORATRX, HED_NUMTRX,PAG_BMAGNETICA,PAG_NUMCTA
                                FROM ect2sp.ctx_pagos_trx
                                WHERE PAG_FCONTABLE BETWEEN TO_DATE(:fechaInicio, 'DD-MM-YYYY') AND TO_DATE(:fechaFin, 'DD-MM-YYYY')
                                    AND PAG_TIPOPAGO IN ('16', '06')
                                    AND HED_LOCAL = :codLocal
                                )
                                SELECT *
                                FROM (
                                    SELECT
	                                    p.PAG_TIPOPAGO  AS TIPO,
	                                    tp.TPA_DESPAGO  AS DESCRIPCION,
	                                    DECODE(p.PAG_TIPOPAGO,'16',p.PAG_BMAGNETICA,'06',p.PAG_NUMCTA) NUMERO,
	                                    IL.CAD_NUMERO AS ""COD EMPRESA"",
	                                    IC.CAD_DESCRIPCION  AS ""NOM EMPRESA"",
	                                    FH.HED_LOCAL  AS ""COD LOCAL"",
	                                    IL.LOC_DESCRIPCION AS ""NOM LOCAL"",
	                                    FH.HED_POS AS CAJA,
	                                    FH.HED_NUMTRX AS TICKET,
	                                    TO_CHAR(FH.HED_FCONTABLE, 'yyyy-mm-dd') AS ""F.CONTABLE"",
	                                    p.PAG_MONTO AS IMPORTE,
	                                    ROW_NUMBER() OVER (ORDER BY FH.HED_FCONTABLE, FH.HED_NUMTRX) AS ROW_NUM,
		                                COUNT(IL.CAD_NUMERO) OVER () AS TOTAL_RECORDS
	                                FROM
	                                    filtered_header FH
	                                JOIN
	                                    filtered_pagos p
	                                    ON
	                                    p.HED_PAIS = FH.HED_PAIS
	                                    AND p.HED_ORIGENTRX = FH.HED_ORIGENTRX
	                                    AND p.HED_LOCAL = FH.HED_LOCAL
	                                    AND p.HED_POS = FH.HED_POS
	                                    AND p.HED_FECHATRX = FH.HED_FECHATRX
	                                    AND p.HED_HORATRX = FH.HED_HORATRX
	                                    AND p.HED_NUMTRX = FH.HED_NUMTRX
	                                JOIN
	                                    ECT2SP.CUA_TIPOS_PAGO tp
	                                    ON
	                                    tp.TPA_CODIGO = p.PAG_TIPOPAGO
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
	                                ilo.NOM_LOC_OFI AS ""NOM. LOCAL"",
	                                irC.CAJ_NOMBRE AS ""NOM. CAJERO"",
	                                irc.caj_rut AS ""NUM. DOCUME"",
	                                TRUNC(IRC.CAJ_FCREACION) AS ""FEC. CREACION"",
	                                IRC.CAJ_USUARIO_CREA AS ""USU. CREACION"",
	                                TRUNC(IRC.CAJ_FBAJA) AS ""FEC. BAJA"",
	                                IRC.CAJ_USUARIO_BAJA AS ""USU. BAJA"",
	                                TRUNC(IDC.FE_INGR_EMPR) AS ""FEC. INGRESO"",
	                                IDC.TI_SITU AS ""EST. COLABORADOR"",
	                                TRUNC(IDC.FE_CESE_TRAB) AS ""FEC. CESE"",
	                                IDC.DE_PUES_TRAB AS ""PUESTO""
                                FROM
	                                ECT2SP.IRS_CAJEROS irc
                                LEFT JOIN EAUTORIZADOR.INT_LOCAL_OFIPLAN ilo ON
	                                ILO.COD_LOC_CT2 = irc.LOC_NUMERO
                                LEFT JOIN EAUTORIZADOR.INT_DATOS_COLABORADOR idc ON
	                                ( IDC.CODIGO_OFISIS = IRC.CAJ_CODIGO_EMP )
                                WHERE
	                                irc.caj_tipo = '01'
	                                AND NOT IRC.CAJ_CODIGO_EMP IS NULL
                                UNION
                                SELECT
	                                irc.CAJ_CODIGO AS ""COD. CAJERO"",
	                                IRC.CAJ_CODIGO_EMP AS ""COD. COLABORADOR"",
	                                irc.caj_activo AS ""EST. CAJERO"",
	                                irc.loc_numero AS ""COD. LOCAL"",
	                                ilo.NOM_LOC_OFI AS ""NOM. LOCAL"",
	                                irC.CAJ_NOMBRE AS ""NOM. CAJERO"",
	                                irc.caj_rut AS ""NUM. DOCUME"",
	                                TRUNC(IRC.CAJ_FCREACION) AS ""FEC. CREACION"",
	                                IRC.CAJ_USUARIO_CREA AS ""USU. CREACION"",
	                                TRUNC(IRC.CAJ_FBAJA) AS ""FEC. BAJA"",
	                                IRC.CAJ_USUARIO_BAJA AS ""USU. BAJA"",
	                                TRUNC(IDC.FE_INGR_EMPR) AS ""FEC. INGRESO"",
	                                IDC.TI_SITU AS ""EST. COLABORADOR"",
	                                TRUNC(IDC.FE_CESE_TRAB) AS ""FEC. CESE"",
	                                IDC.DE_PUES_TRAB AS ""PUESTO""
                                FROM
	                                ECT2SP.IRS_CAJEROS irc
                                LEFT JOIN EAUTORIZADOR.INT_LOCAL_OFIPLAN ilo ON
	                                ILO.COD_LOC_CT2 = irc.LOC_NUMERO
                                LEFT JOIN EAUTORIZADOR.INT_DATOS_COLABORADOR idc ON
	                                ( IDC.NU_DOCU_IDEN = IRC.CAJ_RUT )
                                WHERE
	                                irc.caj_tipo = '01'
	                                AND IRC.CAJ_CODIGO_EMP IS NULL
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
                               WITH combined_data AS (
                                SELECT
	                                irc.CAJ_CODIGO AS ""COD. CAJERO"",
	                                IRC.CAJ_CODIGO_EMP AS ""COD. COLABORADOR"",
	                                irc.caj_activo AS ""EST. CAJERO"",
	                                irc.loc_numero AS ""COD. LOCAL"",
	                                ilo.NOM_LOC_OFI AS ""NOM. LOCAL"",
	                                irC.CAJ_NOMBRE AS ""NOM. CAJERO"",
	                                irc.caj_rut AS ""NUM. DOCUME"",
	                                TRUNC(IRC.CAJ_FCREACION) AS ""FEC. CREACION"",
	                                IRC.CAJ_USUARIO_CREA AS ""USU. CREACION"",
	                                TRUNC(IRC.CAJ_FBAJA) AS ""FEC. BAJA"",
	                                IRC.CAJ_USUARIO_BAJA AS ""USU. BAJA"",
	                                TRUNC(IDC.FE_INGR_EMPR) AS ""FEC. INGRESO"",
	                                IDC.TI_SITU AS ""EST. COLABORADOR"",
	                                TRUNC(IDC.FE_CESE_TRAB) AS ""FEC. CESE"",
	                                IDC.DE_PUES_TRAB AS ""PUESTO""
                                FROM
	                                ECT2SP.IRS_CAJEROS irc
                                LEFT JOIN EAUTORIZADOR.INT_LOCAL_OFIPLAN ilo ON
	                                ILO.COD_LOC_CT2 = irc.LOC_NUMERO
                                LEFT JOIN EAUTORIZADOR.INT_DATOS_COLABORADOR idc ON
	                                ( IDC.CODIGO_OFISIS = IRC.CAJ_CODIGO_EMP )
                                WHERE
	                                irc.caj_tipo = '01'
	                                AND NOT IRC.CAJ_CODIGO_EMP IS NULL
                                UNION
                                SELECT
	                                irc.CAJ_CODIGO AS ""COD. CAJERO"",
	                                IRC.CAJ_CODIGO_EMP AS ""COD. COLABORADOR"",
	                                irc.caj_activo AS ""EST. CAJERO"",
	                                irc.loc_numero AS ""COD. LOCAL"",
	                                ilo.NOM_LOC_OFI AS ""NOM. LOCAL"",
	                                irC.CAJ_NOMBRE AS ""NOM. CAJERO"",
	                                irc.caj_rut AS ""NUM. DOCUME"",
	                                TRUNC(IRC.CAJ_FCREACION) AS ""FEC. CREACION"",
	                                IRC.CAJ_USUARIO_CREA AS ""USU. CREACION"",
	                                TRUNC(IRC.CAJ_FBAJA) AS ""FEC. BAJA"",
	                                IRC.CAJ_USUARIO_BAJA AS ""USU. BAJA"",
	                                TRUNC(IDC.FE_INGR_EMPR) AS ""FEC. INGRESO"",
	                                IDC.TI_SITU AS ""EST. COLABORADOR"",
	                                TRUNC(IDC.FE_CESE_TRAB) AS ""FEC. CESE"",
	                                IDC.DE_PUES_TRAB AS ""PUESTO""
                                FROM
	                                ECT2SP.IRS_CAJEROS irc
                                LEFT JOIN EAUTORIZADOR.INT_LOCAL_OFIPLAN ilo ON
	                                ILO.COD_LOC_CT2 = irc.LOC_NUMERO
                                LEFT JOIN EAUTORIZADOR.INT_DATOS_COLABORADOR idc ON
	                                ( IDC.NU_DOCU_IDEN = IRC.CAJ_RUT )
                                WHERE
	                                irc.caj_tipo = '01'
	                                AND IRC.CAJ_CODIGO_EMP IS NULL
                            )
                            SELECT *
                            FROM (
                                SELECT 
                                    combined_data.*,
                                    ROW_NUMBER() OVER (ORDER BY ""FEC. CREACION"" DESC) AS ROW_NUM,
                                    COUNT(*) OVER () AS TOTAL_RECORDS
                                FROM combined_data
                                ) paginated_data
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
