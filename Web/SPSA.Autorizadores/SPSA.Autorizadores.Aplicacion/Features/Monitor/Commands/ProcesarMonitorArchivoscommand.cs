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
		public string Ruta { get; set; }
		public string Usuario { get; set; }
		public string Clave { get; set; }
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
				_clave = request.Clave;
				response.Data = await ProcesarArchivos(request);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
				_logger.Error(ex, ex.Message);
			}

			return response;
		}

		private async Task<List<ProcesarMonitorArchivoDTO>> ProcesarArchivos(ProcesarMonitorArchivoscommand request)
		{
			List<Mae_Caja> cajas = new List<Mae_Caja>();
			List<Mae_Local> locales = new List<Mae_Local>();
			Mae_Empresa empresa = new Mae_Empresa();

			using (ISGPContexto contexto = new SGPContexto())
			{
				cajas = await contexto.RepositorioMaeCaja.Obtener(x => x.CodEmpresa == request.CodEmpresa
															&& (request.CodCadena == "0" || x.CodCadena == request.CodCadena)
															&& (request.CodRegion == "0" || x.CodRegion == request.CodRegion)
															&& (request.CodZona == "0" || x.CodZona == request.CodZona)
															&& (request.CodLocal == "0" || x.CodLocal == request.CodLocal)).ToListAsync();

				List<string> localesDictinct = cajas.Select(x => x.CodLocal).Distinct().ToList();
				empresa = await contexto.RepositorioMaeEmpresa.Obtener(x => x.CodEmpresa == request.CodEmpresa).FirstOrDefaultAsync();
				locales = await contexto.RepositorioMaeLocal.Obtener(x => localesDictinct.Contains(x.CodLocal)).ToListAsync();
			}
				

			var tasks = cajas.Select(caja => ProcesarCajaAsync(caja, request.Usuario, request.Clave, request.Ruta, locales, empresa.Ruc)).ToList();
			await Task.WhenAll(tasks);
			return tasks.Select(x => x.Result).ToList();
		}

		private async Task<ProcesarMonitorArchivoDTO> ProcesarCajaAsync(Mae_Caja caja, string usuarioCaja, string claveCaja,
			string rutaArchivo, List<Mae_Local> locales, string rucEmpresa)
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
						IEnumerable<SftpFile> archivos = client.ListDirectory($"{rutaArchivo}/{rucEmpresa}/{resultado.NumCaja}");
						resultado.CantidadArchivos = archivos.Where(x => !x.IsDirectory).Count();
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
