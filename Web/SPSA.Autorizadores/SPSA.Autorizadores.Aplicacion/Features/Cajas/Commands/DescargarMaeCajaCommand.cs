﻿using ClosedXML.Excel;
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
                var dataTable = await _contexto.RepositorioMaeCaja.ObtenerCajasPorLocalAsync(request.CodEmpresa, request.CodCadena, request.CodRegion, request.CodZona, request.CodLocal);

                if (dataTable.Rows.Count == 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron datos para del local especificada.";
                    return respuesta;
                }

                string fileName = $"CajasPorLocal_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Cajas");

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
                _logger.Error(ex, "Error al generar el archivo Excel para el local {CodLocal}", request.CodLocal);
            }

            return respuesta;
        }
    }
}
