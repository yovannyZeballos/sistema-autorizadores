using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
    public class ObtenerLocalQuery : IRequest<LocalDTO>
    {
        public string Codigo { get; set; } = string.Empty;
    }

    public class ObtenerLocalHandler : IRequestHandler<ObtenerLocalQuery, LocalDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioLocal _repositorioLocal;

        public ObtenerLocalHandler(IMapper mapper, IRepositorioLocal repositorioLocal)
        {
            _mapper = mapper;
            _repositorioLocal = repositorioLocal;
        }

        public async Task<LocalDTO> Handle(ObtenerLocalQuery request, CancellationToken cancellationToken)
        {
            var localAutorizador = await _repositorioLocal.Obtener(Convert.ToInt32(request.Codigo));
            var localCarteleria = await _repositorioLocal.ObtenerCarteleria(request.Codigo);
            localAutorizador.TipoSO = localCarteleria.TipoSO;
            var localDto = _mapper.Map<LocalDTO>(localAutorizador);
            return localDto;
        }
    }
}
