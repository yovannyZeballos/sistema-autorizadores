using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioSolicitudUsuarioASR : RepositorioGenerico<SGPContexto, ASR_SolicitudUsuario>, IRepositorioSolicitudUsuarioASR
    {
        public RepositorioSolicitudUsuarioASR(SGPContexto context) : base(context) { }

        public SGPContexto SGPContext
        {
            get { return _contexto; }
        }

        public async Task ActualizarMotivoRechazo(int numSolicitud, string motivo, string estado)
        {
            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(@"CALL ""SGP"".""sp_asr_actualizar_motivo_rechazo""(@p_num_solicitud, @p_motivo, @p_ind_aprobado)", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@p_num_solicitud", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = numSolicitud });
                    command.Parameters.Add(new NpgsqlParameter("@p_motivo", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = motivo });
                    command.Parameters.Add(new NpgsqlParameter("@p_ind_aprobado", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = estado });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AprobarSolicitud(ASR_Usuario usuario)
        {
            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(@"CALL ""SGP"".""sp_asr_aprobar_solicitud""(@p_num_solicitud, @p_ind_activo, @p_flg_envio, @p_fec_envio, @p_usu_autoriza, @p_usu_creacion, @p_usu_elimina, @p_fec_elimina)", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@p_num_solicitud", NpgsqlTypes.NpgsqlDbType.Integer) { Value = usuario.NumSolicitud });
                    command.Parameters.Add(new NpgsqlParameter("@p_ind_activo", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.IndActivo ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@p_flg_envio", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.FlgEnvio ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@p_fec_envio", NpgsqlTypes.NpgsqlDbType.Date) { Value = (object)usuario.FecEnvio ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@p_usu_autoriza", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuAutoriza ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@p_usu_creacion", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuCreacion ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@p_usu_elimina", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuElimina ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@p_fec_elimina", NpgsqlTypes.NpgsqlDbType.Date) { Value = (object)usuario.FecElimina ?? DBNull.Value });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<ASR_UsuarioListado>> ListarSolicitudes(string usuarioLogin, string tipoUsuario, string CodEmpresa,
            int numeroPagina, int tamañoPagina)
        {
            List<ASR_UsuarioListado> usuarios = new List<ASR_UsuarioListado>();

            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT * FROM ""SGP"".""sf_asr_solicitud_usuario_listar""(@p_usuario_login, @p_tip_usuario, @p_cod_empresa, @p_page_number, @p_page_size)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@p_usuario_login", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuarioLogin });
                    command.Parameters.Add(new NpgsqlParameter("@p_tip_usuario", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = tipoUsuario });
                    command.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CodEmpresa });
                    command.Parameters.Add(new NpgsqlParameter("@p_page_number", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numeroPagina });
                    command.Parameters.Add(new NpgsqlParameter("@p_page_size", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tamañoPagina });

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                usuarios.Add(new ASR_UsuarioListado
                                {
                                    NumSolicitud = Convert.ToInt32(dr["NUM_SOLICITUD"]),
                                    CodLocal = dr["COD_LOCAL"].ToString(),
                                    CodLocalAlterno = dr["COD_LOCAL_ALTERNO"].ToString(),
                                    CodPais = Convert.ToInt32(dr["COD_PAIS"]),
                                    CodComercio = Convert.ToInt32(dr["COD_COMERCIO"]),
                                    NomLocal = dr["NOM_LOCAL"].ToString(),
                                    CodColaborador = dr["COD_COLABORADOR"] != DBNull.Value ? dr["COD_COLABORADOR"].ToString() : string.Empty,
                                    NoApelPate = dr["NO_APEL_PATE"] != DBNull.Value ? dr["NO_APEL_PATE"].ToString() : string.Empty,
                                    NoApelMate = dr["NO_APEL_MATE"] != DBNull.Value ? dr["NO_APEL_MATE"].ToString() : string.Empty,
                                    NoTrab = dr["NO_TRAB"] != DBNull.Value ? dr["NO_TRAB"].ToString() : string.Empty,
                                    DePuesTrab = dr["DE_PUES_TRAB"] != DBNull.Value ? dr["DE_PUES_TRAB"].ToString() : string.Empty,
                                    IndAprobado = dr["IND_APROBADO"] != DBNull.Value ? dr["IND_APROBADO"].ToString() : string.Empty,
                                    FecSolicita = dr["FEC_SOLICITA"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FEC_SOLICITA"]) : null,
                                    TipDocumentoIdentidad = dr["TI_DOCU_IDEN"] != DBNull.Value ? dr["TI_DOCU_IDEN"].ToString() : string.Empty,
                                    NumDocumentoIdentidad = dr["NU_DOCU_IDEN"] != DBNull.Value ? dr["NU_DOCU_IDEN"].ToString() : string.Empty,
                                    TotalRegistros = dr["TOTAL_REGISTROS"] != DBNull.Value ? Convert.ToInt32(dr["TOTAL_REGISTROS"]) : 0
                                });
                            }
                        }
                        return usuarios;
                    }
                }
            }
        }

        public async Task<List<ASR_UsuarioListado>> ListarUsuarios(string usuarioLogin, string codLocal,
            string usuAprobacion, string tipAccion, string indAprobado, string CodEmpresa,
            int numeroPagina, int tamañoPagina)
        {
            List<ASR_UsuarioListado> usuarios = new List<ASR_UsuarioListado>();

            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT * FROM ""SGP"".""sf_asr_usuario_listar""(@p_usuario_login, @p_cod_local, @p_usu_aprobacion, @p_tip_accion, @p_ind_aprobado, @p_cod_empresa, @p_page_number, @p_page_size)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@p_usuario_login", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuarioLogin });
                    command.Parameters.Add(new NpgsqlParameter("@p_cod_local", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codLocal });
                    command.Parameters.Add(new NpgsqlParameter("@p_usu_aprobacion", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuAprobacion });
                    command.Parameters.Add(new NpgsqlParameter("@p_tip_accion", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = tipAccion });
                    command.Parameters.Add(new NpgsqlParameter("@p_ind_aprobado", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = indAprobado });
                    command.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CodEmpresa });
                    command.Parameters.Add(new NpgsqlParameter("@p_page_number", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numeroPagina });
                    command.Parameters.Add(new NpgsqlParameter("@p_page_size", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tamañoPagina });

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                usuarios.Add(new ASR_UsuarioListado
                                {
                                    NumSolicitud = Convert.ToInt32(dr["NUM_SOLICITUD"]),
                                    CodLocal = dr["COD_LOCAL"].ToString(),
                                    NomLocal = dr["NOM_LOCAL"].ToString(),
                                    CodColaborador = dr["COD_COLABORADOR"] != DBNull.Value ? dr["COD_COLABORADOR"].ToString() : string.Empty,
                                    CodUsuarioAsr = dr["COD_USUARIO_ASR"] != DBNull.Value ? dr["COD_USUARIO_ASR"].ToString() : string.Empty,
                                    NoApelPate = dr["NO_APEL_PATE"] != DBNull.Value ? dr["NO_APEL_PATE"].ToString() : string.Empty,
                                    NoApelMate = dr["NO_APEL_MATE"] != DBNull.Value ? dr["NO_APEL_MATE"].ToString() : string.Empty,
                                    NoTrab = dr["NO_TRAB"] != DBNull.Value ? dr["NO_TRAB"].ToString() : string.Empty,
                                    TipAccion = dr["TIP_ACCION"] != DBNull.Value ? dr["TIP_ACCION"].ToString() : string.Empty,
                                    TipUsuario = dr["TIP_USUARIO"] != DBNull.Value ? dr["TIP_USUARIO"].ToString() : string.Empty,
                                    TipColaborador = dr["TIP_COLABORADOR"] != DBNull.Value ? dr["TIP_COLABORADOR"].ToString() : string.Empty,
                                    DePuesTrab = dr["DE_PUES_TRAB"] != DBNull.Value ? dr["DE_PUES_TRAB"].ToString() : string.Empty,
                                    IndAprobado = dr["IND_APROBADO"] != DBNull.Value ? dr["IND_APROBADO"].ToString() : string.Empty,
                                    UsuSolicita = dr["USU_SOLICITA"] != DBNull.Value ? dr["USU_SOLICITA"].ToString() : string.Empty,
                                    UsuSolNoApelPate = dr["USU_SOL_NO_APEL_PATE"] != DBNull.Value ? dr["USU_SOL_NO_APEL_PATE"].ToString() : string.Empty,
                                    UsuSolNoApelMate = dr["USU_SOL_NO_APEL_MATE"] != DBNull.Value ? dr["USU_SOL_NO_APEL_MATE"].ToString() : string.Empty,
                                    UsuSolNoTrab = dr["USU_SOL_NO_TRAB"] != DBNull.Value ? dr["USU_SOL_NO_TRAB"].ToString() : string.Empty,
                                    FecSolicita = dr["FEC_SOLICITA"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FEC_SOLICITA"]) : null,
                                    UsuAprobacion = dr["USU_APROBACION"] != DBNull.Value ? dr["USU_APROBACION"].ToString() : string.Empty,
                                    UsuAprNoApelPate = dr["USU_APR_NO_APEL_PATE"] != DBNull.Value ? dr["USU_APR_NO_APEL_PATE"].ToString() : string.Empty,
                                    UsuAprNoApelMate = dr["USU_APR_NO_APEL_MATE"] != DBNull.Value ? dr["USU_APR_NO_APEL_MATE"].ToString() : string.Empty,
                                    UsuAprNoTrab = dr["USU_APR_NO_TRAB"] != DBNull.Value ? dr["USU_APR_NO_TRAB"].ToString() : string.Empty,
                                    FecAprobacion = dr["FEC_APROBACION"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FEC_APROBACION"]) : null,
                                    Motivo = dr["MOTIVO"] != DBNull.Value ? dr["MOTIVO"].ToString() : string.Empty,
                                    TotalRegistros = dr["TOTAL_COUNT"] != DBNull.Value ? Convert.ToInt32(dr["TOTAL_COUNT"]) : 0
                                });
                            }
                        }
                        return usuarios;
                    }
                }
            }
        }

        public async Task<List<ASR_UsuarioArchivo>> ListarArchivos(string tipUsuario)
        {
            List<ASR_UsuarioArchivo> archivos = new List<ASR_UsuarioArchivo>();

            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                string query = string.Empty;

                if (tipUsuario == "A")
                {
                    query = @"SELECT * FROM ""SGP"".""sf_asr_generar_archivo_autorizador""()";
                }
                else
                {
                    query = @"SELECT * FROM ""SGP"".""sf_asr_generar_archivo_cajero""()";
                }

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                archivos.Add(new ASR_UsuarioArchivo
                                {
                                    NumSolicitud = Convert.ToInt16(dr["NUM_SOLICITUD"]),
                                    Contenido = dr["CONTENIDO"].ToString(),
                                    NombreArchivo = dr["NOMBRE_ARCHIVO"].ToString(),
                                });
                            }
                        }
                        return archivos;
                    }
                }
            }
        }

        public async Task ActualizarFlagEnvio(long numSolicitud, string flagEnvio)
        {
            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(@"CALL ""SGP"".""sp_asr_actualizar_flg_envio""(@p_num_solicitud, @p_flg_envio)", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@p_num_solicitud", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numSolicitud });
                    command.Parameters.Add(new NpgsqlParameter("@p_flg_envio", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = flagEnvio });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Oracle

        public async Task<List<int>> ObtenerLocalesPorProcesarAsyncOracleSpsa()
        {
            List<int> locales = new List<int>();

            using (var connection = new OracleConnection(CadenasConexion.CadenaConexionCT2))
            {
                await connection.OpenAsync();

                string query = @"
                                SELECT DISTINCT
	                                LOC_NUMERO 
                                FROM
	                                ""ECT2SP"".""CAJEROS_INTERFACE""  
                                WHERE
	                                CAJ_PROCESADO = 'N' 
                                ORDER BY
	                                LOC_NUMERO";

                using (var command = new OracleCommand(query, connection))
                {
                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                locales.Add(dr["LOC_NUMERO"] != DBNull.Value ? Convert.ToInt32(dr["LOC_NUMERO"]) : 0);
                            }
                        }
                        return locales;
                    }
                }
            }
        }

        public async Task<List<ASR_CajeroPaso>> ObtenerCajerosPorProcesarAsyncOracleSpsa(int codLocal)
        {
            List<ASR_CajeroPaso> cajeros = new List<ASR_CajeroPaso>();

            using (var connection = new OracleConnection(CadenasConexion.CadenaConexionCT2))
            {
                await connection.OpenAsync();

                string query = @"
                                SELECT
	                                * 
                                FROM
	                                ""ECT2SP"".""CAJEROS_INTERFACE"" 
                                WHERE
	                                LOC_NUMERO = :P_LOC_NUMERO 
	                                AND CAJ_PROCESADO = 'N'";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("P_LOC_NUMERO", OracleDbType.Int32) { Value = codLocal });

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                cajeros.Add(new ASR_CajeroPaso
                                {
                                    CajCodigo = dr["CAJ_CODIGO"] as string ?? null,
                                    LocNumero = dr["LOC_NUMERO"] != DBNull.Value ? Convert.ToInt32(dr["LOC_NUMERO"]) : 0,
                                    CajNombre = dr["CAJ_NOMBRE"] as string ?? null,
                                    CajRut = dr["CAJ_RUT"] as string ?? null,
                                    CajTipo = dr["CAJ_TIPO"] as string ?? null,
                                    CajActivo = dr["CAJ_ACTIVO"] as string ?? null,
                                    CajCodigoEmp = dr["CAJ_CODIGO_EMP"] as string ?? null,
                                    CajEstado = dr["CAJ_ESTADO"] as string ?? null,
                                    CajNom = dr["CAJ_NOM"] as string ?? null,
                                    CajApellidos = dr["CAJ_APELLIDOS"] as string ?? null,
                                    CajTipoDocId = dr["CAJ_TIPO_DOCID"] as string ?? null,
                                    CajFcreacion = dr["CAJ_FCREACION"] != DBNull.Value ? (DateTime?)dr["CAJ_FCREACION"] : null,
                                    CajFbaja = dr["CAJ_FBAJA"] != DBNull.Value ? (DateTime?)dr["CAJ_FBAJA"] : null,
                                    Cajfactualiza = dr["CAJ_FACTUALIZA"] != DBNull.Value ? (DateTime?)dr["CAJ_FACTUALIZA"] : null,
                                    CajTipoContrato = dr["CAJ_TIPO_CONTRATO"] as string ?? null,
                                    CajCorrExtranjero = dr["CAJ_CORR_EXTRANJERO"] as string ?? null
                                    //CodPais = dr["cod_pais"] != DBNull.Value ? Convert.ToInt32(dr["cod_pais"]) : 0,
                                    //CodComercio = dr["cod_comercio"] != DBNull.Value ? Convert.ToInt32(dr["cod_comercio"]) : 0
                                });
                            }
                        }
                        return cajeros;
                    }
                }
            }
        }

        public async Task ActualizarFlagProcesadoAsyncOracleSpsa(int codLocal, string codCajero, string flagProcesado)
        {
            using (var connection = new OracleConnection(CadenasConexion.CadenaConexionCT2))
            {
                await connection.OpenAsync();

                string query = @"
            UPDATE ""ECT2SP"".""CAJEROS_INTERFACE"" 
               SET ""CAJ_PROCESADO"" = :P_CAJ_PROCESADO
             WHERE ""LOC_NUMERO"" = :P_LOC_NUMERO
               AND ""CAJ_CODIGO"" = :P_CAJ_CODIGO";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":P_LOC_NUMERO", OracleDbType.Int32) { Value = codLocal });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_CODIGO", OracleDbType.Varchar2) { Value = codCajero });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_PROCESADO", OracleDbType.Varchar2) { Value = flagProcesado });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<(int, string)> NuevoCajeroAsyncOracleSpsa(ASR_CajeroPaso cajero)
        {
            try
            {
                var usuario = ConstruirUsuario(cajero.CajUsuarioCrea);

                // 1. Elimina paso previo
                await EliminarCajeroPasoOracleSpsa(cajero.CajRut);

                // 2. Inserta paso
                await InsertarCajeroPasoOracleSpsa(cajero);

                // 3. Importa cajeros
                var (resultado, mensaje) = await EjecutarImportarCajerosAsync(cajero.LocNumero, usuario);

                return (resultado, mensaje);
            }
            catch (Exception ex)
            {
                return (-20020, $"Creacion cajeros falló: {ex.Message}");
            }
        }

        public async Task EliminarCajeroPasoOracleSpsa(string rut)
        {
            using (var connection = new OracleConnection(CadenasConexion.CadenaConexionCT2))
            {
                await connection.OpenAsync();

                string query = @"DELETE FROM ""ECT2SP"".""IRS_CAJEROS_PASO"" WHERE CAJ_RUT = :P_CAJ_RUT";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":P_CAJ_RUT", OracleDbType.Varchar2)
                    {
                        Value = rut ?? (object)DBNull.Value
                    });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task InsertarCajeroPasoOracleSpsa(ASR_CajeroPaso cajero)
        {
            using (var connection = new OracleConnection(CadenasConexion.CadenaConexionCT2))
            {
                await connection.OpenAsync();

                string query = @"INSERT
	                                    INTO
	                                    ""ECT2SP"".""IRS_CAJEROS_PASO"" (
                                        ""CAJ_CODIGO"",
	                                    ""LOC_NUMERO"",
	                                    ""CAJ_NOMBRE"",
	                                    ""CAJ_NOM"",
	                                    ""CAJ_APELLIDOS"",
	                                    ""CAJ_RUT"",
	                                    ""CAJ_TIPO"",
	                                    ""CAJ_TIPO_CONTRATO"",
	                                    ""CAJ_TIPO_DOCID"",
	                                    ""CAJ_CODIGO_EMP"",
	                                    ""CAJ_FCREACION"",
	                                    ""CAJ_USUARIO_CREA"")
                                    VALUES (
                                    :P_CAJ_CODIGO,
                                    :P_LOC_NUMERO,
                                    :p_CAJ_NOMBRE,
                                    :p_CAJ_NOM,
                                    :p_CAJ_APELLIDOS,
                                    :P_CAJ_RUT,
                                    :P_CAJ_TIPO,
                                    :P_CAJ_TIPO_CONTRATO,
                                    :P_CAJ_TIPO_DOCID,
                                    :P_CAJ_CODIGO_EMP,
                                    :P_CAJ_FCREACION,
                                    :P_CAJ_USUARIO_CREA)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":P_CAJ_CODIGO", OracleDbType.Varchar2) { Value = cajero.CajCodigo });
                    command.Parameters.Add(new OracleParameter(":P_LOC_NUMERO", OracleDbType.Int32) { Value = cajero.LocNumero });
                    command.Parameters.Add(new OracleParameter(":p_CAJ_NOMBRE", OracleDbType.Varchar2) { Value = cajero.CajNombre ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":p_CAJ_NOM", OracleDbType.Varchar2) { Value = cajero.CajNom ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":p_CAJ_APELLIDOS", OracleDbType.Varchar2) { Value = cajero.CajApellidos ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_RUT", OracleDbType.Varchar2) { Value = cajero.CajRut ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_TIPO", OracleDbType.Varchar2) { Value = cajero.CajTipo ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_TIPO_CONTRATO", OracleDbType.Varchar2) { Value = cajero.CajTipoContrato ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_TIPO_DOCID", OracleDbType.Varchar2) { Value = cajero.CajTipoDocId ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_CODIGO_EMP", OracleDbType.Varchar2) { Value = cajero.CajCodigoEmp ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_FCREACION", OracleDbType.TimeStamp) { Value = cajero.CajFcreacion ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter(":P_CAJ_USUARIO_CREA", OracleDbType.Varchar2) { Value = cajero.CajUsuarioCrea ?? (object)DBNull.Value });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<(int Resultado, string Mensaje)> EjecutarImportarCajerosAsync(int codLocal, string login)
        {
            using (var connection = new OracleConnection(CadenasConexion.CadenaConexionCT2))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(@"""EAUTORIZADOR"".""PKG_SGC_MANT_CAJEROS"".""IMPORTAR_CAJEROS""", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    command.Parameters.Add("pLOCAL", OracleDbType.Int32).Value = codLocal;
                    command.Parameters.Add("pLOGIN", OracleDbType.Varchar2, 50).Value = login;

                    // Parámetros de salida
                    command.Parameters.Add("outRESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    command.Parameters.Add("outMENSAJE", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    await command.ExecuteNonQueryAsync();


                    // Solución al error: conversión explícita desde OracleDecimal
                    var oracleDecimal = (OracleDecimal)command.Parameters["outRESULTADO"].Value;
                    int resultado = oracleDecimal.ToInt32();
                    string mensaje = command.Parameters["outMENSAJE"].Value?.ToString();

                    return (resultado, mensaje);
                }
            }
        }




        // Postgress

        public async Task<List<int>> ObtenerLocalesPorProcesarAsync(int codPais, int codComercio)
        {
            List<int> locales = new List<int>();

            using (var connection = new NpgsqlConnection(CadenasConexion.CadenaConexionCT3_SPSA))
            {
                await connection.OpenAsync();

                string query = @"SELECT DISTINCT
	                                loc_numero 
                                FROM
	                                cajeros_interface 
                                WHERE
	                                cod_pais = :p_cod_pais 
	                                AND cod_comercio = :p_cod_comercio 
	                                AND caj_procesado = 'N' 
                                ORDER BY
	                                loc_numero";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter(":p_cod_pais", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codPais });
                    command.Parameters.Add(new NpgsqlParameter(":p_cod_comercio", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codComercio });

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                locales.Add(dr["loc_numero"] != DBNull.Value ? Convert.ToInt32(dr["loc_numero"]) : 0);
                            }
                        }
                        return locales;
                    }
                }
            }
        }

        public async Task<List<ASR_CajeroPaso>> ObtenerCajerosPorProcesarAsync(int codPais, int codComercio, int codLocal)
        {
            List<ASR_CajeroPaso> cajeros = new List<ASR_CajeroPaso>();

            using (var connection = new NpgsqlConnection(CadenasConexion.CadenaConexionCT3_SPSA))
            {
                await connection.OpenAsync();

                string query = @"SELECT *
                                FROM ct3m.cajeros_interface
                                WHERE cod_pais = :p_cod_pais AND cod_comercio = :p_cod_comercio AND loc_numero = :p_loc_numero AND caj_procesado = 'N'";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter(":p_cod_pais", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codPais });
                    command.Parameters.Add(new NpgsqlParameter(":p_cod_comercio", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codComercio });
                    command.Parameters.Add(new NpgsqlParameter(":p_loc_numero", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codLocal });

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                cajeros.Add(new ASR_CajeroPaso
                                {
                                    CajCodigo = dr["caj_codigo"] as string ?? null,
                                    LocNumero = dr["loc_numero"] != DBNull.Value ? Convert.ToInt32(dr["loc_numero"]) : 0,
                                    CajNombre = dr["caj_nombre"] as string ?? null,
                                    CajRut = dr["caj_rut"] as string ?? null,
                                    CajTipo = dr["caj_tipo"] as string ?? null,
                                    CajActivo = dr["caj_activo"] as string ?? null,
                                    CajCodigoEmp = dr["caj_codigo_emp"] as string ?? null,
                                    CajEstado = dr["caj_estado"] as string ?? null,
                                    CajNom = dr["caj_nom"] as string ?? null,
                                    CajApellidos = dr["caj_apellidos"] as string ?? null,
                                    CajTipoDocId = dr["caj_tipo_docid"] as string ?? null,
                                    CajFcreacion = dr["caj_fcreacion"] != DBNull.Value ? (DateTime?)dr["caj_fcreacion"] : null,
                                    CajFbaja = dr["caj_fbaja"] != DBNull.Value ? (DateTime?)dr["caj_fbaja"] : null,
                                    Cajfactualiza = dr["caj_factualiza"] != DBNull.Value ? (DateTime?)dr["caj_factualiza"] : null,
                                    CajTipoContrato = dr["caj_tipo_contrato"] as string ?? null,
                                    CajCorrExtranjero = dr["caj_corr_extranjero"] as string ?? null,
                                    CodPais = dr["cod_pais"] != DBNull.Value ? Convert.ToInt32(dr["cod_pais"]) : 0,
                                    CodComercio = dr["cod_comercio"] != DBNull.Value ? Convert.ToInt32(dr["cod_comercio"]) : 0
                                });
                            }
                        }
                        return cajeros;
                    }
                }
            }
        }

        public async Task ActualizarFlagProcesadoAsync(int codPais, int codComercio, int codLocal, string codCajero, string flagProcesado)
        {
            using (var connection = new NpgsqlConnection(CadenasConexion.CadenaConexionCT3_SPSA))
            {
                await connection.OpenAsync();

                string query = @"UPDATE ""ct3m"".""cajeros_interface""
                                    SET ""caj_procesado""  = :p_caj_procesado
                                WHERE ""cod_pais"" = :p_cod_pais
                                    AND ""cod_comercio"" = :p_cod_comercio
                                    AND ""loc_numero"" = :p_loc_numero
                                    AND ""caj_codigo"" = :p_caj_codigo";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter(":p_cod_pais", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codPais });
                    command.Parameters.Add(new NpgsqlParameter(":p_cod_comercio", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codComercio });
                    command.Parameters.Add(new NpgsqlParameter(":p_loc_numero", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = codLocal });
                    command.Parameters.Add(new NpgsqlParameter(":p_caj_codigo", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codCajero });
                    command.Parameters.Add(new NpgsqlParameter(":p_caj_procesado", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = flagProcesado });

                    var fdfd = await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task NuevoCajeroAsync(ASR_CajeroPaso cajero)
        {
            try
            {
                var usuario = ConstruirUsuario(cajero.CajUsuarioCrea);

                // 1. Elimina paso previo
                await EliminarCajeroPaso(cajero.CajRut);

                // 2. Inserta paso
                await InsertarCajeroPaso(cajero);

                // 3. Importa cajeros
                var (ret, err) = await CallImportarCajerosAsync(cajero.LocNumero, usuario);
                if (ret < 0)
                    throw new ApplicationException($"Importar cajeros falló: {err}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Importar cajeros falló: {ex.Message}");
            }

        }

        private string ConstruirUsuario(string codUsuario)
        {
            var padded = codUsuario.PadLeft(12, '0');
            return codUsuario.Substring(0, 2) + padded.Substring(4, 8);
        }

        public async Task EliminarCajeroPaso(string rut)
        {
            using (var connection = new NpgsqlConnection(CadenasConexion.CadenaConexionCT3_SPSA))
            {
                await connection.OpenAsync();

                string query = @"DELETE FROM ""ct3m"".""irs_cajeros_paso"" WHERE caj_rut = :caj_rut";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter(":caj_rut", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = rut ?? (object)DBNull.Value });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task InsertarCajeroPaso(ASR_CajeroPaso cajero)
        {
            using (var connection = new NpgsqlConnection(CadenasConexion.CadenaConexionCT3_SPSA))
            {
                await connection.OpenAsync();

                string query = @"INSERT
	                                    INTO
	                                    ""ct3m"".""irs_cajeros_paso"" (
                                        ""caj_codigo"",
	                                    ""loc_numero"",
	                                    ""caj_nombre"",
	                                    ""caj_nom"",
	                                    ""caj_apellidos"",
	                                    ""caj_rut"",
	                                    ""caj_tipo"",
	                                    ""caj_tipo_contrato"",
	                                    ""caj_tipo_docid"",
	                                    ""caj_codigo_emp"",
	                                    ""caj_fcreacion"",
	                                    ""caj_usuario_crea"",
	                                    ""cod_pais"",
	                                    ""cod_comercio"")
                                    VALUES (
                                    :caj_codigo,
                                    :loc_numero,
                                    :caj_nombre,
                                    :caj_nom,
                                    :caj_apellidos,
                                    :caj_rut,
                                    :caj_tipo,
                                    :caj_tipo_contrato,
                                    :caj_tipo_docid,
                                    :caj_codigo_emp,
                                    :caj_fcreacion,
                                    :caj_usuario_crea,
                                    :cod_pais,
                                    :cod_comercio)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter(":caj_codigo", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajCodigo });
                    command.Parameters.Add(new NpgsqlParameter(":loc_numero", NpgsqlTypes.NpgsqlDbType.Integer) { Value = cajero.LocNumero });
                    command.Parameters.Add(new NpgsqlParameter(":caj_nombre", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajNombre ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_nom", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajNom ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_apellidos", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajApellidos ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_rut", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajRut ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_tipo", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajTipo ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_tipo_contrato", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajTipoContrato ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_tipo_docid", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajTipoDocId ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":caj_codigo_emp", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajCodigoEmp ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_estado", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajEstado ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_activo", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajActivo ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_login", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajLogin ?? (object)DBNull.Value });

                    // Manejo de fechas nulas
                    command.Parameters.Add(new NpgsqlParameter(":caj_fcreacion", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = cajero.CajFcreacion ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_fbaja", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = cajero.CajFbaja ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_factualiza", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = cajero.Cajfactualiza ?? (object)DBNull.Value });

                    // Resto de campos
                    command.Parameters.Add(new NpgsqlParameter(":caj_usuario_crea", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajUsuarioCrea ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_usuario_baja", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajUsuarioBaja ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_usuario_actualiza", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajUsuarioActualiza ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_corr_extranjero", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajCorrExtranjero ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_rendauto", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajRendAuto ?? (object)DBNull.Value });
                    //command.Parameters.Add(new NpgsqlParameter(":caj_carga", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = cajero.CajCarga ?? (object)DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter(":cod_pais", NpgsqlTypes.NpgsqlDbType.Integer) { Value = cajero.CodPais });
                    command.Parameters.Add(new NpgsqlParameter(":cod_comercio", NpgsqlTypes.NpgsqlDbType.Integer) { Value = cajero.CodComercio });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<(int retorno, string errm)> CallImportarCajerosAsync(int locNumero, string usuario)
        {
            using (var connection = new NpgsqlConnection(CadenasConexion.CadenaConexionCT3_SPSA))
            {
                await connection.OpenAsync();

                const string query = @"
                CALL ""ct3m"".""pkg_mant_cajeros_importar_cajeros""(
                    @loc_numero,
                    @usuario,
                    @p_retorno,
                    @p_errm
                );
            ";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("loc_numero", locNumero);
                    cmd.Parameters.AddWithValue("usuario", usuario);

                    var pRet = cmd.Parameters.Add("@p_retorno", NpgsqlTypes.NpgsqlDbType.Integer);
                    pRet.Direction = ParameterDirection.Output;

                    var pErr = cmd.Parameters.Add("@p_errm", NpgsqlTypes.NpgsqlDbType.Varchar, 4000);
                    pErr.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    return ((int)pRet.Value, pErr.Value as string);
                }
            }
        }

    }
}
