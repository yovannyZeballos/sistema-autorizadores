using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
    public class ListarEmpresasMonitorQuery : IRequest<List<EmpresaDTO>>
    {
    }

    public class ListarEmpresasMonitorHandler : IRequestHandler<ListarEmpresasMonitorQuery, List<EmpresaDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioEmpresa _repositorioEmpresa;

        public ListarEmpresasMonitorHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa)
        {
            _mapper = mapper;
            _repositorioEmpresa = repositorioEmpresa;
        }

        public async Task<List<EmpresaDTO>> Handle(ListarEmpresasMonitorQuery request, CancellationToken cancellationToken)
        {
            var empresas = await _repositorioEmpresa.ListarMonitor();
            var empresassDto = _mapper.Map<List<EmpresaDTO>>(empresas);
            return empresassDto;
        }
    }
}
