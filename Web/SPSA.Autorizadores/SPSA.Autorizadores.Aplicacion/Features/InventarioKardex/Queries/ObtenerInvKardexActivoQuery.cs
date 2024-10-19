using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries
{
    public class ObtenerInvKardexActivoQuery : IRequest<GenericResponseDTO<InvKardexActivoDTO>>
    {
        public string Id { get; set; }
        public string Modelo { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Area { get; set; }
        public string Tipo { get; set; }
    }

    public class ObtenerInvKardexActivoHandler : IRequestHandler<ObtenerInvKardexActivoQuery, GenericResponseDTO<InvKardexActivoDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerInvKardexActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<InvKardexActivoDTO>> Handle(ObtenerInvKardexActivoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<InvKardexActivoDTO> { Ok = true };
            try
            {
                InvKardexActivo invKardexActivo = new InvKardexActivo();

                invKardexActivo = await _contexto.RepositorioInvKardexActivo.Obtener(s => s.Id == request.Id).FirstOrDefaultAsync();

                if (invKardexActivo is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Activo no existe";
                    return response;
                }

                var invActivoDto = _mapper.Map<InvKardexActivoDTO>(invKardexActivo);
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
