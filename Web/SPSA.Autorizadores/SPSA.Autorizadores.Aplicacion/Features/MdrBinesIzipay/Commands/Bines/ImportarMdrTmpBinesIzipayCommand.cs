using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ExcelDataReader;
using MediatR;
using Npgsql;
using SPSA.Autorizadores.Aplicacion.DTO;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.Bines
{
    public class ImportarMdrTmpBinesIzipayCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
    }

    public class ImportarMdrTmpBinesIzipayHandler : IRequestHandler<ImportarMdrTmpBinesIzipayCommand, RespuestaComunExcelDTO>
    {
        private readonly string _connectionString;

        public ImportarMdrTmpBinesIzipayHandler()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMdrTmpBinesIzipayCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Ok = true, Errores = new List<ErroresExcelDTO>() };

            try
            {
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

                DataTable dt;
                using (var stream = request.Archivo.InputStream)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var ds = reader.AsDataSet(conf);

                    if (ds.Tables.Count == 0)
                    {
                        return new RespuestaComunExcelDTO
                        {
                            Ok = false,
                            Mensaje = "El archivo Excel no contiene hojas.",
                            Errores = new List<ErroresExcelDTO>()
                        };
                    }

                    dt = ds.Tables[0];
                }

                // Recorrer las filas del DataTable y construir un CSV en memoria
                //    Asumimos que:
                //    - Las columnas están en este orden:
                //      0: NUM_BIN_8
                //      1: NUM_BIN_6
                //      2: MARCA
                //      3: TIPO
                //      4: CATEGORIA
                //      5: CATEGORIA_IZIPAY
                //      6: BANCO_EMISOR
                //      7: VALIDACION
                //
                //    - Hay encabezado, por lo que arrancamos en la fila 1

                var csvBuilder = new StringBuilder();
                for (int fila = 0; fila < dt.Rows.Count; fila++)
                {
                    try
                    {
                        DataRow row = dt.Rows[fila];
                        if (row == null) continue;

                        string Escapar(object valor)
                        {
                            if (valor == null) return "";
                            string s = valor.ToString().Trim();
                            if (string.IsNullOrEmpty(s)) return "";

                            if (s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n"))
                            {
                                var tmp = s.Replace("\"", "\"\"");
                                return $"\"{tmp}\"";
                            }
                            return s;
                        }

                        string numBin8 = Escapar(row[0]);
                        string numBin6 = Escapar(row[1]);
                        string marca = Escapar(row[2]);
                        string tipo = Escapar(row[3]);
                        string categoria = Escapar(row[4]);
                        string categoriaIz = Escapar(row[5]);
                        string rawBanco = row[6]?.ToString().Trim();

                        if (string.IsNullOrEmpty(rawBanco))
                        {
                            if (tipo == "FORANEO")
                            {
                                rawBanco = "FORANEO";
                            }
                            else
                            {
                                rawBanco = "SIN BANCO";
                            }
                        }

                        string bancoEmisor = Escapar(rawBanco);
                        string validacion = Escapar(row[7]);

                        // Si **todas** las columnas están vacías, omitimos la fila
                        if (string.IsNullOrEmpty(numBin8)
                            && string.IsNullOrEmpty(numBin6)
                            && string.IsNullOrEmpty(marca)
                            && string.IsNullOrEmpty(tipo)
                            && string.IsNullOrEmpty(categoria)
                            && string.IsNullOrEmpty(categoriaIz)
                            && string.IsNullOrEmpty(bancoEmisor)
                            && string.IsNullOrEmpty(validacion))
                        {
                            continue;
                        }

                        // Construir la línea CSV (orden EXACTO de columnas en la tabla)
                        csvBuilder.Append(numBin8).Append(",");
                        csvBuilder.Append(numBin6).Append(",");
                        csvBuilder.Append(marca).Append(",");
                        csvBuilder.Append(tipo).Append(",");
                        csvBuilder.Append(categoria).Append(",");
                        csvBuilder.Append(categoriaIz).Append(",");
                        csvBuilder.Append(bancoEmisor).Append(",");
                        csvBuilder.Append(validacion).Append("\n");
                    }
                    catch (Exception exFila)
                    {
                        respuesta.Errores.Add(new ErroresExcelDTO
                        {
                            Fila = fila + 1,
                            Mensaje = exFila.Message
                        });
                    }
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync(cancellationToken);

                    using (var cmdTruncate = new NpgsqlCommand(
                        @"TRUNCATE TABLE ""SGP"".""MDR_TMP_BINES_IZIPAY"";", conn))
                    {
                        await cmdTruncate.ExecuteNonQueryAsync(cancellationToken);
                    }

                    var contenidoCsv = csvBuilder.ToString();
                    if (string.IsNullOrWhiteSpace(contenidoCsv))
                    {
                        respuesta.Mensaje = "No se encontraron filas para importar. La tabla quedó vacía.";
                        return respuesta;
                    }

                    const string copySql = @"
                        COPY ""SGP"".""MDR_TMP_BINES_IZIPAY""
                          (""NUM_BIN_8"", ""NUM_BIN_6"", ""MARCA"", ""TIPO"", ""CATEGORIA"", ""CATEGORIA_IZIPAY"", ""BANCO_EMISOR"", ""VALIDACION"")
                        FROM STDIN WITH (FORMAT csv);";

                    using (var importer = conn.BeginTextImport(copySql))
                    {
                        using (var stringReader = new StringReader(contenidoCsv))
                        {
                            char[] buffer = new char[8192];
                            int leidos;
                            while ((leidos = stringReader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                importer.Write(buffer, 0, leidos);
                            }
                        }
                        importer.Close();
                    }

                    using (var cmdProc = new NpgsqlCommand(@"CALL ""SGP"".""sp_mdr_insertar_bines_desde_tmp""();", conn))
                    {
                        await cmdProc.ExecuteNonQueryAsync(cancellationToken);
                    }
                }

                if (respuesta.Errores.Count > 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Se importaron filas, pero hubo errores en algunas filas.";
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
    }
}
