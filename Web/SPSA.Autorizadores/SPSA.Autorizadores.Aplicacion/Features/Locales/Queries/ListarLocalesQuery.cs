using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{

    public class ListarLocalesQuery : IRequest<List<LocalDTO>>
    {
        public string Ruc { get; set; } = string.Empty;
        public List<LocalDTO> Locales { get; set; } 
    }

    public class ListarLocalesHandler : IRequestHandler<ListarLocalesQuery, List<LocalDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioLocal _repositorioLocal;
        private readonly IRepositorioSeguridad _repositorioSeguridad;

        public ListarLocalesHandler(IMapper mapper, IRepositorioLocal repositorioLocal, IRepositorioSeguridad repositorioSeguridad)
        {
            _mapper = mapper;
            _repositorioLocal = repositorioLocal;
            _repositorioSeguridad = repositorioSeguridad;
        }

        public async Task<List<LocalDTO>> Handle(ListarLocalesQuery request, CancellationToken cancellationToken)
        {
            var locales = await _repositorioLocal.ListarXEmpresa(request.Ruc);
            locales = locales.Where(x => request.Locales.Select(p => p.Codigo.Trim()).Contains(x.Codigo)).ToList();
            var localessDto = _mapper.Map<List<LocalDTO>>(locales);
            return localessDto;
        }
    }
}
