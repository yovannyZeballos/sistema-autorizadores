using Npgsql;
using NpgsqlTypes;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
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

                using (var command = new NpgsqlCommand(@"CALL ""SGP"".""SP_ASR_ACTUALIZAR_MOTIVO_RECHAZO""(@P_NUM_SOLICITUD, @P_MOTIVO, @P_IND_APROBADO)", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@P_NUM_SOLICITUD", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = numSolicitud });
                    command.Parameters.Add(new NpgsqlParameter("@P_MOTIVO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = motivo });
                    command.Parameters.Add(new NpgsqlParameter("@P_IND_APROBADO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = estado });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AprobarSolicitud(ASR_Usuario usuario)
        {
            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(@"CALL ""SGP"".""SP_ASR_APROBAR_SOLICITUD""(@P_NUM_SOLICITUD, @P_IND_ACTIVO, @P_FLG_ENVIO, @P_FEC_ENVIO, @P_USU_AUTORIZA, @P_USU_CREACION, @P_USU_ELIMINA, @P_FEC_ELIMINA)", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@P_NUM_SOLICITUD", NpgsqlTypes.NpgsqlDbType.Integer) { Value = usuario.NumSolicitud });
                    command.Parameters.Add(new NpgsqlParameter("@P_IND_ACTIVO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.IndActivo ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@P_FLG_ENVIO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.FlgEnvio ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@P_FEC_ENVIO", NpgsqlTypes.NpgsqlDbType.Date) { Value = (object)usuario.FecEnvio ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@P_USU_AUTORIZA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuAutoriza ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@P_USU_CREACION", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuCreacion ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@P_USU_ELIMINA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuElimina ?? DBNull.Value });
                    command.Parameters.Add(new NpgsqlParameter("@P_FEC_ELIMINA", NpgsqlTypes.NpgsqlDbType.Date) { Value = (object)usuario.FecElimina ?? DBNull.Value });

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
                    SELECT * FROM ""SGP"".""SF_ASR_SOLICITUD_USUARIO_LISTAR""(@P_USUARIO_LOGIN, @P_TIP_USUARIO, @P_COD_EMPRESA, @P_PAGE_NUMBER, @P_PAGE_SIZE)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@P_USUARIO_LOGIN", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuarioLogin });
                    command.Parameters.Add(new NpgsqlParameter("@P_TIP_USUARIO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = tipoUsuario });
                    command.Parameters.Add(new NpgsqlParameter("@P_COD_EMPRESA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CodEmpresa });
                    command.Parameters.Add(new NpgsqlParameter("@P_PAGE_NUMBER", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numeroPagina });
                    command.Parameters.Add(new NpgsqlParameter("@P_PAGE_SIZE", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tamañoPagina });

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
                    SELECT * FROM ""SGP"".""SF_ASR_USUARIO_LISTAR""(@P_USUARIO_LOGIN, @P_COD_LOCAL, @P_USU_APROBACION, @P_TIP_ACCION, @P_IND_APROBADO, @P_COD_EMPRESA, @P_PAGE_NUMBER, @P_PAGE_SIZE)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@P_USUARIO_LOGIN", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuarioLogin });
                    command.Parameters.Add(new NpgsqlParameter("@P_COD_LOCAL", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codLocal });
                    command.Parameters.Add(new NpgsqlParameter("@P_USU_APROBACION", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuAprobacion });
                    command.Parameters.Add(new NpgsqlParameter("@P_TIP_ACCION", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = tipAccion });
                    command.Parameters.Add(new NpgsqlParameter("@P_IND_APROBADO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = indAprobado });
                    command.Parameters.Add(new NpgsqlParameter("@P_COD_EMPRESA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CodEmpresa });
                    command.Parameters.Add(new NpgsqlParameter("@P_PAGE_NUMBER", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numeroPagina });
                    command.Parameters.Add(new NpgsqlParameter("@P_PAGE_SIZE", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tamañoPagina });

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
                    query = @"SELECT * FROM ""SGP"".""SF_ASR_GENERAR_ARCHIVO_AUTORIZADOR""()";
                }
                else
                {
                    query = @"SELECT * FROM ""SGP"".""SF_ASR_GENERAR_ARCHIVO_CAJERO""()";
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

                using (var command = new NpgsqlCommand(@"CALL ""SGP"".""SP_ASR_ACTUALIZAR_FLG_ENVIO""(@P_NUM_SOLICITUD, @P_FLG_ENVIO)", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@P_NUM_SOLICITUD", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numSolicitud });
                    command.Parameters.Add(new NpgsqlParameter("@P_FLG_ENVIO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = flagEnvio });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

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
            var usuario = ConstruirUsuario(cajero.CajUsuarioCrea);

            // 1) Elimina paso previo
            await EliminarCajeroPaso(cajero.CajRut);

            // 2) Inserta paso
            await InsertarCajeroPaso(cajero);

            // 3) Importa cajeros
            var (ret, err) = await CallImportarCajerosAsync(cajero.LocNumero, usuario);
            if (ret < 0)
                throw new ApplicationException($"Importar cajeros falló: {err}");

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
                    //command.Parameters.Add(new NpgsqlParameter(":cod_pais", NpgsqlTypes.NpgsqlDbType.Integer) { Value = cajero.CodPais });
                    //command.Parameters.Add(new NpgsqlParameter(":cod_comercio", NpgsqlTypes.NpgsqlDbType.Integer) { Value = cajero.CodComercio });

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

        public async Task<(int retorno, string errm)> CallImportarCajerosAsync(int locNumero,  string usuario)
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
