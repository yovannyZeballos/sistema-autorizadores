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
                                throw new InvalidOperationException($"Código alterno inválido o no encontrado para {codLocal}");
                            }

                            var existe = request.Locales.Any(l => l.CodLocalAlterno == codLocalAlterno);
                            if (!existe)
                            {
                                throw new InvalidOperationException($"Local {maeLocal.NomLocal} no está en la solicitud seleccionada.");
                            }

                            var nroSolicitud = request.NroSolicitud;
                            var codComercio = row[0]?.ToString();
                            var nroCaso = row[2]?.ToString();
                            var fecComercio = string.IsNullOrWhiteSpace(row[3]?.ToString()) ? (DateTime?)null : DateTime.Parse(row[3].ToString());
                            var desOperador = row[4]?.ToString();


                            if (await ExisteComercio(codComercio))
                            {
                                throw new InvalidOperationException($"El código comercio {codComercio} existe en {maeLocal.NomLocal}.");
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

                    respuesta.Ok = respuesta.Errores.Count == 0;
                    respuesta.Mensaje = respuesta.Ok
                        ? "Importación completada exitosamente."
                        : "Se importaron filas, pero hubo errores en algunas filas.";
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al importar codigos de comercio");
                var mensaje = ContainsConstraintViolation(ex, "pk_mae_local_comercio")
                ? "El código comercio ya se encuentra en uso."
                : "Ocurrió un error.";

                return new RespuestaComunExcelDTO
                {
                    Ok = false,
                    Mensaje = mensaje,
                    Errores = respuesta.Errores
                };
            }
        }

        private async Task<bool> ExisteComercio(string codComercio)
        {
            return await _contexto.RepositorioMaeCodComercio.Existe(x => x.CodComercio == codComercio );
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