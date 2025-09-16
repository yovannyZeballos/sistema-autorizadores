using MediatR;
using Renci.SshNet;
using Renci.SshNet.Common;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
	public class GenerarArchivoPorLocalCommand : IRequest<DescargarPlantillasDTO>
	{
		public List<GenerarArchivoPorLocalDTO> Locales { get; set; }
	}

	//public class GenerarArchivoPorLocalHandler : IRequestHandler<GenerarArchivoPorLocalCommand, DescargarPlantillasDTO>
	//{
	//	private readonly IRepositorioAutorizadores _repositorioAutorizadores;
	//	private readonly string _usuario;
	//	private readonly string _clave;
	//	private readonly string _ipServidor;
	//	//private readonly string _ruta;

	//	public GenerarArchivoPorLocalHandler(IRepositorioAutorizadores repositorioAutorizadores)
	//	{
	//		_repositorioAutorizadores = repositorioAutorizadores;
	//		_usuario = ConfigurationManager.AppSettings["UsuarioSFTPArchivo"];
	//		_clave = ConfigurationManager.AppSettings["ClaveSFTPArchivo"];
	//		_ipServidor = ConfigurationManager.AppSettings["IPServidorSFTPArchivo"];
	//		//_ruta = ConfigurationManager.AppSettings["RutaSFTPArchivo"];
	//	}

	//	public async Task<DescargarPlantillasDTO> Handle(GenerarArchivoPorLocalCommand request, CancellationToken cancellationToken)
	//	{
	//		var respuestaComun = new DescargarPlantillasDTO();
	//		try
	//		{
	//			var codLocales = string.Join(",", request.Locales.Select(x => x.CodLocal));
	//			await _repositorioAutorizadores.ActualizarEstadoArchivoPorLocal(codLocales);

	//			var respuesta = new StringBuilder();
	//			var archivosDescargados = new List<(string, byte[])>();
	//			//var archivos = new List<(string, byte[])>();
	//			var archivos = new List<string>();

	//			foreach (var local in request.Locales)
	//			{
	//				var resp = await _repositorioAutorizadores.GenerarArchivoLocal(local.CodLocal, local.TipoSo);
	//				foreach (var item in resp.Split('\n'))
	//				{
	//					if (item != "null" && item != "")
	//					{
	//						var nombreArchivo = item.Split('|').Count() == 0 ? "" : $"{item.Split('|')[0]}/{item.Split('|')[1]}";
	//						respuesta.AppendLine(nombreArchivo);
	//						archivos.Add(nombreArchivo);
	//					}
	//				}
	//			}

	//			archivosDescargados = DescargarArchivo(archivos);
	//			var zip = ComprimirArchivos(archivosDescargados);

	//			respuestaComun.Mensaje = respuesta.ToString();
	//			respuestaComun.Ok = true;
	//			respuestaComun.Archivo = zip;
	//			respuestaComun.NombreArchivo = $"Archivos_{DateTime.Now:ddMMyyyyHHmmss}.zip";
	//		}
	//		catch (Exception ex)
	//		{
	//			respuestaComun.Mensaje = $"Error al actualizar estado archivo {ex.Message}";
	//			respuestaComun.Ok = false;
	//		}

	//		return respuestaComun;
	//	}

	//	private List<(string, byte[])> DescargarArchivo(List<string> archivos)
	//	{
	//		var archivosDescargados = new List<(string, byte[])>();


	//		KeyboardInteractiveAuthenticationMethod keybAuth = new KeyboardInteractiveAuthenticationMethod(_usuario);

	//		keybAuth.AuthenticationPrompt += (sender, e) =>
	//		{
	//			e.Prompts.Single().Response = _clave;
	//		};

	//		keybAuth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);


	//		List<AuthenticationMethod> authMethods = new List<AuthenticationMethod>
	//		{
	//			new PasswordAuthenticationMethod(_usuario, _clave),
	//			keybAuth
	//		};

	//		var connectionInfo = new ConnectionInfo(_ipServidor, 22, _usuario, authMethods.ToArray());
	//		using (var client = new SftpClient(connectionInfo))
	//		{
	//			client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
	//			client.Connect();


	//			foreach (var item in archivos)
	//			{
	//				using (MemoryStream stream = new MemoryStream())
	//				{
	//					try
	//					{
	//						client.DownloadFile(item, stream);
	//						archivosDescargados.Add((item, stream.ToArray()));
	//					}
	//					catch (Exception)
	//					{
	//					}
						
	//				}
	//			}

	//			client.Disconnect();
	//			client.Dispose();

	//		}
	//		return archivosDescargados;
	//	}

	//	private string ComprimirArchivos(List<(string, byte[])> archivos)
	//	{
	//		using (MemoryStream zipToOpen = new MemoryStream())
	//		{
	//			using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create, false))
	//			{
	//				foreach (var item in archivos)
	//				{
	//					var lista = item.Item1.Split('/');
	//					var zipEntry = archive.CreateEntry(lista[lista.Length - 1]);

	//					using (var originalFileStream = new MemoryStream(item.Item2))
	//					using (var zipEntryStream = zipEntry.Open())
	//					{
	//						originalFileStream.CopyTo(zipEntryStream);

	//					}
	//				}
					
	//			}
	//			var zip = zipToOpen.ToArray();
	//			return Convert.ToBase64String(zip);
	//		}
			
	//	}

	//	private void HandleKeyEvent(object sender, AuthenticationPromptEventArgs e)
	//	{
	//		foreach (AuthenticationPrompt prompt in e.Prompts)
	//		{
	//			if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
	//			{
	//				prompt.Response = _clave;
	//			}
	//		}
	//	}
	//}
}