using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Commands
{
	/// <summary>
	/// Comando para crear un perfil.
	/// </summary>
	public class CrearPerfilCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código del perfil.
		/// </summary>
		public string CodPerfil { get; set; }

		/// <summary>
		/// Nombre del perfil.
		/// </summary>
		public string NomPerfil { get; set; }
		public string TipPerfil { get; set; }
		public string IndActivo { get; set; }
		public string UsuCreacion { get; set; }
	}

	/// <summary>
	/// Manejador del comando para crear un perfil.
	/// </summary>
	public class CrearPerfilHandler : IRequestHandler<CrearPerfilCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		/// <summary>
		/// Constructor del manejador del comando para crear un perfil.
		/// </summary>
		/// <param name="mapper">Instancia de IMapper.</param>
		public CrearPerfilHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
		}

		/// <summary>
		/// Maneja la ejecución del comando para crear un perfil.
		/// </summary>
		/// <param name="request">Comando para crear un perfil.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Respuesta común del comando.</returns>
		public async Task<RespuestaComunDTO> Handle(CrearPerfilCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				bool existePerfil = await _contexto.RepositorioSegPerfil.Existe(x => x.CodPerfil == request.CodPerfil);
				if (existePerfil)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El perfil ya existe";
					return respuesta;
				}

				var perfil = _mapper.Map<Seg_Perfil>(request);
				perfil.FecCreacion = DateTime.Now;
				_contexto.RepositorioSegPerfil.Agregar(perfil);
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al crear el perfil";
				_logger.Error(ex, respuesta.Mensaje);
			}
			return respuesta;
		}
	}
}
