using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class ImportarInvKardexCommand : IRequest<RespuestaComunExcelDTO>
    {
        public Stream ArchivoExcel { get; set; }
    }

    public class ImportarInvKardexHandler : IRequestHandler<ImportarInvKardexCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarInvKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarInvKardexCommand request, CancellationToken cancellationToken)
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

                                var rowKardex = new InvKardex
                                {
                                    Kardex = worksheet.Cell(row, 1).Value.ToString(),
                                    //Fecha = Convert.ToDateTime(worksheet.Cell(row, 2).Value),
                                    Guia = worksheet.Cell(row, 3).Value.ToString(),
                                    ActivoId = worksheet.Cell(row, 4).Value.ToString(),
                                    Serie = worksheet.Cell(row, 5).Value.ToString(),
                                    OrigenId = worksheet.Cell(row, 6).Value.ToString(),
                                    DestinoId = worksheet.Cell(row, 7).Value.ToString(),
                                    Tk = worksheet.Cell(row, 8).Value.ToString(),
                                    Cantidad = Convert.ToInt32(worksheet.Cell(row, 9).Value),
                                    TipoStock = worksheet.Cell(row, 10).Value.ToString(),
                                    Oc = worksheet.Cell(row, 11).Value.ToString(),
                                    Sociedad = worksheet.Cell(row, 12).Value.ToString(),
                                };

                                object cellValueFecha = worksheet.Cell(row, 2).Value;
                                bool isEmptyOrNullFecha = cellValueFecha == null || string.IsNullOrWhiteSpace(cellValueFecha.ToString());
                                rowKardex.Fecha = (DateTime)(isEmptyOrNullFecha ? null : DateTime.TryParse(cellValueFecha.ToString(), out DateTime fecha1) ? fecha1 : (DateTime?)null);


                                //bool existe = await _contexto.RepositorioInventarioActivo.Existe(x => x.CodEmpresa == rowActivo.CodEmpresa && x.CodCadena == rowActivo.CodCadena && x.CodRegion == rowActivo.CodRegion && x.CodZona == rowActivo.CodZona && x.CodLocal == rowActivo.CodLocal && x.CodActivo == rowActivo.CodActivo && x.CodModelo == rowActivo.CodModelo && x.NomMarca == rowActivo.NomMarca && x.CodSerie == rowActivo.CodSerie);
                                //if (existe)
                                //{
                                //    _contexto.RepositorioInvKardex.Actualizar(rowKardex);
                                //}
                                //else
                                //{
                                _contexto.RepositorioInvKardex.Agregar(rowKardex);
                                //}
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
