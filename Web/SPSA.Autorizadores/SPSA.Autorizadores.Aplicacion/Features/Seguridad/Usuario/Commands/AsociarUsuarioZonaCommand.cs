using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para asociar un usuario a una zona.
	/// </summary>
	public class AsociarUsuarioZonaCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Código de la cadena.
		/// </summary>
		public string CodCadena { get; set; }

		/// <summary>
		/// Código de la region.
		/// </summary>
		public string CodRegion { get; set; }

		/// <summary>
		/// Zonas a asociar al usuario.
		/// </summary>
		public string[] ZonasAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador del comando AsociarUsuarioZonaCommand.
	/// </summary>
	public class AsociarUsuarioZonaHandler : IRequestHandler<AsociarUsuarioZonaCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador.
		/// </summary>
		public AsociarUsuarioZonaHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioZonaCommand.
		/// </summary>
		/// <param name="request">El comando.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Una tarea que representa la operación asincrónica. El resultado de la tarea es una RespuestaComunDTO.</returns>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioZonaCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
			try
			{
				var zonaesAsocidas = await _contexto.RepositorioSegZona
					.Obtener(x => x.CodUsuario == request.CodUsuario 
					&& x.CodEmpresa == request.CodEmpresa 
					&& x.CodCadena == request.CodCadena
					&& x.CodRegion == request.CodRegion)
					.ToListAsync();

				if (zonaesAsocidas.Count > 0)
				{
					_contexto.RepositorioSegZona.EliminarRango(zonaesAsocidas);
				}

				if (request.ZonasAsociadas != null)
				{
					foreach (var zona in request.ZonasAsociadas)
					{
						_contexto.RepositorioSegZona.Agregar(new Seg_Zona
						{
							CodUsuario = request.CodUsuario,
							CodEmpresa = request.CodEmpresa,
							CodCadena = request.CodCadena,
							CodRegion = request.CodRegion,
							CodZona = zona
						});
					}
				}


				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las zonas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}