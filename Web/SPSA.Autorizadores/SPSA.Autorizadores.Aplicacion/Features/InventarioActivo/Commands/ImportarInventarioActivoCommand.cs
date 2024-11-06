using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands
{
    public class ImportarInventarioActivoCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarInventarioActivoHandler : IRequestHandler<ImportarInventarioActivoCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarInventarioActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarInventarioActivoCommand request, CancellationToken cancellationToken)
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
                                string codlocal = worksheet.Cell(row, columnMapping["CodLocal"]).GetString(); ;
                                Mae_Local maeLocal = _contexto.RepositorioMaeLocal.Obtener(x => x.CodLocal == codlocal).FirstOrDefault();

                                if (maeLocal == null)
                                {
                                    maeLocal = new Mae_Local();
                                    maeLocal.CodEmpresa = worksheet.Cell(row, columnMapping["CodEmpresa"]).GetString();
                                    maeLocal.CodCadena = worksheet.Cell(row, columnMapping["CodCadena"]).GetString();
                                    maeLocal.CodRegion = worksheet.Cell(row, columnMapping["CodRegion"]).GetString();
                                    maeLocal.CodZona = worksheet.Cell(row, columnMapping["CodZona"]).GetString();
                                    maeLocal.CodLocal = codlocal;

                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = row,
                                        Mensaje = $"Local incorrecto: {codlocal}"
                                    });
                                    continue;
                                }

                                var rowActivo = new Inv_Activo
                                {
                                    CodEmpresa = maeLocal.CodEmpresa,
                                    CodCadena = maeLocal.CodCadena,
                                    CodRegion = maeLocal.CodRegion,
                                    CodZona = maeLocal.CodZona,
                                    CodLocal = maeLocal.CodLocal,
                                    CodActivo = worksheet.Cell(row, columnMapping["CodActivo"]).GetString(),
                                    CodModelo = worksheet.Cell(row, columnMapping["Modelo"]).GetString(),
                                    NomMarca = worksheet.Cell(row, columnMapping["Marca"]).GetString(),
                                    CodSerie = worksheet.Cell(row, columnMapping["Serie"]).GetString(),
                                    Cantidad = Convert.ToInt32(worksheet.Cell(row, columnMapping["Cantidad"]).GetString()),
                                    Ip = worksheet.Cell(row, columnMapping["IP"]).GetString(),
                                    NomArea = worksheet.Cell(row, columnMapping["Area"]).GetString(),
                                    NumOc = worksheet.Cell(row, columnMapping["OC"]).GetString(),
                                    NumGuia = worksheet.Cell(row, columnMapping["Guia"]).GetString(),
                                    Antiguedad = Convert.ToInt32(worksheet.Cell(row, columnMapping["Antiguedad"]).GetString()),
                                    IndOperativo = worksheet.Cell(row, columnMapping["IndOperativo"]).GetString(),
                                    Observacion = worksheet.Cell(row, columnMapping["Obs/TK"]).GetString(),
                                    Garantia = worksheet.Cell(row, columnMapping["Garantia"]).GetString(),
                                };
                                object cellValueFecSalida = worksheet.Cell(row, columnMapping["FechaSalida"]).Value;
                                bool isEmptyOrNullFecSalida = cellValueFecSalida == null || string.IsNullOrWhiteSpace(cellValueFecSalida.ToString());
                                rowActivo.FecSalida = isEmptyOrNullFecSalida ? null : DateTime.TryParse(cellValueFecSalida.ToString(), out DateTime fecha1) ? fecha1 : (DateTime?)null;

                                object cellValueFecActualiza = worksheet.Cell(row, columnMapping["FechaActualiza"]).Value;
                                bool isEmptyOrNullFecActualiza = cellValueFecActualiza == null || string.IsNullOrWhiteSpace(cellValueFecActualiza.ToString());
                                rowActivo.FecActualiza = isEmptyOrNullFecActualiza ? null : DateTime.TryParse(cellValueFecActualiza.ToString(), out DateTime fecha2) ? fecha2 : (DateTime?)null;

                                bool existe = await _contexto.RepositorioInventarioActivo.Existe(x => x.CodEmpresa == rowActivo.CodEmpresa && x.CodCadena == rowActivo.CodCadena && x.CodRegion == rowActivo.CodRegion && x.CodZona == rowActivo.CodZona && x.CodLocal == rowActivo.CodLocal && x.CodActivo == rowActivo.CodActivo && x.CodModelo == rowActivo.CodModelo && x.NomMarca == rowActivo.NomMarca && x.CodSerie == rowActivo.CodSerie);
                                if (existe)
                                {
                                    _contexto.RepositorioInventarioActivo.Actualizar(rowActivo);
                                }
                                else
                                {
                                    _contexto.RepositorioInventarioActivo.Agregar(rowActivo);
                                }
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
