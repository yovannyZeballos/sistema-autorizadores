using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class ImportarMaeLocalCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarMaeLocalHandler : IRequestHandler<ImportarMaeLocalCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeLocalCommand request, CancellationToken cancellationToken)
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
                            "COD_EMPRESA", "COD_CADENA", "COD_REGION", "COD_ZONA", "COD_LOCAL", "NOM_LOCAL",
                            "TIP_ESTADO", "COD_LOCAL_PMM", "COD_LOCAL_OFIPLAN", "NOM_LOCAL_OFIPLAN",
                            "COD_LOCAL_SUNAT", "IP"
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
                                var nuevoLocal = new Mae_Local
                                {
                                    CodEmpresa = worksheet.Cell(row, columnMapping["COD_EMPRESA"]).GetString(),
                                    CodCadena = worksheet.Cell(row, columnMapping["COD_CADENA"]).GetString(),
                                    CodRegion = worksheet.Cell(row, columnMapping["COD_REGION"]).GetString(),
                                    CodZona = worksheet.Cell(row, columnMapping["COD_ZONA"]).GetString(),
                                    CodLocal = worksheet.Cell(row, columnMapping["COD_LOCAL"]).GetString(),
                                    NomLocal = worksheet.Cell(row, columnMapping["NOM_LOCAL"]).GetString(),
                                    TipEstado = worksheet.Cell(row, columnMapping["TIP_ESTADO"]).GetString(),
                                    CodLocalPMM = worksheet.Cell(row, columnMapping["COD_LOCAL_PMM"]).GetString(),
                                    CodLocalOfiplan = worksheet.Cell(row, columnMapping["COD_LOCAL_OFIPLAN"]).GetString(),
                                    NomLocalOfiplan = worksheet.Cell(row, columnMapping["NOM_LOCAL_OFIPLAN"]).GetString(),
                                    CodLocalSunat = worksheet.Cell(row, columnMapping["COD_LOCAL_SUNAT"]).GetString(),
                                    Ip = worksheet.Cell(row, columnMapping["IP"]).GetString(),
                                };

                                bool existe = await _contexto.RepositorioMaeLocal.Existe(x =>
                                                                                        x.CodEmpresa == nuevoLocal.CodEmpresa && x.CodCadena == nuevoLocal.CodCadena &&
                                                                                        x.CodRegion == nuevoLocal.CodRegion && x.CodZona == nuevoLocal.CodZona &&
                                                                                        x.CodLocal == nuevoLocal.CodLocal);

                                if (existe)
                                {
                                    _contexto.RepositorioMaeLocal.Actualizar(nuevoLocal);
                                }
                                else
                                {
                                    _contexto.RepositorioMaeLocal.Agregar(nuevoLocal);
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
                                    mensajeError = "Ya existe un local con estas características.";
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
