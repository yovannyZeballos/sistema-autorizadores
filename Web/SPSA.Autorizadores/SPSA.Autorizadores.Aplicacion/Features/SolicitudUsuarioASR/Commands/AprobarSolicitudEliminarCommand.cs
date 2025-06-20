using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class AprobarSolicitudEliminarCommand : IRequest<RespuestaComunDTO>
    {
        public List<ASR_UsuarioListado> Solicitudes { get; set; } = new List<ASR_UsuarioListado>();
        public string CodUsuarioAsr { get; set; }
        public string IndActivo { get; set; }
        public string FlgEnvio { get; set; }
        public DateTime? FecEnvio { get; set; }
        public string UsuAutoriza { get; set; }
        public string UsuCreacion { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
        //public string TipoUsuario { get; set; }
    }

    public class AprobarSolicitudEliminarHandler : IRequestHandler<AprobarSolicitudEliminarCommand, RespuestaComunDTO> 
    {
        public async Task<RespuestaComunDTO> Handle(AprobarSolicitudEliminarCommand request, CancellationToken cancellationToken) 
        {
            var respuesta = new RespuestaComunDTO { Ok = true, Mensaje = string.Empty };

            int cantCajeros = 0;
            int cantAutorizadores = 0;

            try
            {
                using (ISGPContexto contexto = new SGPContexto()) 
                {
                    // 1. Aprobar solicitudes e insertar en CajeroPaso
                    foreach (var solicitud in request.Solicitudes)
                    {
                        // Aprobar solicitud de eliminacion
                        var usuario = new ASR_Usuario
                        {
                            NumSolicitud = solicitud.NumSolicitud,
                            CodUsuarioAsr = request.CodUsuarioAsr,
                            IndActivo = request.IndActivo,
                            FlgEnvio = request.FlgEnvio,
                            FecEnvio = request.FecEnvio,
                            UsuAutoriza = request.UsuAutoriza,
                            UsuCreacion = request.UsuCreacion,
                            UsuElimina = request.UsuElimina,
                            FecElimina = request.FecElimina
                        };
                        await contexto.RepositorioSolicitudUsuarioASR.AprobarSolicitud(usuario);

                        if (solicitud.TipUsuario == "C")
                        {
                            var cajeroPaso = new ASR_CajeroPaso
                            {
                                LocNumero = Convert.ToInt16(solicitud.CodLocal),
                                CajNombre = solicitud.NoTrab,
                                CajApellidos = $"{solicitud.NoApelPate} {solicitud.NoApelMate}",
                                CajRut = solicitud.NumDocumentoIdentidad,
                                CajTipo = "01",
                                CajTipoContrato = "1",
                                CajTipoDocId = solicitud.TipDocumentoIdentidad == "DNI" ? "1" : "2",
                                CajCodigoEmp = solicitud.CodColaborador,
                                CajUsuarioCrea = request.UsuCreacion,
                                CajFcreacion = DateTime.Now,
                                CodPais = solicitud.CodPais,
                                CodComercio = solicitud.CodComercio
                            };
                            //var (resultado, mensaje) = await contexto.RepositorioSolicitudUsuarioASR.EliminarCajeroaAsyncOracleSpsa(cajeroPaso);
                            var (resultado, mensaje) = await contexto.RepositorioSolicitudUsuarioASR.EliminarCajeroSpsaAsync(cajeroPaso);

                            if (resultado != 0)
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = mensaje;
                                return respuesta;
                            }

                            respuesta.Mensaje += $"Cajero {cajeroPaso.CajRut} eliminado.\n";
                            cantCajeros++;
                        }
                        else
                        {
                            cantAutorizadores++;
                        }
                    }

                    // 2. Obtener ruta base de archivos
                    ProcesoParametro parametro = await contexto.RepositorioProcesoParametro
                        .Obtener(x => x.CodParametro == "01" && x.CodProceso == 38)
                        .FirstOrDefaultAsync();

                    if (parametro?.ValParametro == null)
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "No se encontró la ruta para guardar los archivos" };
                    }

                    // 3. Generar archivos según tipo de usuario
                    if (cantAutorizadores > 0)
                    {
                        await GenerarArchivoAutorizadorAsync(contexto, parametro.ValParametro, respuesta);
                    }

                    if (cantCajeros > 0)
                    {
                        await GenerarArchivoCajeroSpsaAsync(contexto, parametro.ValParametro, respuesta);
                    }

                    // 4. Guardar cambios finales (flags de envío/procesado)
                    await contexto.GuardarCambiosAsync();

                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        private async Task GenerarArchivoAutorizadorAsync(ISGPContexto contexto, string rutaBase, RespuestaComunDTO respuesta)
        {
            var archivos = await contexto.RepositorioSolicitudUsuarioASR.ListarArchivos("A");
            var nombresArchivos = archivos.Select(x => x.NombreArchivo).Distinct().ToList();

            foreach (var nombreArchivo in nombresArchivos)
            {
                var ruta = Path.Combine(rutaBase, nombreArchivo);
                using (var writer = new StreamWriter(ruta, false, Encoding.Default))
                {
                    foreach (var linea in archivos.Where(x => x.NombreArchivo == nombreArchivo))
                    {
                        writer.WriteLine(linea.Contenido);
                        await contexto.RepositorioSolicitudUsuarioASR.ActualizarFlagEnvio(linea.NumSolicitud, "S");
                    }
                }
                respuesta.Mensaje += $"Archivo autorizador {nombreArchivo} generado | ";
            }
        }

        private async Task GenerarArchivoCajeroSpsaAsync(ISGPContexto contexto, string rutaBase, RespuestaComunDTO respuesta)
        {
            // 1. Traes todos los locales pendientes
            var locales = await contexto.RepositorioSolicitudUsuarioASR.ObtenerLocalesPorProcesarAsync(10, 30);

            foreach (int numeroLocal in locales)
            {
                var nombreArchivo = $"US-{DateTime.Now:yyyyMMdd-HHmmss}-{numeroLocal}.local";
                var ruta = Path.Combine(rutaBase, nombreArchivo);

                // 2. Obtienes los cajeros sin procesar
                var cajeros = await contexto.RepositorioSolicitudUsuarioASR.ObtenerCajerosPorProcesarAsync(10, 30, numeroLocal);

                // 3. Generas el archivo
                using (var writer = new StreamWriter(ruta, false, Encoding.Default))
                {
                    foreach (var cajero in cajeros)
                    {
                        // Formato de contenido
                        var estado = cajero.CajEstado == "1" ? "3" : cajero.CajEstado;
                        var tipoDoc = cajero.CajTipoDocId == "1" ? "DNI" : "CEX";
                        var corrExt = string.IsNullOrEmpty(cajero.CajCorrExtranjero) ? "000000000000" : cajero.CajCorrExtranjero;
                        var codigoLocal = cajero.CajCodigo.Length > 8
                            ? cajero.CajCodigo.Substring(cajero.CajCodigo.Length - 8)
                            : cajero.CajCodigo;

                        var contenido = $"{estado},{codigoLocal},CAJERO,{cajero.CajNom},{cajero.CajApellidos},{estado},{tipoDoc},{corrExt}";
                        writer.WriteLine(contenido);
                        await contexto.RepositorioSolicitudUsuarioASR.ActualizarFlagProcesadoAsync(cajero.CodPais, cajero.CodComercio, cajero.LocNumero, cajero.CajCodigo, "S");
                    }
                }
                respuesta.Mensaje += $"Archivo cajero {nombreArchivo} generado | ";
            }
        }

        private async Task GenerarArchivoCajeroAsyncOracleSpsa(ISGPContexto contexto, string rutaBase, RespuestaComunDTO respuesta, int numeroLocal)
        {
            // 1. Traes todos los locales pendientes
            //var locales = await contexto.RepositorioSolicitudUsuarioASR.ObtenerLocalesPorProcesarAsyncOracleSpsa();

            //foreach (int numeroLocal in locales)
            //{
            var nombreArchivo = $"US-{DateTime.Now:yyyyMMdd-HHmmss}-{numeroLocal}.local";
            var ruta = Path.Combine(rutaBase, nombreArchivo);

            // 2. Obtienes los cajeros sin procesar
            var cajeros = await contexto.RepositorioSolicitudUsuarioASR.ObtenerCajerosPorProcesarAsyncOracleSpsa(numeroLocal);

            // 3. Generas el archivo
            using (var writer = new StreamWriter(ruta, false, Encoding.Default))
            {
                foreach (var cajero in cajeros)
                {
                    // Formato de contenido
                    var estado = cajero.CajEstado == "1" ? "3" : cajero.CajEstado;
                    var tipoDoc = cajero.CajTipoDocId == "1" ? "DNI" : "CEX";
                    var corrExt = string.IsNullOrEmpty(cajero.CajCorrExtranjero) ? "000000000000" : cajero.CajCorrExtranjero;
                    var codigoLocal = cajero.CajCodigo.Length > 8
                        ? cajero.CajCodigo.Substring(cajero.CajCodigo.Length - 8)
                        : cajero.CajCodigo;

                    var contenido = $"{estado},{codigoLocal},CAJERO,{cajero.CajNom},{cajero.CajApellidos},{estado},{tipoDoc},{corrExt}";
                    writer.WriteLine(contenido);
                    await contexto.RepositorioSolicitudUsuarioASR.ActualizarFlagProcesadoAsyncOracleSpsa(cajero.LocNumero, cajero.CajCodigo, "S");
                }
            }
            respuesta.Mensaje += $"Archivo cajero {nombreArchivo} generado\n";
            // }
        }
    }
}
