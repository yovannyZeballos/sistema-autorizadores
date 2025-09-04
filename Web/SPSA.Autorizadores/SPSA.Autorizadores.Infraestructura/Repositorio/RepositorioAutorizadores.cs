using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Types;
using System.Diagnostics;
using Npgsql;
using NpgsqlTypes;
using System.IO;
using System.Linq;
using System.Text;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioAutorizadores : CadenasConexion, IRepositorioAutorizadores
    {
        private readonly int _commandTimeout;

        public RepositorioAutorizadores()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task Crear(Autorizador autorizador)
        {
            const string sql = @"
                                SELECT *
                                FROM ""SGP"".autorizador_fn_registra_sup_manual(
                                    @p_codemp,
                                    @p_nomemp,
                                    @p_apepat,
                                    @p_apemat,
                                    @p_numdoc,
                                    @p_estemp,
                                    @p_local,
                                    @p_usrcre
                                )";

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = _commandTimeout;

                    cmd.Parameters.Add(new NpgsqlParameter("@p_codemp", NpgsqlDbType.Text) { Value = autorizador.Codigo });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_nomemp", NpgsqlDbType.Text) { Value = autorizador.Nombres });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_apepat", NpgsqlDbType.Text) { Value = autorizador.ApellidoPaterno });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_apemat", NpgsqlDbType.Text) { Value = autorizador.ApellidoMaterno });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_numdoc", NpgsqlDbType.Text) { Value = autorizador.NumeroDocumento });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_estemp", NpgsqlDbType.Text) { Value = autorizador.Estado });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_local", NpgsqlDbType.Integer) { Value = autorizador.CodLocal });
                    cmd.Parameters.Add(new NpgsqlParameter("@p_usrcre", NpgsqlDbType.Text) { Value = autorizador.UsuarioCreacion });

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var error = reader.GetInt32(reader.GetOrdinal("pinnu_error"));
                            var mensaje = reader.GetString(reader.GetOrdinal("pinvc_msgerr"));

                            if (error == -1)
                                throw new Exception(mensaje);
                        }
                        else
                        {
                            throw new Exception("La función no devolvió resultado.");
                        }
                    }
                }
            }
        }


        //public async Task Crear(Autorizador autorizador)
        //{
        //    using (var connection = new OracleConnection(CadenaConexionAutorizadores))
        //    {
        //        var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_REGISTRA_SUP_MANUAL", connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.CommandTimeout = _commandTimeout;

        //        await command.Connection.OpenAsync();
        //        command.Parameters.Add("PINVC_CODEMP", OracleDbType.Varchar2, autorizador.Codigo, ParameterDirection.Input);
        //        command.Parameters.Add("PINVC_NOMEMP", OracleDbType.Varchar2, autorizador.Nombres, ParameterDirection.Input);
        //        command.Parameters.Add("PINVC_APEPAT", OracleDbType.Varchar2, autorizador.ApellidoPaterno, ParameterDirection.Input);
        //        command.Parameters.Add("PINVC_APEMAT", OracleDbType.Varchar2, autorizador.ApellidoMaterno, ParameterDirection.Input);
        //        command.Parameters.Add("PINVC_NUMDOC", OracleDbType.Varchar2, autorizador.NumeroDocumento, ParameterDirection.Input);
        //        command.Parameters.Add("PINVC_ESTEMP", OracleDbType.Varchar2, autorizador.Estado, ParameterDirection.Input);
        //        command.Parameters.Add("PINNU_LOCAL", OracleDbType.Decimal, autorizador.CodLocal, ParameterDirection.Input);
        //        command.Parameters.Add("PINVC_USRCRE", OracleDbType.Varchar2, autorizador.UsuarioCreacion, ParameterDirection.Input);
        //        command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
        //        command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

        //        await command.ExecuteNonQueryAsync();

        //        var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
        //        var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

        //        if (error == -1)
        //            throw new Exception(mensjaeError);

        //        connection.Close();
        //        connection.Dispose();
        //    }
        //}

        public async Task<string> GenerarArchivo(string tipoSO)
        {
            // 1) Ajusta esta ruta (puede venir de appSettings)
            var outputDirectory = @"C:\archivos\dev"; // p.ej. para Windows service / IIS

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var sbResultado = new StringBuilder();

            // Diccionario: fileName -> (lines, aut_ids)
            var files = new Dictionary<string, (List<string> Lines, List<string> AutIds, string LocText)>(StringComparer.OrdinalIgnoreCase);

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            {
                await connection.OpenAsync();

                // 2) Obtener contenido desde Postgres
                const string sqlContenido = @"SELECT loc_text, nom_arch, linea, aut_id FROM ""SGP"".autorizador_fn_contenido_archivo(@p_tipo)";

                using (var cmd = new NpgsqlCommand(sqlContenido, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = _commandTimeout;
                    cmd.Parameters.Add(new NpgsqlParameter("@p_tipo", NpgsqlDbType.Text) { Value = tipoSO });

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var locText = reader.GetString(reader.GetOrdinal("loc_text"));
                            var nomArch = reader.GetString(reader.GetOrdinal("nom_arch"));
                            var linea = reader.GetString(reader.GetOrdinal("linea"));
                            var autId = reader.GetString(reader.GetOrdinal("aut_id"));

                            if (!files.TryGetValue(nomArch, out var bucket))
                                files[nomArch] = (new List<string>(), new List<string>(), locText);

                            files[nomArch].Lines.Add(linea);
                            files[nomArch].AutIds.Add(autId);
                        }
                    }
                }

                // Si no hay nada que generar, devolvemos vacío
                if (files.Count == 0)
                    return string.Empty;

                // 3) Escribir cada archivo y acumular ids “marcables”
                var autIdsMarcados = new List<string>();
                foreach (var kv in files)
                {
                    var fileName = kv.Key;
                    var (lines, autIds, locText) = kv.Value;
                    var fullPath = Path.Combine(outputDirectory, fileName);

                    try
                    {
                        // UTF-8 sin BOM (similar a COPY ENCODING 'UTF8')
                        using (var sw = new StreamWriter(fullPath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
                        {
                            foreach (var line in lines)
                                sw.WriteLine(line);
                        }

                        // Marcar este grupo como “enviado” (si luego falla el update, entrará al catch)
                        autIdsMarcados.AddRange(autIds);

                        // Línea de salida tipo Oracle: dir|archivo|existe|tamaño|<sin_block>
                        var len = new FileInfo(fullPath).Length;
                        sbResultado.Append(outputDirectory)
                                   .Append('|').Append(fileName)
                                   .Append('|').Append('1')      // existe
                                   .Append('|').Append(len)      // tamaño en bytes
                                   .Append('|').Append("")       // no tenemos block_size -> vacío
                                   .Append('\n');
                    }
                    catch (Exception exWrite)
                    {
                        // Si no pudimos escribir este archivo, NO marcamos esos aut_ids
                        // y devolvemos “no existe”
                        sbResultado.Append(outputDirectory)
                                   .Append('|').Append(fileName)
                                   .Append('|').Append('0')      // no existe / fallo al escribir
                                   .Append('|').Append('0')
                                   .Append('|').Append("")
                                   .Append('\n');

                        // Loguea si quieres
                        // EventLog.WriteEntry("SGP_Autorizadores", $"[ERROR escribir {fileName}] {exWrite}", EventLogEntryType.Error);
                    }
                }

                // 4) Marcar enviados en BD (solo los que sí se escribieron)
                if (autIdsMarcados.Count > 0)
                {
                    const string sqlMarcar = @"SELECT ""SGP"".autorizador_fn_marcar_enviados(@p_ids)";

                    using (var cmdMark = new NpgsqlCommand(sqlMarcar, connection))
                    {
                        cmdMark.CommandType = CommandType.Text;
                        cmdMark.CommandTimeout = _commandTimeout;

                        var arr = autIdsMarcados.Distinct().ToArray();
                        var p = new NpgsqlParameter("@p_ids", NpgsqlDbType.Array | NpgsqlDbType.Text) { Value = arr };
                        cmdMark.Parameters.Add(p);

                        await cmdMark.ExecuteNonQueryAsync();
                    }
                }

                return sbResultado.ToString();
            }
        }

        //public async Task<string> GenerarArchivo(string tipoSO)
        //{
        //    using (var connection = new OracleConnection(CadenaConexionAutorizadores))
        //    {
        //        try
        //        {
        //            using (var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_GENERA_ARCHIVO_TIPO", connection)
        //            {
        //                CommandType = CommandType.StoredProcedure,
        //                CommandTimeout = _commandTimeout
        //            })
        //            {
        //                await connection.OpenAsync();

        //                command.Parameters.Add("vTIPO_SO", OracleDbType.Varchar2, tipoSO, ParameterDirection.Input);
        //                command.Parameters.Add("resultado", OracleDbType.Clob, ParameterDirection.Output);

        //                await command.ExecuteNonQueryAsync();

        //                var value = command.Parameters["resultado"].Value;

        //                if (value != DBNull.Value && value is OracleClob clob)
        //                {
        //                    using (clob) // <-- Esto llama Dispose automáticamente
        //                    {
        //                        return clob.IsNull ? string.Empty : clob.Value;
        //                    }
        //                }
        //                return string.Empty;
        //            }
        //        }
        //        catch (OracleException ex) when (ex.Number == 29283)
        //        {
        //            OracleConnection.ClearPool(connection); // <== La magia que evita reiniciar IIS
        //            EventLog.WriteEntry("SGP_Autorizadores", $"[ORA-29283], Error: {ex}", EventLogEntryType.Error);
        //            return string.Empty;
        //        }
        //        catch (Exception ex)
        //        {
        //            EventLog.WriteEntry("SGP_Autorizadores", $"[ERROR GenerarArchivo], Error: {ex}", EventLogEntryType.Error);
        //            return string.Empty;
        //        }
        //    }
        //}

        public async Task<DataTable> ListarColaboradores(string codigoLocal, string codigoEmpresa)
        {
            const string sql = @"SELECT * FROM ""SGP"".autorizador_fn_mant_lista_cola_ofisis(@p_empr, @p_local)";

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = _commandTimeout;

                command.Parameters.Add(new NpgsqlParameter("@p_empr", NpgsqlDbType.Text) { Value = codigoEmpresa });
                command.Parameters.Add(new NpgsqlParameter("@p_local", NpgsqlDbType.Text) { Value = codigoLocal });

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        //public async Task<DataTable> ListarColaboradores(string codigoLocal, string codigoEmpresa)
        //{
        //    using (var connection = new OracleConnection(CadenaConexionAutorizadores))
        //    {
        //        var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MANT_LISTA_COLA_OFISIS", connection)
        //        {
        //            CommandType = CommandType.StoredProcedure,
        //            CommandTimeout = _commandTimeout
        //        };

        //        await command.Connection.OpenAsync();
        //        command.Parameters.Add("vCOD_EMPR", OracleDbType.Varchar2, codigoEmpresa, ParameterDirection.Input);
        //        command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codigoLocal, ParameterDirection.Input);
        //        command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

        //        var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        //        var datatable = new DataTable();
        //        datatable.Load(dr);


        //        connection.Close();
        //        connection.Dispose();

        //        return datatable;

        //    }

        //}

        public async Task<DataTable> ListarAutorizador(string codigoLocal)
        {
            var localParam = 0;
            if (!string.IsNullOrWhiteSpace(codigoLocal) && int.TryParse(codigoLocal, out var parsed))
                localParam = parsed;

            const string sql = @"SELECT * FROM ""SGP"".autorizador_fn_mant_lista_cola_autorizados(@p_local)";

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = _commandTimeout;

                command.Parameters.Add(new NpgsqlParameter("@p_local", NpgsqlDbType.Integer) { Value = localParam });

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    var datatable = new DataTable();
                    datatable.Load(reader);
                    return datatable;
                }
            }
        }

        //public async Task<DataTable> ListarAutorizador(string codigoLocal)
        //{
        //    var colaboradores = new List<Autorizador>();
        //    using (var connection = new OracleConnection(CadenaConexionAutorizadores))
        //    {
        //        var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MANT_LISTA_COLA_AUTORIZADOS", connection)
        //        {
        //            CommandType = CommandType.StoredProcedure,
        //            CommandTimeout = _commandTimeout
        //        };

        //        await command.Connection.OpenAsync();
        //        command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codigoLocal, ParameterDirection.Input);
        //        command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

        //        var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        //        var datatable = new DataTable();
        //        datatable.Load(dr);


        //        connection.Close();
        //        connection.Dispose();

        //        return datatable;

        //        //if (dr != null && dr.HasRows)
        //        //{
        //        //    while (await dr.ReadAsync())
        //        //    {
        //        //        colaboradores.Add(new Autorizador
        //        //        (
        //        //            dr["emp_codigo"].ToString(),
        //        //            dr["emp_nombre"].ToString(),
        //        //            dr["emp_apepat"].ToString(),
        //        //            dr["emp_apemat"].ToString(),
        //        //            dr["emp_numdoc"].ToString(),
        //        //            dr["emp_estado"].ToString(),
        //        //            Convert.ToInt32(dr["loc_numero"]),
        //        //            dr["EMP_USRCREA"].ToString(),
        //        //            dr["AUT_CODAUTORIZA"].ToString(),
        //        //            Convert.ToDateTime(dr["EMP_FECCREA"]).ToString("dd/MM/yyyy"),
        //        //            dr["tar_numtar"].ToString(),
        //        //            dr["TAR_ESTIMP"].ToString() == "A" ? "S" : "N"
        //        //        ));
        //        //    }
        //        //}

        //        //connection.Close();
        //        //connection.Dispose();

        //        //return colaboradores;
        //    }

        //}

        public async Task ActualizarEstadoArchivo(Autorizador autorizador)
        {
            const string sql = @"SELECT * FROM ""SGP"".autorizador_fn_cambio_generacion_archivo(
                                    @paut_codautoriza,
                                    @ptar_numtar,
                                    @pinnu_local,
                                    @pemp_codigo
                                );";

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = _commandTimeout;

                // Parámetros
                command.Parameters.Add(new NpgsqlParameter("@paut_codautoriza", NpgsqlDbType.Text) { Value = autorizador.CodigoAutorizador });
                command.Parameters.Add(new NpgsqlParameter("@ptar_numtar", NpgsqlDbType.Text) { Value = autorizador.NumeroTarjeta });
                command.Parameters.Add(new NpgsqlParameter("@pinnu_local", NpgsqlDbType.Numeric) { Value = autorizador.CodLocal });
                command.Parameters.Add(new NpgsqlParameter("@pemp_codigo", NpgsqlDbType.Text) { Value = autorizador.Codigo });

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var error = reader.GetInt32(0);       // columna pinnu_error
                        var mensaje = reader.GetString(1);   // columna pinvc_msgerr

                        if (error == -1)
                            throw new Exception(mensaje);
                    }
                }
            }
        }

        //public async Task ActualizarEstadoArchivo(Autorizador autorizador)
        //{
        //    using (var connection = new OracleConnection(CadenaConexionAutorizadores))
        //    {
        //        var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_CAMBIO_GENERACION_ARCHIVO", connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.CommandTimeout = _commandTimeout;

        //        await command.Connection.OpenAsync();
        //        command.Parameters.Add("PAUT_CODAUTORIZA", OracleDbType.Varchar2, autorizador.CodigoAutorizador, ParameterDirection.Input);
        //        command.Parameters.Add("PTAR_NUMTAR", OracleDbType.Varchar2, autorizador.NumeroTarjeta, ParameterDirection.Input);
        //        command.Parameters.Add("PINNU_LOCAL", OracleDbType.Varchar2, autorizador.CodLocal, ParameterDirection.Input);
        //        command.Parameters.Add("PEMP_CODIGO", OracleDbType.Varchar2, autorizador.Codigo, ParameterDirection.Input);
        //        command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
        //        command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

        //        await command.ExecuteNonQueryAsync();

        //        var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
        //        var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

        //        if (error == -1)
        //            throw new Exception(mensjaeError);

        //        connection.Close();
        //        connection.Dispose();
        //    }
        //}

        public async Task Eliminar(Autorizador autorizador)
        {
            const string sql = @"SELECT * FROM ""SGP"".autorizador_fn_eliminar_autorizador(
                                    @paut_codautoriza,
                                    @pemp_codigo,
                                    @paut_usrmod
                                );";

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = _commandTimeout;

                command.Parameters.Add(new NpgsqlParameter("@paut_codautoriza", NpgsqlDbType.Text) { Value = autorizador.CodigoAutorizador });
                command.Parameters.Add(new NpgsqlParameter("@pemp_codigo", NpgsqlDbType.Text) { Value = autorizador.Codigo });
                command.Parameters.Add(new NpgsqlParameter("@paut_usrmod", NpgsqlDbType.Text) { Value = autorizador.UsuarioCreacion });

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    if (!await reader.ReadAsync())
                        throw new Exception("La función no devolvió resultado.");

                    var error = reader.GetInt32(0);   // pinnu_error
                    var mensaje = reader.GetString(1);  // pinvc_msgerr

                    if (error == -1)
                        throw new Exception(mensaje);
                }
            }
        }

        //public async Task Eliminar(Autorizador autorizador)
        //{
        //    using (var connection = new OracleConnection(CadenaConexionAutorizadores))
        //    {
        //        var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_ELIMINAR_AUTORIZADOR", connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.CommandTimeout = _commandTimeout;

        //        await command.Connection.OpenAsync();
        //        command.Parameters.Add("PAUT_CODAUTORIZA", OracleDbType.Varchar2, autorizador.CodigoAutorizador, ParameterDirection.Input);
        //        command.Parameters.Add("PEMP_CODIGO", OracleDbType.Varchar2, autorizador.Codigo, ParameterDirection.Input);
        //        command.Parameters.Add("PAUT_USRMOD", OracleDbType.Varchar2, autorizador.UsuarioCreacion, ParameterDirection.Input);
        //        command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
        //        command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

        //        await command.ExecuteNonQueryAsync();

        //        var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
        //        var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

        //        if (error == -1)
        //            throw new Exception(mensjaeError);

        //        connection.Close();
        //        connection.Dispose();
        //    }
        //}

        public async Task<DataTable> ListarColaboradoresCesados()
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_G3_LISTA_ELIMINADOS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async Task<DataTable> ListarColaboradoresMass(string codEmpresa)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_G6_LISTA_COLAB_MASS_SIN_AUT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("VCOD_EMPRESA", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async Task ActualizarEstadoArchivoPorLocal(string locales)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_CAMBIO_ESTADO_AUT_LOCAL", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                await command.Connection.OpenAsync();
                command.Parameters.Add("PINNU_LOCALES", OracleDbType.Varchar2, locales, ParameterDirection.Input);
                command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
                command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
                var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

                if (error == -1)
                    throw new Exception(mensjaeError);

                connection.Close();
                connection.Dispose();
            }
        }

        public async Task<string> GenerarArchivoLocal(decimal codLocal, string tipoSO)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                try
                {
                    using (var command = new OracleCommand("PKG_SGC_CAJERO.SP_GENERA_ARCHIVO_TIPO_LOCAL", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = _commandTimeout
                    })
                    {
                        await connection.OpenAsync();

                        command.Parameters.Add("vTIPO_SO", OracleDbType.Varchar2, tipoSO, ParameterDirection.Input);
                        command.Parameters.Add("PINNU_LOCAL", OracleDbType.Decimal, codLocal, ParameterDirection.Input);
                        command.Parameters.Add("resultado", OracleDbType.Varchar2, 500, "", ParameterDirection.Output);

                        await command.ExecuteNonQueryAsync();

                        var value = command.Parameters["resultado"].Value;

                        if (value != DBNull.Value && value is OracleClob clob)
                        {
                            using (clob) // <-- Esto llama Dispose automáticamente
                            {
                                return clob.IsNull ? string.Empty : clob.Value;
                            }
                        }

                        return string.Empty;
                    }
                }
                catch (OracleException ex) when (ex.Number == 29283)
                {
                    OracleConnection.ClearPool(connection); // <== La magia que evita reiniciar IIS
                    EventLog.WriteEntry("SGP_Autorizadores", $"[ORA-29283], Error: {ex}", EventLogEntryType.Error);
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("SGP_Autorizadores", $"[ERROR GenerarArchivo], Error: {ex}", EventLogEntryType.Error);
                    return string.Empty;
                }
            }
        }

    }
}
