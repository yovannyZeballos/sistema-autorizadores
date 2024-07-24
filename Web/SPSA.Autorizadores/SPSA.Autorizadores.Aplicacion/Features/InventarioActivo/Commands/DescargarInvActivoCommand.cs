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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands
{
    public class DescargarInvActivoCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodEmpresa { get; set; }
    }

    public class DescargarInvActivoHandler : IRequestHandler<DescargarInvActivoCommand, DescargarMaestroDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarInvActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarInvActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                // Realiza la consulta a la base de datos y guarda los resultados en memoria
                // Realiza la consulta a la base de datos y proyecta a un objeto anónimo
                var lista = await _contexto.RepositorioInventarioActivo
                    .Obtener(x => x.CodEmpresa == request.CodEmpresa)
                    .OrderBy(x => x.CodLocal)
                    .Select(x => new
                    {
                        x.CodEmpresa,
                        x.CodCadena,
                        x.CodRegion,
                        x.CodZona,
                        x.CodLocal,
                        x.CodActivo,
                        x.InvTipoActivo.NomActivo,
                        x.CodModelo,
                        x.NomMarca,
                        x.CodSerie,
                        x.Ip,
                        x.NomArea,
                        x.NumOc,
                        x.NumGuia,
                        x.FecSalida,
                        x.Antiguedad,
                        x.IndOperativo,
                        x.Observacion,
                        x.Garantia,
                        x.FecActualiza
                    })
                    .ToListAsync(cancellationToken);

                // Verifica si la lista está vacía
                if (lista == null || !lista.Any())
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron registros.";
                    return respuesta;
                }

                // Genera el nombre del archivo
                string fileName = $"Activos_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                // Crea el archivo Excel
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Activos");

                    if (ws == null)
                    {
                        throw new NullReferenceException("La hoja de cálculo 'Activos' no se pudo crear.");
                    }

                    var properties = lista.First().GetType().GetProperties();
                    if (properties == null || properties.Length == 0)
                    {
                        throw new NullReferenceException("No se pudieron obtener las propiedades del objeto.");
                    }

                    // Escribe el encabezado
                    var headerRow = ws.Row(1);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        headerRow.Cell(i + 1).Value = properties[i].Name;
                    }

                    // Escribe los datos
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
