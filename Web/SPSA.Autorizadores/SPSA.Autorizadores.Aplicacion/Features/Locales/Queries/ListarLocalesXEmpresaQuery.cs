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

    public class ListarLocalesXEmpresaQuery : IRequest<ListarLocalXEmpresaResponseDTO>
    {
        public string CodEmpresa { get; set; }
    }

    public class ListarLocalesXEmpresaHandler : IRequestHandler<ListarLocalesXEmpresaQuery, ListarLocalXEmpresaResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioSovosLocal _repositorioLocal;

        public ListarLocalesXEmpresaHandler(IMapper mapper, IRepositorioSovosLocal repositorioLocal)
        {
            _mapper = mapper;
            _repositorioLocal = repositorioLocal;
        }

        public async Task<ListarLocalXEmpresaResponseDTO> Handle(ListarLocalesXEmpresaQuery request, CancellationToken cancellationToken)
        {
            var response = new ListarLocalXEmpresaResponseDTO();
            try
            {
				var locales = await _repositorioLocal.ListarPorEmpresa(request.CodEmpresa);
				response.Locales = _mapper.Map<List<SovosLocalDTO>>(locales);
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
