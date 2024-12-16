using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;

namespace SPSA.Autorizadores.Aplicacion.Features.Reportes.Commands
{
    public class DescargarValesRedimidosCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodLocal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class DescargarValesRedimidosHandler : IRequestHandler<DescargarValesRedimidosCommand, DescargarMaestroDTO>
    {
        private readonly IRepositorioReportes _repositorioReportes;
        private readonly ILogger _logger;

        public DescargarValesRedimidosHandler(IRepositorioReportes repositorioReportes)
        {
            _repositorioReportes = repositorioReportes;
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarValesRedimidosCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var dataTable = await _repositorioReportes.ListarValesRedimidosAsync(request.CodLocal, request.FechaInicio, request.FechaFin);
                string fileName = $"ValesRedimidos_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("ValesRedimidos");

                    var headerRow = ws.Row(1);
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        headerRow.Cell(i + 1).Value = dataTable.Columns[i].ColumnName;
                    }

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var dataRow = dataTable.Rows[i];
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            var cellValue = dataRow[j];
                            ws.Cell(i + 2, j + 1).Value = cellValue != DBNull.Value ? "'" + cellValue.ToString() : "";
                        }
                    }

                    ws.Columns().AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        respuesta.Archivo = Convert.ToBase64String(stream.ToArray());
                        respuesta.NombreArchivo = fileName;
                        respuesta.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }
    }

}
