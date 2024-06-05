using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Queries
{
    public class ObtenerInvActivoQuery : IRequest<GenericResponseDTO<ObtenerInvActivoDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string CodActivo { get; set; }
        public string CodModelo { get; set; }
        public string NomMarca { get; set; }
        public string CodSerie { get; set; }
    }

    public class ObtenerInvActivoHandler : IRequestHandler<ObtenerInvActivoQuery, GenericResponseDTO<ObtenerInvActivoDTO>>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerInvActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerInvActivoDTO>> Handle(ObtenerInvActivoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerInvActivoDTO> { Ok = true };
            try
            {
                Inv_Activo invActivo = new Inv_Activo();

                invActivo = await _contexto.RepositorioInventarioActivo.Obtener(s => s.CodEmpresa == request.CodEmpresa
                                                                                    && s.CodCadena == request.CodCadena
                                                                                    && s.CodRegion == request.CodRegion 
                                                                                    && s.CodZona == request.CodZona 
                                                                                    && s.CodLocal == request.CodLocal
                                                                                    && s.CodActivo == request.CodActivo
                                                                                    && s.CodModelo == request.CodModelo
                                                                                    && s.NomMarca == request.NomMarca
                                                                                    && s.CodSerie == request.CodSerie).FirstOrDefaultAsync();

                if (invActivo is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Activo no existe";
                    return response;
                }

                var invActivoDto = _mapper.Map<ObtenerInvActivoDTO>(invActivo);
                response.Data = invActivoDto;
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
