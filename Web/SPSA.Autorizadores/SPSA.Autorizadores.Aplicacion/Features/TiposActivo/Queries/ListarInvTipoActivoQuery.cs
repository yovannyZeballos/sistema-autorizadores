using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Queries
{
    public class ListarInvTipoActivoQuery : IRequest<GenericResponseDTO<List<InvTipoActivoDTO>>>
    {
    }

    public class ListarInvTipoActivoHandler : IRequestHandler<ListarInvTipoActivoQuery, GenericResponseDTO<List<InvTipoActivoDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarInvTipoActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<InvTipoActivoDTO>>> Handle(ListarInvTipoActivoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<InvTipoActivoDTO>> { Ok = true, Data = new List<InvTipoActivoDTO>() };

            try
            {
                var activos = await _contexto.RepositorioInvTipoActivo.Obtener(x => x.IndEstado == "A")
                                                                        .OrderBy(x => x.CodActivo)
                                                                        .ToListAsync();
                response.Data = _mapper.Map<List<InvTipoActivoDTO>>(activos);
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
