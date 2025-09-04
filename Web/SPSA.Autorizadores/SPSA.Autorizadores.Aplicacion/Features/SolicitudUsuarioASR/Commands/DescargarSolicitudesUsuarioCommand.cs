using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class DescargarSolicitudesUsuarioCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodLocalAlterno { get; set; }
    }

    public class DescargarSolicitudesUsuarioHandler : IRequestHandler<DescargarSolicitudesUsuarioCommand, DescargarMaestroDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarSolicitudesUsuarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarSolicitudesUsuarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var lista = await _contexto.RepositorioSolicitudUsuarioASR
                    .Obtener(x => x.CodLocal == request.CodLocalAlterno)
                    .OrderBy(x => x.CodLocal)
                    .ToListAsync(cancellationToken);

                if (lista == null || !lista.Any())
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron registros.";
                    return respuesta;
                }

                string fileName = $"SolicitudesUsuariosASR{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Solicitudes");

                    if (ws == null)
                    {
                        throw new NullReferenceException("La hoja de cálculo 'Solicitudes' no se pudo crear.");
                    }

                    var properties = lista.First().GetType().GetProperties();
                    if (properties == null || properties.Length == 0)
                    {
                        throw new NullReferenceException("No se pudieron obtener las propiedades del objeto.");
                    }

                    var headerRow = ws.Row(1);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        headerRow.Cell(i + 1).Value = properties[i].Name;
                    }

                    for (int i = 0; i < lista.Count; i++)
                    {
                        var rowData = lista[i];
                        if (rowData != null)
                        {
                            for (int j = 0; j < properties.Length; j++)
                            {
                                var propValue = properties[j].GetValue(rowData);
                                ws.Cell(i + 2, j + 1).Value = propValue != null ? "'" + propValue.ToString() : "";
                            }
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
