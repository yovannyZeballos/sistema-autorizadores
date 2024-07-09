using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using ExcelDataReader;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands
{
    public class ImportarExcelInvCajaCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
        public string Usuario { get; set; }
        public JerarquiaOrganizacionalDTO JerarquiaOrganizacional { get; set; }
    }

    public class ImportarExcelInvCajaHandler : IRequestHandler<ImportarExcelInvCajaCommand, RespuestaComunExcelDTO>
    {

        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;

        public ImportarExcelInvCajaHandler()
        {
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarExcelInvCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream))
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        FilterSheet = (tableReader, sheetIndex) => tableReader.Name.ToLower().Contains("plantilla"),
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                        },
                        UseColumnDataType = true
                    }).ToAllStringFields();


                    if (ds.Tables.Count > 0)
                    {
                        IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                        var formatStrings = new string[] { "dd/MM/yyyy HH:mm:ss", "d/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "d-MM-yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "d/MM/yyyy H:mm:ss", "dd-MM-yyyy H:mm:ss", "d-MM-yyyy H:mm:ss" };


                        var codActivos = await _contexto.RepositorioInvTipoActivo.Obtener()
                            .AsNoTracking()
                            .Select(x => x.CodActivo)
                            .ToListAsync();

                        var invCajasList = new List<InvCajas>();


                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                DataRow row = ds.Tables[0].Rows[i];

                                if (!int.TryParse(row[6].ToString(), out int numCaja))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Número de caja incorrecto: {row[6]}"
                                    });
                                    continue;
                                }


                                var fechaGarantiaCpuString = row[18].ToString() ?? "";
                                var fechaGarantiaImpresoraString = row[23].ToString() ?? "";
                                var fechaGarantiaDynakeyString = row[28].ToString() ?? "";
                                var fechaGarantiaBalanzaString = row[33].ToString() ?? "";
                                var fechaGarantiaGavetaString = row[38].ToString() ?? "";
                                var fechaGarantiaPantallaString = row[42].ToString() ?? "";

                                var fechaInicioLisingString = "";
                                var fechaFinLisingString = "";

                                DateTime fecGarantiaCpu = DateTime.MinValue;
                                DateTime fecGarantiaImpresora = DateTime.MinValue;
                                DateTime fecGarantiaDynakey = DateTime.MinValue;
                                DateTime fecGarantiaBalanza = DateTime.MinValue;
                                DateTime fecGarantiaGaveta = DateTime.MinValue;
                                DateTime fecGarantiaPantalla = DateTime.MinValue;

                                DateTime fechaInicioLising = DateTime.MinValue;
                                DateTime fechaFinLising = DateTime.MinValue;

                                if (fechaGarantiaCpuString != "" && !DateTime.TryParseExact(fechaGarantiaCpuString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantiaCpu))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaCpuString}"
                                    });
                                    continue;
                                }

                                if (fechaGarantiaImpresoraString != "" && !DateTime.TryParseExact(fechaGarantiaImpresoraString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantiaImpresora))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaImpresoraString}"
                                    });
                                    continue;
                                }

                                if (fechaGarantiaDynakeyString != "" && !DateTime.TryParseExact(fechaGarantiaDynakeyString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantiaDynakey))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaDynakeyString}"
                                    });
                                    continue;
                                }

                                if (fechaGarantiaBalanzaString != "" && !DateTime.TryParseExact(fechaGarantiaBalanzaString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantiaBalanza))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaBalanzaString}"
                                    });
                                    continue;
                                }

                                if (fechaGarantiaGavetaString != "" && !DateTime.TryParseExact(fechaGarantiaGavetaString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantiaGaveta))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaGavetaString}"
                                    });
                                    continue;
                                }

                                if (fechaGarantiaPantallaString != "" && !DateTime.TryParseExact(fechaGarantiaPantallaString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantiaPantalla))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaPantallaString}"
                                    });
                                    continue;
                                }

                                if (fechaInicioLisingString != "" && !DateTime.TryParseExact(fechaInicioLisingString, formatStrings, formatProvider, DateTimeStyles.None, out fechaInicioLising))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha inicio Lising incorrecto: {fechaInicioLisingString}"
                                    });
                                    continue;
                                }

                                if (fechaFinLisingString != "" && !DateTime.TryParseExact(fechaFinLisingString, formatStrings, formatProvider, DateTimeStyles.None, out fechaFinLising))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha fin Lising incorrecto: {fechaFinLisingString}"
                                    });
                                    continue;
                                }

                                //ADD CPU
                                var invCpu = new InvCajas
                                {
                                    CodEmpresa = row[0].ToString(),
                                    CodCadena = row[1].ToString(),
                                    CodRegion = row[2].ToString(),
                                    CodZona = row[3].ToString(),
                                    CodLocal = row[4].ToString(),
                                    NumCaja = numCaja,
                                    CodActivo = "01",
                                    CodModelo = row[8].ToString(),
                                    CodSerie = row[16].ToString(),
                                    NumAdenda = row[17].ToString(),
                                    FecGarantia = fecGarantiaCpu == DateTime.MinValue ? null : (DateTime?)fecGarantiaCpu,
                                    TipEstado = row[19].ToString(),
                                    TipProcesador = row[9].ToString(),
                                    Memoria = row[10].ToString(),
                                    DesSo = row[11].ToString(),
                                    VerSo = row[12].ToString(),
                                    CapDisco = row[13].ToString(),
                                    TipDisco = row[14].ToString(),
                                    DesPuertoBalanza = row[15].ToString(),
                                    TipoCaja = "",
                                    Hostname = row[7].ToString(),
                                    FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
                                    FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
                                };

                                if (!ValidarTamañoCampos(invCpu, respuesta.Errores, i + 2))
                                    continue;


                                if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invCpu, respuesta.Errores, i + 2))
                                    continue;

                                if (!codActivos.Contains(invCpu.CodActivo))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"El código de activo ingresado no existe: {invCpu.CodActivo}"
                                    });
                                    continue;
                                }

                                invCajasList.Add(invCpu);

                                //ADD IMPRESORA
                                var invImpresora = new InvCajas
                                {
                                    CodEmpresa = row[0].ToString(),
                                    CodCadena = row[1].ToString(),
                                    CodRegion = row[2].ToString(),
                                    CodZona = row[3].ToString(),
                                    CodLocal = row[4].ToString(),
                                    NumCaja = numCaja,
                                    CodActivo = "02",
                                    CodModelo = row[20].ToString(),
                                    CodSerie = row[21].ToString(),
                                    NumAdenda = row[22].ToString(),
                                    FecGarantia = fecGarantiaImpresora == DateTime.MinValue ? null : (DateTime?)fecGarantiaImpresora,
                                    TipEstado = row[24].ToString(),
                                    TipProcesador = row[9].ToString(),
                                    Memoria = row[10].ToString(),
                                    DesSo = row[11].ToString(),
                                    VerSo = row[12].ToString(),
                                    CapDisco = row[13].ToString(),
                                    TipDisco = row[14].ToString(),
                                    DesPuertoBalanza = row[15].ToString(),
                                    TipoCaja = "",
                                    Hostname = row[7].ToString(),
                                    FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
                                    FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
                                };

                                if (!ValidarTamañoCampos(invImpresora, respuesta.Errores, i + 2))
                                    continue;


                                if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invImpresora, respuesta.Errores, i + 2))
                                    continue;

                                if (!codActivos.Contains(invImpresora.CodActivo))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"El código de activo ingresado no existe: {invImpresora.CodActivo}"
                                    });
                                    continue;
                                }

                                invCajasList.Add(invImpresora);

                                //ADD DYNAKEY
                                var invDynakey = new InvCajas
                                {
                                    CodEmpresa = row[0].ToString(),
                                    CodCadena = row[1].ToString(),
                                    CodRegion = row[2].ToString(),
                                    CodZona = row[3].ToString(),
                                    CodLocal = row[4].ToString(),
                                    NumCaja = numCaja,
                                    CodActivo = "03",
                                    CodModelo = row[25].ToString(),
                                    CodSerie = row[26].ToString(),
                                    NumAdenda = row[27].ToString(),
                                    FecGarantia = fecGarantiaDynakey == DateTime.MinValue ? null : (DateTime?)fecGarantiaDynakey,
                                    TipEstado = row[29].ToString(),
                                    TipProcesador = row[9].ToString(),
                                    Memoria = row[10].ToString(),
                                    DesSo = row[11].ToString(),
                                    VerSo = row[12].ToString(),
                                    CapDisco = row[13].ToString(),
                                    TipDisco = row[14].ToString(),
                                    DesPuertoBalanza = row[15].ToString(),
                                    TipoCaja = "",
                                    Hostname = row[7].ToString(),
                                    FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
                                    FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
                                };

                                if (!ValidarTamañoCampos(invDynakey, respuesta.Errores, i + 2))
                                    continue;


                                if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invDynakey, respuesta.Errores, i + 2))
                                    continue;

                                if (!codActivos.Contains(invDynakey.CodActivo))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"El código de activo ingresado no existe: {invDynakey.CodActivo}"
                                    });
                                    continue;
                                }

                                invCajasList.Add(invDynakey);

                                //ADD BALANZA
                                var invBalanza = new InvCajas
                                {
                                    CodEmpresa = row[0].ToString(),
                                    CodCadena = row[1].ToString(),
                                    CodRegion = row[2].ToString(),
                                    CodZona = row[3].ToString(),
                                    CodLocal = row[4].ToString(),
                                    NumCaja = numCaja,
                                    CodActivo = "04",
                                    CodModelo = row[30].ToString(),
                                    CodSerie = row[31].ToString(),
                                    NumAdenda = row[32].ToString(),
                                    FecGarantia = fecGarantiaBalanza == DateTime.MinValue ? null : (DateTime?)fecGarantiaBalanza,
                                    TipEstado = row[34].ToString(),
                                    TipProcesador = row[9].ToString(),
                                    Memoria = row[10].ToString(),
                                    DesSo = row[11].ToString(),
                                    VerSo = row[12].ToString(),
                                    CapDisco = row[13].ToString(),
                                    TipDisco = row[14].ToString(),
                                    DesPuertoBalanza = row[15].ToString(),
                                    TipoCaja = "",
                                    Hostname = row[7].ToString(),
                                    FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
                                    FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
                                };

                                if (!ValidarTamañoCampos(invBalanza, respuesta.Errores, i + 2))
                                    continue;


                                if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invBalanza, respuesta.Errores, i + 2))
                                    continue;

                                if (!codActivos.Contains(invBalanza.CodActivo))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"El código de activo ingresado no existe: {invBalanza.CodActivo}"
                                    });
                                    continue;
                                }

                                invCajasList.Add(invBalanza);

                                //ADD GAVETA
                                var invGaveta = new InvCajas
                                {
                                    CodEmpresa = row[0].ToString(),
                                    CodCadena = row[1].ToString(),
                                    CodRegion = row[2].ToString(),
                                    CodZona = row[3].ToString(),
                                    CodLocal = row[4].ToString(),
                                    NumCaja = numCaja,
                                    CodActivo = "05",
                                    CodModelo = row[35].ToString(),
                                    CodSerie = row[36].ToString(),
                                    NumAdenda = row[37].ToString(),
                                    FecGarantia = fecGarantiaGaveta == DateTime.MinValue ? null : (DateTime?)fecGarantiaGaveta,
                                    TipEstado = row[39].ToString(),
                                    TipProcesador = row[9].ToString(),
                                    Memoria = row[10].ToString(),
                                    DesSo = row[11].ToString(),
                                    VerSo = row[12].ToString(),
                                    CapDisco = row[13].ToString(),
                                    TipDisco = row[14].ToString(),
                                    DesPuertoBalanza = row[15].ToString(),
                                    TipoCaja = "",
                                    Hostname = row[7].ToString(),
                                    FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
                                    FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
                                };

                                if (!ValidarTamañoCampos(invGaveta, respuesta.Errores, i + 2))
                                    continue;


                                if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invGaveta, respuesta.Errores, i + 2))
                                    continue;

                                if (!codActivos.Contains(invGaveta.CodActivo))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"El código de activo ingresado no existe: {invGaveta.CodActivo}"
                                    });
                                    continue;
                                }

                                invCajasList.Add(invGaveta);

                                //ADD PANTALLA
                                var invPantalla = new InvCajas
                                {
                                    CodEmpresa = row[0].ToString(),
                                    CodCadena = row[1].ToString(),
                                    CodRegion = row[2].ToString(),
                                    CodZona = row[3].ToString(),
                                    CodLocal = row[4].ToString(),
                                    NumCaja = numCaja,
                                    CodActivo = "06",
                                    CodModelo = row[40].ToString(),
                                    CodSerie = row[41].ToString(),
                                    NumAdenda = "",
                                    FecGarantia = fecGarantiaPantalla == DateTime.MinValue ? null : (DateTime?)fecGarantiaPantalla,
                                    TipEstado = row[39].ToString(),
                                    TipProcesador = row[9].ToString(),
                                    Memoria = row[10].ToString(),
                                    DesSo = row[11].ToString(),
                                    VerSo = row[12].ToString(),
                                    CapDisco = row[13].ToString(),
                                    TipDisco = row[14].ToString(),
                                    DesPuertoBalanza = row[15].ToString(),
                                    TipoCaja = "",
                                    Hostname = row[7].ToString(),
                                    FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
                                    FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
                                };

                                if (!ValidarTamañoCampos(invPantalla, respuesta.Errores, i + 2))
                                    continue;


                                if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invPantalla, respuesta.Errores, i + 2))
                                    continue;

                                if (!codActivos.Contains(invPantalla.CodActivo))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"El código de activo ingresado no existe: {invPantalla.CodActivo}"
                                    });
                                    continue;
                                }

                                invCajasList.Add(invPantalla);
                            }
                            catch (Exception ex)
                            {
                                _contexto.Rollback();

                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = i + 2,
                                    Mensaje = ex.Message
                                });
                            }
                        }

                        var batchSize = 100;
                        var invCajasExistentes = new List<InvCajas>();

                        for (int i = 0; i < invCajasList.Count; i += batchSize)
                        {
                            var batch = invCajasList.Skip(i).Take(batchSize).ToList();
                            var codigosUnicos = batch.Select(y => $"{y.CodEmpresa}{y.CodCadena}{y.CodRegion}{y.CodZona}{y.CodLocal}{y.NumCaja}{y.CodActivo}").ToList();

                            var batchResult = await _contexto.RepositorioInvCajas.Obtener(x => codigosUnicos.Any(y => y == x.CodEmpresa + x.CodCadena + x.CodRegion + x.CodZona + x.CodLocal + x.NumCaja + x.CodActivo)).ToListAsync();

                            invCajasExistentes.AddRange(batchResult);
                        }

                        foreach (var invCajas in invCajasList)
                        {
                            var invCajaExistente = invCajasExistentes.FirstOrDefault(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena
                            && x.CodRegion == invCajas.CodRegion && x.CodZona == invCajas.CodZona && x.CodLocal == invCajas.CodLocal
                            && x.NumCaja == invCajas.NumCaja && x.CodActivo == invCajas.CodActivo);

                            if (invCajaExistente != null)
                            {
                                ActualizarInvCaja(invCajas, invCajaExistente);
                                await _contexto.GuardarCambiosAsync();
                            }
                            else
                            {
                                try
                                {
                                    AgregarInvCaja(invCajas);
                                    await _contexto.GuardarCambiosAsync();
                                }
                                catch (Exception ex)
                                {
                                    string errorMessage = string.Empty;

                                    Exception innerEx = ex.InnerException;
                                    while (innerEx != null)
                                    {
                                        //errorMessage += " Error: " + innerEx.Message;

                                        if (innerEx is Npgsql.PostgresException postgresEx)
                                        {
                                            errorMessage += " " + postgresEx.Detail;
                                        }

                                        innerEx = innerEx.InnerException;
                                    }

                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = 0,
                                        //Mensaje = $"Local {invCajas.CodActivo} - NumCaja {invCajas.NumCaja} - CodAcivo {invCajas.CodActivo}: {errorMessage}"
                                        Mensaje = $"{errorMessage}"
                                    });
                                    continue;
                                }
                            }
                        }
                    }

                    respuesta.Ok = respuesta.Errores.Count == 0;
                    respuesta.Mensaje = respuesta.Ok ? "Archivo importado correctamente." : "archivo importado con algunos errores.";

                    //await _contexto.GuardarCambiosAsync();
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }


            return respuesta;
        }

        private bool ValidarJerarquiaOrganizacional(JerarquiaOrganizacionalDTO jerarquiaOrganizacional, InvCajas invCajas, List<ErroresExcelDTO> errores, int fila)
        {
            var exito = true;

            if (!jerarquiaOrganizacional.EmpresasAsociadas.Any(x => x.CodEmpresa == invCajas.CodEmpresa))
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = $"Empresa {invCajas.CodEmpresa} no asociada al usuario"
                });

                exito = false;
            }

            return exito;
        }

        private void ActualizarInvCaja(InvCajas invCajas, InvCajas invCajaBD)
        {
            invCajaBD.CapDisco = invCajas.CapDisco;
            invCajaBD.CodModelo = invCajas.CodModelo;
            invCajaBD.CodSerie = invCajas.CodSerie;
            invCajaBD.DesPuertoBalanza = invCajas.DesPuertoBalanza;
            invCajaBD.FecGarantia = invCajas.FecGarantia;
            invCajaBD.Memoria = invCajas.Memoria;
            invCajaBD.TipDisco = invCajas.TipDisco;
            invCajaBD.TipEstado = invCajas.TipEstado;
            invCajaBD.TipProcesador = invCajas.TipProcesador;
            invCajaBD.VerSo = invCajas.VerSo;
            invCajaBD.DesSo = invCajas.DesSo;
            invCajaBD.NumAdenda = invCajas.NumAdenda;
            invCajaBD.TipoCaja = invCajas.TipoCaja;
            invCajaBD.Hostname = invCajas.Hostname;
            invCajaBD.FechaInicioLising = invCajas.FechaInicioLising;
            invCajaBD.FechaFinLising = invCajas.FechaFinLising;
        }

        private void AgregarInvCaja(InvCajas invCajas)
        {
            _contexto.RepositorioInvCajas.Agregar(invCajas);
        }

        private bool ValidarTamañoCampos(InvCajas invCajas, List<ErroresExcelDTO> errores, int fila)
        {
            var exito = true;

            if (invCajas.CodEmpresa.Length > 2)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = $"El codigo de enpresa no debe tener más de 2 digitos"
                });
                exito = false;
            }

            if (invCajas.CodCadena.Length > 2)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = $"El codigo de cadena no debe tener más de 2 digitos"
                });
                exito = false;
            }

            if (invCajas.CodRegion.Length > 2)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = $"El codigo de región no debe tener más de 2 digitos"
                });
                exito = false;
            }

            if (invCajas.CodZona.Length > 3)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = $"El codigo de zona no debe tener más de 3 digitos"
                });
                exito = false;
            }

            if (invCajas.CodLocal.Length > 4)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = $"El codigo de local no debe tener más de 4 digitos"
                });
                exito = false;
            }

            if (invCajas.CodModelo.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El modelo no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.CodSerie.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "La serie no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.NumAdenda.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "La adenda no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.TipEstado.Length > 1)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El estado no debe tener más de 1 caracteres"
                });
                exito = false;
            }

            if (invCajas.TipProcesador.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El procesador no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.Memoria.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "La memoria no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.DesSo.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El sistema operativo no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.VerSo.Length > 20)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "La versión del sistema operativo no debe tener más de 20 caracteres"
                });
                exito = false;
            }

            if (invCajas.CapDisco.Length > 20)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "La capacidad del disco no debe tener más de 20 caracteres"
                });
                exito = false;
            }

            if (invCajas.TipDisco.Length > 20)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El tipo de disco no debe tener más de 20 caracteres"
                });
                exito = false;
            }

            if (invCajas.DesPuertoBalanza.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El puerto de la balanza no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            if (invCajas.TipoCaja.Length > 100)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El tipo de caja no debe tener más de 100 caracteres"
                });
                exito = false;
            }

            if (invCajas.Hostname.Length > 50)
            {
                errores.Add(new ErroresExcelDTO
                {
                    Fila = fila,
                    Mensaje = "El hostname no debe tener más de 50 caracteres"
                });
                exito = false;
            }

            return exito;
        }

    }
}
