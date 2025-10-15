using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Kardex
{
    public class DescargarMovKardexPorFechasCommand : IRequest<RespuestaComunExcelDTO>
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class DescargarMovKardexPorFechasHandler : IRequestHandler<DescargarMovKardexPorFechasCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public DescargarMovKardexPorFechasHandler()
        {
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(DescargarMovKardexPorFechasCommand r, CancellationToken ct)
        {
            var resp = new RespuestaComunExcelDTO();

            try
            {
                var table = await _contexto.RepositorioMovKardex.DescargarMovKardexPorFechas(r.FechaInicio, r.FechaFin);
                if (table.Rows.Count == 0)
                {
                    resp.Ok = false;
                    resp.Mensaje = "No se encontraron movimientos en el rango indicado.";
                    return resp;
                }

                IWorkbook wb = new XSSFWorkbook();
                ISheet sh = wb.CreateSheet("Kardex");

                // Cabeceras
                var head = sh.CreateRow(0);
                for (int c = 0; c < table.Columns.Count; c++)
                    head.CreateCell(c).SetCellValue(table.Columns[c].ColumnName);

                // Filas
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = sh.CreateRow(i + 1);
                    for (int j = 0; j < table.Columns.Count; j++)
                        row.CreateCell(j).SetCellValue(table.Rows[i][j]?.ToString() ?? "");
                }

                for (int c = 0; c < table.Columns.Count; c++) sh.AutoSizeColumn(c);

                using (var ms = new MemoryStream())
                {
                    wb.Write(ms);
                    resp.Ok = true;
                    resp.Mensaje = "Archivo generado correctamente.";
                    resp.NombreArchivo = $"Kardex_{r.FechaInicio:yyyyMMdd}_{r.FechaFin:yyyyMMdd}.xlsx";
                    resp.Archivo = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DescargarMovKardexPorFechasHandler error: {Message}", ex.Message);
                resp.Ok = false;
                resp.Mensaje = $"Error al generar archivo: {ex.Message}";
            }

            return resp;
        }
    }
}
