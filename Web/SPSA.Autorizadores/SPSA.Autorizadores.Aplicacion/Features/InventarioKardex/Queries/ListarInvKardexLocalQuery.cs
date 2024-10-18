using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries
{
    public class ListarInvKardexLocalQuery : IRequest<GenericResponseDTO<List<InvKardexLocalDTO>>>
    {
    }

    public class ListarInvKardexLocalHandler : IRequestHandler<ListarInvKardexLocalQuery, GenericResponseDTO<List<InvKardexLocalDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarInvKardexLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<InvKardexLocalDTO>>> Handle(ListarInvKardexLocalQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<InvKardexLocalDTO>> { Ok = true, Data = new List<InvKardexLocalDTO>() };

            try
            {
                var activos = await _contexto.RepositorioInvKardexLocal.Obtener()
                                                                        .OrderBy(x => x.NomLocal)
                                                                        .ToListAsync();
                response.Data = _mapper.Map<List<InvKardexLocalDTO>>(activos);
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
