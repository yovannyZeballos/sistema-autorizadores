using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Ubigeos.Queries
{
    public class ObtenerUbigeoQuery : IRequest<GenericResponseDTO<ObtenerUbiDistritoDTO>>
    {
        public string CodUbigeo { get; set; }
    }

    public class ObtenerUbigeoHandler : IRequestHandler<ObtenerUbigeoQuery, GenericResponseDTO<ObtenerUbiDistritoDTO>>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerUbigeoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerUbiDistritoDTO>> Handle(ObtenerUbigeoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerUbiDistritoDTO> { Ok = true };
            try
            {
                var distrito = await _contexto.RepositorioUbiDistrito.Obtener(s => s.CodUbigeo == request.CodUbigeo).FirstOrDefaultAsync();
                if (distrito is null)
                {
                    response.Ok = false;
                    response.Mensaje = "El disitrito no existe";
                    return response;
                }

                var distritoDto = _mapper.Map<ObtenerUbiDistritoDTO>(distrito);
                response.Data = distritoDto;
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
