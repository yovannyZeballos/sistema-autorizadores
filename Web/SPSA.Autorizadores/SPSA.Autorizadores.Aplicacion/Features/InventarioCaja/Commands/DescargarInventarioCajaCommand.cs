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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
    public class DescargarInventarioCajaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public string CodEmpresa { get; set; }
    }

    public class DescargarInventarioCajaHandler : IRequestHandler<DescargarInventarioCajaCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public DescargarInventarioCajaHandler()
        {
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(DescargarInventarioCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO();

            try
            {
                var table = await _contexto.RepositorioInvCajas.DescargarInventarioCajas(request.CodEmpresa);

                if (table.Rows.Count == 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron datos para la empresa seleccionada.";
                    return respuesta;
                }

                // Usar NPOI para generar Excel
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Inventario");

                // Crear cabeceras
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(table.Columns[i].ColumnName);
                }

                // Crear filas con datos
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(table.Rows[i][j]?.ToString() ?? "");
                    }
                }

                // Guardar en memoria
                using (var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    respuesta.Ok = true;
                    respuesta.Mensaje = "Archivo generado correctamente";
                    respuesta.NombreArchivo = $"Inventario_{request.CodEmpresa}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    respuesta.Archivo = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = $"Error al generar archivo: {ex.Message}";
            }


            return respuesta;
        }
    }
}
