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

            try
            {
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

                    if (ds.Tables.Count > 0)
                    {
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

                        for (int i = 6; i < dt.Rows.Count-18; i++)
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

                                if (!string.IsNullOrEmpty(codLocal) &&
                                    !listaLocales.Any(l => l.CodLocal == codLocal))
                                {
                                    listaLocales.Add(new LocalEmpresaDTO
                                    {
                                        CodLocal = codLocal,
                                        NomLocal = nombreComercial,
                                        CodEmpresa = codEmpresa,
                                        NomEmpresa = razonSocial,
                                        CodLocalAlterno = await _contexto.RepositorioMaeLocal
                                            .Obtener(x => x.CodEmpresa == codEmpresa && x.CodLocal == codLocal)
                                            .Select(x => x.CodLocalAlterno)
                                            .FirstOrDefaultAsync()
                                    });
                                }
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

                        // Crear Solicitud Cabecera con la lista de detalles asociada
                        var solicitudCab = new CCom_SolicitudCab
                        {
                            TipEstado = "S",
                            FecSolicitud = DateTime.Now,
                            UsuSolicitud = request.Usuario,
                            FecCreacion = DateTime.Now,
                            Detalles = listaLocales.Select(local => new CCom_SolicitudDet
                            {
                                CodLocalAlterno = int.Parse(local.CodLocalAlterno),
                                FecCreacion = DateTime.Now,
                                UsuCreacion = request.Usuario,
                                TipEstado = "P",
                            }).ToList()
                        };

                        _contexto.RepositorioCComSolicitudCab.Agregar(solicitudCab);
                        await _contexto.GuardarCambiosAsync();

                        respuesta.Ok = respuesta.Errores.Count == 0;
                        respuesta.Mensaje = respuesta.Ok ? "Importación completada exitosamente." : "Se importaron filas, pero hubo errores en algunas filas.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, "Error al importar solicitud cod comercio");
            }

            return respuesta;
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
