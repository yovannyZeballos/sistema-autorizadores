using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

    public class ListarColaboradoresQuery : IRequest<List<ColaboradorDTO>>
    {
        public string CodigoLocal { get; set; } = string.Empty;
    }

    public class ListarColaboradoresHandler : IRequestHandler<ListarColaboradoresQuery, List<ColaboradorDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public ListarColaboradoresHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa , IRepositorioAutorizadores repositorioAutorizadores)
        {
            _mapper = mapper;
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<List<ColaboradorDTO>> Handle(ListarColaboradoresQuery request, CancellationToken cancellationToken)
        {
            var colaboradores = await _repositorioAutorizadores.ListarColaboradores(request.CodigoLocal);
            var colaboradoresDto = _mapper.Map<List<ColaboradorDTO>>(colaboradores);
            return colaboradoresDto;
        }
    }
}
