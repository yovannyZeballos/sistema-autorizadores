using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{

    public class ListarEmpresasQuery : IRequest<List<EmpresaDTO>>
    {
    }

    public class ListarEmpresasHandler : IRequestHandler<ListarEmpresasQuery, List<EmpresaDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioEmpresa _repositorioEmpresa;

        public ListarEmpresasHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa)
        {
            _mapper = mapper;
            _repositorioEmpresa = repositorioEmpresa;
        }

        public async Task<List<EmpresaDTO>> Handle(ListarEmpresasQuery request, CancellationToken cancellationToken)
        {
            var empresas = await _repositorioEmpresa.Listar();
            var empresassDto = _mapper.Map<List<EmpresaDTO>>(empresas);
            return empresassDto;
        }
    }
}
