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
                                var nuevoActivo = new Inv_Activo
                                {
                                    CodEmpresa = worksheet.Cell(row, 1).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, 2).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, 3).Value.ToString(),
                                    CodZona = worksheet.Cell(row, 4).Value.ToString(),
                                    CodLocal = worksheet.Cell(row, 5).Value.ToString(),
                                    CodActivo = worksheet.Cell(row, 6).Value.ToString(),

                                    CodModelo = worksheet.Cell(row, 7).Value.ToString(),
                                    CodSerie = worksheet.Cell(row, 8).Value.ToString(),
                                    NomMarca = worksheet.Cell(row, 9).Value.ToString(),
                                    Ip = worksheet.Cell(row, 10).Value.ToString(),
                                    NomArea = worksheet.Cell(row, 11).Value.ToString(),
                                    NumOc = worksheet.Cell(row, 12).Value.ToString(),
                                    NumGuia = worksheet.Cell(row, 13).Value.ToString(),
                                    FecSalida = Convert.ToDateTime(worksheet.Cell(row, 14).Value),
                                    Antiguedad = Convert.ToInt32(worksheet.Cell(row, 15).Value),
                                    IndOperativo = worksheet.Cell(row, 16).Value.ToString(),
                                    Observacion = worksheet.Cell(row, 17).Value.ToString(),
                                    Garantia = worksheet.Cell(row, 18).Value.ToString(),
                                    FecActualiza = Convert.ToDateTime(worksheet.Cell(row, 19).Value)
                                };

                                bool existe = await _contexto.RepositorioInventarioActivo.Existe(x => x.CodEmpresa == nuevoActivo.CodEmpresa && x.CodCadena == nuevoActivo.CodCadena && x.CodRegion == nuevoActivo.CodRegion && x.CodZona == nuevoActivo.CodZona && x.CodLocal == nuevoActivo.CodLocal && x.CodActivo == nuevoActivo.CodActivo);
                                if (existe)
                                {
                                    //_contexto.RepositorioMaeLocal.Actualizar(nuevoLocal);
                                }
                                else
                                {
                                    _contexto.RepositorioInventarioActivo.Agregar(nuevoActivo);
                                }
                                await _contexto.GuardarCambiosAsync();
                            }
                            catch (Exception ex)
                            {
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
