using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

    public class ListarAutorizadoresQuery : IRequest<List<AutorizadorDTO>>
    {
        public string CodigoLocal { get; set; } = string.Empty;
    }

    public class ListarAutorizadoresHandler : IRequestHandler<ListarAutorizadoresQuery, List<AutorizadorDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public ListarAutorizadoresHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa , IRepositorioAutorizadores repositorioAutorizadores)
        {
            _mapper = mapper;
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<List<AutorizadorDTO>> Handle(ListarAutorizadoresQuery request, CancellationToken cancellationToken)
        {
            var autorizadores = await _repositorioAutorizadores.ListarAutorizador(request.CodigoLocal);
            var autorizadoresDto = _mapper.Map<List<AutorizadorDTO>>(autorizadores);
            return autorizadoresDto;
        }
    }
}
