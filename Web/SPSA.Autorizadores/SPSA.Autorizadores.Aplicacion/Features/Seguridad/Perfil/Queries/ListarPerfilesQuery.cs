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
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Queries
{
	/// <summary>
	/// Query para listar los perfils.
	/// </summary>
	public class ListarPerfilesQuery : IRequest<GenericResponseDTO<List<ListarPerfilDTO>>>
	{
	}

	public class ListarPerfilesHandler : IRequestHandler<ListarPerfilesQuery, GenericResponseDTO<List<ListarPerfilDTO>>>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;


		public ListarPerfilesHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
		}

		/// <summary>
		/// Maneja la solicitud para listar los perfiles.
		/// </summary>
		/// <param name="request">La solicitud de listar perfils.</param>
		/// <param name="cancellationToken">El token de cancelación.</param>
		/// <returns>La respuesta con la lista de perfils.</returns>
		public async Task<GenericResponseDTO<List<ListarPerfilDTO>>> Handle(ListarPerfilesQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarPerfilDTO>> { Ok = true };
			try
			{
				var perfil = await _contexto.RepositorioSegPerfil.Obtener().ToListAsync();
				var perfilDto = _mapper.Map<List<ListarPerfilDTO>>(perfil);
				response.Data = perfilDto;
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar los perfils";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
