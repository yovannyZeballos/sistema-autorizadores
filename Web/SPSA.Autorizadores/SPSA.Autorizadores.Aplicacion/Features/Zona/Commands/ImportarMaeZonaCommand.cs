
using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Linq;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Zona.Commands
{
    public class ImportarMaeZonaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarMaeZonaHandler : IRequestHandler<ImportarMaeZonaCommand, RespuestaComunExcelDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeZonaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeZonaCommand request, CancellationToken cancellationToken)
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
                                var nuevoZona = new Mae_Zona
                                {
                                    CodEmpresa = worksheet.Cell(row, 1).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, 2).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, 3).Value.ToString(),
                                    CodZona = worksheet.Cell(row, 4).Value.ToString(),
                                    NomZona = worksheet.Cell(row, 5).Value.ToString(),
                                    CodCordina = worksheet.Cell(row, 6).Value.ToString()
                                };

                                bool existe = await _contexto.RepositorioMaeZona.Existe(x => x.CodEmpresa == nuevoZona.CodEmpresa && x.CodCadena == nuevoZona.CodCadena && x.CodRegion == nuevoZona.CodRegion && x.CodZona== nuevoZona.CodZona);
                                if (existe)
                                {
                                    _contexto.RepositorioMaeZona.Actualizar(nuevoZona);
                                }
                                else
                                {
                                    _contexto.RepositorioMaeZona.Agregar(nuevoZona);
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

                        //await _contexto.GuardarCambiosAsync();
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
