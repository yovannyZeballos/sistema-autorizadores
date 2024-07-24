using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para actualizar un usuario existente.
	/// </summary>
	public class ActualizarUsuarioCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// C�digo del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// C�digo del colaborador.
		/// </summary>
		public string CodColaborador { get; set; }

		/// <summary>
		/// Indicador de si el usuario est� activo.
		/// </summary>
		public string IndActivo { get; set; }

		/// <summary>
		/// Tipo de usuario.
		/// </summary>
		public string TipUsuario { get; set; }

		/// <summary>
		/// Direcci�n de correo electr�nico del usuario.
		/// </summary>
		public string DirEmail { get; set; }

		/// <summary>
		/// Usuario que realiza la modificaci�n.
		/// </summary>
		public string UsuModifica { get; set; }
	}

	/// <summary>
	/// Manejador para el comando de actualizaci�n de usuario.
	/// </summary>
	public class ActualizarUsuarioHandler : IRequestHandler<ActualizarUsuarioCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		/// <summary>
		/// Constructor para el manejador de actualizaci�n de usuario.
		/// </summary>
		/// <param name="mapper">Un mapeador autom�tico.</param>
		public ActualizarUsuarioHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
		}

		/// <summary>
		/// Maneja el comando de actualizaci�n de usuario.
		/// </summary>
		/// <param name="request">El comando de actualizaci�n de usuario.</param>
		/// <param name="cancellationToken">Un token de cancelaci�n.</param>
		/// <returns>Una respuesta com�n.</returns>
		public async Task<RespuestaComunDTO> Handle(ActualizarUsuarioCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				var usuario = await _contexto.RepositorioSegUsuario.Obtener(x => x.CodUsuario == request.CodUsuario).FirstOrDefaultAsync();
				if (usuario == null)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El usuario no existe";
					return respuesta;
				}

				_mapper.Map(request, usuario);
				usuario.FecElimina = DateTime.Now;
				usuario.UsuElimina = request.UsuModifica;
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurri� un error al actualizar el usuario";
				_logger.Error(ex, "Ocurri� un error al actualizar el usuario");
			}
			return respuesta;
		}
	}
}