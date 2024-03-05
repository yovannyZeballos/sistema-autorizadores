

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

namespace SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands
{
    public class ImportarMaeRegionCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarMaeRegionHandler : IRequestHandler<ImportarMaeRegionCommand, RespuestaComunExcelDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeRegionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeRegionCommand request, CancellationToken cancellationToken)
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
                                var nuevoRegion = new Mae_Region
                                {
                                    CodEmpresa = worksheet.Cell(row, 1).Value.ToString(),
                                    CodCadena = worksheet.Cell(row, 2).Value.ToString(),
                                    CodRegion = worksheet.Cell(row, 3).Value.ToString(),
                                    NomRegion = worksheet.Cell(row, 4).Value.ToString(),
                                    CodRegional = worksheet.Cell(row, 5).Value.ToString()
                                };

                                bool existe = await _contexto.RepositorioMaeRegion.Existe(x => x.CodEmpresa == nuevoRegion.CodEmpresa && x.CodCadena == nuevoRegion.CodCadena && x.CodRegion == nuevoRegion.CodRegion);
                                if (existe)
                                {
                                    _contexto.RepositorioMaeRegion.Actualizar(nuevoRegion);
                                }
                                else
                                {
                                    _contexto.RepositorioMaeRegion.Agregar(nuevoRegion);
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
