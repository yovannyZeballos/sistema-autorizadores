using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Npgsql;
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
using System.Transactions;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands
{
    public class ImportarInventarioActivoCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarInventarioActivoHandler : IRequestHandler<ImportarInventarioActivoCommand, RespuestaComunExcelDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarInventarioActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
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

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var rowActivo = new Inv_Activo
                                {
                                    CodEmpresa = worksheet.Cell(row, 4).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, 5).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, 6).Value.ToString(),
                                    CodZona = worksheet.Cell(row, 7).Value.ToString(),
                                    CodLocal = worksheet.Cell(row, 8).Value.ToString(),
                                    CodActivo = worksheet.Cell(row, 10).Value.ToString(),
                                    CodModelo = worksheet.Cell(row, 12).Value.ToString(),
                                    NomMarca = worksheet.Cell(row, 13).Value.ToString(),
                                    CodSerie = worksheet.Cell(row, 14).Value.ToString(),
                                    Ip = worksheet.Cell(row, 16).Value.ToString(),
                                    NomArea = worksheet.Cell(row, 17).Value.ToString(),
                                    NumOc = worksheet.Cell(row, 18).Value.ToString(),
                                    NumGuia = worksheet.Cell(row, 19).Value.ToString(),
                                    //FecSalida = Convert.ToDateTime(worksheet.Cell(row, 14).Value),
                                    Antiguedad = Convert.ToInt32(worksheet.Cell(row, 21).Value),
                                    IndOperativo = worksheet.Cell(row, 22).Value.ToString(),
                                    Observacion = worksheet.Cell(row, 24).Value.ToString(),
                                    Garantia = worksheet.Cell(row, 25).Value.ToString(),
                                    //FecActualiza = Convert.ToDateTime(worksheet.Cell(row, 19).Value)
                                };

                                object cellValueFecSalida = worksheet.Cell(row, 20).Value;
                                bool isEmptyOrNullFecSalida = cellValueFecSalida == null || string.IsNullOrWhiteSpace(cellValueFecSalida.ToString());
                                rowActivo.FecSalida = isEmptyOrNullFecSalida ? null : DateTime.TryParse(cellValueFecSalida.ToString(), out DateTime fecha1) ? fecha1 : (DateTime?)null;

                                object cellValueFecActualiza = worksheet.Cell(row, 26).Value;
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
