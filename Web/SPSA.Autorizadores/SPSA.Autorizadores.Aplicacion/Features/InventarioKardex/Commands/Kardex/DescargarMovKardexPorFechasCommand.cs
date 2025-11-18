using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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
            var resp = new RespuestaComunExcelDTO();
            string tempPath = null;

            try
            {
                var table = await _repositorioMovKardex.DescargarMovKardexPorFechas(r.FechaInicio, r.FechaFin);
                if (table == null || table.Rows.Count == 0)
                {
                    resp.Ok = false;
                    resp.Mensaje = "No se encontraron movimientos en el rango indicado.";
                    return resp;
                }

                tempPath = Path.Combine(Path.GetTempPath(), $"kardex_{Guid.NewGuid():N}.xlsx");

                using (var doc = SpreadsheetDocument.Create(tempPath, SpreadsheetDocumentType.Workbook))
                {
                    var wbPart = doc.AddWorkbookPart();
                    wbPart.Workbook = new Workbook();

                    var wsPart = wbPart.AddNewPart<WorksheetPart>();
                    using (var w = OpenXmlWriter.Create(wsPart))
                    {
                        w.WriteStartElement(new Worksheet());
                        w.WriteStartElement(new SheetData());

                        // Header
                        w.WriteStartElement(new Row());
                        for (int c = 0; c < table.Columns.Count; c++)
                            WriteStringCell(w, table.Columns[c].ColumnName ?? string.Empty);
                        w.WriteEndElement(); // </Row>

                        // Data (todo como texto)
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            var dr = table.Rows[i];
                            w.WriteStartElement(new Row());
                            for (int j = 0; j < table.Columns.Count; j++)
                            {
                                var s = dr[j] == null || dr[j] is DBNull ? string.Empty : dr[j].ToString();
                                WriteStringCell(w, s);
                            }
                            w.WriteEndElement(); // </Row>
                        }

                        w.WriteEndElement(); // </SheetData>
                        w.WriteEndElement(); // </Worksheet>
                    }

                    var sheets = wbPart.Workbook.AppendChild(new Sheets());
                    sheets.Append(new Sheet
                    {
                        Id = wbPart.GetIdOfPart(wsPart),
                        SheetId = 1U,
                        Name = "Kardex"
                    });

                    wbPart.Workbook.Save();
                }

                var bytes = File.ReadAllBytes(tempPath);
                resp.Ok = true;
                resp.Mensaje = "Archivo generado correctamente.";
                resp.NombreArchivo = $"Kardex_{r.FechaInicio:yyyyMMdd}_{r.FechaFin:yyyyMMdd}.xlsx";
                resp.Archivo = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DescargarMovKardexPorFechasHandler error");
                resp.Ok = false;
                resp.Mensaje = $"Error al generar archivo: {ex.Message}";
            }
            finally
            {
                try { if (!string.IsNullOrEmpty(tempPath) && File.Exists(tempPath)) File.Delete(tempPath); } catch { }
            }

            return resp;
        }

        private static void WriteStringCell(OpenXmlWriter w, string value)
        {
            w.WriteElement(new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value ?? string.Empty)
            });
        }
    }
}
