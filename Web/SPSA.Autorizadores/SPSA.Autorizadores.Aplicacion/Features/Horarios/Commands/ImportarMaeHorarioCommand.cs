using System.Collections.Generic;
using System;
using System.IO;
using AutoMapper;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands
{
    public class ImportarMaeHorarioCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarMaeHorarioHandler : IRequestHandler<ImportarMaeHorarioCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeHorarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeHorarioCommand request, CancellationToken cancellationToken)
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
                            "NUM_DIA", "COD_DIA", "HOR_OPEN", "HOR_CLOSE", "MIN_LMT"
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

                                int numDia;
                                if (!int.TryParse(worksheet.Cell(row, columnMapping["NUM_DIA"]).Value.ToString(), out numDia))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = row,
                                        Mensaje = "El valor de NumDia no es un número válido."
                                    });
                                    continue;
                                }

                                var nuevoHorario = new Mae_Horario
                                {
                                    CodEmpresa = maeLocal.CodEmpresa,
                                    CodCadena = maeLocal.CodCadena,
                                    CodRegion = maeLocal.CodRegion,
                                    CodZona = maeLocal.CodZona,
                                    CodLocal = maeLocal.CodLocal,
                                    NumDia = numDia,
                                    CodDia = worksheet.Cell(row, columnMapping["COD_DIA"]).GetString(),
                                    HorOpen = worksheet.Cell(row, columnMapping["HOR_OPEN"]).GetString(),
                                    HorClose = worksheet.Cell(row, columnMapping["HOR_CLOSE"]).GetString(),
                                    MinLmt = worksheet.Cell(row, columnMapping["MIN_LMT"]).GetString()
                                };

                                bool existe = await _contexto.RepositorioMaeHorario.Existe(x =>
                                                                                        x.CodEmpresa == nuevoHorario.CodEmpresa && x.CodCadena == nuevoHorario.CodCadena &&
                                                                                        x.CodRegion == nuevoHorario.CodRegion && x.CodZona == nuevoHorario.CodZona &&
                                                                                        x.CodLocal == nuevoHorario.CodLocal && x.NumDia == nuevoHorario.NumDia);

                                if (existe)
                                {
                                    _contexto.RepositorioMaeHorario.Actualizar(nuevoHorario);
                                }
                                else
                                {
                                    _contexto.RepositorioMaeHorario.Agregar(nuevoHorario);
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
                                    mensajeError = "Ya existe horario con estas características.";
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
