
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{
    public class ObtenerMaeEmpresaQuery : IRequest<GenericResponseDTO<ObtenerMaeEmpresaDTO>>
    {
        public string CodEmpresa { get; set; }
    }

    public class ObtenerMaeEmpresaHandler : IRequestHandler<ObtenerMaeEmpresaQuery, GenericResponseDTO<ObtenerMaeEmpresaDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeEmpresaDTO>> Handle(ObtenerMaeEmpresaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeEmpresaDTO> { Ok = true };
            try
            {
                var empresa = await _contexto.RepositorioMaeEmpresa.Obtener(s => s.CodEmpresa == request.CodEmpresa).FirstOrDefaultAsync();
                if (empresa is null)
                {
                    response.Ok = false;
                    response.Mensaje = "La empresa no existe";
                    return response;
                }

                var empresaDto = _mapper.Map<ObtenerMaeEmpresaDTO>(empresa);
                response.Data = empresaDto;
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = "Ocurrió un error al obtener empresa";
                _logger.Error(ex, "Ocurrió un error al obtener empresa");
            }
            return response;
        }
    }
}
