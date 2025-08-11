using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.AreaGestion;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.AreaGestion
{
    public class ListarMaeAreaGestionQuery : IRequest<GenericResponseDTO<List<ListarMaeAreaGestionDto>>>
    {
    }

    public class ListarMaeAreaGestionHandler : IRequestHandler<ListarMaeAreaGestionQuery, GenericResponseDTO<List<ListarMaeAreaGestionDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeAreaGestionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeAreaGestionDto>>> Handle(ListarMaeAreaGestionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeAreaGestionDto>>
            {
                Ok = true,
                Data = new List<ListarMaeAreaGestionDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMaeAreaGestion
                    .Obtener(x => x.IndActivo == "S")
                    .OrderBy(x => x.NomAreaGestion)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMaeAreaGestionDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de registros obtenido correctamente.";
            }
            catch (System.Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }

            return response;
        }
    }
}
