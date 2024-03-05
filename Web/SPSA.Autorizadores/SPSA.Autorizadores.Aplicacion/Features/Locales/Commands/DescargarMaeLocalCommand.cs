using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;

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
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarMaestroLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarMaeLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            //try
            //{
            //    var dt = await _repositorioMaestroLocal.Descargar(request.CodEmpresa, request.CodCadena, request.CodRegion, request.CodZona);
            //    dt.TableName = "Locales";
            //    string fileName = $"MaestroLocales_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
            //    using (XLWorkbook wb = new XLWorkbook())
            //    {
            //        wb.Worksheets.Add(dt);
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            wb.SaveAs(stream);
            //            respuesta.Archivo = Convert.ToBase64String(stream.ToArray());
            //            respuesta.NombreArchivo = fileName;
            //            respuesta.Ok = true;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    respuesta.Ok = false;
            //    respuesta.Mensaje = ex.Message;
            //}
            return respuesta;
        }
    }
}
