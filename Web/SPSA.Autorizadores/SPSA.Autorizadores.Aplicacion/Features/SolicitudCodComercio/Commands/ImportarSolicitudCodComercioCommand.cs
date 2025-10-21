using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Web;
using AutoMapper;
using ExcelDataReader;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using System.Linq;
using System.Data.Entity;
using SPSA.Autorizadores.Dominio.Entidades;
using System.IO;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Commands
{
    public class ImportarSolicitudCodComercioCommand : IRequest<RespuestaComunExcelDTO>
    {
        public HttpPostedFileBase Archivo { get; set; }
        public string Usuario { get; set; }
    }

    public class ImportarSolicitudCodComercioHandler : IRequestHandler<ImportarSolicitudCodComercioCommand, RespuestaComunExcelDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ImportarSolicitudCodComercioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunExcelDTO> Handle(ImportarSolicitudCodComercioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
            var listaLocales = new List<LocalEmpresaDTO>();


            if (request.Archivo == null || request.Archivo.ContentLength == 0)
            {
                return new RespuestaComunExcelDTO
                {
                    Ok = false,
                    Mensaje = "No se encontró ningún archivo para procesar.",
                    Errores = new List<ErroresExcelDTO>()
                };
            }

            string ext = Path.GetExtension(request.Archivo.FileName).ToLower();
            if (ext != ".xlsx")
            {
                return new RespuestaComunExcelDTO
                {
                    Ok = false,
                    Mensaje = "Sólo se permiten archivos con extensión .xlsx.",
                    Errores = new List<ErroresExcelDTO>()
                };
            }

            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream))
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = false
                        },
                        UseColumnDataType = true
                    }).ToAllStringFields();

                    if (ds.Tables.Count == 0)
                        throw new InvalidOperationException("El archivo no contiene ninguna hoja de datos.");

                    var dt = ds.Tables[0];

                    string razonSocial = "";
                    for (int col = 2; col <= 6; col++)
                    {
                        var celda = dt.Rows[2][col]?.ToString();
                        if (!string.IsNullOrWhiteSpace(celda))
                        {
                            razonSocial += (razonSocial != "" ? " " : "") + celda.Trim();
                        }
                    }

                    string codEmpresa = razonSocial.Split(' ')[0];

                    for (int i = 6; i < dt.Rows.Count - 18; i++)
                    //for (int i = 6; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            var row = dt.Rows[i];
                            if (row == null || string.IsNullOrEmpty(row[1]?.ToString())) continue;

                            string nombreComercial = row[1].ToString();
                            var codLocalToken = nombreComercial.Split(' ')[0];
                            var codLocal = codLocalToken.TrimStart('0');
                            if (string.IsNullOrEmpty(codLocal))
                            {
                                codLocal = "0";
                            }

                            var codLocalAlterno = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == codEmpresa && x.CodLocal == codLocal).Select(x => x.CodLocalAlterno).FirstOrDefaultAsync(cancellationToken);

                            if (codLocalAlterno is null)
                            {
                                throw new InvalidOperationException($"{nombreComercial} no contiene código alterno.");
                            }

                            listaLocales.Add(new LocalEmpresaDTO
                            {
                                CodLocal = codLocal,
                                NomLocal = nombreComercial,
                                CodEmpresa = codEmpresa,
                                NomEmpresa = razonSocial,
                                CodLocalAlterno = codLocalAlterno ?? "0"
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

                    // Verificar duplicados por CodLocal
                    var duplicados = listaLocales
                        .GroupBy(l => l.CodLocal)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicados.Any())
                    {
                        respuesta.Ok = false;
                        respuesta.Mensaje = "Existen códigos de local duplicados.";

                        // Para cada código duplicado incluimos un error con una etiqueta <img>
                        foreach (var cod in duplicados)
                        {
                            respuesta.Errores.Add(new ErroresExcelDTO
                            {
                                Fila = 0, // sin fila asociada
                                Mensaje = $"Código duplicado: {cod} "
                            });
                        }
                        return respuesta;
                    }

                    // Crear Solicitud Cabecera con la lista de detalles asociada
                    var solicitudCab = new CCom_SolicitudCab
                    {
                        TipEstado = "S",
                        RznSocial = razonSocial,
                        FecSolicitud = DateTime.Now,
                        UsuSolicitud = request.Usuario,
                        FecCreacion = DateTime.Now,
                        Detalles = listaLocales.Select(local => new CCom_SolicitudDet
                        {
                            CodEmpresa = local.CodEmpresa,
                            CodLocal = local.CodLocal,
                            FecCreacion = DateTime.Now,
                            UsuCreacion = request.Usuario,
                            TipEstado = "P",
                        }).ToList()
                    };

                    _contexto.RepositorioCComSolicitudCab.Agregar(solicitudCab);
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
                _logger.Error(ex, "Error al importar solicitud cod comercio");
                return new RespuestaComunExcelDTO
                {
                    Ok = false,
                    Mensaje = "Ocurrió un error inesperado: " + ex.Message,
                    Errores = respuesta.Errores
                };
            }
        }

        public class LocalEmpresaDTO
        {
            public string CodLocalAlterno { get; set; }
            public string CodLocal { get; set; }
            public string NomLocal { get; set; }
            public string CodEmpresa { get; set; }
            public string NomEmpresa { get; set; }
        }
    }
}
