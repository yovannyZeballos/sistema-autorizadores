using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para crear un usuario.
	/// </summary>
	public class CrearUsuarioCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código del colaborador.
		/// </summary>
		public string CodColaborador { get; set; }

		/// <summary>
		/// Indicador de si el usuario está activo.
		/// </summary>
		public string IndActivo { get; set; }

		/// <summary>
		/// Tipo de usuario.
		/// </summary>
		public string TipUsuario { get; set; }

		/// <summary>
		/// Dirección de correo electrónico del usuario.
		/// </summary>
		public string DirEmail { get; set; }

		/// <summary>
		/// Usuario de creación del sistema.
		/// </summary>
		public string UsuCreacion { get; set; }
	}

	/// <summary>
	/// Manejador para el comando de creación de usuario.
	/// </summary>
	public class CrearUsuarioHandler : IRequestHandler<CrearUsuarioCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		/// <summary>
		/// Constructor para el manejador de creación de usuario.
		/// </summary>
		/// <param name="mapper">Un mapeador automático.</param>
		public CrearUsuarioHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
		}

		/// <summary>
		/// Maneja el comando de creación de usuario.
		/// </summary>
		/// <param name="request">El comando de creación de usuario.</param>
		/// <param name="cancellationToken">Un token de cancelación.</param>
		/// <returns>Una respuesta común.</returns>
		public async Task<RespuestaComunDTO> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				bool existeSistema = await _contexto.RepositorioSegUsuario.Existe(x => x.CodUsuario == request.CodUsuario);
				if (existeSistema)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El usuario ya existe";
					return respuesta;
				}

				var usuario = _mapper.Map<Seg_Usuario>(request);
				usuario.FecCreacion = DateTime.Now;
				_contexto.RepositorioSegUsuario.Agregar(usuario);
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al crear el usuario";
				_logger.Error(ex, "Ocurrió un error al crear el usuario");
			}
			return respuesta;
		}
	}

}
