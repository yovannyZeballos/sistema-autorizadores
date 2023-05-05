using ExcelDataReader;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands
{
    public class ImportarCajasCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodFormato { get; set; }
    }

    public class ImportarCajasHandler : IRequestHandler<ImportarCajasCommand, RespuestaComunExcelDTO>
    {
        private readonly IRepositorioSovosCaja _repositorioSovosCaja;

        public ImportarCajasHandler(IRepositorioSovosCaja repositorioSovosCaja)
        {
            _repositorioSovosCaja = repositorioSovosCaja;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarCajasCommand request, CancellationToken cancellationToken)
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
                        }
                    });


                    foreach (DataTable dt in ds.Tables)
                    {
                        var errores = Validar(dt, request);

                        if(errores.Count > 0)
                        {
                            respuesta.Ok = false;
                            respuesta.Errores = errores;
                            respuesta.Mensaje = "Se encontraron algunos errores en el archivo";
                            return respuesta;
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var codFormatoExcel = row["COD_FORMATO"].ToString();
                            var codEmpresaExcel = row["COD_EMPRESA"].ToString();
                            var codLocalExcel = row["COD_LOCAL"].ToString();

                            if (codEmpresaExcel != request.CodEmpresa)
                            {
                                respuesta.Ok = false;
                            }

                            await _repositorioSovosCaja.Crear(new SovosCaja(row["COD_EMPRESA"].ToString(), row["COD_LOCAL"].ToString(), 
                                row["COD_FORMATO"].ToString(), Convert.ToDecimal(row["NUM_POS"]), row["IP_ADDRES"].ToString(), row["TIP_OS"].ToString(), "A"));
                        }
                    }

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

        private List<ErroresExcelDTO> Validar(DataTable dt, ImportarCajasCommand request)
        {
            var errores = new List<ErroresExcelDTO>();
            var fila = 1;
            foreach (DataRow row in dt.Rows)
            {
                var codFormatoExcel = row["COD_FORMATO"].ToString();
                var codEmpresaExcel = row["COD_EMPRESA"].ToString();
                var codLocalExcel = row["COD_LOCAL"].ToString();
                var soExcel = row["TIP_OS"].ToString();
                var nroCajaExcel = row["NUM_POS"].ToString();
                var ipExcel = row["IP_ADDRES"].ToString();

                if (codEmpresaExcel != request.CodEmpresa)
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"Empresa no coincide con el seleccionado - {codEmpresaExcel}" });

                if (codFormatoExcel != request.CodFormato)
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"Formato no coincide con el seleccionado - {codFormatoExcel}" });

                if (codLocalExcel != request.CodLocal)
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"Local no coincide con el seleccionado - {codLocalExcel}" });

                if (soExcel != "L" && soExcel != "W")
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"Sistema Operativo incorrecta - {soExcel}" });

                if (!decimal.TryParse(nroCajaExcel, out decimal nroCaja))
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"Numero de caja incorrecta - {nroCajaExcel}" });

                if (nroCaja <= 0)
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"Numero de caja incorrecta - {nroCajaExcel}" });

                if (!Regex.IsMatch(ipExcel, "^((25[0-5]|(2[0-4]|1\\d|[1-9]|)\\d)\\.?\\b){4}$"))
                    errores.Add(new ErroresExcelDTO { Fila = fila, Mensaje = $"IP no tiene el formato correcto - {ipExcel}" });

                fila++;
            }

            return errores;
        }

    }
}
