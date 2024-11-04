using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class DescargarInvKardexLocalCommand : IRequest<DescargarMaestroDTO>
    {
    }

    public class DescargarInvKardexLocalHandler : IRequestHandler<DescargarInvKardexLocalCommand, DescargarMaestroDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarInvKardexLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarInvKardexLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var listaActivos = await _contexto.RepositorioInvKardexLocal.Obtener().ToListAsync();

                string fileName = $"LocalesKardex_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Hoja1");

                    var headerRow = ws.Row(1);
                    var properties = listaActivos.First().GetType().GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        headerRow.Cell(i + 1).Value = properties[i].Name;
                    }

                    for (int i = 0; i < listaActivos.Count; i++)
                    {
                        var rowData = listaActivos[i];
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
