using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class DescargarMaeLocalCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
    }

    public class DescargarMaestroLocalHandler : IRequestHandler<DescargarMaeLocalCommand, DescargarMaestroDTO>
    {
        private readonly IRepositorioMaestroLocal _repositorioMaestroLocal;

        public DescargarMaestroLocalHandler(IRepositorioMaestroLocal repositorioMaestroLocal)
        {
            _repositorioMaestroLocal = repositorioMaestroLocal;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarMaeLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var dt = await _repositorioMaestroLocal.Descargar(request.CodEmpresa, request.CodCadena, request.CodRegion, request.CodZona);
                dt.TableName = "Locales";
                string fileName = $"MaestroLocales_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
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
