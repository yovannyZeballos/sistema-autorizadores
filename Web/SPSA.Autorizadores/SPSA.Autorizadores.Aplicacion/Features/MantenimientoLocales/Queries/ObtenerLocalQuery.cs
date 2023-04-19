using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries
{
    public class ObtenerLocalQuery : IRequest<SovosLocalDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodFormato { get; set; }
        public string CodLocal { get; set; }
    }

    public class ObtenerLocalHandler : IRequestHandler<ObtenerLocalQuery, SovosLocalDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioSovosLocal _repositorioSovosLocal;

        public ObtenerLocalHandler(IMapper mapper, IRepositorioSovosLocal repositorioSovosLocal)
        {
            _mapper = mapper;
            _repositorioSovosLocal = repositorioSovosLocal;
        }

        public async Task<SovosLocalDTO> Handle(ObtenerLocalQuery request, CancellationToken cancellationToken)
        {
            var local = await _repositorioSovosLocal.ObtenerLocal(request.CodEmpresa, request.CodFormato, request.CodLocal);
            var localDto = _mapper.Map<SovosLocalDTO>(local);
            return localDto;
        }
    }
}
