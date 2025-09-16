using System.Collections.Generic;
using System;
using System.IO;
using AutoMapper;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Linq;
using System.Web;
using ExcelDataReader;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using System.Globalization;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands
{
    public class ImportarMaeColaboradorExtCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
        public string Usuario { get; set; }
        public JerarquiaOrganizacionalDTO JerarquiaOrganizacional { get; set; }
    }

    public class ImportarMaeColaboradorExtHandler : IRequestHandler<ImportarMaeColaboradorExtCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };

            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream)) 
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                        },
                        UseColumnDataType = true
                    }).ToAllStringFields();

                    if (ds.Tables.Count > 0)
                    {
                        IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                        var formatStrings = new string[] { "yyyy-MM-dd", "dd/MM/yyyy HH:mm:ss", "d/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "d-MM-yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "d/MM/yyyy H:mm:ss", "dd-MM-yyyy H:mm:ss", "d-MM-yyyy H:mm:ss" };

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++) 
                        {
                            try
                            {
                                var row = ds.Tables[0].Rows[i];
                                var colaborador = new Mae_ColaboradorExt
                                {
                                    //CodLocalAlterno = row["COD_LOCAL_ALTERNO"].ToString(),
                                    //CodigoOfisis = row["CODIGO_OFISIS"].ToString(),
                                    CodigoOfisis = "0",
                                    //FechaIngresoEmpresa = DateTime.ParseExact(row["FechaIngresoEmpresa"].ToString(), formatStrings, formatProvider),
                                    //FechaCeseTrabajador = string.IsNullOrEmpty(row["FechaCeseTrabajador"].ToString()) ? (DateTime?)null : DateTime.ParseExact(row["FechaCeseTrabajador"].ToString(), formatStrings, formatProvider),
                                    ApelPaterno = row["NO_APEL_PATE"].ToString(),
                                    ApelMaterno = row["NO_APEL_MATE"].ToString(),
                                    NombreTrabajador = row["NO_TRAB"].ToString(),
                                    TipoDocIdent = row["TI_DOCU_IDEN"].ToString(),
                                    NumDocIndent = row["NU_DOCU_IDEN"].ToString(),
                                    IndActivo = row["TI_SITU"].ToString(),
                                    PuestoTrabajo = row["NO_PUES_TRAB"].ToString(),
                                    MotiSepa = row["NO_MOTI_SEPA"].ToString(),
                                    IndPersonal = row["IND_PERSONAL"].ToString(),
                                    TipoUsuario = row["TIP_USUARIO"].ToString()
                                };

                                var fechaIngresoString = row["FE_INGR_EMPR"].ToString() ?? "";
                                var fechaCeseString = row["FE_CESE_TRAB"].ToString() ?? "";

                                DateTime fechaIngreso = DateTime.MinValue;
                                DateTime fechaCese = DateTime.MinValue;

                                if (fechaIngresoString != "" && !DateTime.TryParseExact(fechaIngresoString, formatStrings, formatProvider, DateTimeStyles.None, out fechaIngreso))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Ingreso incorrecto: {fechaIngresoString}"
                                    });
                                    continue;
                                }

                                if (fechaCeseString != "" && !DateTime.TryParseExact(fechaCeseString, formatStrings, formatProvider, DateTimeStyles.None, out fechaCese))
                                {
                                    respuesta.Errores.Add(new ErroresExcelDTO
                                    {
                                        Fila = i + 2,
                                        Mensaje = $"Fecha Cese incorrecto: {fechaCeseString}"
                                    });
                                    continue;
                                }

                                colaborador.FechaIngresoEmpresa = fechaIngreso;
                                colaborador.FechaCeseTrabajador = fechaCese;

                               // var colaboradorExistente = await _contexto.RepositorioMaeColaboradorExt.Obtener(s => s.CodLocalAlterno == colaborador.CodLocalAlterno && s.CodigoOfisis == colaborador.CodigoOfisis).FirstOrDefaultAsync();
                                
                                //if (colaboradorExistente != null)
                                //{
                                //    colaboradorExistente.CodLocalAlterno = colaborador.CodLocalAlterno;
                                //    colaboradorExistente.CodigoOfisis = colaborador.CodigoOfisis;
                                //    colaboradorExistente.FechaIngresoEmpresa = colaborador.FechaIngresoEmpresa;
                                //    colaboradorExistente.FechaCeseTrabajador = colaborador.FechaCeseTrabajador;
                                //    colaboradorExistente.TiSitu = colaborador.TiSitu;
                                //    colaboradorExistente.PuestoTrabajo = colaborador.PuestoTrabajo;
                                //    colaboradorExistente.MotiSepa = colaborador.MotiSepa;
                                //    colaboradorExistente.IndPersonal = colaborador.IndPersonal;
                                //    colaboradorExistente.TipoUsuario = colaborador.TipoUsuario;
                                //    colaboradorExistente.FecModifica = DateTime.UtcNow;
                                //    colaboradorExistente.UsuModifica = request.Usuario;

                                //    await _contexto.GuardarCambiosAsync();
                                //}
                                //else
                                //{
                                    try
                                    {
                                        colaborador.UsuCreacion = request.Usuario;
                                        colaborador.FecCreacion = DateTime.UtcNow;
                                        _contexto.RepositorioMaeColaboradorExt.Agregar(colaborador);
                                        await _contexto.GuardarCambiosAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        string errorMessage = string.Empty;

                                        Exception innerEx = ex.InnerException;
                                        while (innerEx != null)
                                        {
                                            if (innerEx is Npgsql.PostgresException postgresEx)
                                            {
                                                errorMessage += " " + postgresEx.Detail;
                                            }

                                            innerEx = innerEx.InnerException;
                                        }

                                        respuesta.Errores.Add(new ErroresExcelDTO
                                        {
                                            Fila = i + 2,
                                            Mensaje = $"{errorMessage}"
                                        });

                                        // Descartar cambios de la entidad problemática
                                        _contexto.RepositorioMaeColaboradorExt.DescartarCambios(colaborador);

                                        continue;
                                    }  
                                //}
                            }
                            catch (Exception ex)
                            {
                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = i + 2,
                                    Mensaje = $"{ex.Message}"
                                });
                            }
                        }
                    }
                    respuesta.Ok = respuesta.Errores.Count == 0;
                    respuesta.Mensaje = respuesta.Ok ? "Archivo importado correctamente." : "archivo importado con algunos errores.";
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
