using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands
{
    public class ImportarAperturaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarAperturaHandler : IRequestHandler<ImportarAperturaCommand, RespuestaComunExcelDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarAperturaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarAperturaCommand request, CancellationToken cancellationToken)
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
                                var nuevoApertura = new Dominio.Entidades.Apertura
                                {
                                    //CodEmpresa = worksheet.Cell(row, 1).Value.ToString(),
                                    //CodCadena = worksheet.Cell(row, 2).Value.ToString(),
                                    //CodRegion = worksheet.Cell(row, 3).Value.ToString(),
                                    //CodZona = worksheet.Cell(row, 4).Value.ToString(),
                                    //NomZona = worksheet.Cell(row, 5).Value.ToString(),
                                    //CodCordina = worksheet.Cell(row, 6).Value.ToString()
                                };

                                bool existe = await _contexto.RepositorioApertura.Existe(x => x.CodLocalPMM == nuevoApertura.CodLocalPMM);
                                if (existe)
                                {
                                    _contexto.RepositorioApertura.Actualizar(nuevoApertura);
                                }
                                else
                                {
                                    _contexto.RepositorioApertura.Agregar(nuevoApertura);
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
