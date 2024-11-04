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
    public class ObtenerInvKardexLocalQuery : IRequest<GenericResponseDTO<InvKardexLocalDTO>>
    {
        public int Id { get; set; }
        public string Sociedad { get; set; }
        public string NomLocal { get; set; }
    }

    public class ObtenerInvKardexLocalHandler : IRequestHandler<ObtenerInvKardexLocalQuery, GenericResponseDTO<InvKardexLocalDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerInvKardexLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<InvKardexLocalDTO>> Handle(ObtenerInvKardexLocalQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<InvKardexLocalDTO> { Ok = true };
            try
            {
                InvKardexLocal invKardexLocal = new InvKardexLocal();

                invKardexLocal = await _contexto.RepositorioInvKardexLocal.Obtener(s => s.Id == request.Id).FirstOrDefaultAsync();

                if (invKardexLocal is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Local no existe";
                    return response;
                }

                var invLocalDto = _mapper.Map<InvKardexLocalDTO>(invKardexLocal);
                response.Data = invLocalDto;
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
