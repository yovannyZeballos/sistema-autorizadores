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
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeLocalCommand request, CancellationToken cancellationToken)
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
                                var nuevoLocal = new Mae_Local
                                {
                                    CodEmpresa = worksheet.Cell(row, 1).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, 2).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, 3).Value.ToString(),
                                    CodZona = worksheet.Cell(row, 4).Value.ToString(),
                                    CodLocal = worksheet.Cell(row, 5).Value.ToString(),
                                    NomLocal = worksheet.Cell(row, 6).Value.ToString(),
                                    TipEstado = worksheet.Cell(row, 7).Value.ToString(),
                                    CodLocalPMM = worksheet.Cell(row, 8).Value.ToString(),
                                    CodLocalOfiplan = worksheet.Cell(row, 9).Value.ToString(),
                                    NomLocalOfiplan = worksheet.Cell(row, 10).Value.ToString(),
                                    CodLocalSunat = worksheet.Cell(row, 11).Value.ToString()
                                };

                                bool existe = await _contexto.RepositorioMaeLocal.Existe(x => x.CodEmpresa == nuevoLocal.CodEmpresa && x.CodCadena == nuevoLocal.CodCadena && x.CodRegion == nuevoLocal.CodRegion && x.CodZona == nuevoLocal.CodZona && x.CodLocal == nuevoLocal.CodLocal);
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
