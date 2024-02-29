using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
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
	/// Clase de consulta para listar empresas.
	/// </summary>
	public class ListarEmpresasQuery : IRequest<GenericResponseDTO<List<ListarEmpresaDTO>>>
	{
        public string CodUsuario { get; set; }
    }

	/// <summary>
	/// Manejador de la consulta para listar empresas.
	/// </summary>
	public class ListarEmpresasHandler : IRequestHandler<ListarEmpresasQuery, GenericResponseDTO<List<ListarEmpresaDTO>>>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor del manejador de la consulta para listar empresas.
		/// </summary>
		public ListarEmpresasHandler(IMapper mapper)
		{
			_contexto = new BCTContexto();
			_mapper = mapper;
		}

		/// <summary>
		/// Método para manejar la consulta y obtener la lista de empresas.
		/// </summary>
		/// <param name="request">Consulta para listar empresas.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Respuesta genérica con la lista de empresas.</returns>
		public async Task<GenericResponseDTO<List<ListarEmpresaDTO>>> Handle(ListarEmpresasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarEmpresaDTO>> { Ok = true, Data = new List<ListarEmpresaDTO>() };

			try
			{
				var empresas = await _contexto.RepositorioMaeEmpresa.Obtener().ToListAsync();
				var empresasAsociadas = await _contexto.RepositorioSegEmpresa.Obtener(x => x.CodUsuario == request.CodUsuario).ToListAsync();

				var empresasDto = _mapper.Map<List<ListarEmpresaDTO>>(empresas);

				foreach (var item in empresasDto)
				{
					item.IndAsociado = empresasAsociadas.Exists(x => x.CodEmpresa == item.CodEmpresa);
				}

				response.Data = empresasDto.OrderBy(x => x.CodEmpresa).ToList();
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