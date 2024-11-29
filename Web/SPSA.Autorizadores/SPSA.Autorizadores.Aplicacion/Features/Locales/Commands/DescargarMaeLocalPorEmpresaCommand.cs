using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class DescargarMaeLocalPorEmpresaCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodEmpresa { get; set; }
    }

    public class DescargarMaeLocalPorEmpresaHandler : IRequestHandler<DescargarMaeLocalPorEmpresaCommand, DescargarMaestroDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarMaeLocalPorEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarMaeLocalPorEmpresaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var dataTable = await _contexto.RepositorioMaeLocal.ObtenerLocalesPorEmpresaAsync(request.CodEmpresa);

                if (dataTable.Rows.Count == 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron datos para la empresa especificada.";
                    return respuesta;
                }

                string fileName = $"LocalesPorEmpresa_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Locales");

                    var headerRow = ws.Row(1);
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        headerRow.Cell(i + 1).Value = dataTable.Columns[i].ColumnName;
                    }

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var row = dataTable.Rows[i];
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            ws.Cell(i + 2, j + 1).Value = row[j] != DBNull.Value ? row[j].ToString() : "";
                        }
                    }

                    ws.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
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
                _logger.Error(ex, "Error al generar el archivo Excel para la empresa {CodEmpresa}", request.CodEmpresa);
            }

            return respuesta;
        }
    }
}
