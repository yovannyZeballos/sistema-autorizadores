using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using ExcelDataReader;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Commands
{
    public class ImportarMaeLocalComercioCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
        public string Usuario { get; set; }
        public decimal NroSolicitud { get; set; }
        public List<SolicitudCComercioDetDTO> Locales { get; set; }
    }

    public class ImportarMaeLocalComercioHandler : IRequestHandler<ImportarMaeLocalComercioCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarMaeLocalComercioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarMaeLocalComercioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
            var listaComercios = new List<Mae_CodComercio>();

            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream))
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = false
                        }
                    }).ToAllStringFields();

                    var dt = ds.Tables[0];

                    string razonSocial = dt.Rows[0][1]?.ToString()?.Trim();
                    string codEmpresa = razonSocial?.Split(' ')[0];

                    for (int i = 3; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            var row = dt.Rows[i];
                            if (row == null || string.IsNullOrWhiteSpace(row[0]?.ToString()))
                                continue;

                            string nombreComercial = row[1]?.ToString();
                            var codLocalToken = nombreComercial.Split(' ')[0];
                            var codLocal = codLocalToken.TrimStart('0');

                            var maeLocal = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == codEmpresa && x.CodLocal == codLocal).FirstOrDefaultAsync();

                            string codLocalAlternoStr = maeLocal?.CodLocalAlterno;

                            if (!int.TryParse(codLocalAlternoStr, out int codLocalAlterno))
                            {
                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = i + 1,
                                    Mensaje = $"Código alterno inválido o no encontrado para {codLocal}"
                                });
                                continue;
                            }

                            var existe = request.Locales.Any(l => l.CodLocalAlterno == codLocalAlterno);
                            if (!existe)
                            {
                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = i + 1,
                                    Mensaje = $"Local {maeLocal.NomLocal} no está en la solicitud seleccionada."
                                });
                                continue;
                            }

                            var nroSolicitud = request.NroSolicitud;
                            var codComercio = row[0]?.ToString();
                            var nroCaso = row[2]?.ToString();
                            var fecComercio = string.IsNullOrWhiteSpace(row[3]?.ToString()) ? (DateTime?)null : DateTime.Parse(row[3].ToString());
                            var desOperador = row[4]?.ToString();

                            if (await ExisteComercioActivoEnOtroLocal(codComercio, nroSolicitud, codLocalAlterno))
                            {
                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = i + 1,
                                    Mensaje = $"Código comercio {codComercio} ya se encuentra en uso."
                                });
                                continue;
                            }

                            if (await ExisteComercioEnLocal(codComercio, nroSolicitud, codLocalAlterno))
                            {
                                respuesta.Errores.Add(new ErroresExcelDTO
                                {
                                    Fila = i + 1,
                                    Mensaje = $"El código comercio {codComercio} existe en {maeLocal.NomLocal}."
                                });
                                continue;
                            }

                            listaComercios.Add(new Mae_CodComercio
                            {
                                NroSolicitud = nroSolicitud,
                                CodLocalAlterno = codLocalAlterno,
                                CodComercio = codComercio,
                                NomCanalVta = "LINEAL",
                                DesOperador = desOperador,
                                NroCaso = nroCaso,
                                FecComercio = fecComercio,
                                IndActiva = "S",
                                FecCreacion = DateTime.Now,
                                UsuCreacion = request.Usuario
                            });

                        }
                        catch (Exception exFila)
                        {
                            respuesta.Errores.Add(new ErroresExcelDTO
                            {
                                Fila = i + 1,
                                Mensaje = exFila.Message
                            });
                        }
                    }

                    if (respuesta.Errores.Any())
                    {
                        respuesta.Ok = false;
                        return respuesta;
                    }

                    var codsLocalesSolicitud = request.Locales.Select(l => l.CodLocalAlterno).Distinct().ToList();
                    var codsLocalesExcel = listaComercios.Select(c => c.CodLocalAlterno).Distinct().ToList();

                    var codsFaltantes = codsLocalesSolicitud
                                        .Where(cod => !codsLocalesExcel.Contains(cod))
                                        .ToList();

                    if (codsFaltantes.Any())
                    {
                        foreach (var codFaltante in codsFaltantes)
                        {
                            respuesta.Errores.Add(new ErroresExcelDTO
                            {
                                Fila = 0,
                                Mensaje = $"El local con CodLocalAlterno {codFaltante} no está en la solicitud seleccionada."
                            });
                        }

                        respuesta.Ok = false;

                        return respuesta;
                    }

                    foreach (var entidad in listaComercios)
                    {
                        _contexto.RepositorioMaeCodComercio.Agregar(entidad);

                        var solicitudCab = await _contexto.RepositorioCComSolicitudCab.Obtener(x => x.NroSolicitud == entidad.NroSolicitud).FirstOrDefaultAsync();

                        if (solicitudCab != null)
                        {
                            solicitudCab.TipEstado = "R";
                            solicitudCab.FecRecepcion = DateTime.UtcNow;
                            solicitudCab.UsuRecepcion = request.Usuario;

                            _contexto.RepositorioCComSolicitudCab.Actualizar(solicitudCab);
                        }

                        var solicitudDet = await _contexto.RepositorioCComSolicitudDet
                            .Obtener(x => x.NroSolicitud == entidad.NroSolicitud && x.CodLocalAlterno == entidad.CodLocalAlterno)
                            .FirstOrDefaultAsync();

                        if (solicitudDet != null)
                        {
                            solicitudDet.TipEstado = "R";
                            solicitudDet.FecModifica = DateTime.UtcNow;
                            solicitudDet.UsuModifica = request.Usuario;

                            _contexto.RepositorioCComSolicitudDet.Actualizar(solicitudDet);
                        }
                    }

                    await _contexto.GuardarCambiosAsync();

                    respuesta.Ok = true;
                    respuesta.Mensaje = "Archivo importado correctamente.";

                }
            }
            catch (Exception ex)
            {
                var mensaje = ContainsConstraintViolation(ex, "pk_mae_local_comercio")
                ? "El código comercio ya se encuentra en uso."
                : "Ocurrió un error.";


                respuesta.Ok = false;
                //respuesta.Mensaje = "Error al procesar el archivo: " + ex.Message;
                respuesta.Mensaje = mensaje;
                _logger.Error(ex, "Error al importar MAE_LOCAL_COMERCIO");
            }

            return respuesta;
        }

        private async Task<bool> ExisteComercioActivoEnOtroLocal(string codComercio, decimal nroSolicitud, int codLocalAlterno)
        {
            return await _contexto.RepositorioMaeCodComercio.Existe(x =>
                x.CodComercio == codComercio &&
                x.IndActiva == "S" &&
                !(x.NroSolicitud == nroSolicitud && x.CodLocalAlterno == codLocalAlterno)
            );
        }

        private async Task<bool> ExisteComercioEnLocal(string codComercio, decimal nroSolicitud, int codLocalAlterno)
        {
            return await _contexto.RepositorioMaeCodComercio.Existe(x =>
                x.NroSolicitud == nroSolicitud && 
                x.CodLocalAlterno == codLocalAlterno &&
                x.CodComercio == codComercio
            );
        }

        private bool ContainsConstraintViolation(Exception ex, string constraintName)
        {
            while (ex != null)
            {
                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Contains(constraintName))
                {
                    return true;
                }
                ex = ex.InnerException;
            }
            return false;
        }
    }
}