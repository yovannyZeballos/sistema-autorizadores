using AutoMapper;
using MediatR;
using Renci.SshNet;
using Renci.SshNet.Common;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarMonitorCommand : IRequest<RespuestaComunDTO>
	{
		public string CodEmpresa { get; set; }
	}

	public class ProcesarMonitorHandler : IRequestHandler<ProcesarMonitorCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioMonitorReporte _repositorioLocalMonitor;
		private readonly IRepositorioSovosLocal _repositorioSovosLocal;
		private readonly IRepositorioMonitorComando _repositorioMonitorComando;
		private readonly IMapper _mapper;

		private readonly string _usuario;
		private readonly string _clave;
		private readonly int _maximoTareasEncoladas;

		private string COMANDO_EXISTE_ARCHIVO;
		private string COMANDO_HORA_INICIO;
		private string COMANDO_HORA_FIN;
		private string NOMBRE_ARCHIVO;

		public ProcesarMonitorHandler(IRepositorioMonitorReporte repositorioLocalMonitor, IRepositorioSovosLocal repositorioSovosLocal,
			IRepositorioMonitorComando repositorioMonitorComando, IMapper mapper)
		{
			_usuario = ConfigurationManager.AppSettings["Usuario"];
			_clave = ConfigurationManager.AppSettings["Clave"];
			_maximoTareasEncoladas = Convert.ToInt32(ConfigurationManager.AppSettings["MaximoProcesosBloque"]);
			_repositorioLocalMonitor = repositorioLocalMonitor;
			_repositorioSovosLocal = repositorioSovosLocal;
			_repositorioMonitorComando = repositorioMonitorComando;
			_mapper = mapper;

		}

		public async Task<RespuestaComunDTO> Handle(ProcesarMonitorCommand request, CancellationToken cancellationToken)
		{
			var fechaProceso = DateTime.Now;
			var fechaCierre = fechaProceso.AddDays(-1).Date;
			var tareasCalculo = new List<Task<ProcesoMonitorDTO>>();
			var resultadosProceso = new List<ProcesoMonitorDTO>();
			var respuesta = new RespuestaComunDTO();

			try
			{
				var timer = new Stopwatch();
				timer.Start();

				using (ISGPContexto contexto = new SGPContexto())
				{
					List<ProcesoParametro> parametros = await contexto.RepositorioProcesoParametro
						.Obtener(x => x.CodProceso == Constantes.CodigoProcesoMonitorCierreEOD)
						.ToListAsync();

					COMANDO_EXISTE_ARCHIVO = parametros.FirstOrDefault(x => x.CodParametro == Constantes.CodigoComandoObtenerArchivo_ProcesoMonitorCierreEOD).ValParametro;
					COMANDO_HORA_INICIO = parametros.FirstOrDefault(x => x.CodParametro == Constantes.CodigoComandoHoraInicioArchivo_ProcesoMonitorCierreEOD).ValParametro;
					COMANDO_HORA_FIN = parametros.FirstOrDefault(x => x.CodParametro == Constantes.CodigoComandoHoraFinArchivo_ProcesoMonitorCierreEOD).ValParametro;
					NOMBRE_ARCHIVO = parametros.FirstOrDefault(x => x.CodParametro == Constantes.CodigoComandoNombreArchivo_ProcesoMonitorCierreEOD).ValParametro;

					var localesAProcesar = await contexto.RepositorioMonCierreEOD.Listar(request.CodEmpresa, fechaCierre, (int)TipoMonitor.CIERRE_FIN_DIA);

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
					var monCierres = resultadosProceso.Select(x => new MonCierreEOD
					{
						CodCadena = x.CodCadena,
						CodEmpresa = x.CodEmpresa,
						CodLocal = x.CodLocal,
						CodRegion = x.CodRegion,
						CodZona = x.CodZona,
						Estado = x.Estado,
						FechaCierre = fechaCierre,
						FechaProceso = fechaProceso,
						HoraInicio = x.HoraInicio,
						HoraFin = x.HoraFin,
						Observacion = x.Observacion != null && x.Observacion.Length > 250 ? x.Observacion.Substring(0, 250) : x.Observacion,
						Tipo = (int)TipoMonitor.CIERRE_FIN_DIA
					}).ToList();

					await InsertarMonCierreEOD(monCierres, contexto);

					timer.Stop();
					TimeSpan timeTaken = timer.Elapsed;

					respuesta.Ok = true;
					respuesta.Mensaje = $"Proceso exitoso, el proceso se ejecuto en {timeTaken:hh\\:mm\\:ss}";
				}
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return respuesta;
		}

		private ProcesoMonitorDTO Procesar(Mae_Local local)
		{
			var procesoMonitorDTO = new ProcesoMonitorDTO
			{
				CodLocal = local.CodLocal,
				CodEmpresa = local.CodEmpresa,
				CodCadena = local.CodCadena,
				CodRegion = local.CodRegion,
				CodZona = local.CodZona
			};

			string[] formats = new[] { "yyyy-MM-dd", "yyyy-MM-d", "yyyy-M-dd", "yyyy-M-d" };

			try
			{
				KeyboardInteractiveAuthenticationMethod keybAuth = new KeyboardInteractiveAuthenticationMethod(_usuario);
				keybAuth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);

				var connectionInfo = new ConnectionInfo(local.Ip.Trim(), _usuario, keybAuth);

				using (var client = new SshClient(connectionInfo))
				{
					client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
					client.Connect();

					var comandoExisteArchivoResult = client.RunCommand(COMANDO_EXISTE_ARCHIVO);
					var nombreArchivo = comandoExisteArchivoResult.Result.Replace("\n", "");

					if (nombreArchivo != NOMBRE_ARCHIVO)
					{
						procesoMonitorDTO.Estado = ((int)EstadoMonitor.NO_SE_HA_REALIZADO_CIERRE).ToString();
						procesoMonitorDTO.HoraFin = "--:--:--";
						procesoMonitorDTO.HoraInicio = "--:--:--";
						procesoMonitorDTO.Observacion = Constantes.MensajeArchivoNoEncontrado;
						return procesoMonitorDTO;
					}

					var comandoHoraInicioResult = client.RunCommand(COMANDO_HORA_INICIO);
					procesoMonitorDTO.HoraInicio = comandoHoraInicioResult.Result.Replace("\n", "").Trim();

					if (string.IsNullOrWhiteSpace(procesoMonitorDTO.HoraInicio))
					{
						procesoMonitorDTO.Estado = ((int)EstadoMonitor.NO_SE_HA_REALIZADO_CIERRE).ToString();
						procesoMonitorDTO.HoraFin = "--:--:--";
						procesoMonitorDTO.HoraInicio = "--:--:--";
						procesoMonitorDTO.Observacion = Constantes.MensajeFechaInicioNoEncontrado;
						return procesoMonitorDTO;
					}

					var comandoHoraFinResult = client.RunCommand(COMANDO_HORA_FIN);
					procesoMonitorDTO.HoraFin = comandoHoraFinResult.Result.Replace("\n", "").Trim();

					if (string.IsNullOrWhiteSpace(procesoMonitorDTO.HoraFin))
					{
						procesoMonitorDTO.Estado = ((int)EstadoMonitor.NO_SE_HA_REALIZADO_CIERRE).ToString();
						procesoMonitorDTO.HoraFin = "--:--:--";
						procesoMonitorDTO.Observacion = Constantes.MensajeFechaFinNoEncontrado;
						return procesoMonitorDTO;
					}

					procesoMonitorDTO.Estado = ((int)EstadoMonitor.CIERRE_REALIZADO).ToString();

					client.Disconnect();
					client.Dispose();
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

		private async Task InsertarMonCierreEOD(List<MonCierreEOD> monCierres, ISGPContexto contexto)
		{
			foreach (var monCierreEOD in monCierres)
			{
				var fechaCierre = monCierreEOD.FechaCierre.Value.Date;
				var existe = await contexto.RepositorioMonCierreEOD.Existe(x => x.CodEmpresa == monCierreEOD.CodEmpresa &&
																				x.CodCadena == monCierreEOD.CodCadena &&
																				x.CodRegion == monCierreEOD.CodRegion &&
																				x.CodZona == monCierreEOD.CodZona &&
																				x.CodLocal == monCierreEOD.CodLocal &&
																				x.FechaCierre == fechaCierre &&
																				x.Tipo == monCierreEOD.Tipo);
				if (existe) 
				{
					MonCierreEODHist hist = _mapper.Map<MonCierreEODHist>(monCierreEOD);
					contexto.RepositorioMonCierreEODHist.Agregar(hist);
				}

				contexto.RepositorioMonCierreEOD.AgregarActualizar(monCierreEOD);
			}
			await contexto.GuardarCambiosAsync();
		}
	}
}

