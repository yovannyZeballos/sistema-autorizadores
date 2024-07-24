using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
    public class ObtenerInvCajaQuery : IRequest<GenericResponseDTO<ObtenerInvCajaDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public decimal NumCaja { get; set; }
        public string CodActivo { get; set; }
    }

    public class ObtenerInvCajaHandler : IRequestHandler<ObtenerInvCajaQuery, GenericResponseDTO<ObtenerInvCajaDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerInvCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerInvCajaDTO>> Handle(ObtenerInvCajaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerInvCajaDTO> { Ok = true };
            try
            {
                InvCajas invCaja = new InvCajas();

                invCaja = await _contexto.RepositorioInvCajas.Obtener(s => s.CodEmpresa == request.CodEmpresa
                                                                                    && s.CodCadena == request.CodCadena
                                                                                    && s.CodRegion == request.CodRegion
                                                                                    && s.CodZona == request.CodZona
                                                                                    && s.CodLocal == request.CodLocal
                                                                                    && s.NumCaja == request.NumCaja
                                                                                    && s.CodActivo == request.CodActivo).FirstOrDefaultAsync();
                if (invCaja is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Inventario caja no existe";
                    return response;
                }

                var invCajaDto = _mapper.Map<ObtenerInvCajaDTO>(invCaja);
                response.Data = invCajaDto;
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
