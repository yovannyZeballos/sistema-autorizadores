using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class DescargarMaeLocalXEmpresaCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodEmpresa { get; set; }
    }

    public class DescargarMaeLocalXEmpresaHandler : IRequestHandler<DescargarMaeLocalXEmpresaCommand, DescargarMaestroDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarMaeLocalXEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarMaeLocalXEmpresaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var listaLocales = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa).ToListAsync();

                string fileName = $"LocalesPorEmpresa_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Locales");

                    var headerRow = ws.Row(1);
                    var properties = listaLocales.First().GetType().GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        headerRow.Cell(i + 1).Value = properties[i].Name;
                    }

                    for (int i = 0; i < listaLocales.Count; i++)
                    {
                        var rowData = listaLocales[i];
                        for (int j = 0; j < properties.Length; j++)
                        {
                            var propValue = properties[j].GetValue(rowData);
                            ws.Cell(i + 2, j + 1).Value = propValue != null ? "'" + propValue.ToString() : "";
                        }
                    }

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
