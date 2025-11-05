using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ExcelDataReader;
using MediatR;
using Npgsql;
using NpgsqlTypes;
using SPSA.Autorizadores.Aplicacion.DTO;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.Bines
{
    public class ImportarMdrBinesInretailCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
        public long CodPeriodo { get; set; }
    }

    public class ImportarMdrBinesInretailHandler : IRequestHandler<ImportarMdrBinesInretailCommand, RespuestaComunExcelDTO>
    {
        private readonly string _connectionString;

        public ImportarMdrBinesInretailHandler()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMdrBinesInretailCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Ok = true, Errores = new List<ErroresExcelDTO>() };

            try
            {
                if (request.CodPeriodo <= 0)
                {
                    return new RespuestaComunExcelDTO
                    {
                        Ok = false,
                        Mensaje = "El período seleccionado es inválido.",
                        Errores = new List<ErroresExcelDTO>()
                    };
                }

                if (request.Archivo == null || request.Archivo.ContentLength == 0)
                {
                    return new RespuestaComunExcelDTO
                    {
                        Ok = false,
                        Mensaje = "No se encontró ningún archivo para procesar.",
                        Errores = new List<ErroresExcelDTO>()
                    };
                }

                string ext = Path.GetExtension(request.Archivo.FileName).ToLower();
                if (ext != ".xlsx")
                {
                    return new RespuestaComunExcelDTO
                    {
                        Ok = false,
                        Mensaje = "Sólo se permiten archivos con extensión .xlsx.",
                        Errores = new List<ErroresExcelDTO>()
                    };
                }

                // ------------------------------------------------------------------
                // 1) Leer Excel -> DataTable (elige la hoja más grande)
                // ------------------------------------------------------------------
                DataTable dt = null;
                using (var stream = request.Archivo.InputStream)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                    };
                    var ds = reader.AsDataSet(conf);

                    if (ds.Tables.Count == 0)
                        return new RespuestaComunExcelDTO { Ok = false, Mensaje = "El archivo Excel no contiene hojas.", Errores = new List<ErroresExcelDTO>() };

                    // Selecciona la hoja más “grande” (más columnas y filas) para evitar depender del índice
                    int bestIdx = 0;
                    int bestScore = -1;
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        var t = ds.Tables[i];
                        int score = (t?.Columns?.Count ?? 0) + (t?.Rows?.Count ?? 0);
                        if (score > bestScore) { bestScore = score; bestIdx = i; }
                    }
                    dt = ds.Tables[bestIdx];
                }

                // ------------------------------------------------------------------
                // 2) Catálogos: operadores y clasificaciones
                // operadorMap: key = nom_operador normalizado -> cod_operador (01,02,...)
                // clasifMap: key = codOperador + '|' + nom_clasificacion normalizado -> cod_clasificacion (00,01,...)
                // ------------------------------------------------------------------
                var operadorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var clasifMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync(cancellationToken);

                    // Operadores
                    const string sqlOps = @"SELECT ""cod_operador"", ""nom_operador"" FROM ""SGP"".""mdr_operador"";";
                    using (var cmd = new NpgsqlCommand(sqlOps, conn))
                    using (var rd = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await rd.ReadAsync(cancellationToken))
                        {
                            var cod = (rd["cod_operador"]?.ToString() ?? "").Trim();
                            var nom = (rd["nom_operador"]?.ToString() ?? "").Trim();
                            if (!string.IsNullOrEmpty(cod) && !string.IsNullOrEmpty(nom))
                            {
                                var key = NormalizaClave(nom);
                                if (!operadorMap.ContainsKey(key)) operadorMap.Add(key, cod);
                            }
                        }
                    }

                    // Clasificaciones (incluye '00' S/N que ya definiste)
                    const string sqlClas = @"SELECT ""cod_operador"", ""cod_clasificacion"", ""nom_clasificacion""
                                             FROM ""SGP"".""mdr_clasificacion"";";
                    using (var cmd = new NpgsqlCommand(sqlClas, conn))
                    using (var rd = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await rd.ReadAsync(cancellationToken))
                        {
                            var codOp = (rd["cod_operador"]?.ToString() ?? "").Trim();
                            var codCla = (rd["cod_clasificacion"]?.ToString() ?? "").Trim();
                            var nom = (rd["nom_clasificacion"]?.ToString() ?? "").Trim();
                            if (!string.IsNullOrEmpty(codOp) && !string.IsNullOrEmpty(codCla) && !string.IsNullOrEmpty(nom))
                            {
                                var key = codOp + "|" + NormalizaClave(nom);
                                if (!clasifMap.ContainsKey(key)) clasifMap.Add(key, codCla);
                            }
                        }
                    }
                }


                // ------------------------------------------------------------------
                // 3) Construir CSV (orden EXACTO de columnas en destino):
                //     longitud, bin_8_9, bin_6, marca, cod_operador, tipo_tarjeta,
                //     subtipo_tarjeta, subproducto, categoria_tarjeta, banco_emisor,
                //     traduccion_tarifario
                // ------------------------------------------------------------------
                var csvBuilder = new StringBuilder();

                // Helper local para escapar CSV + normalizar a UPPER y sin tildes
                string Escapar(object valor)
                {
                    if (valor == null) return "";
                    string s = valor.ToString().Trim();
                    if (string.IsNullOrEmpty(s)) return "";
                    s = QuitarDiacriticos(s).ToUpperInvariant();
                    if (s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n"))
                    {
                        var tmp = s.Replace("\"", "\"\"");
                        return $"\"{tmp}\"";
                    }
                    return s;
                }

                // Mapeo esperado de columnas en el Excel:
                // 0: longitud
                // 1: BIN 8 y 9
                // 2: BIN 6
                // 3: MARCA
                // 4: Tipo Tarjeta
                // 5: Subtipo Tarjeta
                // 6: Subproducto
                // 7: Categoria Tarjeta
                // 8: Banco Emisor
                // 9: Traducción tarifario InRetail
                int columnasMinimas = 10;

                for (int fila = 0; fila < dt.Rows.Count; fila++)
                {
                    try
                    {
                        var row = dt.Rows[fila];
                        if (row == null) continue;

                        if (dt.Columns.Count < columnasMinimas)
                            throw new Exception($"La hoja seleccionada no contiene al menos {columnasMinimas} columnas.");

                        string rawBin89 = (row[1]?.ToString() ?? "").Trim();
                        string rawBin6 = (row[2]?.ToString() ?? "").Trim();

                        // Omitir filas totalmente vacías
                        bool filaVacia =
                            string.IsNullOrWhiteSpace(rawBin89) &&
                            string.IsNullOrWhiteSpace(rawBin6) &&
                            string.IsNullOrWhiteSpace(row[3]?.ToString()) &&
                            string.IsNullOrWhiteSpace(row[4]?.ToString()) &&
                            string.IsNullOrWhiteSpace(row[5]?.ToString()) &&
                            string.IsNullOrWhiteSpace(row[6]?.ToString()) &&
                            string.IsNullOrWhiteSpace(row[7]?.ToString()) &&
                            string.IsNullOrWhiteSpace(row[8]?.ToString()) &&
                            string.IsNullOrWhiteSpace(row[9]?.ToString());
                        if (filaVacia) continue;

                        // Validaciones mínimas
                        if (string.IsNullOrWhiteSpace(rawBin89))
                        {
                            respuesta.Errores.Add(new ErroresExcelDTO { Fila = fila + 2, Mensaje = "BIN 8/9 vacío." });
                            continue;
                        }
                        if (rawBin89.Length != 8 && rawBin89.Length != 9)
                        {
                            respuesta.Errores.Add(new ErroresExcelDTO { Fila = fila + 2, Mensaje = $"BIN 8/9 con longitud inválida ({rawBin89.Length})." });
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(rawBin6) || rawBin6.Length != 6)
                        {
                            respuesta.Errores.Add(new ErroresExcelDTO { Fila = fila + 2, Mensaje = "BIN 6 vacío o con longitud distinta de 6." });
                            continue;
                        }

                        // longitud: si viene mal, corregimos igualando al largo del BIN 8/9
                        short longitudOk;
                        var rawLongitud = (row[0]?.ToString() ?? "").Trim();
                        if (!short.TryParse(rawLongitud, out longitudOk) || (longitudOk != 8 && longitudOk != 9) || longitudOk != rawBin89.Length)
                        {
                            longitudOk = (short)rawBin89.Length;
                            if (!string.IsNullOrEmpty(rawLongitud))
                            {
                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = fila + 2,
                                    Mensaje = $"Longitud corregida a {longitudOk} por inconsistencia con BIN 8/9."
                                });
                            }
                        }

                        // Operador desde MARCA
                        string marcaRaw = (row[3]?.ToString() ?? "").Trim();
                        string codOp = null;
                        if (!string.IsNullOrEmpty(marcaRaw))
                        {
                            var key = NormalizaClave(marcaRaw);
                            string found;
                            if (operadorMap.TryGetValue(key, out found))
                                codOp = found; // 01, 02...
                        }
                        if (string.IsNullOrEmpty(codOp))
                        {
                            // No hay operador mapeado -> no podemos cumplir NOT NULL + FK
                            respuesta.Errores.Add(new ErroresExcelDTO { Fila = fila + 2, Mensaje = $"Marca '{marcaRaw}' no mapeada en SGP.mdr_operador." });
                            continue;
                        }

                        // Clasificación desde "Traducción tarifario InRetail"
                        string nomClasRaw = (row[9]?.ToString() ?? "").Trim();
                        string codClas = null;
                        if (!string.IsNullOrWhiteSpace(nomClasRaw))
                        {
                            var ckey = codOp + "|" + NormalizaClave(nomClasRaw);
                            string found;
                            if (clasifMap.TryGetValue(ckey, out found))
                                codClas = found;
                        }
                        // Si no hay match, usar '00' (S/N) para el operador encontrado
                        if (string.IsNullOrEmpty(codClas)) codClas = "00";

                        // Banco emisor: si viene vacío y el tipo es FORÁNEO, usar FORÁNEO; en caso contrario "SIN BANCO"
                        string tipoTarjeta = (row[4]?.ToString() ?? "").Trim();
                        string rawBanco = (row[8]?.ToString() ?? "").Trim();
                        if (string.IsNullOrEmpty(rawBanco))
                            rawBanco = (QuitarDiacriticos(tipoTarjeta).ToUpperInvariant() == "FORANEO") ? "FORANEO" : "SIN BANCO";

                        // Construcción de línea CSV en el orden exacto del COPY
                        csvBuilder
                            .Append(longitudOk.ToString(CultureInfo.InvariantCulture)).Append(',')
                            .Append(Escapar(rawBin89)).Append(',')
                            .Append(Escapar(rawBin6)).Append(',')
                            .Append(Escapar(marcaRaw)).Append(',')
                            .Append(Escapar(row[4])).Append(',')    // tipo_tarjeta
                            .Append(Escapar(row[5])).Append(',')    // subtipo_tarjeta
                            .Append(Escapar(row[6])).Append(',')    // subproducto
                            .Append(Escapar(row[7])).Append(',')    // categoria_tarjeta
                            .Append(Escapar(rawBanco)).Append(',')  // banco_emisor
                            .Append(Escapar(nomClasRaw)).Append(',')// traduccion_tarifario
                            .Append(Escapar(codOp)).Append(',')     // cod_operador (NOT NULL)
                            .Append(Escapar(codClas))               // cod_clasificacion (NOT NULL)
                            .Append('\n');

                    }
                    catch (Exception exFila)
                    {
                        respuesta.Errores.Add(new ErroresExcelDTO { Fila = fila + 2, Mensaje = exFila.Message });
                    }
                }

                var contenidoCsv = csvBuilder.ToString();
                if (string.IsNullOrWhiteSpace(contenidoCsv))
                {
                    return new RespuestaComunExcelDTO
                    {
                        Ok = false,
                        Mensaje = "No se encontraron filas válidas para importar.",
                        Errores = respuesta.Errores
                    };
                }

                // ------------------------------------------------------------------
                // 4) Cargar en PostgreSQL (TRUNCATE + COPY + de-duplicado opcional)
                // ------------------------------------------------------------------
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync(cancellationToken);

                    // Limpieza previa (si manejas la tabla como staging)
                    using (var cmdTruncate = new NpgsqlCommand(@"TRUNCATE TABLE ""SGP"".""mdr_bines_inretail"";", conn))
                        await cmdTruncate.ExecuteNonQueryAsync(cancellationToken);

                    const string copySql = @"
                        COPY ""SGP"".""mdr_bines_inretail""
                        (""longitud"", ""bin_8_9"", ""bin_6"", ""marca"", ""tipo_tarjeta"", ""subtipo_tarjeta"",
                         ""subproducto"", ""categoria_tarjeta"", ""banco_emisor"", ""traduccion_tarifario"",
                         ""cod_operador"", ""cod_clasificacion"")
                        FROM STDIN WITH (FORMAT csv);";

                    using (var importer = conn.BeginTextImport(copySql))
                    {
                        importer.Write(contenidoCsv);
                    }

                    // (Opcional) eliminar duplicados por par (bin_6, bin_8_9)
                    const string dedupSql = @"
                        WITH dupe AS (
                            SELECT ctid, ROW_NUMBER() OVER (PARTITION BY ""bin_6"", ""bin_8_9"" ORDER BY ctid) rn
                            FROM ""SGP"".""mdr_bines_inretail""
                        )
                        DELETE FROM ""SGP"".""mdr_bines_inretail"" t
                        USING dupe d
                        WHERE t.ctid = d.ctid AND d.rn > 1;";
                    using (var cmdDedup = new NpgsqlCommand(dedupSql, conn))
                    {
                        await cmdDedup.ExecuteNonQueryAsync(cancellationToken);
                    }

                    using (var cmdProc = new NpgsqlCommand(@"CALL ""SGP"".""mdr_bines_sp_insertar_desde_tmp""(@p_cod_periodo);", conn))
                    {
                        cmdProc.Parameters.AddWithValue("p_cod_periodo", NpgsqlDbType.Bigint, request.CodPeriodo);
                        await cmdProc.ExecuteNonQueryAsync(cancellationToken);
                    }


                }

                if (respuesta.Errores.Count > 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Importación completada con advertencias/errores en algunas filas.";
                }
                else
                {
                    respuesta.Ok = true;
                    respuesta.Mensaje = "Importación completada exitosamente.";
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                return new RespuestaComunExcelDTO
                {
                    Ok = false,
                    Mensaje = "Error al procesar el archivo: " + ex.Message,
                    Errores = respuesta.Errores
                };
            }
        }

        // ------------------ Helpers ------------------
        private static string NormalizaClave(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var sin = QuitarDiacriticos(s).ToUpperInvariant();
            // para evitar falsos negativos, quitamos espacios
            sin = sin.Replace(" ", "");
            return sin;
        }

        private static string QuitarDiacriticos(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
