using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Command
{
    public class ImportarMaeCajaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarMaeCajaHandler : IRequestHandler<ImportarMaeCajaCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };

            try
            {
                using (var stream = request.ArchivoExcel)
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var headerRow = worksheet.FirstRowUsed();
                        var rowCount = worksheet.RowsUsed().Count();

                        // Crear un diccionario para mapear los nombres de las columnas a sus índices
                        var columnMap = headerRow.Cells()
                            .ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber);


                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Validación y conversión segura de NumCaja
                                int numCaja;
                                if (!int.TryParse(worksheet.Cell(row, columnMap["NUM_CAJA"]).Value.ToString(), out numCaja))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = row,
                                        Mensaje = "El valor de NumCaja no es un número válido."
                                    });
                                    continue;
                                }

                                var nuevoCaja = new Mae_Caja
                                {
                                    CodEmpresa = worksheet.Cell(row, columnMap["COD_EMPRESA"]).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, columnMap["COD_CADENA"]).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, columnMap["COD_REGION"]).Value.ToString(),
                                    CodZona = worksheet.Cell(row, columnMap["COD_ZONA"]).Value.ToString(),
                                    CodLocal = worksheet.Cell(row, columnMap["COD_LOCAL"]).Value.ToString(),
                                    NumCaja = numCaja,
                                    IpAddress = worksheet.Cell(row, columnMap["IP_ADDRESS"]).Value.ToString(),
                                    TipOs = worksheet.Cell(row, columnMap["TIP_OS"]).Value.ToString(),
                                    TipEstado = worksheet.Cell(row, columnMap["TIP_ESTADO"]).Value.ToString(),
                                };

                                bool existe = await _contexto.RepositorioMaeCaja.Existe(x =>
                                                                                        x.CodEmpresa == nuevoCaja.CodEmpresa && x.CodCadena == nuevoCaja.CodCadena &&
                                                                                        x.CodRegion == nuevoCaja.CodRegion && x.CodZona == nuevoCaja.CodZona &&
                                                                                        x.CodLocal == nuevoCaja.CodLocal && x.NumCaja == nuevoCaja.NumCaja);

                                if (existe)
                                {
                                    _contexto.RepositorioMaeCaja.Actualizar(nuevoCaja);
                                    await _contexto.GuardarCambiosAsync();
                                }
                                else
                                {
                                    try
                                    {
                                        _contexto.RepositorioMaeCaja.Agregar(nuevoCaja);
                                        await _contexto.GuardarCambiosAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        string errorMessage = string.Empty;

                                        Exception innerEx = ex.InnerException;
                                        while (innerEx != null)
                                        {
                                            //errorMessage += " Error: " + innerEx.Message;

                                            if (innerEx is Npgsql.PostgresException postgresEx)
                                            {
                                                errorMessage += " " + postgresEx.Detail;
                                            }

                                            innerEx = innerEx.InnerException;
                                        }

                                        respuesta.Errores.Add(new ErroresExcelDTO
                                        {
                                            Fila = 0,
                                            Mensaje = $"{errorMessage}"
                                        });

                                        // Descartar cambios de la entidad problemática
                                        _contexto.RepositorioMaeCaja.DescartarCambios(nuevoCaja);

                                        continue;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                //_contexto.Rollback();

                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = row,
                                    Mensaje = ex.Message
                                });
                                continue;
                            }
                        }

                        respuesta.Ok = respuesta.Errores.Count == 0;
                        respuesta.Mensaje = respuesta.Ok ? "Archivo importado correctamente." : "archivo importado con algunos errores.";

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
