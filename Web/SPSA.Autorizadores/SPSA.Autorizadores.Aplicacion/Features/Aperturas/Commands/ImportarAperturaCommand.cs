using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands
{
    public class ImportarAperturaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarAperturaHandler : IRequestHandler<ImportarAperturaCommand, RespuestaComunExcelDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarAperturaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarAperturaCommand request, CancellationToken cancellationToken)
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

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var nuevoApertura = new Dominio.Entidades.Apertura
                                {
                                    NomLocalPMM = worksheet.Cell(row, 2).Value.ToString().ToUpper(),
                                    Administrador = worksheet.Cell(row, 3).Value.ToString(),
                                    NumTelefono = worksheet.Cell(row, 4).Value.ToString(),
                                    Email = worksheet.Cell(row, 5).Value.ToString(),
                                    Direccion = worksheet.Cell(row, 6).Value.ToString(),
                                    CodLocalPMM = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    CodLocalSAP = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    CodLocalSAPNew = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    CodComercio = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    CodCentroCosto = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    //FecApertura = worksheet.Cell(row, 15).Value,
                                    TipEstado = worksheet.Cell(row, 16).Value.ToString(),
                                    CodEAN = worksheet.Cell(row, 18).Value.ToString(),
                                    CodSUNAT = worksheet.Cell(row, 19).Value.ToString(),
                                    NumGuias = worksheet.Cell(row, 20).Value.ToString(),
                                    CentroDistribu = worksheet.Cell(row, 21).Value.ToString(),
                                    TdaEspejo = worksheet.Cell(row, 22).Value.ToString(),
                                    Mt2Sala = worksheet.Cell(row, 23).Value.ToString(),
                                    Spaceman = worksheet.Cell(row, 24).Value.ToString(),
                                    ZonaPricing = worksheet.Cell(row, 25).Value.ToString(),
                                    Geolocalizacion = worksheet.Cell(row, 26).Value.ToString(),
                                    //FecCierre = worksheet.Cell(row, 27).Value,
                                };

                                object cellValueFecApertura = worksheet.Cell(row, 15).Value;
                                bool isEmptyOrNullFecApertura = cellValueFecApertura == null || string.IsNullOrWhiteSpace(cellValueFecApertura.ToString());
                                nuevoApertura.FecApertura = isEmptyOrNullFecApertura ? null : DateTime.TryParse(cellValueFecApertura.ToString(), out DateTime fecha1) ? fecha1 : (DateTime?)null;

                                object cellValueFecCierre = worksheet.Cell(row, 27).Value;
                                bool isEmptyOrNullFecCierre = cellValueFecCierre == null || string.IsNullOrWhiteSpace(cellValueFecCierre.ToString());
                                nuevoApertura.FecApertura = isEmptyOrNullFecCierre ? null : DateTime.TryParse(isEmptyOrNullFecCierre.ToString(), out DateTime fecha2) ? fecha2 : (DateTime?)null;


                                bool existe = await _contexto.RepositorioApertura.Existe(x => x.CodLocalPMM == nuevoApertura.CodLocalPMM);
                                if (existe)
                                {
                                    //_contexto.RepositorioApertura.Actualizar(nuevoApertura);
                                    //respuesta.Errores.Add(new ErroresExcelDTO
                                    //{
                                    //    Fila = row,
                                    //    Mensaje = $"Local apertura {nuevoApertura.CodLocalPMM} ya existe!"
                                    //});
                                }
                                else
                                {
                                    nuevoApertura.UsuCreacion = "EXCEL_IMPORT";
                                    nuevoApertura.FecCreacion = DateTime.Now;
                                    _contexto.RepositorioApertura.Agregar(nuevoApertura);
                                }
                                await _contexto.GuardarCambiosAsync();
                            }
                            catch (Exception ex)
                            {
                                _contexto.Rollback();

                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = row,
                                    Mensaje = ex.Message
                                });
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
