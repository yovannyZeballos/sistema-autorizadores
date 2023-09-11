using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{

    public class ListarEmpresasQuery : IRequest<ListarEmpresaResponseDTO>
    {
    }

    public class ListarEmpresasHandler : IRequestHandler<ListarEmpresasQuery, ListarEmpresaResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioEmpresa _repositorioEmpresa;

        public ListarEmpresasHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa)
        {
            _mapper = mapper;
            _repositorioEmpresa = repositorioEmpresa;
        }

        public async Task<ListarEmpresaResponseDTO> Handle(ListarEmpresasQuery request, CancellationToken cancellationToken)
        {
            var response = new  ListarEmpresaResponseDTO();
            try
            {
				var empresas = await _repositorioEmpresa.LocListarEmpresa();
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
