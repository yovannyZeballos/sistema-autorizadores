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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Commands
{
	/// <summary>
	/// Comando para crear un sistema.
	/// </summary>
	public class CrearSistemaCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código del sistema.
		/// </summary>
		public string CodSistema { get; set; }

		/// <summary>
		/// Nombre del sistema.
		/// </summary>
		public string NomSistema { get; set; }

		/// <summary>
		/// Sigla del sistema.
		/// </summary>
		public string Sigla { get; set; }

		/// <summary>
		/// Obtiene o establece el indicador de activo del sistema.
		/// </summary>
		public string IndActivo { get; set; }

		/// <summary>
		/// Usuario de creación del sistema.
		/// </summary>
		public string UsuCreacion { get; set; }
	}

	/// <summary>
	/// Manejador del comando para crear un sistema.
	/// </summary>
	public class CrearSistemaHandler : IRequestHandler<CrearSistemaCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		/// <summary>
		/// Constructor del manejador del comando para crear un sistema.
		/// </summary>
		/// <param name="mapper">Instancia de IMapper.</param>
		public CrearSistemaHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
		}

		/// <summary>
		/// Maneja la ejecución del comando para crear un sistema.
		/// </summary>
		/// <param name="request">Comando para crear un sistema.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Respuesta común del comando.</returns>
		public async Task<RespuestaComunDTO> Handle(CrearSistemaCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				bool existeSistema = await _contexto.RepositorioSegSistema.Existe(x => x.CodSistema == request.CodSistema);
				if (existeSistema)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El sistema ya existe";
					return respuesta;
				}

				var sistema = _mapper.Map<Seg_Sistema>(request);
				sistema.FecCreacion = DateTime.Now;
				_contexto.RepositorioSegSistema.Agregar(sistema);
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al crear el sistema";
				_logger.Error(ex, "Ocurrió un error al crear el sistema");
			}
			return respuesta;
		}
	}
}
