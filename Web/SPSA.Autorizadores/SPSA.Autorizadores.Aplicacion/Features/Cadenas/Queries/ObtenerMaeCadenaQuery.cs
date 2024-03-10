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

namespace SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries
{
    public class ObtenerMaeCadenaQuery : IRequest<GenericResponseDTO<ObtenerMaeCadenaDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
    }

    public class ObtenerMaeCadenaHandler : IRequestHandler<ObtenerMaeCadenaQuery, GenericResponseDTO<ObtenerMaeCadenaDTO>>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeCadenaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeCadenaDTO>> Handle(ObtenerMaeCadenaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeCadenaDTO> { Ok = true };
            try
            {
                var cadena = await _contexto.RepositorioMaeCadena.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodCadena == request.CodCadena).FirstOrDefaultAsync();
                if (cadena is null)
                {
                    response.Ok = false;
                    response.Mensaje = "La cadena no existe";
                    return response;
                }

                var cadenaDto = _mapper.Map<ObtenerMaeCadenaDTO>(cadena);
                response.Data = cadenaDto;
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
