using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.DTOs;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class AprobarSolicitudCommand : IRequest<RespuestaComunDTO>
    {
        //public string CodEmpresa { get; set; }
        public List<int> NumSolicitudes { get; set; }

        public List<ASR_UsuarioListado> Solicitudes { get; set; }
        public string CodUsuarioAsr { get; set; }
        public string IndActivo { get; set; }
        public string FlgEnvio { get; set; }
        public DateTime? FecEnvio { get; set; }
        public string UsuAutoriza { get; set; }
        public string UsuCreacion { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
        public string TipoUsuario { get; set; }

    }

    public class AprobarSolicitudHandler : IRequestHandler<AprobarSolicitudCommand, RespuestaComunDTO>
    {
        public async Task<RespuestaComunDTO> Handle(AprobarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true, Mensaje = "| " };
            try
            {
                using (ISGPContexto contexto = new SGPContexto())
                {
                    foreach (var solicitud in request.Solicitudes)
                    {
                        await contexto.RepositorioSolicitudUsuarioASR.AprobarSolicitud(new ASR_Usuario
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
                        });

                        ASR_CajeroPaso cajero = new ASR_CajeroPaso();  
                        cajero.CajCodigo = solicitud.CodColaborador;
                        cajero.LocNumero = Convert.ToInt16(solicitud.CodLocal);
                        cajero.CajNombre = $"{solicitud.NoTrab} {solicitud.NoApelPate} {solicitud.NoApelMate}" ;
                        cajero.CajNom = solicitud.NoTrab;
                        cajero.CajApellidos = $"{solicitud.NoApelPate} {solicitud.NoApelMate}";
                        cajero.CajRut = solicitud.NumDocumentoIdentidad;
                        //cajero.CajTipo = solicitud.Apellidos;
                        //cajero.CajTipoContrato = solicitud.Apellidos;
                        cajero.CajTipoDocId = solicitud.TipDocumentoIdentidad;
                        cajero.CajCodigoEmp = solicitud.CodColaborador;
                        //cajero.CajEstado = solicitud.Estado;
                        //cajero.CajActivo = solicitud.CajActivo;
                        //cajero.CajLogin = solicitud.CajLogin;
                        //cajero.CajUsuarioCrea = solicitud.CajUsuarioCrea;
                        //cajero.CajUsuarioBaja = solicitud.CajUsuarioBaja;
                        //cajero.CajUsuarioActualiza = solicitud.CajUsuarioActualiza;
                        //cajero.CajCorrExtranjero = solicitud.CajCorrExtranjero;
                        //cajero.CajRendAuto = solicitud.CajRendAuto;
                        //cajero.CajCarga = solicitud.CajCarga;
                        cajero.CodPais = 10;
                        //cajero.CodComercio = solicitud.CodComercio;



                        await contexto.RepositorioSolicitudUsuarioASR.InsertarCajeroPaso(cajero);
                    }


                    //var aprobarSolicitudesTasks = request.NumSolicitudes.Select(numSolicitud => AprobarSolicitudAsync(contexto, numSolicitud, request)).ToList();
                    //await Task.WhenAll(aprobarSolicitudesTasks);
                    respuesta.Mensaje += string.Join(" | ", request.NumSolicitudes.Select(s => $"Solicitud Nro {s} aprobado |"));

                    ProcesoParametro parametro = await contexto.RepositorioProcesoParametro
                        .Obtener(x => x.CodParametro == "01" && x.CodProceso == 38)
                        .FirstOrDefaultAsync();

                    if (parametro?.ValParametro == null)
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "No se encontró la ruta para guardar los archivos" };
                    }

                    if (request.TipoUsuario == "A")
                    {
                        List<ASR_UsuarioArchivo> archivos = await contexto.RepositorioSolicitudUsuarioASR.ListarArchivos("A");
                        List<string> nombresArchivos = archivos.Select(x => x.NombreArchivo).Distinct().ToList();

                        foreach (var nombreArchivo in nombresArchivos)
                        {
                            string rutaArchivo = Path.Combine(parametro.ValParametro, nombreArchivo);
                            using (StreamWriter writer = new StreamWriter(rutaArchivo, false, Encoding.Default))
                            {
                                foreach (var linea in archivos.Where(x => x.NombreArchivo == nombreArchivo))
                                {
                                    writer.WriteLine(linea.Contenido);
                                    await contexto.RepositorioSolicitudUsuarioASR.ActualizarFlagEnvio(linea.NumSolicitud, "S");
                                }
                            }
                            respuesta.Mensaje += $"Archivo {nombreArchivo} generado | ";
                        }
                        await contexto.GuardarCambiosAsync();
                    }
                    else
                    {
                        List<int> locales = await contexto.RepositorioSolicitudUsuarioASR.ObtenerLocalesPorProcesar(30);

                        foreach (int numeroLocal in locales)
                        {
                            string nombreArchivo = $"US-{DateTime.Now:yyyyMMdd-HHmmss}-{numeroLocal}.local";

                            List<ASR_CajeroPaso> cajeros = await contexto.RepositorioSolicitudUsuarioASR.ObtenerCajerosPorProcesar(30, numeroLocal);

                            string rutaArchivo = Path.Combine(parametro.ValParametro, nombreArchivo);
                            using (StreamWriter writer = new StreamWriter(rutaArchivo, false, Encoding.Default))
                            {
                                foreach (var cajero in cajeros)
                                {
                                    // Generación del contenido
                                    string estado = cajero.CajEstado == "1" ? "3" : cajero.CajEstado;
                                    string tipoDoc = cajero.CajTipoDocId == "1" ? "DNI" : "CEX";
                                    string corrExtranjero = string.IsNullOrEmpty(cajero.CajCorrExtranjero) ? "000000000000" : cajero.CajCorrExtranjero;
                                    string codigoLocal = cajero.CajCodigo.Length > 8 ? cajero.CajCodigo.Substring(cajero.CajCodigo.Length - 8) : cajero.CajCodigo;

                                    // Armado del contenido
                                    string contenido = $"{estado},{codigoLocal},CAJERO,{cajero.CajNom ?? string.Empty},{cajero.CajApellidos ?? string.Empty},{estado},{tipoDoc},{corrExtranjero}";

                                    writer.WriteLine(contenido);
                                    //await contexto.RepositorioSolicitudUsuarioASR.ActualizarFlagProcesado(30, numeroLocal, cajero.CajCodigo, "S");
                                    //await contexto.RepositorioSolicitudUsuarioASR.InsertarCajeroPaso(cajero);
                                    //await contexto.RepositorioSolicitudUsuarioASR.ProcesarCajero(comercio, numeroLocal, cajero.CajCodigo, "S", cajero);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        private async Task AprobarSolicitudAsync(ISGPContexto contexto, int numSolicitud, AprobarSolicitudCommand request)
        {
            await contexto.RepositorioSolicitudUsuarioASR.AprobarSolicitud(new ASR_Usuario
            {
                NumSolicitud = numSolicitud,
                CodUsuarioAsr = request.CodUsuarioAsr,
                IndActivo = request.IndActivo,
                FlgEnvio = request.FlgEnvio,
                FecEnvio = request.FecEnvio,
                UsuAutoriza = request.UsuAutoriza,
                UsuCreacion = request.UsuCreacion,
                UsuElimina = request.UsuElimina,
                FecElimina = request.FecElimina
            });
        }

        private int ObtenerComercioPorCodigo(string codEmpresa)
        {
            switch (codEmpresa)
            {
                case "10": // HPSA  
                    return 10;
                case "09": // TPSA  
                    return 20;
                default: // SPSA  
                    return 30;
            }
        }
    }
}
