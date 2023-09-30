using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{

    public class ListarEmpresasOfiplanQuery : IRequest<ListarEmpresaResponseDTO>
    {
    }

    public class ListarEmpresasOfiplanHandler : IRequestHandler<ListarEmpresasOfiplanQuery, ListarEmpresaResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioEmpresa _repositorioEmpresa;

        public ListarEmpresasOfiplanHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa)
        {
            _mapper = mapper;
            _repositorioEmpresa = repositorioEmpresa;
        }

        public async Task<ListarEmpresaResponseDTO> Handle(ListarEmpresasOfiplanQuery request, CancellationToken cancellationToken)
        {
			var response = new ListarEmpresaResponseDTO();
			try
			{
				var empresas = await _repositorioEmpresa.ListarOfiplan();
				response.Empresas = _mapper.Map<List<EmpresaDTO>>(empresas);
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
