using MediatR;
using Renci.SshNet;
using Renci.SshNet.Common;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
    public class ProcesarMonitorCommand : IRequest<RespuestaComunDTO>
    {
        //public List<ProcesoRequestDTO> ProcesoRequest { get; set; }
        public string CodEmpresa { get; set; }
        public DateTime FechaCierre { get; set; }
    }

    public class ProcesarMonitorHandler : IRequestHandler<ProcesarMonitorCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioMonitorReporte _repositorioLocalMonitor;
        private readonly IRepositorioSovosLocal _repositorioSovosLocal;

        private readonly string _usuario;
        private readonly string _clave;
        private readonly int _maximoTareasEncoladas;

        private const string COMANDO_EXISTE_ARCHIVO = "find /home/NCR/webfront-endofday/safe/ -type f -mtime -1 | cut -d/ -f 6 | cut -d- -f1";
        private const string COMANDO_HORA_INICIO = "find /home/NCR/webfront-endofday/safe/ -type f -mtime -1 | xargs cat | grep BEGIN | awk '{print $2}' | cut -d, -f1";
        private const string COMANDO_HORA_FIN = "find /home/NCR/webfront-endofday/safe/ -type f -mtime -1 | xargs cat | grep END | awk '{print $2}' | cut -d, -f1";
        private const string NOMBRE_ARCHIVO = "WF_EODSteps";

        public ProcesarMonitorHandler(IRepositorioMonitorReporte repositorioLocalMonitor, IRepositorioSovosLocal repositorioSovosLocal)
        {
            _usuario = ConfigurationManager.AppSettings["Usuario"];
            _clave = ConfigurationManager.AppSettings["Clave"];
            _maximoTareasEncoladas = Convert.ToInt32(ConfigurationManager.AppSettings["MaximoProcesosBloque"]);
            _repositorioLocalMonitor = repositorioLocalMonitor;
            _repositorioSovosLocal = repositorioSovosLocal;
        }

        public async Task<RespuestaComunDTO> Handle(ProcesarMonitorCommand request, CancellationToken cancellationToken)
        {
            var fechaProceso = DateTime.Now;
            var tareasCalculo = new List<Task<ProcesoMonitorDTO>>();
            var resultadosProceso = new List<ProcesoMonitorDTO>();
            var respuesta = new RespuestaComunDTO();

            try
            {
                var timer = new Stopwatch();
                timer.Start();

                var localesAProcesar = await _repositorioSovosLocal.Listar(request.CodEmpresa, request.FechaCierre);

                int cantidadTareas = 0;
                foreach (var local in localesAProcesar)
                {
                    tareasCalculo.Add(Task.Run(() => Procesar(local)));

                    cantidadTareas++;
                    if (cantidadTareas == _maximoTareasEncoladas)
                    {
                        await Task.WhenAll(tareasCalculo);
                        resultadosProceso.AddRange(tareasCalculo.Select(t => t.Result));
                        tareasCalculo.Clear();
                        cantidadTareas = 0;
                    }
                }

                if (cantidadTareas > 0)
                {
                    await Task.WhenAll(tareasCalculo);
                    resultadosProceso.AddRange(tareasCalculo.Select(t => t.Result));
                }

                //Insertar BD
                foreach (var resultado in resultadosProceso)
                {
                    var localMonitor = new MonitorReporte(request.CodEmpresa, resultado.CodLocal, fechaProceso,
                       request.FechaCierre, resultado.HoraInicio, resultado.HoraFin, resultado.Estado, resultado.Observacion, resultado.CodFormato);

                    await _repositorioLocalMonitor.Crear(localMonitor);
                }

                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;

                respuesta.Ok = true;
                respuesta.Mensaje = $"Proceso exitoso, el proceso se ejecuto en {timeTaken.ToString(@"hh\:mm\:ss")}";
            }
            catch (Exception ex )
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        private ProcesoMonitorDTO Procesar(SovosLocal local)
        {
            var procesoMonitorDTO = new ProcesoMonitorDTO
            {
                CodLocal = local.CodLocal,
                CodFormato = local.CodFormato,
            };

            try
            {
                KeyboardInteractiveAuthenticationMethod keybAuth = new KeyboardInteractiveAuthenticationMethod(_usuario);
                keybAuth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);

                var connectionInfo = new ConnectionInfo(local.Ip, _usuario, keybAuth);

                using (var client = new SshClient(connectionInfo))
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
                    client.Connect();

                    var comandoExisteArchivoResult = client.RunCommand(COMANDO_EXISTE_ARCHIVO);
                    var nombreArchivo = comandoExisteArchivoResult.Result.Replace("\n", "");

                    if(nombreArchivo != NOMBRE_ARCHIVO)
                    {
                        procesoMonitorDTO.Estado = ((int)EstadoMonitor.NO_SE_HA_REALIZADO_CIERRE).ToString();
                        procesoMonitorDTO.HoraFin = "--:--:--";
                        procesoMonitorDTO.HoraInicio = "--:--:--";
                        return procesoMonitorDTO;
                    }

                    var comandoHoraInicioResult = client.RunCommand(COMANDO_HORA_INICIO);
                    procesoMonitorDTO.HoraInicio = comandoHoraInicioResult.Result.Replace("\n", "");
                    var comandoHoraFinResult = client.RunCommand(COMANDO_HORA_FIN);
                    procesoMonitorDTO.HoraFin = comandoHoraFinResult.Result.Replace("\n", "");
                    procesoMonitorDTO.Estado = ((int)EstadoMonitor.CIERRE_REALIZADO).ToString();
                }

            }
            catch (Exception ex)
            {
                procesoMonitorDTO.Estado = ((int)EstadoMonitor.PENDIENTE_VALIDACION_CIERRE).ToString();
                procesoMonitorDTO.Observacion = "SIN CONEXIÓN AL SERVER | " + ex.Message;
                procesoMonitorDTO.HoraFin = "--:--:--";
                procesoMonitorDTO.HoraInicio = "--:--:--";
            }

            return procesoMonitorDTO;
        }


        private void HandleKeyEvent(object sender, AuthenticationPromptEventArgs e)
        {
            foreach (AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = _clave;
                }
            }
        }
    }
}
