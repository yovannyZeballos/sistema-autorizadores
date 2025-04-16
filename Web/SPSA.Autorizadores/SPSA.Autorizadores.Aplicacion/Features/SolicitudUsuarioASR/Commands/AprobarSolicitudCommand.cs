using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
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
		public List<int> NumSolicitudes { get; set; }
		public string CodUsuarioAsr { get; set; }
		public string IndActivo { get; set; }
		public string FlgEnvio { get; set; }
		public DateTime? FecEnvio { get; set; }
		public string UsuAutoriza { get; set; }
		public string UsuCreacion { get; set; }
		public string UsuElimina { get; set; }
		public DateTime? FecElimina { get; set; }

	}

	public class AprobarSolicitudHandler : IRequestHandler<AprobarSolicitudCommand, RespuestaComunDTO>
	{
		public async Task<RespuestaComunDTO> Handle(AprobarSolicitudCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true, Mensaje = "Solicitud aprobada" };
			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					foreach (var numSolicitud in request.NumSolicitudes)
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

					List<ASR_UsuarioArchivo> archivos = await contexto.RepositorioSolicitudUsuarioASR.ListarArchivos();
					List<string> nombresArchivos = archivos.Select(x => x.NombreArchivo).Distinct().ToList();
					ProcesoParametro parametro = await contexto.RepositorioProcesoParametro
						.Obtener(x => x.CodParametro == "01" && x.CodProceso == 38 )
						.FirstOrDefaultAsync();

					if (parametro?.ValParametro == null)
					{
						return new RespuestaComunDTO { Ok = false, Mensaje = "No se encontró la ruta para guardar los archivos" };
					}


					foreach (var nombreArchivo in nombresArchivos)
					{
						string rutaArchivo = Path.Combine(parametro.ValParametro, nombreArchivo);
						using (StreamWriter writer = new StreamWriter(rutaArchivo, false, Encoding.Default))
						{
							foreach (var linea in archivos.Where(x => x.NombreArchivo == nombreArchivo))
							{
								writer.WriteLine(linea.Contenido);
								await contexto.RepositorioSolicitudUsuarioASR.ActualizarFlagEnvio(linea.NumSolicitud,"S");
							}
						}
					}
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
	}
}
