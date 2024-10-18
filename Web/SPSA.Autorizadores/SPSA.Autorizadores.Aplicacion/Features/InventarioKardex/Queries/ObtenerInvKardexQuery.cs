using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries
{
    public class ObtenerInvKardexQuery : IRequest<GenericResponseDTO<InvKardexDTO>>
    {
        public int Id { get; set; }
    }

    public class ObtenerInvKardexHandler : IRequestHandler<ObtenerInvKardexQuery, GenericResponseDTO<InvKardexDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerInvKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<InvKardexDTO>> Handle(ObtenerInvKardexQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<InvKardexDTO> { Ok = true };
            try
            {
                InvKardex invKardex = new InvKardex();

                invKardex = await _contexto.RepositorioInvKardex.Obtener(s => s.Id == request.Id).FirstOrDefaultAsync();

                if (invKardex is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Activo no existe";
                    return response;
                }

                var invActivoDto = _mapper.Map<InvKardexDTO>(invKardex);
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
