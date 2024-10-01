using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using System.Linq;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries
{
    public class ListarInvKardexActivoQuery : IRequest<GenericResponseDTO<List<InvKardexActivoDTO>>>
    {
    }

    public class ListarInvKardexActivoHandler : IRequestHandler<ListarInvKardexActivoQuery, GenericResponseDTO<List<InvKardexActivoDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarInvKardexActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<InvKardexActivoDTO>>> Handle(ListarInvKardexActivoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<InvKardexActivoDTO>> { Ok = true, Data = new List<InvKardexActivoDTO>() };

            try
            {
                var activos = await _contexto.RepositorioInvKardexActivo.Obtener()
                                                                        .OrderBy(x => x.Id)
                                                                        .ToListAsync();
                response.Data = _mapper.Map<List<InvKardexActivoDTO>>(activos);
                //response.Ok = true;
                //response.Mensaje = "Se ha generado la lista correctamente";
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
