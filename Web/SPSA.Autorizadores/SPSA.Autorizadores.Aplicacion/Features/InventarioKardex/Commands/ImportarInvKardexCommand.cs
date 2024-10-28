using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class ImportarInvKardexCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarInvKardexHandler : IRequestHandler<ImportarInvKardexCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarInvKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarInvKardexCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };

            try
            {
                using (var stream = request.ArchivoExcel)
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rowCount = worksheet.RowsUsed().Count();

                        var columnMapping = worksheet.Row(1).CellsUsed()
                            .ToDictionary(cell => cell.GetString(), cell => cell.Address.ColumnNumber);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var rowKardex = new InvKardex
                                {
                                    Kardex = worksheet.Cell(row, columnMapping["KARDEX"]).GetString(),
                                    Guia = worksheet.Cell(row, columnMapping["GUIA"]).GetString(),
                                    ActivoId = worksheet.Cell(row, columnMapping["ACTIVO_ID"]).GetString(),
                                    Serie = worksheet.Cell(row, columnMapping["SERIE"]).GetString(),
                                    OrigenId = worksheet.Cell(row, columnMapping["ORIGEN_ID"]).GetString(),
                                    DestinoId = worksheet.Cell(row, columnMapping["DESTINO_ID"]).GetString(),
                                    Tk = worksheet.Cell(row, columnMapping["TK"]).GetString(),
                                    Cantidad = worksheet.Cell(row, columnMapping["CANTIDAD"]).GetValue<int>(),
                                    TipoStock = worksheet.Cell(row, columnMapping["TIPO_STOCK"]).GetString(),
                                    Oc = worksheet.Cell(row, columnMapping["OC"]).GetString(),
                                    Sociedad = worksheet.Cell(row, columnMapping["SOCIEDAD"]).GetString(),
                                };

                                object cellValueFecha = worksheet.Cell(row, columnMapping["FECHA"]).Value;
                                bool isEmptyOrNullFecha = cellValueFecha == null || string.IsNullOrWhiteSpace(cellValueFecha.ToString());
                                rowKardex.Fecha = (DateTime)(isEmptyOrNullFecha ? (DateTime?)null : DateTime.Parse(cellValueFecha.ToString()));

                                _contexto.RepositorioInvKardex.Agregar(rowKardex);
                                await _contexto.GuardarCambiosAsync();
                            }
                            catch (Exception ex)
                            {
                                _contexto.Rollback();

                                if (ex.HResult == -2146233079)
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = row,
                                        Mensaje = "Ya existe un activo con estas caracteristicas."
                                    });
                                }
                                else
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = row,
                                        Mensaje = ex.Message
                                    });
                                }
                            }
                        }

                        if (respuesta.Errores.Count == 0)
                        {
                            respuesta.Ok = true;
                            respuesta.Mensaje = "Archivo importado correctamente";
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "Se encontraron algunos errores en el archivo";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, "Ocurrió un error al importar excel");
            }

            return respuesta;
        }
    }
}
