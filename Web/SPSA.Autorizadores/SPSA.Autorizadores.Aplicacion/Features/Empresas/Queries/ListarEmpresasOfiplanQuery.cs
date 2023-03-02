using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{

    public class ListarEmpresasOfiplanQuery : IRequest<List<EmpresaDTO>>
    {
    }

    public class ListarEmpresasOfiplanHandler : IRequestHandler<ListarEmpresasOfiplanQuery, List<EmpresaDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioEmpresa _repositorioEmpresa;

        public ListarEmpresasOfiplanHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa)
        {
            _mapper = mapper;
            _repositorioEmpresa = repositorioEmpresa;
        }

        public async Task<List<EmpresaDTO>> Handle(ListarEmpresasOfiplanQuery request, CancellationToken cancellationToken)
        {
            var empresas = await _repositorioEmpresa.ListarOfiplan();
            var empresassDto = _mapper.Map<List<EmpresaDTO>>(empresas);
            return empresassDto;
        }
    }
}
