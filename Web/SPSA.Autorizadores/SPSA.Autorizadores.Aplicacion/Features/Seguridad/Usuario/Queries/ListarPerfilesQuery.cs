using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries
{
	/// <summary>
	/// Clase de consulta para listar perfiles.
	/// </summary>
	public class ListarPerfilesQuery : IRequest<GenericResponseDTO<List<ListarPerfilDTO>>>
	{
		public string CodUsuario { get; set; }
	}

	/// <summary>
	/// Manejador de la consulta para listar perfiles.
	/// </summary>
	public class ListarPerfilesHandler : IRequestHandler<ListarPerfilesQuery, GenericResponseDTO<List<ListarPerfilDTO>>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador de la consulta para listar perfiles.
		/// </summary>
		public ListarPerfilesHandler(IMapper mapper)
		{
			_contexto = new SGPContexto();
			_mapper = mapper;
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Método para manejar la consulta y obtener la lista de perfiles.
		/// </summary>
		/// <param name="request">Consulta para listar perfiles.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Respuesta genérica con la lista de perfiles.</returns>
		public async Task<GenericResponseDTO<List<ListarPerfilDTO>>> Handle(ListarPerfilesQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarPerfilDTO>> { Ok = true, Data = new List<ListarPerfilDTO>() };

			try
			{
				var perfiles = await _contexto.RepositorioSegPerfil.Obtener(x => x.IndActivo == "A").ToListAsync();
				var perfilesAsociadas = await _contexto.RepositorioSegPerfilUsuario.Obtener(x => x.CodUsuario == request.CodUsuario).ToListAsync();

				var perfilesDto = _mapper.Map<List<ListarPerfilDTO>>(perfiles);

				foreach (var item in perfilesDto)
				{
					item.IndAsociado = perfilesAsociadas.Exists(x => x.CodPerfil == item.CodPerfil);
				}

				response.Data = perfilesDto;
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar los perfiles";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}