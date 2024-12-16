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

namespace SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands
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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-PE");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-PE");

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

                        var expectedColumns = new[]
                        {
                            "COD_EMPRESA", "COD_CADENA", "COD_REGION", "COD_ZONA", "COD_LOCAL",
                            "NUM_CAJA", "IP_ADDRESS", "TIP_OS", "TIP_ESTADO", "TIP_UBICACION","TIP_CAJA"
                        };

                        foreach (var col in expectedColumns)
                        {
                            if (!columnMapping.ContainsKey(col))
                            {
                                throw new Exception($"La columna '{col}' no se encontró en el archivo Excel. Verifica que el archivo contenga todas las columnas requeridas.");
                            }
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var rowCells = worksheet.Row(row).CellsUsed().ToDictionary(cell => cell.WorksheetColumn().ColumnNumber(), cell => cell);
                                string codlocal = rowCells[columnMapping["COD_LOCAL"]].GetString();

                                Mae_Local maeLocal = _contexto.RepositorioMaeLocal.Obtener(x => x.CodLocal == codlocal).FirstOrDefault();

                                if (maeLocal == null)
                                {
                                    maeLocal = new Mae_Local
                                    {
                                        CodEmpresa = worksheet.Cell(row, columnMapping["COD_EMPRESA"]).GetString(),
                                        CodCadena = worksheet.Cell(row, columnMapping["COD_CADENA"]).GetString(),
                                        CodRegion = worksheet.Cell(row, columnMapping["COD_REGION"]).GetString(),
                                        CodZona = worksheet.Cell(row, columnMapping["COD_ZONA"]).GetString(),
                                        CodLocal = codlocal
                                    };

                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = row,
                                        Mensaje = $"Local incorrecto: {codlocal}"
                                    });
                                    continue;
                                }

                                int numCaja;
                                if (!int.TryParse(worksheet.Cell(row, columnMapping["NUM_CAJA"]).Value.ToString(), out numCaja))
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
                                    CodEmpresa = maeLocal.CodEmpresa,
                                    CodCadena = maeLocal.CodCadena,
                                    CodRegion = maeLocal.CodRegion,
                                    CodZona = maeLocal.CodZona,
                                    CodLocal = maeLocal.CodLocal,
                                    NumCaja = numCaja,
                                    IpAddress = worksheet.Cell(row, columnMapping["IP_ADDRESS"]).GetString(),
                                    TipOs = worksheet.Cell(row, columnMapping["TIP_OS"]).GetString(),
                                    TipEstado = worksheet.Cell(row, columnMapping["TIP_ESTADO"]).GetString(),
                                    TipUbicacion = worksheet.Cell(row, columnMapping["TIP_UBICACION"]).GetString(),
                                    TipCaja = worksheet.Cell(row, columnMapping["TIP_CAJA"]).GetString(),
                                };

                                bool existe = await _contexto.RepositorioMaeCaja.Existe(x =>
                                                                                        x.CodEmpresa == nuevoCaja.CodEmpresa && x.CodCadena == nuevoCaja.CodCadena &&
                                                                                        x.CodRegion == nuevoCaja.CodRegion && x.CodZona == nuevoCaja.CodZona &&
                                                                                        x.CodLocal == nuevoCaja.CodLocal && x.NumCaja == nuevoCaja.NumCaja);

                                if (existe)
                                {
                                    _contexto.RepositorioMaeCaja.Actualizar(nuevoCaja);
                                }
                                else
                                {
                                    _contexto.RepositorioMaeCaja.Agregar(nuevoCaja);
                                }
                                await _contexto.GuardarCambiosAsync();
                            }
                            catch (Exception ex)
                            {
                                _contexto.Rollback();

                                string mensajeError = string.Empty;

                                Exception innerEx = ex.InnerException;
                                while (innerEx != null)
                                {
                                    if (innerEx is Npgsql.PostgresException postgresEx)
                                    {
                                        mensajeError += " " + innerEx.Message;
                                    }
                                    innerEx = innerEx.InnerException;
                                }

                                if (ex.HResult == -2146233079)
                                {
                                    mensajeError = "Ya existe una caja con estas características.";
                                }

                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = row,
                                    Mensaje = mensajeError
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
