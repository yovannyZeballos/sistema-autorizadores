using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Kardex
{
    public class DescargarMovKardexPorFechasCommand : IRequest<RespuestaComunExcelDTO>
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class DescargarMovKardexPorFechasHandler : IRequestHandler<DescargarMovKardexPorFechasCommand, RespuestaComunExcelDTO>
    {
        private readonly IRepositorioMovKardex _repositorioMovKardex;
        private readonly ILogger _logger;

        public DescargarMovKardexPorFechasHandler(IRepositorioMovKardex repositorioMovKardex)
        {
            _repositorioMovKardex = repositorioMovKardex;
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(DescargarMovKardexPorFechasCommand r, CancellationToken ct)
        {
            var resp = new RespuestaComunExcelDTO { Ok = false, Mensaje = "No procesado" };
            string tempPath = null;

            try
            {
                // 1) Datos
                DataTable table = await _repositorioMovKardex.DescargarMovKardexPorFechas(r.FechaInicio, r.FechaFin);
                if (table == null || table.Rows.Count == 0)
                {
                    resp.Mensaje = "No se encontraron movimientos en el rango indicado.";
                    return resp;
                }

                // 2) Generar XLSX en archivo temporal (streaming con SXSSF)
                tempPath = Path.Combine(Path.GetTempPath(), $"kardex_{Guid.NewGuid():N}.xlsx");

                using (var xssf = new XSSFWorkbook())
                using (var wb = new SXSSFWorkbook(xssf, 500)) // ventana de 500 filas en memoria
                {
                    var sh = wb.CreateSheet("Kardex");
                    var helper = wb.GetCreationHelper();

                    // Estilos
                    var fontBold = wb.CreateFont(); fontBold.IsBold = true;
                    var headerStyle = wb.CreateCellStyle(); headerStyle.SetFont(fontBold);

                    var df = helper.CreateDataFormat();
                    var dateStyle = wb.CreateCellStyle();
                    dateStyle.DataFormat = df.GetFormat("yyyy-mm-dd hh:mm:ss");

                    int colCount = table.Columns.Count;
                    int rowCount = table.Rows.Count;
                    var maxLen = new int[colCount]; // longitudes para ancho de columna

                    // Cabeceras
                    var head = sh.CreateRow(0);
                    for (int c = 0; c < colCount; c++)
                    {
                        var name = table.Columns[c].ColumnName ?? string.Empty;
                        var cell = head.CreateCell(c);
                        cell.SetCellValue(name);
                        cell.CellStyle = headerStyle;
                        maxLen[c] = Math.Max(maxLen[c], name.Length);
                    }

                    // Filas (compatible C# 7.3, sin 'or patterns')
                    for (int i = 0; i < rowCount; i++)
                    {
                        var row = sh.CreateRow(i + 1);
                        var dr = table.Rows[i];

                        for (int j = 0; j < colCount; j++)
                        {
                            var cell = row.CreateCell(j);
                            var val = dr[j];

                            if (val == null || val == DBNull.Value)
                            {
                                cell.SetCellValue(string.Empty);
                                continue;
                            }

                            // DateTime / DateTimeOffset
                            if (val is DateTime)
                            {
                                var dt = (DateTime)val;
                                cell.SetCellValue(dt);
                                cell.CellStyle = dateStyle;
                                maxLen[j] = Math.Max(maxLen[j], 19); // "yyyy-mm-dd hh:mm:ss"
                            }
                            else if (val is DateTimeOffset)
                            {
                                var dto = (DateTimeOffset)val;
                                cell.SetCellValue(dto.UtcDateTime);
                                cell.CellStyle = dateStyle;
                                maxLen[j] = Math.Max(maxLen[j], 19);
                            }
                            else
                            {
                                // Numérico / Boolean / Texto por TypeCode
                                var tc = Type.GetTypeCode(val.GetType());
                                switch (tc)
                                {
                                    case TypeCode.Byte:
                                    case TypeCode.SByte:
                                    case TypeCode.Int16:
                                    case TypeCode.UInt16:
                                    case TypeCode.Int32:
                                    case TypeCode.UInt32:
                                    case TypeCode.Int64:
                                    case TypeCode.UInt64:
                                    case TypeCode.Single:
                                    case TypeCode.Double:
                                    case TypeCode.Decimal:
                                        {
                                            double d = Convert.ToDouble(val);
                                            cell.SetCellValue(d);
                                            maxLen[j] = Math.Max(maxLen[j], val.ToString().Length);
                                            break;
                                        }
                                    case TypeCode.Boolean:
                                        {
                                            bool b = Convert.ToBoolean(val);
                                            cell.SetCellValue(b);
                                            maxLen[j] = Math.Max(maxLen[j], b.ToString().Length);
                                            break;
                                        }
                                    default:
                                        {
                                            string s = val.ToString();
                                            cell.SetCellValue(s);
                                            maxLen[j] = Math.Max(maxLen[j], s == null ? 0 : s.Length);
                                            break;
                                        }
                                }
                            }
                        }

                        // Liberar por bloques si manejas millones de filas
                        // if ((i % 5000) == 0) ((SXSSFSheet)sh).FlushRows(5000);
                    }

                    // Anchos de columna "a ojo" (sin AutoSizeColumn → evita SixLabors)
                    for (int c = 0; c < colCount; c++)
                    {
                        int widthChars = Math.Min(60, Math.Max(10, maxLen[c] + 2)); // 10..60
                        sh.SetColumnWidth(c, widthChars * 256);
                    }

                    // UX
                    sh.CreateFreezePane(0, 1);
                    sh.SetAutoFilter(new CellRangeAddress(0, rowCount, 0, colCount - 1));

                    // Guardar a disco (no en memoria)
                    using (var fs = File.Create(tempPath))
                    {
                        wb.Write(fs);
                    }

                    try { wb.Dispose(); } catch { try { wb.Close(); } catch { } }
                }

                // 3) Leer archivo y convertir a Base64 (tu DTO lo requiere)
                byte[] fileBytes = File.ReadAllBytes(tempPath);
                resp.Archivo = Convert.ToBase64String(fileBytes);
                resp.NombreArchivo = $"Kardex_{r.FechaInicio:yyyyMMdd}_{r.FechaFin:yyyyMMdd}.xlsx";
                resp.Ok = true;
                resp.Mensaje = "Archivo generado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.Error("DescargarMovKardexPorFechasHandler error: {Message}", ex.Message);
                resp.Ok = false;
                resp.Mensaje = $"Error al generar archivo: {ex.Message}";
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempPath))
                {
                    try
                    {
                        if (File.Exists(tempPath)) File.Delete(tempPath);
                    }
                    catch { /* ignore */ }
                }
            }

            return resp;
        }
    }
}
