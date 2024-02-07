using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{
	public class ListarListBoxQuery : IRequest<GenericResponseDTO<List<ListBoxDTO>>>
	{
        public string Usuario { get; set; }
    }

	public class ListarListBoxHandler : IRequestHandler<ListarListBoxQuery, GenericResponseDTO<List<ListBoxDTO>>>
	{
		private readonly IRepositorioProcesos _repositorioProcesos;
		private readonly IMapper _mapper;

		public ListarListBoxHandler(IRepositorioProcesos repositorioProcesos, IMapper mapper)
		{
			_repositorioProcesos = repositorioProcesos;
			_mapper = mapper;
		}

		public async Task<GenericResponseDTO<List<ListBoxDTO>>> Handle(ListarListBoxQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListBoxDTO>>();
			try
			{
				var data = await _repositorioProcesos.ListarListBox(request.Usuario);
				response.Data = _mapper.Map<List<ListBoxDTO>>(data);
				response.Ok = true;
			}
			catch (System.Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return response;
		}
	}
}
