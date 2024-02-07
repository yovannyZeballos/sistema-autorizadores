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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Commands
{
	///<summary>
	/// Comando para actualizar un sistema.
	/// </summary>
	public class ActualizarSistemaCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Obtiene o establece el código del sistema.
		/// </summary>
		public string CodSistema { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del sistema.
		/// </summary>
		public string NomSistema { get; set; }

		/// <summary>
		/// Obtiene o establece el indicador de activo del sistema.
		/// </summary>
		public string IndActivo { get; set; }

		/// <summary>
		/// Obtiene o establece la sigla del sistema.
		/// </summary>
		public string Sigla { get; set; }

		/// <summary>
		/// Obtiene o establece el usuario que realiza la modificación.
		/// </summary>
		public string UsuModifica { get; set; }
	}

	/// <summary>
	/// Manejador para el comando de actualización de sistema.
	/// </summary>
	public class ActualizarSistemaHandler : IRequestHandler<ActualizarSistemaCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		/// <summary>
		/// Inicializa una nueva instancia de la clase <see cref="ActualizarSistemaHandler"/>.
		/// </summary>
		/// <param name="mapper">El mapeador de objetos.</param>
		public ActualizarSistemaHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
		}

		/// <summary>
		/// Maneja la ejecución del comando de actualización de sistema.
		/// </summary>
		/// <param name="request">El comando de actualización de sistema.</param>
		/// <param name="cancellationToken">El token de cancelación.</param>
		/// <returns>Respuesta común del comando.</returns>
		public async Task<RespuestaComunDTO> Handle(ActualizarSistemaCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				var sistema = await _contexto.RepositorioSegSistema.Obtener(x => x.CodSistema == request.CodSistema).FirstOrDefaultAsync();
				if (sistema == null)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El sistema no existe";
					return respuesta;
				}

				_mapper.Map(request, sistema);
				sistema.FecModifica = DateTime.Now;
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al actualizar el sistema";
				_logger.Error(ex, "Ocurrió un error al actualizar el sistema");
			}
			return respuesta;
		}
	}

}