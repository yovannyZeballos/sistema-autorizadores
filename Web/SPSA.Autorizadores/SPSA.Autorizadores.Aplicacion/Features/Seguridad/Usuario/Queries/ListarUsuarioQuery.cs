using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries
{
	/// <summary>
	/// Query para listar todos los usuarios.
	/// </summary>
	public class ListarUsuarioQuery : IRequest<GenericResponseDTO<List<ListarUsuarioDTO>>>
	{
	}

	/// <summary>
	/// Manejador para el query de listar todos los usuarios.
	/// </summary>
	public class ListarUsuarioHandler : IRequestHandler<ListarUsuarioQuery, GenericResponseDTO<List<ListarUsuarioDTO>>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor para el manejador de listar todos los usuarios.
		/// </summary>
		/// <param name="mapper">Un mapeador automático.</param>
		public ListarUsuarioHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
		}

		/// <summary>
		/// Maneja el query de listar todos los usuarios.
		/// </summary>
		/// <param name="request">El query de listar todos los usuarios.</param>
		/// <param name="cancellationToken">Un token de cancelación.</param>
		/// <returns>Una respuesta genérica con la lista de todos los usuarios.</returns>
		public async Task<GenericResponseDTO<List<ListarUsuarioDTO>>> Handle(ListarUsuarioQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarUsuarioDTO>> { Ok = true };
			try
			{
                //var usuariosds = await _contexto.RepositorioSegUsuario.PruebasProcedure();
                var usuarios = await _contexto.RepositorioSegUsuario.Obtener().ToListAsync();
				response.Data = _mapper.Map<List<ListarUsuarioDTO>>(usuarios);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}
			return response;
		}
	}
}