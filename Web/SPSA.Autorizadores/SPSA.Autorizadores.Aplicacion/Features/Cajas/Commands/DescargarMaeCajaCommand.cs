using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands
{
    public class DescargarMaeCajaCommand : IRequest<DescargarMaestroDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
    }

    public class DescargarMaestroCajaHandler : IRequestHandler<DescargarMaeCajaCommand, DescargarMaestroDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DescargarMaestroCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<DescargarMaestroDTO> Handle(DescargarMaeCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new DescargarMaestroDTO();

            try
            {
                var listaCajas = await _contexto.RepositorioMaeCaja.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal).ToListAsync();

                string fileName = $"CajasPorLocal_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Cajas");

                    var headerRow = ws.Row(1);
                    var properties = listaCajas.First().GetType().GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        headerRow.Cell(i + 1).Value = properties[i].Name;
                    }

                    for (int i = 0; i < listaCajas.Count; i++)
                    {
                        var rowData = listaCajas[i];
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


            //var respuesta = new DescargarMaestroDTO();

            //try
            //{
            //    var dt = await _repositorioMaestroCaja.Descargar(request.CodEmpresa, request.CodCadena, request.CodRegion, request.CodZona, request.CodLocal);
            //    dt.TableName = "Cajas";
            //    string fileName = $"Cajas_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
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
            //return respuesta;
        }
    }
}
