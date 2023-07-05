using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries
{
	public class ListarInventarioTipoQuery : IRequest<ListarInventarioTipoDTO>
	{
		public string Codigo { get; set; }
	}

	public class ListarInventarioTipoHandler : IRequestHandler<ListarInventarioTipoQuery, ListarInventarioTipoDTO>
	{
		private readonly IRepositorioInventarioTipo _repositorioInventarioTipo;
		private readonly IMapper _mapper;

		public ListarInventarioTipoHandler(IRepositorioInventarioTipo repositorioInventarioTipo, IMapper mapper)
		{
			_repositorioInventarioTipo = repositorioInventarioTipo;
			_mapper = mapper;
		}

		public async Task<ListarInventarioTipoDTO> Handle(ListarInventarioTipoQuery request, CancellationToken cancellationToken)
		{
			var response = new ListarInventarioTipoDTO();

			try
			{
				var inventario = await _repositorioInventarioTipo.Listar(request.Codigo);
				var inventarioTipoDTO = _mapper.Map<List<InventarioTipoDTO>>(inventario);
				response.Tipos = inventarioTipoDTO;
				response.Ok = true;
			}
			catch (Exception ex)
			{
				response.Mensaje = ex.Message;
				response.Ok = false;
			}

			return response;
		}
	}
}
