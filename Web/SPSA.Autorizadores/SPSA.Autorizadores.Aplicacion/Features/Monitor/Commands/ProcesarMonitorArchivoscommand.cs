using MediatR;
using Renci.SshNet.Common;
using Renci.SshNet;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet.Sftp;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor
{
	public class ProcesarMonitorArchivoscommand : IRequest<GenericResponseDTO<List<ProcesarMonitorArchivoDTO>>>
	{
		public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
		public string CodRegion { get; set; }
		public string CodZona { get; set; }
		public string CodLocal { get; set; }
	}

	public class ProcesarMonitorArchivosHandler : IRequestHandler<ProcesarMonitorArchivoscommand, GenericResponseDTO<List<ProcesarMonitorArchivoDTO>>>
	{
		private readonly ILogger _logger;
		private string _clave;

		public ProcesarMonitorArchivosHandler()
		{
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<List<ProcesarMonitorArchivoDTO>>> Handle(ProcesarMonitorArchivoscommand request, CancellationToken cancellationToken)
		{
			GenericResponseDTO<List<ProcesarMonitorArchivoDTO>> response = new GenericResponseDTO<List<ProcesarMonitorArchivoDTO>> { Ok = true };

			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					var parametros = await contexto.RepositorioProcesoParametro
						.Obtener(x => x.CodProceso == Constantes.CodigoProcesoMonitorArchivos)
						.ToDictionaryAsync(x => x.CodParametro, x => x.ValParametro);

					parametros.TryGetValue(Constantes.CodigoUsuarioCaja_ProcesoMonitorArchivo, out var usuarioCaja);
					parametros.TryGetValue(Constantes.CodigoClaveCaja_ProcesoMonitorArchivo, out var claveCaja);
					parametros.TryGetValue(Constantes.CodigoRutaArchivos1_ProcesoMonitorArchivo, out var rutaArchivos1);
					parametros.TryGetValue(Constantes.CodigoRutaArchivos2_ProcesoMonitorArchivo, out var rutaArchivos2);

					if (string.IsNullOrEmpty(usuarioCaja) || string.IsNullOrEmpty(claveCaja) || string.IsNullOrEmpty(rutaArchivos1) || string.IsNullOrEmpty(rutaArchivos2))
					{
						response.Ok = false;
						response.Mensaje = "No se encontraron los parametros de configuración para el proceso de monitor de archivos.";
						_logger.Error(response.Mensaje);
						return response;
					}

					_clave = claveCaja;

					response.Data = await ProcesarArchivos(contexto, request, usuarioCaja, claveCaja, rutaArchivos1, rutaArchivos2);
				}
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
				_logger.Error(ex, ex.Message);
			}

			return response;
		}

		private async Task<List<ProcesarMonitorArchivoDTO>> ProcesarArchivos(ISGPContexto contexto, ProcesarMonitorArchivoscommand request, string usuarioCaja,
			string claveCaja, string rutaArchivos1, string rutaArchivos2)
		{
			List<Mae_Caja> cajas = await contexto.RepositorioMaeCaja.Obtener(x => x.CodEmpresa == request.CodEmpresa
															&& (request.CodCadena == "0" || x.CodCadena == request.CodCadena)
															&& (request.CodRegion == "0" || x.CodRegion == request.CodRegion)
															&& (request.CodZona == "0" || x.CodZona == request.CodZona)
															&& (request.CodLocal == "0" || x.CodLocal == request.CodLocal)).ToListAsync();

			List<string> localesDictinct = cajas.Select(x => x.CodLocal).Distinct().ToList();
			Mae_Empresa empresa = await contexto.RepositorioMaeEmpresa.Obtener(x => x.CodEmpresa == request.CodEmpresa).FirstOrDefaultAsync();
			var locales = await contexto.RepositorioMaeLocal.Obtener(x => localesDictinct.Contains(x.CodLocal)).ToListAsync();

			var tasks = cajas.Select(caja => ProcesarCajaAsync(caja, usuarioCaja, claveCaja, rutaArchivos1, rutaArchivos2, locales, empresa.Ruc)).ToList();
			await Task.WhenAll(tasks);
			return tasks.Select(x => x.Result).ToList();
		}

		private async Task<ProcesarMonitorArchivoDTO> ProcesarCajaAsync(Mae_Caja caja, string usuarioCaja, string claveCaja,
			string rutaArchivos1, string rutaArchivos2, List<Mae_Local> locales, string rucEmpresa)
		{

			return await Task.Run(() =>
			{
				ProcesarMonitorArchivoDTO resultado = new ProcesarMonitorArchivoDTO
				{
					CantidadArchivos = 0,
					CodLocal = caja.CodLocal,
					DesLocal = locales.FirstOrDefault(x => x.CodLocal == caja.CodLocal)?.NomLocal,
					IpCaja = caja.IpAddress,
					NumCaja = caja.NumCaja,
					Observacion = string.Empty
				};

				try
				{
					KeyboardInteractiveAuthenticationMethod keybAuth = new KeyboardInteractiveAuthenticationMethod(usuarioCaja);
					keybAuth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
					var connectionInfo = new ConnectionInfo(caja.IpAddress, usuarioCaja, keybAuth);

					using (var client = new SftpClient(connectionInfo))
					{
						client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
						client.Connect();
						IEnumerable<SftpFile> archivos = client.ListDirectory($"{rutaArchivos1}{rutaArchivos2}/{rucEmpresa}/{resultado.NumCaja}");
						resultado.CantidadArchivos = archivos.Count();
						client.Disconnect();
					}
				}
				catch (Exception ex)
				{
					resultado.Observacion = ex.Message;
					_logger.Error(ex, ex.Message);
				}
				return resultado;
			});
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
