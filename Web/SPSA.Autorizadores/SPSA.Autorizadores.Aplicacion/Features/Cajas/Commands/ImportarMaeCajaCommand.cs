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
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
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
                        var rowCount = worksheet.RowsUsed().Count();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var nuevoCaja = new Mae_Caja
                                {
                                    CodEmpresa = worksheet.Cell(row, 1).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, 2).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, 3).Value.ToString(),
                                    CodZona = worksheet.Cell(row, 4).Value.ToString(),
                                    CodLocal = worksheet.Cell(row, 5).Value.ToString(),
                                    NumCaja = Convert.ToInt32(worksheet.Cell(row, 6).Value.ToString()),
                                    IpAddress = worksheet.Cell(row, 7).Value.ToString(),
                                    TipOs = worksheet.Cell(row, 8).Value.ToString(),
                                    TipEstado = worksheet.Cell(row, 9).Value.ToString(),
                                };

                                bool existe = await _contexto.RepositorioMaeCaja.Existe(x => x.CodEmpresa == nuevoCaja.CodEmpresa && x.CodCadena == nuevoCaja.CodCadena && x.CodRegion == nuevoCaja.CodRegion && x.CodZona == nuevoCaja.CodZona && x.CodLocal == nuevoCaja.CodLocal && x.NumCaja == nuevoCaja.NumCaja);
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
