using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
    public class ObtenerLocalOfiplanQuery : IRequest<LocalOfiplanDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodSede { get; set; }
    }

    public class ObtenerLocalOfiplanHandler : IRequestHandler<ObtenerLocalOfiplanQuery, LocalOfiplanDTO>
    {
        private readonly IRepositorioLocalOfiplan _repositorioLocalOfiplan;
        private readonly IMapper _mapper;

        public ObtenerLocalOfiplanHandler(IRepositorioLocalOfiplan repositorioLocalOfiplan, IMapper mapper)
        {
            _repositorioLocalOfiplan = repositorioLocalOfiplan;
            _mapper = mapper;
        }

        public async Task<LocalOfiplanDTO> Handle(ObtenerLocalOfiplanQuery request, CancellationToken cancellationToken)
        {
            var localOfiplanDTO = new LocalOfiplanDTO();
            try
            {
                var localOfiplan = await _repositorioLocalOfiplan.ObtenerLocal(request.CodEmpresa, request.CodSede);
                localOfiplanDTO =  _mapper.Map<LocalOfiplanDTO>(localOfiplan);
                localOfiplanDTO.Ok = true;
            }
            catch (System.Exception ex)
            {
                localOfiplanDTO.Ok = false;
                localOfiplanDTO.Mensaje = ex.Message;
            }

            return localOfiplanDTO;
        }
    }
}
