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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Commands
{
	///<summary>
	/// Comando para actualizar un perfi.
	/// </summary>
	public class ActualizarPerfilCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Obtiene o establece el código del perfi.
		/// </summary>
		public string CodPerfil { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del perfi.
		/// </summary>
		public string NomPerfil { get; set; }
		public string TipPerfil { get; set; }
		public string IndActivo { get; set; }

		/// <summary>
		/// Obtiene o establece el usuario que realiza la modificación.
		/// </summary>
		public string UsuModifica { get; set; }
	}

	/// <summary>
	/// Manejador para el comando de actualización de perfi.
	/// </summary>
	public class ActualizarPerfilHandler : IRequestHandler<ActualizarPerfilCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		/// <summary>
		/// Inicializa una nueva instancia de la clase <see cref="ActualizarPerfilHandler"/>.
		/// </summary>
		/// <param name="mapper">El mapeador de objetos.</param>
		public ActualizarPerfilHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
		}

		/// <summary>
		/// Maneja la ejecución del comando de actualización de perfi.
		/// </summary>
		/// <param name="request">El comando de actualización de perfi.</param>
		/// <param name="cancellationToken">El token de cancelación.</param>
		/// <returns>Respuesta común del comando.</returns>
		public async Task<RespuestaComunDTO> Handle(ActualizarPerfilCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				var perfi = await _contexto.RepositorioSegPerfil.Obtener(x => x.CodPerfil == request.CodPerfil).FirstOrDefaultAsync();
				if (perfi == null)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El perfil no existe";
					return respuesta;
				}

				_mapper.Map(request, perfi);
				perfi.FecModifica = DateTime.Now;
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al actualizar el perfi";
				_logger.Error(ex, respuesta.Mensaje);
			}
			return respuesta;
		}
	}
}
