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
	/// Comando para asociar un usuario con una perfil.
	/// </summary>
	public class AsociarUsuarioPerfilCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código de usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código de usuario de creación.
		/// </summary>
		public string UsuCreacion { get; set; }

		/// <summary>
		/// Perfiles asociadas.
		/// </summary>
		public string[] PerfilesAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador para el comando AsociarUsuarioPerfilCommand.
	/// </summary>
	public class AsociarUsuarioPerfilHandler : IRequestHandler<AsociarUsuarioPerfilCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador AsociarUsuarioPerfilHandler.
		/// </summary>
		public AsociarUsuarioPerfilHandler()
		{
			_contexto = new SGPContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioPerfilCommand.
		/// </summary>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioPerfilCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
			try
			{
				var perfilsAsocidas = await _contexto.RepositorioSegPerfilUsuario.Obtener(x => x.CodUsuario == request.CodUsuario).ToListAsync();
				if (perfilsAsocidas.Count > 0)
				{
					_contexto.RepositorioSegPerfilUsuario.EliminarRango(perfilsAsocidas);
				}

				if (request.PerfilesAsociadas != null)
				{
					foreach (var perfil in request.PerfilesAsociadas)
					{
						_contexto.RepositorioSegPerfilUsuario.Agregar(new Seg_PerfilUsuario
						{
							CodUsuario = request.CodUsuario,
							CodPerfil = perfil,
							IndActivo = "A",
							UsuCreacion = request.UsuCreacion,
							FecCreacion = DateTime.Now
						});
					}
				}


				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las perfiles";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
