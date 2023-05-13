using ExcelDataReader;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Web;
using SPSA.Autorizadores.Aplicacion.Extensiones;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands
{
    public class ImportarInventarioCajaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
    }

    public class ImportarInventarioCajaHandler : IRequestHandler<ImportarInventarioCajaCommand, RespuestaComunExcelDTO>
    {
        private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;

        public ImportarInventarioCajaHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja)
        {
            _repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarInventarioCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO();
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream))
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        FilterSheet = (tableReader, sheetIndex) =>
                        {
                            var name = tableReader.Name.ToLower();
                            if (name.Contains("datos"))
                                return true;
                            else
                                return false;
                        },
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                        },
                        UseColumnDataType = true
                    }).ToAllStringFields();

                    await _repositorioSovosInventarioCaja.Insertar(ds.Tables[0]);

                    respuesta.Ok = true;
                    respuesta.Mensaje = "Archivo importado correctamente";
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }
    }
}
