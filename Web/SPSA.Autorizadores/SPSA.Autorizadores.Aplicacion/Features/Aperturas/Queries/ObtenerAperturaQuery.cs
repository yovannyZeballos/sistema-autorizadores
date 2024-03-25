using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Aperturas.Queries
{
    public class ObtenerAperturaQuery : IRequest<GenericResponseDTO<ObtenerAperturaDTO>>
    {
        public string CodLocalPMM { get; set; }
    }

    public class ObtenerAperturaHandler : IRequestHandler<ObtenerAperturaQuery, GenericResponseDTO<ObtenerAperturaDTO>>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerAperturaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerAperturaDTO>> Handle(ObtenerAperturaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerAperturaDTO> { Ok = true };
            try
            {
                var apertura = await _contexto.RepositorioApertura.Obtener(s => s.CodLocalPMM == request.CodLocalPMM).FirstOrDefaultAsync();
                if (apertura is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Local Apertura no existe";
                    return response;
                }

                var aperturaDto = _mapper.Map<ObtenerAperturaDTO>(apertura);
                response.Data = aperturaDto;
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }
            return response;
        }
    }
}
