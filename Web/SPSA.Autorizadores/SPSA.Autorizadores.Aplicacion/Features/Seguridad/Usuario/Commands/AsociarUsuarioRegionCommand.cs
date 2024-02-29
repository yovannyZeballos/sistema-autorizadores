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
	/// Comando para asociar un usuario a una region.
	/// </summary>
	public class AsociarUsuarioRegionCommand : IRequest<RespuestaComunDTO>
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
		/// Regions a asociar al usuario.
		/// </summary>
		public string[] RegionesAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador del comando AsociarUsuarioRegionCommand.
	/// </summary>
	public class AsociarUsuarioRegionHandler : IRequestHandler<AsociarUsuarioRegionCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador.
		/// </summary>
		public AsociarUsuarioRegionHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioRegionCommand.
		/// </summary>
		/// <param name="request">El comando.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Una tarea que representa la operación asincrónica. El resultado de la tarea es una RespuestaComunDTO.</returns>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioRegionCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
			try
			{
				var regionesAsocidas = await _contexto.RepositorioSegRegion
					.Obtener(x => x.CodUsuario == request.CodUsuario && x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena)
					.ToListAsync();

				if (regionesAsocidas.Count > 0)
				{
					_contexto.RepositorioSegRegion.EliminarRango(regionesAsocidas);
				}

				if (request.RegionesAsociadas != null)
				{
					foreach (var region in request.RegionesAsociadas)
					{
						_contexto.RepositorioSegRegion.Agregar(new Seg_Region
						{
							CodUsuario = request.CodUsuario,
							CodEmpresa = request.CodEmpresa,
							CodCadena = request.CodCadena,
							CodRegion = region
						});
					}
				}


				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las regiones";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}